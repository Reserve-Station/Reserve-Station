// SPDX-License-Identifier: AGPL-3.0-or-later

namespace Content.Shared.WashingMachine.Events;

public sealed partial class WashingMachineIsBeingWashed : EntityEventArgs
{
    public EntityUid WashingMachine;
    public HashSet<EntityUid> Items;

    public WashingMachineIsBeingWashed(EntityUid washingMachine, HashSet<EntityUid> items)
    {
        WashingMachine = washingMachine;
        Items = items;
    }
}
