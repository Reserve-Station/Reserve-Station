// SPDX-FileCopyrightText: 2025 Space Station 14 Contributors
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System.Text.RegularExpressions;
using Content.Goobstation.Shared.MisandryBox.Smites;
using Content.Server.Body.Systems;
using Content.Server.Chat.Systems;
using Content.Server.Explosion.EntitySystems;
using Content.Server.Medical;
using Content.Server.Body.Components;
using Content.Server.Chat.Managers;
using Content.Shared._Reserve.Vahter.Chat;
using Content.Shared.Body.Part;
using Content.Shared.Chat;
using Content.Shared.Popups;
using Content.Shared.CCVar;
using Content.Shared.Info;
using Content.Shared.Speech.Muting;
using Content.Shared.StatusEffect;
using Content.Shared.Stunnable;
using Robust.Server.Player;
using Robust.Shared.Configuration;
using Robust.Shared.Network;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Timer = Robust.Shared.Timing.Timer;
using Content.Shared._Shitmed.Body.Organ;
using Content.Shared.Body.Components;

namespace Content.Server._Reserve.Vahter.Chat;

public sealed class BanwordFilterSystem : EntitySystem
{
    [Dependency] private readonly IConfigurationManager _cfg = default!;
    [Dependency] private readonly IPrototypeManager _proto = default!;
    [Dependency] private readonly IPlayerManager _playerManager = default!;
    [Dependency] private readonly SharedPopupSystem _popup = default!;
    [Dependency] private readonly ILogManager _logManager = default!;
    [Dependency] private readonly StatusEffectsSystem _statusEffect = default!;
    [Dependency] private readonly BodySystem _bodySystem = default!;
    [Dependency] private readonly IChatManager _chatManager = default!;
    [Dependency] private readonly SharedTransformSystem _transformSystem = default!;
    [Dependency] private readonly SharedStunSystem _stun = default!;
    [Dependency] private readonly VomitSystem _vomit = default!;
    [Dependency] private readonly ExplosionSystem _explosionSystem = default!;
    [Dependency] private readonly BloodstreamSystem _bloodstreamSystem = default!;
    [Dependency] private readonly INetManager _net = default!;

    private ChatSystem Chat => _chat ??= EntityManager.System<ChatSystem>();
    private ChatSystem? _chat;
    private ISawmill _sawmill = default!;

    private readonly HashSet<string> _words = new();
    private bool _cached;

    private bool _enabled;
    private int _minimumForMute;
    private int _muteDuration;
    private int _minimumForTorture;
    private int _tortureStunDuration;
    private int _minimumForExecution;

    public override void Initialize()
    {
        base.Initialize();

        _cfg.OnValueChanged(VahterCVars.Enabled, v => _enabled = v, true);
        _cfg.OnValueChanged(VahterCVars.MinimumForMute, v => _minimumForMute = v, true);
        _cfg.OnValueChanged(VahterCVars.TraitMuteDuration, v => _muteDuration = v, true);
        _cfg.OnValueChanged(VahterCVars.MinimumForTorture, v => _minimumForTorture = v, true);
        _cfg.OnValueChanged(VahterCVars.TortureStunDuration, v => _tortureStunDuration = v, true);
        _cfg.OnValueChanged(VahterCVars.MinimumForExecution, v => _minimumForExecution = v, true);

        _proto.PrototypesReloaded += _ => CacheWords();
        _sawmill = _logManager.GetSawmill("vahter");
    }

    private void CacheWords()
    {
        _words.Clear();
        foreach (var proto in _proto.EnumeratePrototypes<BanwordListPrototype>())
        {
            foreach (var word in proto.Words)
                _words.Add(word.ToLowerInvariant());
        }

        _cached = true;
    }

    /// <summary>
    /// Проверяет сообщение на наличие банвордов.
    /// Если найдено - считает нарушения и применяет наказания.
    /// Возвращает true, если сообщение нужно заблокировать.
    /// </summary>
    public bool CheckMessage(string message, EntityUid source)
    {
        if (!_enabled || string.IsNullOrEmpty(message))
            return false;

        if (!_cached)
            CacheWords();

        var foundCount = 0;
        foreach (var token in Regex.Split(message, @"[^\p{L}\d]+"))
        {
            if (token.Length > 0 && _words.Contains(token.ToLowerInvariant()))
                foundCount++;
        }

        if (foundCount == 0)
            return false;

        if (!_playerManager.TryGetSessionByEntity(source, out var session) || session == null)
            return true;

        if (foundCount >= _minimumForExecution)
        {
            _statusEffect.TryAddStatusEffect<MutedComponent>( // суки спамят
                source,
                "Muted",
                TimeSpan.FromSeconds(60),
                true);
            ApplyExecution(source, session);
            ShowRules(session);
        }
        else if (foundCount >= _minimumForTorture)
        {
            _statusEffect.TryAddStatusEffect<MutedComponent>( // суки спамят
                source,
                "Muted",
                TimeSpan.FromSeconds(60),
                true);
            ApplyTorture(source, session);
        }
        else if (foundCount >= _minimumForMute)
            ApplyMute(source, session);

        _sawmill.Info(
            $"{session.Name} ({session.UserId}) отправил сообщение с банвордами (всего {foundCount} слов)");
        _chatManager.SendAdminAnnouncement(Loc.GetString("banword-admin-warning",
            ("playerName", session.Name),
            ("userId", session.UserId),
            ("count", foundCount)));

        return true;
    }

