// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Goobstation.Maths.FixedPoint; // Reserve - for FixedPoint2
using Content.Shared.Chemistry.Components;
using Content.Shared.Chemistry.EntitySystems;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.Clothing.Components;
using Content.Shared.Fluids;
using Content.Shared.Inventory;
using Content.Shared.Item;
using Content.Shared.Slippery;
using Content.Shared.WashingMachine.Events;
using Robust.Shared.Containers; // Gaby
using Content.Shared.Stains.Components; // Gaby
using Content.Shared.Verbs; // Gaby
using Content.Shared.DoAfter; // Gaby
using Content.Shared.Popups; // Gaby
using Robust.Shared.Prototypes;
using Robust.Shared.Utility; // Gaby

namespace Content.Shared.Stains;

public abstract partial class SharedStainableSystem : EntitySystem
{
    [Dependency] private readonly SharedAppearanceSystem _appearance = default!;
    [Dependency] private readonly SharedItemSystem _item = default!;
    [Dependency] protected readonly SharedSolutionContainerSystem Solution = default!;
    [Dependency] private readonly InventorySystem _inventory = default!; // Gaby
    [Dependency] private readonly SharedContainerSystem _container = default!; // Gaby
    [Dependency] private readonly SharedDoAfterSystem _doAfter = default!; // Gaby
    [Dependency] private readonly SharedPuddleSystem _puddle = default!; // Gaby
    [Dependency] private readonly SharedPopupSystem _popup = default!; // Gaby
    [Dependency] private readonly IPrototypeManager _prototype = default!; // Reserve edit: fire-nerf

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<StainableComponent, MapInitEvent>(OnMapInit);

        SubscribeLocalEvent<StainableComponent, InventoryRelayedEvent<SlippedEvent>>(OnSlipped);
        SubscribeLocalEvent<StainableComponent, InventoryRelayedEvent<SpilledOnEvent>>(OnSpilledOn);

        SubscribeLocalEvent<StainableComponent, WashingMachineIsBeingWashed>(OnWashed);

