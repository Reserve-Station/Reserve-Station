using Content.Shared.Actions;
using Content.Shared.Inventory;
using Robust.Shared.GameObjects;
using Robust.Shared.IoC;
using Content.Shared.Hands;
using Content.Shared.Hands.EntitySystems;
using Content.Server.Hands.Systems;
using Content.Shared._FreakyStation.CyberPunk;
namespace Content.Server._FreakyStation.CyberPunk
{
    public sealed class ActionSpawnOnUseSystem : EntitySystem
    {
        [Dependency] private readonly IEntityManager _entityManager = default!;
        [Dependency] private readonly InventorySystem _inventorySystem = default!;
        [Dependency] private readonly SharedHandsSystem _handsSystem = default!;
        public override void Initialize()
        {
            base.Initialize();
            SubscribeLocalEvent<ActionSpawnOnUseComponent, ActionSpawnOnUseEvent>(OnActionUsed);
        }

        private void OnActionUsed(EntityUid uid, ActionSpawnOnUseComponent component, InstantActionEvent args)
        {
            var user = args.Performer;

            // Впереди гремучий лес - шкибиди доп доп ес ес
            if (component.RemoveOnSecondUse && component.SpawnedItem != null)
            {
                if (_entityManager.EntityExists(component.SpawnedItem.Value))
                {
                    _entityManager.DeleteEntity(component.SpawnedItem.Value);
                }
                component.SpawnedItem = null;
                return;
            }
            if (component.DropPreviousItem)
            {
                if (component.SpawnSlot == "active-hand")
                {
                    if (_handsSystem.TryGetActiveHand(user, out var activeHand) && activeHand.HeldEntity != null)
                    {
                        _handsSystem.TryDrop(user, activeHand.HeldEntity.Value);
                    }
                }
                else
                {
                    if (_inventorySystem.TryGetSlot(user, component.SpawnSlot, out var slotEntity))
                    {
                        _inventorySystem.TryUnequip(user, component.SpawnSlot);
                    }
                }
            }
            var xform = _entityManager.GetComponent<TransformComponent>(user);
            var coordinates = xform.Coordinates;
            var spawnedItem = _entityManager.SpawnEntity(component.SpawnPrototype, coordinates);
            component.SpawnedItem = spawnedItem;
            if (!string.IsNullOrEmpty(component.SpawnSlot))
            {
                if (component.SpawnSlot == "active-hand")
                {
                    _handsSystem.TryPickup(user, spawnedItem);
                }
                else
                {
                    _inventorySystem.TryEquip(user, spawnedItem, component.SpawnSlot);
                }
            }
        }
    }
}
