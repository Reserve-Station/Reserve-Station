// SPDX-FileCopyrightText: 2026 Space Station 14 Contributors
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Shared.Eui;
using Robust.Shared.Serialization;

namespace Content.Shared._Reserve.Inventory.UI;

[Serializable, NetSerializable]
public sealed class AntagSelectionEuiState : EuiStateBase
{
    public List<AntagRuleEntry> Rules { get; init; } = new();
}

[Serializable, NetSerializable]
public sealed record AntagRuleEntry(string RuleId, string DisplayName, bool ForAlive);

public static class AntagSelectionEuiMsg
{
    [Serializable, NetSerializable]
    public sealed class Close : EuiMessageBase
    {
    }

    [Serializable, NetSerializable]
    public sealed class SelectRule : EuiMessageBase
    {
        public string RuleId = string.Empty;
    }
}
