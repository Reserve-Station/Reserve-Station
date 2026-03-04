// SPDX-FileCopyrightText: 2025 Space Station 14 Contributors
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Robust.Shared.Configuration;

namespace Content.Server._Reserve.Vahter;

[CVarDefs]
public sealed class VahterCVars
{
    /// <summary>
    /// Включена ли система?
    /// </summary>
    public static readonly CVarDef<bool> Enabled =
        CVarDef.Create("vahter.enabled", false, CVar.SERVERONLY);

    /// <summary>
    /// Минимальное количество банвордов для наложения немоты.
    /// </summary>
    public static readonly CVarDef<int> MinimumForMute =
        CVarDef.Create("vahter.min_for_mute", 2, CVar.SERVERONLY);

    /// <summary>
    /// Продолжительность немоты в секундах.
    /// </summary>
    public static readonly CVarDef<int> TraitMuteDuration =
        CVarDef.Create("vahter.mute_duration", 60, CVar.SERVERONLY);

    /// <summary>
    /// Минимальное количество банвордов для запуска второго уровня.
    /// </summary>
    public static readonly CVarDef<int> MinimumForTorture =
        CVarDef.Create("vahter.min_for_torture", 3, CVar.SERVERONLY);

    /// <summary>
    /// Длительность стана в секундах на всех уровнях.
    /// </summary>
    public static readonly CVarDef<int> TortureStunDuration =
        CVarDef.Create("vahter.torture_stun_duration", 30, CVar.SERVERONLY);

    /// <summary>
    /// Минимальное количество банвордов для третьего уровня.
    /// </summary>
    public static readonly CVarDef<int> MinimumForExecution =
        CVarDef.Create("vahter.min_for_execution", 4, CVar.SERVERONLY);

    /// <summary>
    /// Показывать правила?
    /// </summary>
    public static readonly CVarDef<bool> ShowRules =
        CVarDef.Create("vahter.show_rules", false, CVar.SERVERONLY);
}
