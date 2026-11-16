// SPDX-License-Identifier: AGPL-3.0-or-later

namespace Content.Shared.WashingMachine.Events;

public sealed partial class WashingMachineWashedEvent : EntityEventArgs
{
    public EntityUid WashingMachine;
    public HashSet<EntityUid> Items;

    public WashingMachineWashedEvent(EntityUid washingMachine, HashSet<EntityUid> items)
    {
        WashingMachine = washingMachine;
        Items = items;
    }
}
