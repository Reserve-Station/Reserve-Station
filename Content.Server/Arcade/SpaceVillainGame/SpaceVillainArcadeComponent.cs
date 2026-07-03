// SPDX-FileCopyrightText: 2023 DrSmugleaf <DrSmugleaf@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 Leon Friedrich <60421075+ElectroJr@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 TemporalOroboros <TemporalOroboros@gmail.com>
// SPDX-FileCopyrightText: 2023 lzk <124214523+lzk228@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Tayrtahn <tayrtahn@gmail.com>
// SPDX-FileCopyrightText: 2025 Aiden <28298836+Aidenkrz@users.noreply.github.com>
//
// SPDX-License-Identifier: MIT

using Content.Shared.Arcade;
using Robust.Shared.Audio;
using Robust.Shared.Localization; // Reserve edit: space-vilian-fix
using Robust.Shared.Prototypes;

namespace Content.Server.Arcade.SpaceVillain;

[RegisterComponent]
public sealed partial class SpaceVillainArcadeComponent : SharedSpaceVillainArcadeComponent
{
    /// <summary>
    /// Unused flag that can be hacked via wires.
    /// Name suggests that it was intended to either make the health/mana values underflow while playing the game or turn the arcade machine into an infinite prize fountain.
    /// </summary>
    [ViewVariables]
    public bool OverflowFlag;

    /// <summary>
    /// The current session of the SpaceVillain game for this arcade machine.
    /// </summary>
    [ViewVariables]
    public SpaceVillainGame? Game = null;

    /// <summary>
    /// The sound played when a new session of the SpaceVillain game is begun.
    /// </summary>
    [DataField("newGameSound")]
    public SoundSpecifier NewGameSound = new SoundPathSpecifier("/Audio/Effects/Arcade/newgame.ogg");

    /// <summary>
    /// The sound played when the player chooses to attack.
    /// </summary>
    [DataField("playerAttackSound")]
    public SoundSpecifier PlayerAttackSound = new SoundPathSpecifier("/Audio/Effects/Arcade/player_attack.ogg");

    /// <summary>
    /// The sound played when the player chooses to heal.
    /// </summary>
    [DataField("playerHealSound")]
    public SoundSpecifier PlayerHealSound = new SoundPathSpecifier("/Audio/Effects/Arcade/player_heal.ogg");

    /// <summary>
    /// The sound played when the player chooses to regain mana.
    /// </summary>
    [DataField("playerChargeSound")]
    public SoundSpecifier PlayerChargeSound = new SoundPathSpecifier("/Audio/Effects/Arcade/player_charge.ogg");

    /// <summary>
    /// The sound played when the player wins.
    /// </summary>
    [DataField("winSound")]
    public SoundSpecifier WinSound = new SoundPathSpecifier("/Audio/Effects/Arcade/win.ogg");

    /// <summary>
    /// The sound played when the player loses.
    /// </summary>
    [DataField("gameOverSound")]
    public SoundSpecifier GameOverSound = new SoundPathSpecifier("/Audio/Effects/Arcade/gameover.ogg");

    /// <summary>
    /// The prefixes that can be used to create the game name.
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite)]
    [DataField("possibleFightVerbs")]
    // Reserve edit start: space-vilian-fix
    public List<LocId> PossibleFightVerbs = new()
    {
        "space-villain-game-fight-verb-defeat",
        "space-villain-game-fight-verb-annihilate",
        "space-villain-game-fight-verb-save",
        "space-villain-game-fight-verb-strike",
        "space-villain-game-fight-verb-stop",
        "space-villain-game-fight-verb-destroy",
        "space-villain-game-fight-verb-robust",
        "space-villain-game-fight-verb-romance",
        "space-villain-game-fight-verb-pwn",
        "space-villain-game-fight-verb-own",
    };

    /// <summary>
    /// The first names/titles that can be used to construct the name of the villain.
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite)]
    [DataField("possibleFirstEnemyNames")]
    public List<LocId> PossibleFirstEnemyNames = new()
    {
        "space-villain-game-enemy-first-the-automatic",
        "space-villain-game-enemy-first-farmer",
        "space-villain-game-enemy-first-lord",
        "space-villain-game-enemy-first-professor",
        "space-villain-game-enemy-first-the-cuban",
        "space-villain-game-enemy-first-the-evil",
        "space-villain-game-enemy-first-the-dread-king",
        "space-villain-game-enemy-first-the-space",
        "space-villain-game-enemy-first-the-great",
        "space-villain-game-enemy-first-duke",
        "space-villain-game-enemy-first-general",
    };

    /// <summary>
    /// The last names that can be used to construct the name of the villain.
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite)]
    [DataField("possibleLastEnemyNames")]
    public List<LocId> PossibleLastEnemyNames = new()
    {
        "space-villain-game-enemy-last-melonoid",
        "space-villain-game-enemy-last-murdertron",
        "space-villain-game-enemy-last-sorcerer",
        "space-villain-game-enemy-last-ruin",
        "space-villain-game-enemy-last-jeff",
        "space-villain-game-enemy-last-ectoplasm",
        "space-villain-game-enemy-last-crushulon",
        "space-villain-game-enemy-last-uhangoid",
        "space-villain-game-enemy-last-vhakoid",
        "space-villain-game-enemy-last-peteoid",
        "space-villain-game-enemy-last-slime",
        "space-villain-game-enemy-last-griefer",
        "space-villain-game-enemy-last-erper",
        "space-villain-game-enemy-last-lizard-man",
        "space-villain-game-enemy-last-unicorn",
    };
    // Reserve edit end: space-vilian-fix

    /// <summary>
    /// The prototypes that can be dispensed as a reward for winning the game.
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite)]
    [DataField]
    public List<EntProtoId> PossibleRewards = new();

    /// <summary>
    /// The minimum number of prizes the arcade machine can have.
    /// </summary>
    [DataField("rewardMinAmount")]
    public int RewardMinAmount;

    /// <summary>
    /// The maximum number of prizes the arcade machine can have.
    /// </summary>
    [DataField("rewardMaxAmount")]
    public int RewardMaxAmount;

    /// <summary>
    /// The remaining number of prizes the arcade machine can dispense.
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite)]
    public int RewardAmount = 0;
}