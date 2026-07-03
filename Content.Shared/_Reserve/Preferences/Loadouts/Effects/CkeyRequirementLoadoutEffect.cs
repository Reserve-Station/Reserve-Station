// SPDX-FileCopyrightText: 2026 Ceterai <ceterai@protonmail.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System.Diagnostics.CodeAnalysis;
using Robust.Shared.Player;
using Robust.Shared.Utility;

namespace Content.Shared.Preferences.Loadouts.Effects;

/// <summary>
/// Checks for a Ckey match.
/// </summary>
public sealed partial class CkeyRequirementLoadoutEffect : LoadoutEffect
{
    [DataField(required: true)]
    public List<string> Ckey = new();

    public override bool Validate(HumanoidCharacterProfile profile, RoleLoadout loadout, ICommonSession? session, IDependencyCollection collection, [NotNullWhen(false)] out FormattedMessage? reason)
    {
        reason = FormattedMessage.FromUnformatted(Loc.GetString("loadout-group-unique-reserve-items-restriction"));

        if (session == null)
            return false;

        var name = session.Name.ToLower();
        if (name.StartsWith("localhost@"))
            name = name[10..];

        foreach (var testCkey in Ckey)
            if (testCkey.ToLower() == name)
            {
                reason = null;
                return true;
            }

        return false;
    }
}
