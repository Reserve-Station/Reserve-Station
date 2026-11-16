// SPDX-License-Identifier: AGPL-3.0-or-later

using Robust.Shared.Serialization;

namespace Content.Shared._Reserve.Antags.Vampires;

/// <summary>
/// Visual layers for DrunkBlood alert digits
/// </summary>
public enum VampireVisualLayers : byte
{
    Digit1,
    Digit2,
    Digit3,
    Digit4,
}

/// <summary>
/// Appearance data for the extended fangs drawn on the vampire's face
/// </summary>
[Serializable, NetSerializable]
public enum VampireFangsVisuals : byte
{
    Extended,
    Layer,
}

[Serializable, NetSerializable]
public sealed partial class VampireClassClosedBuiMsg : BoundUserInterfaceMessage { }

[Serializable, NetSerializable]
public enum VampireClassUiKey : byte
{
    Key
}

[Serializable, NetSerializable]
public enum VampireLocateUiKey : byte
{
    Key
}

[Serializable, NetSerializable]
public sealed class VampireClassChosenBuiMsg : BoundUserInterfaceMessage
{
    public string Choice { get; init; } = string.Empty;
}

[Serializable, NetSerializable]
public readonly record struct VampireLocateTarget(NetEntity Target, string DisplayName);

[Serializable, NetSerializable]
public sealed partial class VampireLocateBuiState : BoundUserInterfaceState
{
    public List<VampireLocateTarget> Targets { get; init; } = new();
}

[Serializable, NetSerializable]
public sealed partial class VampireLocateSelectedBuiMsg : BoundUserInterfaceMessage
{
    public NetEntity Target { get; init; }
}
