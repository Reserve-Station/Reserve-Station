// SPDX-License-Identifier: AGPL-3.0-or-later

using Robust.Shared.Serialization;

namespace Content.Shared.WashingMachine;

[Serializable, NetSerializable]
public enum WashingMachineState : byte
{
    Idle,
    Washing,
    Broken
}
