// SPDX-FileCopyrightText: 2025 ReserveBot <211949879+ReserveBot@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Svarshik <96281939+lexaSvarshik@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System.Linq;
using Content.Server.Chat.Managers;
using Content.Server.Chat.Systems;
using Content.Server.GameTicking;
using Content.Goobstation.Maths.FixedPoint;
using Content.Server.GameTicking.Rules;
using Content.Server.KillTracking;
using Content.Server.Mind;
using Content.Server.Points;
using Content.Server.RoundEnd;
using Content.Server.Roles;
using Content.Server.Station.Systems;
using Content.Server._CorvaxNext.BattleRoyale.Components;
using Content.Server._CorvaxNext.BattleRoyale.Rules.Components;
using Robust.Shared.Audio;
using Content.Shared.Bed.Sleep;
using Content.Shared.CombatMode.Pacification;
using Content.Shared.Chat;
using Content.Shared.Eye.Blinding.Components;
using Content.Shared.GameTicking.Components;
using Content.Shared.Mind.Components;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Points;
using Content.Shared.Traits.Assorted;
using Content.Server.Traits.Assorted;
using Robust.Server.Player;
using Robust.Shared.Map;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Random;
using Robust.Shared.Timing;
using Robust.Shared.Utility;
using Robust.Shared.Enums;
using Robust.Shared.Prototypes;
using Robust.Shared.GameObjects;
using Content.Server.Parallax;
using Content.Shared.Shuttles.Components;
using Content.Shared.Parallax.Biomes;
using Robust.Shared.Map.Components;
using Content.Shared.Light.Components;
using Content.Shared.Station.Components;
using Content.Server.Clothing.Systems;

namespace Content.Server._CorvaxNext.BattleRoyale.Rules
{
    /// <summary>
    /// Battle Royale game mode where the last player standing wins.
    /// </summary>
    public sealed class BattleRoyaleRuleSystem : GameRuleSystem<BattleRoyaleRuleComponent>
    {
        [Dependency] private readonly IPlayerManager _player = default!;
        [Dependency] private readonly MindSystem _mind = default!;
        [Dependency] private readonly PointSystem _point = default!;
        [Dependency] private readonly RoundEndSystem _roundEnd = default!;
        [Dependency] private readonly MobStateSystem _mobState = default!;
        [Dependency] private readonly IChatManager _chatManager = default!;
        [Dependency] private readonly ChatSystem _chatSystem = default!;
        [Dependency] private readonly IRobustRandom _random = default!;
        [Dependency] private readonly StationSpawningSystem _stationSpawning = default!;
        [Dependency] private readonly RoleSystem _role = default!;
        [Dependency] private readonly SharedMapSystem _mapSystem = default!;
        [Dependency] private readonly BiomeSystem _biomes = default!;
        [Dependency] private readonly MetaDataSystem _metaData = default!;
        [Dependency] private readonly IEntityManager _entManager = default!;
        [Dependency] private readonly OutfitSystem _outfitSystem = default!;
        [Dependency] private readonly IPrototypeManager _protoManager = default!;

        private ISawmill _sawmill = default!;

        private const int MaxNormalCallouts = 60;
        private const int MaxEnvironmentalCallouts = 10;
        public Color MapLight = Color.FromHex("#D8B059");

        private readonly List<ProtoId<BiomeTemplatePrototype>> _biome = new()
        {
            "JustWater",
        };

        private EntityUid? _battleRoyaleStation;

        public override void Initialize()
        {
            base.Initialize();

            _sawmill = Logger.GetSawmill("BattleRoyale");

            SubscribeLocalEvent<RulePlayerSpawningEvent>(OnRulePlayerSpawning);
            SubscribeLocalEvent<MobStateChangedEvent>(OnMobStateChanged);
            SubscribeLocalEvent<KillReportedEvent>(OnKillReported);
            SubscribeLocalEvent<RefreshLateJoinAllowedEvent>(OnRefreshLateJoinAllowed);
            SubscribeLocalEvent<PlayerDetachedEvent>(OnPlayerDetached);
        }

