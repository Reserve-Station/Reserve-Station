// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Server._Funkystation.Atmos.Events;
using Content.Server.Atmos.Components;
using Content.Server.Atmos.EntitySystems;
using Content.Server.Stains;
using Content.Shared.Atmos;
using Content.Shared.Atmos.Components;
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.Hands.EntitySystems;
using Content.Shared.IgnitionSource;
using Content.Shared.Interaction;
using Content.Shared.Inventory;
using Content.Shared.Stains;
using Content.Shared.Stains.Components;
using Content.Shared.Temperature;
using Content.Shared.Weapons.Melee.Events;
using Content.Goobstation.Maths.FixedPoint;
using Robust.Shared.Containers;
using Robust.Shared.Prototypes;

namespace Content.Server._Funkystation.Stains
{
    public sealed class FlammableStainsSystem : EntitySystem
    {
        [Dependency] private readonly FlammableSystem _flammable = null!;
        [Dependency] private readonly InventorySystem _inventory = null!;
        [Dependency] private readonly SharedSolutionContainerSystem _solution = null!;
        [Dependency] private readonly IPrototypeManager _prototypeManager = null!;
        [Dependency] private readonly EntityLookupSystem _lookup = null!;
        // Reserve edit start: fire-nerf
        [Dependency] private readonly SharedHandsSystem _hands = null!;
        [Dependency] private readonly SharedContainerSystem _container = null!;
        [Dependency] private readonly StainableSystem _stains = null!;

        /// <summary>
        /// 300°C. A pure reagent of flammability 1 ignites 50 K below this.
        /// </summary>
        private const float IgnitionTemperatureBase = 573.15f;

        private const float IgnitionTemperaturePerFlammability = 50f;

        /// <summary>
        /// Fuel strength of one clothing slot below this is a trace and does not ignite.
        /// 0.05 is a quarter-splash of welding fuel in a 5u stain.
        /// </summary>
        private const float MinimumStainStrength = 0.05f;

        /// <summary>
        /// Puddle fires start around this volume. Boots are always in a floor fire; the jumpsuit joins here.
        /// </summary>
        private const float TorsoFireVolume = 100f;

        /// <summary>
        /// Units of each flammable reagent removed per second while the wearer is on fire, multiplied by flammability.
        /// Welding fuel (2) loses 0.2u per second.
        /// </summary>
        private const float StainBurnPerFlammability = 0.1f;

        private float _burnAccumulator;
        // Reserve edit end: fire-nerf

        public override void Initialize()
        {
            base.Initialize();
            SubscribeLocalEvent<InventoryComponent, TileFireEvent>(OnTileFire, before: [typeof(FlammableSystem)]);
            SubscribeLocalEvent<GridAtmosphereComponent, TileExposedEvent>(OnTileExposed);
            // Reserve edit start: fire-nerf
            SubscribeLocalEvent<InventoryComponent, InteractUsingEvent>(OnDirectFlame);
            SubscribeLocalEvent<IgnitionSourceComponent, MeleeHitEvent>(OnHotMelee);
            // Reserve edit end: fire-nerf
        }

        // Reserve edit start: fire-nerf
        public override void Update(float frameTime)
        {
            base.Update(frameTime);

            _burnAccumulator += frameTime;
            if (_burnAccumulator < 1f)
                return;

            _burnAccumulator -= 1f;

            var query = EntityQueryEnumerator<OnFireComponent, InventoryComponent, FlammableComponent>();
            while (query.MoveNext(out var uid, out _, out var inventory, out var flammable))
            {
                if (!flammable.OnFire)
                    continue;

                BurnWornStains(uid, inventory);
            }
        }

        // Reserve edit start: fire-nerf
        private void OnDirectFlame(Entity<InventoryComponent> ent, ref InteractUsingEvent args)
        {
            var isHot = new IsHotEvent();
            RaiseLocalEvent(args.Used, isHot);
            if (!isHot.IsHot)
                return;

            TryIgniteSoaked(ent.Owner, GetFlameTemperature(args.Used));
        }