        SubscribeLocalEvent<StainableComponent, GetVerbsEvent<Verb>>(AddWringVerb); // Gaby
        SubscribeLocalEvent<StainableComponent, WringStainDoAfterEvent>(OnWringDoAfter); // Gaby
    }

    private void OnMapInit(Entity<StainableComponent> ent, ref MapInitEvent args)
    {
        if (string.IsNullOrEmpty(ent.Comp.SolutionId))
            return;

        if (!Solution.TryGetSolution(ent.Owner, ent.Comp.SolutionId, out var solution))
            return;

        solution.Value.Comp.Solution.CanReact = false;
        UpdateVisuals(ent);
    }

    private void OnSlipped(Entity<StainableComponent> ent, ref InventoryRelayedEvent<SlippedEvent> args)
    {
        if (IsStainBlocked(ent)) // Gaby
            return;

        if (!Solution.TryGetSolution(ent.Owner, ent.Comp.SolutionId, out var target))
            return;

        var ev = new GetStainableSolutionEvent(ent.Owner);
        RaiseLocalEvent(args.Args.Slipper, ev);

        if (!ev.Handled || ev.Solution == null)
            return;

        // Gaby - Pass target.Value because TryGetSolution returns a nullable struct
        TransferStain(target.Value, ev.Solution, ent.Comp.StainVolume);

        UpdateVisuals(ent);
        StainForensics(ent, target.Value);

        DirtyOwnerAppearance(ent.Owner); // Gaby
    }

    private void OnSpilledOn(Entity<StainableComponent> ent, ref InventoryRelayedEvent<SpilledOnEvent> args)
    {
        if (IsStainBlocked(ent)) // Gaby
            return;

        if (!Solution.TryGetSolution(ent.Owner, ent.Comp.SolutionId, out var target))
            return;

        // Gaby - Pass target.Value because TryGetSolution returns a nullable struct
        TransferStain(target.Value, args.Args.Solution, ent.Comp.StainVolume);

        UpdateVisuals(ent);
        StainForensics(ent, target.Value);

        DirtyOwnerAppearance(ent.Owner); // Gaby
    }

    // Reserve edit start: fire-nerf
    /// <summary>
    /// Wets worn clothing from a spray. Slots that already hold flammable liquid are soaked first,
    /// so a small extinguisher puff reaches the fuel instead of the first slots in inventory order.
    /// </summary>
    public void SoakWornClothing(EntityUid wearer, Solution spray)
    {
        if (spray.Volume <= FixedPoint2.Zero || !TryComp<InventoryComponent>(wearer, out var inventory))
            return;

        SoakMatching(wearer, inventory, spray, flammableOnly: true);
        SoakMatching(wearer, inventory, spray, flammableOnly: false);
    }

    public void RefreshStainVisuals(EntityUid clothing)
    {
        if (!TryComp<StainableComponent>(clothing, out var stain))
            return;

        UpdateVisuals((clothing, stain));
    }

    private void SoakMatching(EntityUid wearer, InventoryComponent inventory, Solution spray, bool flammableOnly)
    {
        foreach (var slot in inventory.Slots)
        {
            if (spray.Volume <= FixedPoint2.Zero)
                return;

            if ((slot.SlotFlags & SlotFlags.WITHOUT_POCKET) == 0)
                continue;

            if (!_inventory.TryGetSlotEntity(wearer, slot.Name, out var slotEnt, inventory))
                continue;

            if (!TryComp<StainableComponent>(slotEnt, out var stain))
                continue;

            if (IsStainBlocked((slotEnt.Value, stain)))
                continue;

            if (!Solution.TryGetSolution(slotEnt.Value, stain.SolutionId, out var target, out var solution))
                continue;

            if (flammableOnly && !HasFlammableReagent(solution))
                continue;

            var amount = stain.StainVolume;
            if (amount > spray.Volume)
                amount = spray.Volume;

            TransferStain(target.Value, spray, amount);
            UpdateVisuals((slotEnt.Value, stain));
            DirtyOwnerAppearance(slotEnt.Value);
        }
    }

    private void TransferStain(Entity<SolutionComponent> target, Solution source, FixedPoint2 amount)
    {
        // SplitSolution mutates the source. Water stays in the portion so it can dilute fuel.
        var taken = source.SplitSolution(amount);
        if (taken.Volume <= FixedPoint2.Zero)
            return;

        if (!Solution.TryMixAndOverflow(target, taken, target.Comp.Solution.MaxVolume, out var overflow))
        {
            source.AddSolution(taken, _prototype);
            return;
        }

        if (overflow.Volume > FixedPoint2.Zero)
            source.AddSolution(overflow, _prototype);
    }

    private bool HasFlammableReagent(Solution solution)
    {
        foreach (var (reagent, _) in solution.Contents)
        {
            if (_prototype.TryIndex<ReagentPrototype>(reagent.Prototype, out var proto) && proto.Flammability > 0)
                return true;
        }

        return false;
    }
    // Reserve edit end: fire-nerf

    private bool IsStainBlocked(Entity<StainableComponent> item) // Gaby
    {
        if (!_container.TryGetContainingContainer(item.Owner, out var container))
            return false;
        var wearer = container.Owner;

        if (!TryComp<InventoryComponent>(wearer, out var inventory))
            return false;

        // Get the flags of the slot the item is currently in
        if (!_inventory.TryGetSlot(wearer, container.ID, out var slotDef, inventory))
            return false;

        var inSlotFlag = slotDef.SlotFlags;

        foreach (var slot in inventory.Slots)
        {
            if (!_inventory.TryGetSlotEntity(wearer, slot.Name, out var slotItem, inventory))
                continue;

            if (TryComp<StainBlockerComponent>(slotItem, out var blocker))
            {
                if ((blocker.Slots & inSlotFlag) != 0)
                {
                    return true;
                }
            }
        }

        return false;
    }

    private void OnWashed(Entity<StainableComponent> ent, ref WashingMachineIsBeingWashed args)
    {
        if (Solution.TryGetSolution(ent.Owner, ent.Comp.SolutionId, out var solution))
        {
            WashingForensics(ent, solution.Value, args.WashingMachine);
            Solution.RemoveAllSolution(solution.Value);
            UpdateVisuals(ent);
        }

        // Reserve edit start: fire-nerf
        // Hoods and hardsuit helmets live on the suit, not in its pockets.
        foreach (var attached in EnumerateAttachedClothing(ent.Owner))
        {
            if (!TryComp<StainableComponent>(attached, out var stain))
                continue;

            if (!Solution.TryGetSolution(attached, stain.SolutionId, out var attachedSolution, out var attachedStain) || attachedStain.Volume <= 0)
                continue;

            WashingForensics((attached, stain), attachedSolution.Value, args.WashingMachine);
            Solution.RemoveAllSolution(attachedSolution.Value);
            UpdateVisuals((attached, stain));
            DirtyOwnerAppearance(attached);
        }
        // Reserve edit end: fire-nerf
    }

    protected virtual void StainForensics(Entity<StainableComponent> ent, Entity<SolutionComponent> solution)
    {
    }

    protected virtual void WashingForensics(Entity<StainableComponent> ent, Entity<SolutionComponent> solution, EntityUid washingMachine)
    {
    }

    private void UpdateVisuals(Entity<StainableComponent> ent)
    {
        _item.VisualsChanged(ent.Owner);

        // there isnt a value to parse as its calculated on every change
        // so just do a blanket update and calculate on the client
        if (TryComp<AppearanceComponent>(ent.Owner, out var appearance))
        {
            _appearance.QueueUpdate(ent.Owner, appearance);

            if (TryComp<MetaDataComponent>(ent.Owner, out var meta) && meta.EntityLifeStage < EntityLifeStage.Terminating)
                Dirty(ent.Owner, appearance);
        }
    }

    protected virtual void DirtyOwnerAppearance(EntityUid owner) // Gaby
    {
    }

    private void AddWringVerb(Entity<StainableComponent> ent, ref GetVerbsEvent<Verb> args)
    {
        if (args.Using != ent.Owner)
            return;
        if (!args.CanAccess || !args.CanInteract)
            return;
        // Reserve edit start: fire-nerf
        if (!HasStain(ent) && !AttachedHasStain(ent.Owner))
            return;
        // Reserve edit end: fire-nerf

        var user = args.User;
        var verb = new Verb
        {
            Text = Loc.GetString("stain-verb-wring"),
            Icon = new SpriteSpecifier.Texture(new("/Textures/Interface/VerbIcons/bubbles.svg.192dpi.png")),
            Act = () =>
            {
                var doAfterArgs = new DoAfterArgs(EntityManager, user, ent.Comp.CleanseDelay, new WringStainDoAfterEvent(), ent.Owner, target: ent.Owner)
                {
                    BreakOnMove = true,
                    BreakOnDamage = true,
                    NeedHand = true,
                    DuplicateCondition = DuplicateConditions.SameTool | DuplicateConditions.SameTarget
                };
                _doAfter.TryStartDoAfter(doAfterArgs);
            },
        };
        args.Verbs.Add(verb);
    }

    private void OnWringDoAfter(Entity<StainableComponent> ent, ref WringStainDoAfterEvent args)
    {
        if (args.Handled || args.Cancelled)
            return;

        args.Handled = true;

        var spilled = TryWring(ent, args.User);

        // Reserve edit start: fire-nerf
        foreach (var attached in EnumerateAttachedClothing(ent.Owner))
        {
            if (!TryComp<StainableComponent>(attached, out var stain))
                continue;

            spilled |= TryWring((attached, stain), args.User);
        }
        // Reserve edit end: fire-nerf

        if (spilled)
            _popup.PopupEntity(Loc.GetString("stain-verb-wring-success", ("item", ent.Owner)), args.User, args.User);
    }

    // Reserve edit start: fire-nerf
    private bool HasStain(Entity<StainableComponent> ent)
    {
        return Solution.TryGetSolution(ent.Owner, ent.Comp.SolutionId, out _, out var stainSolution) && stainSolution.Volume > 0;
    }

    private bool AttachedHasStain(EntityUid clothing)
    {
        foreach (var attached in EnumerateAttachedClothing(clothing))
        {
            if (TryComp<StainableComponent>(attached, out var stain) && HasStain((attached, stain)))
                return true;
        }

        return false;
    }

    private IEnumerable<EntityUid> EnumerateAttachedClothing(EntityUid clothing)
    {
        if (!TryComp<ToggleableClothingComponent>(clothing, out var toggleable))
            yield break;

        foreach (var (attached, _) in toggleable.ClothingUids)
            yield return attached;
    }
    // Reserve edit end: fire-nerf

    private bool TryWring(Entity<StainableComponent> ent, EntityUid user)
    {
        if (!Solution.TryGetSolution(ent.Owner, ent.Comp.SolutionId, out var stainSoln, out var stainSolution) || stainSolution.Volume <= 0)
            return false;

        var puddleSolution = Solution.SplitSolution(stainSoln.Value, stainSolution.Volume);
        UpdateVisuals(ent);
        DirtyOwnerAppearance(ent.Owner);
        return _puddle.TrySpillAt(user, puddleSolution, out _);
    }
}