    private void VomitOrgans(EntityUid source)
    {
        if (TryComp<BloodstreamComponent>(source, out var bloodstream))
            _bloodstreamSystem.SpillAllSolutions((source, bloodstream));

        if (!TryComp<BodyComponent>(source, out var body))
            return;

        var baseXform = Transform(source);
        foreach (var organ in _bodySystem.GetBodyOrganEntityComps<TransformComponent>((source, body)))
        {
            if (HasComp<BrainComponent>(organ.Owner) || HasComp<EyeComponent>(organ.Owner) ||
                HasComp<StomachComponent>(organ.Owner) || HasComp<HeartComponent>(organ.Owner))
                continue;

            _transformSystem.PlaceNextTo((organ.Owner, organ.Comp1), (source, baseXform));
        }
    }

    private void ShowRules(ICommonSession session)
    {
        if (!_cfg.GetCVar(VahterCVars.ShowRules))
            return;
        var seconds = _cfg.GetCVar(CCVars.RulesWaitTime);
        var coreRules = _cfg.GetCVar(CCVars.RulesFile);
        var message = new SendRulesInformationMessage
            { PopupTime = seconds, CoreRules = coreRules, ShouldShowRules = true };
        _net.ServerSendMessage(message, session.Channel);
    }

    private void ApplyMute(EntityUid source, ICommonSession session)
    {
        _popup.PopupEntity(Loc.GetString("banword-filter-popup-level-one"), source, session, PopupType.Large);
        _statusEffect.TryAddStatusEffect<MutedComponent>(
            source,
            "Muted",
            TimeSpan.FromSeconds(_muteDuration),
            true);
    }

    private void ApplyTorture(EntityUid source, ICommonSession session)
    {
        _popup.PopupEntity(Loc.GetString("banword-filter-popup-level-two"), source, session, PopupType.Large);

        _stun.TryUpdateStunDuration(source, TimeSpan.FromSeconds(_tortureStunDuration));
        _stun.TryKnockdown(source, TimeSpan.FromSeconds(_tortureStunDuration));
        Chat.TrySendInGameICMessage(
            source,
            Loc.GetString("banword-filter-torture-me-action-one"),
            InGameICChatType.Emote,
            ChatTransmitRange.Normal,
            player: session,
            ignoreActionBlocker: true,
            forced: true);

        var netSource = GetNetEntity(source);
        Timer.Spawn(TimeSpan.FromSeconds(3),
            () =>
            {
                var ent = GetEntity(netSource);
                if (!Exists(ent))
                    return;

                if (_playerManager.TryGetSessionByEntity(ent, out var sess) && sess != null)
                {
                    Chat.TrySendInGameICMessage(
                        ent,
                        Loc.GetString("banword-filter-torture-me-action-two"),
                        InGameICChatType.Emote,
                        ChatTransmitRange.Normal,
                        player: sess,
                        ignoreActionBlocker: true,
                        forced: true);
                }

                VomitOrgans(ent);
            });
    }

    private void ApplyExecution(EntityUid source, ICommonSession session)
    {
        _popup.PopupEntity(Loc.GetString("banword-filter-popup-level-three"), source, session, PopupType.Large);
        _stun.TryUpdateStunDuration(source, TimeSpan.FromSeconds(_tortureStunDuration));
        _stun.TryKnockdown(source, TimeSpan.FromSeconds(_tortureStunDuration));

        Chat.TrySendInGameICMessage(
            source,
            Loc.GetString("banword-filter-torture-me-action-one"),
            InGameICChatType.Emote,
            ChatTransmitRange.Normal,
            player: session,
            ignoreActionBlocker: true,
            forced: true);

        foreach (var part in _bodySystem.GetBodyChildrenOfType(source, BodyPartType.Hand))
        {
            _transformSystem.AttachToGridOrMap(part.Id);
        }

        var netSource = GetNetEntity(source);
        Timer.Spawn(TimeSpan.FromSeconds(3),
            () =>
            {
                var ent = GetEntity(netSource);
                if (!Exists(ent))
                    return;

                if (_playerManager.TryGetSessionByEntity(ent, out var sess) && sess != null)
                {
                    Chat.TrySendInGameICMessage(
                        source,
                        Loc.GetString("banword-filter-torture-me-action-two"),
                        InGameICChatType.Emote,
                        ChatTransmitRange.Normal,
                        player: session,
                        ignoreActionBlocker: true,
                        forced: true);
                }

                VomitOrgans(ent);

                var ogun = EntityManager.System<ThunderstrikeSystem>();
                ogun.Smite(ent, kill: true);
            });
    }
}