        private void SetupStation(EntityUid stationUid, EntityUid gridUid, MapId mapId)
        {
            if (!_mapSystem.TryGetMap(mapId, out var mapUid))
            {
                _sawmill.Error($"Battle Royale: Map {mapUid} not found!");
                return;
            }

            _battleRoyaleStation = stationUid;

            _metaData.SetEntityName(mapUid.Value, "BATTLE ROYALE");

            EnsureComp<PreventPilotComponent>(gridUid);

            // Setup planet
            var bm = _random.Pick(_biome);
            _biomes.EnsurePlanet(mapUid.Value, _protoManager.Index(bm), null, null, mapLight: MapLight);

            // Add MapLightComponent
            var lighting = _entManager.EnsureComponent<MapLightComponent>(mapUid.Value);
            lighting.AmbientLightColor = MapLight;

            // Change ImplicitRoofComponent - See, this is hardcoded due to only reason that our main map for this rule is a planet
            if (TryComp<BattleRoyaleMapComponent>(stationUid, out var station))
                if (station.ClearImplicitRoofComponent)
                {
                    var roof = _entManager.EnsureComponent<ImplicitRoofComponent>(gridUid);
                    roof.Color = MapLight;
                }

            _sawmill.Info($"Battle Royale: Configured map {MetaData(mapUid.Value).EntityName}");

            _entManager.Dirty(mapUid.Value, lighting);
        }

        private void OnRefreshLateJoinAllowed(RefreshLateJoinAllowedEvent ev)
        {
            if (CheckBattleRoyaleActive())
            {
                ev.Disallow();
            }
        }

        private void OnRulePlayerSpawning(RulePlayerSpawningEvent ev)
        {
            var query = EntityQueryEnumerator<BattleRoyaleRuleComponent, GameRuleComponent>();
            while (query.MoveNext(out var uid, out var br, out var gameRule))
            {
                if (!GameTicker.IsGameRuleActive(uid, gameRule))
                    continue;

                if (_battleRoyaleStation == null || !Exists(_battleRoyaleStation.Value))
                {
                    var stationQuery = EntityQueryEnumerator<BattleRoyaleMapComponent>();
                    while (stationQuery.MoveNext(out var stationUid, out _))
                    {
                        if (TryComp<StationDataComponent>(stationUid, out var stationData) && stationData.Grids.Count > 0)
                        {
                            var gridUid = stationData.Grids.First();
                            if (TryComp<TransformComponent>(gridUid, out var xform))
                            {
                                var mapId = xform.MapID;
                                if (mapId != MapId.Nullspace)
                                {
                                    SetupStation(stationUid, gridUid, mapId);
                                    break;
                                }
                            }
                        }
                    }
                }

                // Only Battle Royale station should be used
                if (_battleRoyaleStation == null || !Exists(_battleRoyaleStation.Value))
                {
                    _sawmill.Error("Battle Royale: Station not found! Make sure the map is loaded.");
                    return;
                }

                var station = _battleRoyaleStation.Value;

                // Spawning player on that map
                foreach (var session in ev.PlayerPool.ToList())
                {
                    SpawnBattleRoyalePlayer(session, station, br);
                    _sawmill.Info($"Battle Royale: Spawning {session.Name} on map {station}");
                    ev.PlayerPool.Remove(session);
                }
            }
        }

        private void SpawnBattleRoyalePlayer(ICommonSession session, EntityUid station, BattleRoyaleRuleComponent br)
        {
            var profile = GameTicker.GetPlayerProfile(session);

            // mind
            var newMind = _mind.CreateMind(session.UserId, profile.Name);
            _mind.SetUserId(newMind, session.UserId);

            // Spawning mob
            var mobMaybe = _stationSpawning.SpawnPlayerCharacterOnStation(station, null, profile);
            if (mobMaybe == null)
            {
                _sawmill.Error($"Battle Royale: Failed to spawn player {session.Name}");
                return;
            }
            var mob = mobMaybe.Value;

            _mind.TransferTo(newMind, mob);

            _role.MindAddRole(newMind, br.Role);

            SetupBattleRoyalePlayer(mob, br);

            GameTicker.PlayerJoinGame(session);

            _sawmill.Info($"Battle Royale: Spawned player {session.Name} as {ToPrettyString(mob)}");
        }

        private void SetupBattleRoyalePlayer(EntityUid player, BattleRoyaleRuleComponent br)
        {
            _sawmill.Info($"Setting up Battle Royale player: {ToPrettyString(player)}");

            // Add required components
            EnsureComp<KillTrackerComponent>(player);
            EnsureComp<SleepingComponent>(player);

            _outfitSystem.SetOutfit(player, br.Gear);

            // Add pacification
            var pacified = EnsureComp<PacifiedComponent>(player);
            var removePacifiedTime = br.PacificationDuration;
            Timer.Spawn(removePacifiedTime, () =>
            {
                if (Deleted(player))
                    return;

                RemComp<PacifiedComponent>(player);
                _sawmill.Debug($"Removed pacification from {ToPrettyString(player)}");
            });

            // Add temporary blurry vision
            var blurry = EnsureComp<BlurryVisionComponent>(player);
            Timer.Spawn(TimeSpan.FromSeconds(15), () =>
            {
                if (Deleted(player))
                    return;

                RemComp<BlurryVisionComponent>(player);
                _sawmill.Debug($"Removed blurry vision from {ToPrettyString(player)}");
            });

            // Remove negative traits
            RemComp<PainNumbnessComponent>(player);
            RemComp<PermanentBlindnessComponent>(player);
            RemComp<NarcolepsyComponent>(player);

            _sawmill.Info($"Battle Royale player setup complete: {ToPrettyString(player)}");
        }

