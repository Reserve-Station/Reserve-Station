// SPDX-FileCopyrightText: 2026 Space Station 14 Contributors
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Shared.Eui;
using Robust.Shared.Serialization;

namespace Content.Shared._Reserve.Inventory.UI;

[Serializable, NetSerializable]
public sealed class InventoryEuiState : EuiStateBase
{
    public bool IsLoading { get; init; }
    public List<InventoryItemData>? Items { get; init; }
    public string? ErrorMessage { get; init; }
}

[Serializable, NetSerializable]
public sealed record InventoryItemData(
    string ItemId,
    string ItemName,
    string? Description,
    int Rarity,
    bool CanBeUsedIngame,
    string? IconPath = null
);

public static class InventoryEuiMsg
{
    [Serializable, NetSerializable]
    public sealed class Close : EuiMessageBase
    {
    }

    [Serializable, NetSerializable]
    public sealed class UseItem : EuiMessageBase
    {
        public string ItemId = string.Empty;
    }
}
