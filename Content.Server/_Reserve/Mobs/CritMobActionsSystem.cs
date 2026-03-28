// SPDX-FileCopyrightText: 2026 Reserve Station
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Server.Chat.Systems;
using Content.Server.Popups;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Robust.Server.Console;
using Robust.Shared.Player;
using Content.Shared.Speech.Muting;
using Content.Shared.Chat;

namespace Content.Server._Reserve.Mobs;

public sealed partial class ReserveCritActionsSystem : EntitySystem
{
    [Dependency] private readonly ChatSystem _chat = default!;
    [Dependency] private readonly MobStateSystem _mobState = default!;
    [Dependency] private readonly PopupSystem _popupSystem = default!;
    [Dependency] private readonly IServerConsoleHost _host = default!;

    private static readonly string[] RageQuitPhrases =
    [
        "ragequit-phrase-1",
        "ragequit-phrase-2",
        "ragequit-phrase-3",
        "ragequit-phrase-4",
        "ragequit-phrase-5",
        "ragequit-phrase-6",
        "ragequit-phrase-7",
        "ragequit-phrase-8"
    ];

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<MobStateActionsComponent, CritRageQuitEvent>(OnRageQuit);
    }

    private void OnRageQuit(EntityUid uid, MobStateActionsComponent component, CritRageQuitEvent args)
    {
        if (!TryComp<ActorComponent>(uid, out var actor) || !_mobState.IsCritical(uid))
            return;

        if (HasComp<MutedComponent>(uid))
        {
            _popupSystem.PopupEntity(Loc.GetString("ragequit-muted"), uid, uid);
            return;
        }

        var phraseKey = RageQuitPhrases[Random.Shared.Next(RageQuitPhrases.Length)];
        var rageQuitMessage = Loc.GetString(phraseKey);
        _chat.TrySendInGameICMessage(uid, rageQuitMessage, InGameICChatType.Speak, ChatTransmitRange.Normal, checkRadioPrefix: false, ignoreActionBlocker: true);
        _host.ExecuteCommand(actor.PlayerSession, "ghost");
        args.Handled = true;
    }
}
