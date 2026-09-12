// SPDX-License-Identifier: MIT

using Robust.Shared.Serialization;

namespace Content.Shared._Reserve.VendingMachines;

[Serializable, NetSerializable]
public enum VendingMachineKeypadSound : byte
{
    Beep,
    Success,
    Error,
    Timeout
}

[Serializable, NetSerializable]
public sealed class VendingMachineKeypadAudioMessage(VendingMachineKeypadSound soundType, float pitch = 1f)
    : BoundUserInterfaceMessage
{
    public readonly VendingMachineKeypadSound SoundType = soundType;
    public readonly float Pitch = pitch;
}