        private void OnHotMelee(Entity<IgnitionSourceComponent> ent, ref MeleeHitEvent args)
        {
            if (!ent.Comp.Ignited)
                return;

            foreach (var hit in args.HitEntities)
                TryIgniteSoaked(hit, ent.Comp.Temperature);
        }

        /// <summary>
        /// A flame pressed against the person, not a spark on the tile. Every worn stain can catch.
        /// </summary>
        private void TryIgniteSoaked(EntityUid target, float temperature)
        {
            if (!TryComp<InventoryComponent>(target, out var inventory))
                return;

            ApplyStainFire(target, inventory, SlotFlags.WITHOUT_POCKET, temperature);
        }

        private float GetFlameTemperature(EntityUid source)
        {
            if (TryComp<IgnitionSourceComponent>(source, out var ignition))
                return ignition.Temperature;

            return 700f;
        }
        // Reserve edit end: fire-nerf

        private void OnTileFire(EntityUid uid, InventoryComponent component, ref TileFireEvent args)
        {
            ApplyStainFire(uid, component, GetFireSlots(args.Volume), args.Temperature);
        }

        private void OnTileExposed(EntityUid gridUid, GridAtmosphereComponent component, ref TileExposedEvent args)
        {
            var entities = new HashSet<EntityUid>();
            _lookup.GetLocalEntitiesIntersecting(gridUid, args.Tile, entities, 0f);

            foreach (var ent in entities)
            {
                if (!TryComp<InventoryComponent>(ent, out var inv))
                    continue;

                ApplyStainFire(ent, inv, GetSparkSlots(ent, args.SparkSource, args.Volume), args.Temperature);
            }
        }

        /// <summary>
        /// Raises fire stacks up to the fuel on the slots this flame can actually reach.
        /// A later cooler event does not pull stacks back down.
        /// </summary>
        private void ApplyStainFire(EntityUid uid, InventoryComponent inv, SlotFlags slots, float temperature)
        {
            if (!TryComp<FlammableComponent>(uid, out var flammable))
                return;

            // Negative stacks are water still on the body. A stain must not jump them straight back to burning.
            if (flammable.FireStacks < 0f)
                return;

            var strength = 0f;
            foreach (var slot in inv.Slots)
            {
                if ((slot.SlotFlags & slots) == 0)
                    continue;

                if (!_inventory.TryGetSlotEntity(uid, slot.Name, out var slotEnt, inv))
                    continue;

                if (IsSlotStainBlocked(uid, slot, inv))
                    continue;

                if (!TryComp<StainableComponent>(slotEnt, out var stain) ||
                    !_solution.TryGetSolution(slotEnt.Value, stain.SolutionId, out _, out var solution))
                    continue;

                GetStainFire(stain, solution, out var ignitability, out var slotStrength);
                if (ignitability <= 0f || slotStrength < MinimumStainStrength)
                    continue;

                var ignitionTemp = IgnitionTemperatureBase - IgnitionTemperaturePerFlammability * ignitability;
                if (temperature < ignitionTemp)
                    continue;

                strength += slotStrength;
            }

            if (strength <= flammable.FireStacks)
                return;

            _flammable.SetFireStacks(uid, strength, flammable, ignite: true);
        }

        private void BurnWornStains(EntityUid uid, InventoryComponent inv)
        {
            foreach (var slot in inv.Slots)
            {
                if (!_inventory.TryGetSlotEntity(uid, slot.Name, out var slotEnt, inv))
                    continue;

                if (!TryComp<StainableComponent>(slotEnt, out var stain) ||
                    !_solution.TryGetSolution(slotEnt.Value, stain.SolutionId, out var soln, out var solution))
                    continue;

                if (!BurnFlammableReagents(soln.Value, solution))
                    continue;

                _stains.RefreshStainVisuals(slotEnt.Value);
            }
        }

