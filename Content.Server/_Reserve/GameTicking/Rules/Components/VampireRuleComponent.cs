// SPDX-License-Identifier: AGPL-3.0-or-later

using Robust.Shared.Prototypes;
namespace Content.Server._Reserve.GameTicking.Rules.Components;

[RegisterComponent]
public sealed partial class VampireRuleComponent : Component
{
    public readonly List<EntityUid> VampireMinds = new();

    public readonly List<ProtoId<EntityPrototype>> BaseObjectives = new()
    {
        "VampireKillRandomPersonObjective",
        "VampireDrainObjective"
    };

    public readonly List<ProtoId<EntityPrototype>> EscapeObjectives = new()
    {
        "VampireSurviveObjective",
        "VampireEscapeObjective"
    };

    public readonly List<ProtoId<EntityPrototype>> StealObjectives = new()
    {
        "CMOHyposprayVampireStealObjective",
        "RDHardsuitVampireStealObjective",
        "EnergyShotgunVampireStealObjective",
        "EnergyMagnumVampireStealObjective",
        "HOSWeaponVampireStealObjective",
        "MagbootsVampireStealObjective",
        "ClipboardVampireStealObjective",
        "CaptainIDVampireStealObjective",
        "CaptainJetpackVampireStealObjective",
        "CaptainGunVampireStealObjective",
        "VampireCMOHardsuitStealObjective",
        "VampireCEHardsuitStealObjective",
        "VampireHOSHardsuitStealObjective",
        "VampireQMHardsuitStealObjective",
        "VampireCaptainHardsuitStealObjective",
        "VampireNTRepHardsuitStealObjective",
        "VampireBSOHardsuitStealObjective",
        "VampireBSOWeaponStealObjective"
    };
}