        private bool CheckBattleRoyaleActive()
        {
            var query = EntityQueryEnumerator<BattleRoyaleRuleComponent, ActiveGameRuleComponent>();
            return query.MoveNext(out _, out _, out _);
        }

        protected override void Started(EntityUid uid, BattleRoyaleRuleComponent component, GameRuleComponent gameRule, GameRuleStartedEvent args)
        {
            base.Started(uid, component, gameRule, args);

            _sawmill.Info($"Battle Royale rule started: {ToPrettyString(uid)}");

            Timer.Spawn(TimeSpan.FromSeconds(5), () =>
            {
                CheckLastManStanding(uid, component);
            });

            Timer.Spawn(TimeSpan.FromMinutes(2), () =>
            {
                if (!Exists(uid) || !TryComp<GameRuleComponent>(uid, out var gameRuleComp))
                    return;

                if (!GameTicker.IsGameRuleActive(uid, gameRuleComp))
                    return;

                var message = Loc.GetString("battle-royale-kill-or-be-killed");
                var title = Loc.GetString("battle-royale-title");

                var sound = new SoundPathSpecifier("/Audio/Ambience/Antag/traitor_start.ogg");

                _sawmill.Info("Dispatching Battle Royale announcement");
                _chatSystem.DispatchGlobalAnnouncement(message, title, true, sound, Color.Red);
            });
        }

        private void OnMobStateChanged(MobStateChangedEvent args)
        {
            if (args.NewMobState != MobState.Dead)
                return;

            var query = EntityQueryEnumerator<BattleRoyaleRuleComponent, GameRuleComponent>();
            while (query.MoveNext(out var uid, out var br, out var gameRule))
            {
                if (!GameTicker.IsGameRuleActive(uid, gameRule))
                    continue;

                CheckLastManStanding(uid, br);
            }
        }

        private void OnKillReported(ref KillReportedEvent ev)
        {
            var query = EntityQueryEnumerator<BattleRoyaleRuleComponent, PointManagerComponent, GameRuleComponent>();
            while (query.MoveNext(out var uid, out var br, out var point, out var gameRule))
            {
                if (!GameTicker.IsGameRuleActive(uid, gameRule))
                    continue;

                if (ev.Primary is KillPlayerSource player)
                {
                    _point.AdjustPointValue(player.PlayerId, 1, uid, point);
                }

                if (ev.Assist is KillPlayerSource assist)
                {
                    _point.AdjustPointValue(assist.PlayerId, 0.5f, uid, point);
                }

                SendKillCallout(uid, ref ev);
            }
        }

        private void SendKillCallout(EntityUid uid, ref KillReportedEvent ev)
        {
            if (ev.Primary is KillEnvironmentSource || ev.Suicide)
            {
                var calloutNumber = _random.Next(0, MaxEnvironmentalCallouts + 1);
                var calloutId = $"death-match-kill-callout-env-{calloutNumber}";
                var victimName = GetEntityName(ev.Entity);
                var message = Loc.GetString(calloutId, ("victim", victimName));
                _chatManager.ChatMessageToAll(ChatChannel.Server, message, message, uid, false, true, Color.OrangeRed);
                return;
            }

            string killerString;
            if (ev.Primary is KillPlayerSource primarySource)
            {
                var primaryName = GetPlayerName(primarySource.PlayerId);
                if (ev.Assist is KillPlayerSource assistSource)
                {
                    var assistName = GetPlayerName(assistSource.PlayerId);
                    killerString = Loc.GetString("death-match-assist", ("primary", primaryName), ("secondary", assistName));
                }
                else
                {
                    killerString = primaryName;
                }

                var calloutNumber = _random.Next(0, MaxNormalCallouts + 1);
                var calloutId = $"death-match-kill-callout-{calloutNumber}";
                var victimName = GetEntityName(ev.Entity);
                var message = Loc.GetString(calloutId, ("killer", killerString), ("victim", victimName));
                _chatManager.ChatMessageToAll(ChatChannel.Server, message, message, uid, false, true, Color.OrangeRed);
            }
            else if (ev.Primary is KillNpcSource npcSource)
            {
                killerString = GetEntityName(npcSource.NpcEnt);
                var calloutNumber = _random.Next(0, MaxNormalCallouts + 1);
                var calloutId = $"death-match-kill-callout-{calloutNumber}";
                var victimName = GetEntityName(ev.Entity);
                var message = Loc.GetString(calloutId, ("killer", killerString), ("victim", victimName));
                _chatManager.ChatMessageToAll(ChatChannel.Server, message, message, uid, false, true, Color.OrangeRed);
            }
        }