        private bool BurnFlammableReagents(Entity<SolutionComponent> soln, Solution solution)
        {
            var removedAny = false;
            foreach (var reagentQuantity in solution.Contents.ToArray())
            {
                if (!_prototypeManager.TryIndex<ReagentPrototype>(reagentQuantity.Reagent.Prototype, out var proto) ||
                    proto.Flammability <= 0)
                    continue;

                var amount = FixedPoint2.New(StainBurnPerFlammability * proto.Flammability);
                if (amount > reagentQuantity.Quantity)
                    amount = reagentQuantity.Quantity;

                if (amount <= FixedPoint2.Zero)
                    continue;

                _solution.RemoveReagent(soln, reagentQuantity.Reagent, amount);
                removedAny = true;
            }

            return removedAny;
        }

        /// <summary>
        /// Ignitability is the mixture (a drop of pure fuel is still fuel).
        /// Strength is that mixture scaled by how full the stain capacity is, then by the clothing fire scale.
        /// </summary>
        private void GetStainFire(StainableComponent stain, Solution solution, out float ignitability, out float strength)
        {
            ignitability = 0f;
            strength = 0f;

            if (solution.Volume <= FixedPoint2.Zero)
                return;

            var volume = solution.Volume.Float();
            var capacity = stain.MaxVolume.Float();
            if (capacity <= 0f)
                capacity = solution.MaxVolume.Float();

            if (volume <= 0f || capacity <= 0f)
                return;

            foreach (var (reagent, quantity) in solution.Contents)
            {
                if (!_prototypeManager.TryIndex<ReagentPrototype>(reagent.Prototype, out var proto))
                    continue;

                var qty = quantity.Float();
                ignitability += proto.Flammability * (qty / volume);
                strength += proto.Flammability * (qty / capacity);
            }

            strength *= stain.FireScale;
        }

        /// <summary>
        /// Floor fire. Boots always. Jumpsuit and coat once the fire has some size. Hands and head only when it fills the tile.
        /// </summary>
        private static SlotFlags GetFireSlots(float volume)
        {
            var slots = SlotFlags.FEET;
            if (volume >= TorsoFireVolume)
                slots |= SlotFlags.INNERCLOTHING | SlotFlags.OUTERCLOTHING | SlotFlags.LEGS;

            if (volume >= Atmospherics.CellVolume * 0.4f)
                slots |= SlotFlags.GLOVES | SlotFlags.HEAD | SlotFlags.MASK;

            return slots;
        }

        /// <summary>
        /// A spark is not a floor fire. A match in the hand only sees gloves. A cigarette sees the face.
        /// Anything else on the tile is treated as a flame at foot height, unless the exposure is already tile-sized.
        /// </summary>
        private SlotFlags GetSparkSlots(EntityUid wearer, EntityUid? spark, float volume)
        {
            if (spark is { } sparkUid)
            {
                if (_hands.IsHolding(wearer, sparkUid))
                    return SlotFlags.GLOVES;

                if (_container.TryGetContainingContainer(sparkUid, out var container) &&
                    container.Owner == wearer &&
                    _inventory.TryGetSlot(wearer, container.ID, out var slot))
                    return ExpandWornFlameSlot(slot.SlotFlags);
            }

            return GetFireSlots(volume);
        }

        private static SlotFlags ExpandWornFlameSlot(SlotFlags flags)
        {
            if ((flags & SlotFlags.MASK) != 0)
                return SlotFlags.MASK | SlotFlags.HEAD;

            if ((flags & SlotFlags.HEAD) != 0)
                return SlotFlags.HEAD | SlotFlags.MASK;

            return flags;
        }
        // Reserve edit end: fire-nerf

        private bool IsSlotStainBlocked(EntityUid wearer, SlotDefinition slotDef, InventoryComponent inv)
        {
            foreach (var slot in inv.Slots)
            {
                if (!_inventory.TryGetSlotEntity(wearer, slot.Name, out var slotEnt, inv))
                    continue;

                if (TryComp<StainBlockerComponent>(slotEnt, out var blocker))
                {
                    if ((blocker.Slots & slotDef.SlotFlags) != 0)
                        return true;
                }
            }
            return false;
        }
    }
}
