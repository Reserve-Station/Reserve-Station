// SPDX-FileCopyrightText: 2025 ReserveBot <211949879+ReserveBot@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Svarshik <96281939+lexaSvarshik@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Shared.Roles;
using Content.Goobstation.Maths.FixedPoint;
using Content.Shared.Storage;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.Map;

namespace Content.Server._CorvaxNext.BattleRoyale.Rules.Components;

[RegisterComponent, Access(typeof(BattleRoyaleRuleSystem))]
public sealed partial class BattleRoyaleRuleComponent : Component
{
    [DataField("gear", customTypeSerializer: typeof(PrototypeIdSerializer<StartingGearPrototype>)), ViewVariables(VVAccess.ReadWrite)]
    public string Gear = "BattleRoyaleGear";

    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public TimeSpan RoundEndDelay = TimeSpan.FromSeconds(10f);

    [DataField]
    public EntityUid? Victor;

    [DataField, AutoNetworkedField, ViewVariables(VVAccess.ReadWrite)]
    public bool WinnerAnnounced = false;

    [DataField]
    public MapId? MapId;

    [DataField]
    public List<EntityUid> SpawnPoints = new();

    [DataField]
    public TimeSpan PacificationDuration = TimeSpan.FromMinutes(2);

    /// <summary>
    /// The role prototype ID to assign to all Battle Royale players.
    /// </summary>
    [DataField("role")]
    public string Role = "BattleRoyaleFighter";

    /// <summary>
    /// The map prototype ID to use for Battle Royale. If null, uses the current map.
    /// </summary>
    [DataField("mapPrototype")]
    public string? MapPrototype;
}