        private string GetPlayerName(NetUserId userId)
        {
            if (!_player.TryGetSessionById(userId, out var session))
                return "Unknown";

            if (session.AttachedEntity == null)
                return session.Name;

            return Loc.GetString("death-match-name-player",
                ("name", MetaData(session.AttachedEntity.Value).EntityName),
                ("username", session.Name));
        }

        private string GetEntityName(EntityUid entity)
        {
            if (TryComp<ActorComponent>(entity, out var actor))
            {
                return Loc.GetString("death-match-name-player",
                    ("name", MetaData(entity).EntityName),
                    ("username", actor.PlayerSession.Name));
            }

            return Loc.GetString("death-match-name-npc",
                ("name", MetaData(entity).EntityName));
        }

        private void CheckLastManStanding(EntityUid uid, BattleRoyaleRuleComponent component)
        {
            var alivePlayers = GetAlivePlayers();

            if (alivePlayers.Count == 1)
            {
                if (!component.WinnerAnnounced || component.Victor == null || component.Victor.Value != alivePlayers.First())
                {
                    component.Victor = alivePlayers.First();
                    if (!component.WinnerAnnounced && _mind.TryGetMind(component.Victor.Value, out var mindId, out var mind) &&
                    _player.TryGetSessionById(mind.UserId, out var session))
                    {
                        component.WinnerAnnounced = true;
                        var victorName = MetaData(component.Victor.Value).EntityName;
                        var playerName = session.Name ?? victorName;
                        if (Timing.CurTime < TimeSpan.FromSeconds(10))
                        {
                            _chatManager.DispatchServerAnnouncement(
                                Loc.GetString("battle-royale-single-player", ("player", playerName)));
                        }
                        else
                        {
                            _chatManager.DispatchServerAnnouncement(
                                Loc.GetString("battle-royale-winner-announcement", ("player", playerName)));
                        }
                        Timer.Spawn(component.RoundEndDelay, () =>
                        {
                            if (GameTicker.RunLevel == GameRunLevel.InRound)
                                _roundEnd.EndRound();
                        });
                    }
                }
            }
            else if (alivePlayers.Count == 0)
            {
                component.Victor = null;
                _roundEnd.EndRound();
            }
        }

        private void OnPlayerDetached(PlayerDetachedEvent ev)
        {
            var query = EntityQueryEnumerator<BattleRoyaleRuleComponent, GameRuleComponent>();
            while (query.MoveNext(out var uid, out var br, out var gameRule))
            {
                if (!GameTicker.IsGameRuleActive(uid, gameRule))
                    continue;
                CheckLastManStanding(uid, br);
            }
        }

        private List<EntityUid> GetAlivePlayers()
        {
            var result = new List<EntityUid>();
            var mobQuery = EntityQueryEnumerator<MobStateComponent, ActorComponent>();

            while (mobQuery.MoveNext(out var uid, out var mobState, out var actor))
            {
                if (HasComp<IsDeadICComponent>(uid))
                    continue;

                if (actor.PlayerSession?.Status != SessionStatus.Connected &&
                    actor.PlayerSession?.Status != SessionStatus.InGame)
                    continue;

                if (_mobState.IsAlive(uid, mobState))
                    result.Add(uid);
            }

            return result;
        }

        protected override void AppendRoundEndText(EntityUid uid,
            BattleRoyaleRuleComponent component,
            GameRuleComponent gameRule,
            ref RoundEndTextAppendEvent args)
        {
            if (!TryComp<PointManagerComponent>(uid, out var point))
                return;

            if (component.Victor != null && _mind.TryGetMind(component.Victor.Value, out var victorMindId, out var victorMind) &&
            _player.TryGetSessionById(victorMind.UserId, out var session))
            {
                var victorName = MetaData(component.Victor.Value).EntityName;
                var victorPlayerName = session.Name ?? victorName;
                args.AddLine(Loc.GetString("battle-royale-winner", ("player", victorPlayerName)));
                args.AddLine("");
            }

            args.AddLine(Loc.GetString("battle-royale-scoreboard-header"));
            args.AddLine(new FormattedMessage(point.Scoreboard).ToMarkup());
        }
    }
}
