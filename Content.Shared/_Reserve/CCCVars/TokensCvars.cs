// SPDX-FileCopyrightText: 2026 Space Station 14 Contributors
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Robust.Shared.Configuration;

namespace Content.Shared._Reserve.TokenCvars;

[CVarDefs]
// ReSharper disable once InconsistentNaming
public sealed class TokenCvars
{
    public static readonly CVarDef<float> LowTierTokenChance =
        CVarDef.Create("low_tier.chance", 0.9f, CVar.SERVERONLY);

    public static readonly CVarDef<int> LowTierTokenMinAlive =
        CVarDef.Create("low_tier.min_alive", 10, CVar.SERVERONLY);

    public static readonly CVarDef<int> LowTierTokenMaxAntagAlive =
        CVarDef.Create("low_tier.max_antag_alive", 5, CVar.SERVERONLY);

    public static readonly CVarDef<int> LowTierTokenMinSecAlive =
        CVarDef.Create("low_tier.min_sec_alive", 1, CVar.SERVERONLY);


    public static readonly CVarDef<float> GhostTierTokenChance =
        CVarDef.Create("ghost_tier.chance", 0.70f, CVar.SERVERONLY);

    public static readonly CVarDef<int> GhostTierTokenMinAlive =
        CVarDef.Create("ghost_tier.min_alive", 15, CVar.SERVERONLY);

    public static readonly CVarDef<int> GhostTierTokenMaxAntagAlive =
        CVarDef.Create("ghost_tier.max_antag_alive", 3, CVar.SERVERONLY);

    public static readonly CVarDef<int> GhostTierTokenMinSecAlive =
        CVarDef.Create("ghost_tier.min_sec_alive", 2, CVar.SERVERONLY);


    public static readonly CVarDef<float> MidTierTokenChance =
        CVarDef.Create("mid_tier.chance", 0.65f, CVar.SERVERONLY);

    public static readonly CVarDef<int> MidTierTokenMinAlive =
        CVarDef.Create("mid_tier.min_alive", 20, CVar.SERVERONLY);

    public static readonly CVarDef<int> MidTierTokenMinSecAlive =
        CVarDef.Create("mid_tier.min_sec_alive", 3, CVar.SERVERONLY);

    public static readonly CVarDef<int> MidTierTokenMaxAntagAlive =
        CVarDef.Create("mid_tier.max_antag_alive", 4, CVar.SERVERONLY);


    public static readonly CVarDef<float> HighTierTokenChance =
        CVarDef.Create("high_tier.chance", 0.5f, CVar.SERVERONLY);

    public static readonly CVarDef<int> HighTierTokenMinAlive =
        CVarDef.Create("high_tier.min_alive", 35, CVar.SERVERONLY);

    public static readonly CVarDef<int> HighTierTokenMinSecAlive =
        CVarDef.Create("high_tier.min_sec_alive", 4, CVar.SERVERONLY);

    public static readonly CVarDef<int> HighTierTokenMaxAntagAlive =
        CVarDef.Create("high_tier.max_antag_alive", 3, CVar.SERVERONLY);
}
