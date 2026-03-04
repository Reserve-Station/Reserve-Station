// SPDX-FileCopyrightText: 2026 Space Station 14 Contributors
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Shared.Eui;
using Robust.Shared.Serialization;

namespace Content.Shared._Reserve.Inventory.UI;

[Serializable, NetSerializable]
public sealed class CosmeticSelectionEuiState : EuiStateBase
{
    public List<string> ProtoIds { get; init; } = new();
}

[Serializable, NetSerializable]
public static class CosmeticSelectionEuiMsg
{
    [Serializable, NetSerializable]
    public sealed class SelectItem : EuiMessageBase
    {
        public string ProtoId { get; init; } = string.Empty;
    }

    [Serializable, NetSerializable]
    public sealed class Close : EuiMessageBase { }
}
