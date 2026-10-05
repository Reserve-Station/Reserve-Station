// SPDX-FileCopyrightText: 2025 Space Station 14 Contributors
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System.Linq;
using Content.Shared._Orion.ServerProtection.Chat;
using Content.Shared.Administration.Managers;
using Content.Shared.CCVar;
using Robust.Server.Player;
using Robust.Shared.Configuration;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;

namespace Content.Server._Orion.ServerProtection.Chat;

//
// License-Identifier: AGPL-3.0-or-later
//

public sealed class ChatProtectionSystem : EntitySystem
{
    [Dependency] private readonly IConfigurationManager _cfg = default!;
    [Dependency] private readonly IPrototypeManager _proto = default!;
    [Dependency] private readonly IPlayerManager _playerManager = default!;
    [Dependency] private readonly ISharedAdminManager _admin = default!;
    [Dependency] private readonly ServerProtectionPunishmentSystem _punishment = default!;

    private ISawmill _log = default!;
    private readonly List<ChatProtectionListPrototype> _index = new();
    private readonly HashSet<string> _icWords = new();
    private readonly HashSet<string> _oocWords = new();

    private bool _protectionEnabled;
    private bool _eraseEnabled;
    private bool _banEnabled;
    private bool _kickEnabled;
    private bool _deleteMessagesEnabled;
    private int _banDuration;
    private bool _cacheDone;

    public override void Initialize()
    {
        base.Initialize();

        _log = Logger.GetSawmill("serverprotection.chat_protection");
        _proto.PrototypesReloaded += OnPrototypesReloaded;

        _cfg.OnValueChanged(CCVars.ChatProtectionEnabled, value => _protectionEnabled = value, true);
        _cfg.OnValueChanged(CCVars.ChatProtectionBanEnabled, value => _banEnabled = value, true);
        _cfg.OnValueChanged(CCVars.ChatProtectionKickEnabled, value => _kickEnabled = value, true);
        _cfg.OnValueChanged(CCVars.ChatProtectionEraseEnabled, value => _eraseEnabled = value, true);
        _cfg.OnValueChanged(CCVars.ChatProtectionDeleteMessages, value => _deleteMessagesEnabled = value, true);
        _cfg.OnValueChanged(CCVars.ChatProtectionBanDuration, value => _banDuration = value, true);
    }

    private void CachePrototypes()
    {
        _index.Clear();
        _icWords.Clear();
        _oocWords.Clear();

        foreach (var proto in _proto.EnumeratePrototypes<ChatProtectionListPrototype>())
        {
            _index.Add(proto);

            switch (proto.ID) // Handled by "Prototypes/_Orion/chat_protection.yml"
            {
                case "IC_BannedWords":
                    foreach (var word in proto.Words)
                    {
                        _icWords.Add(word);
                    }

                    break;

                case "OOC_BannedWords":
                    foreach (var word in proto.Words)
                    {
                        _oocWords.Add(word);
                    }

                    break;
            }
        }

        _log.Info($"Кэшировано {_icWords.Count} IC и {_oocWords.Count} OOC запрещённых слов.");
    }

    private void OnPrototypesReloaded(PrototypesReloadedEventArgs args)
    {
        CachePrototypes();
    }

    public bool CheckICMessage(string message, EntityUid player)
    {
        if (!_protectionEnabled || string.IsNullOrEmpty(message))
            return false;

        if (!TryGetSession(player, out var session))
            return false;

        if (session == null)
            return false;

        if (_admin.IsAdmin(player, true))
           return false;

        if (!_cacheDone)
            CachePrototypes();

        foreach (var word in _icWords)
        {
            if (!message.Contains(word, StringComparison.OrdinalIgnoreCase))
                continue;

            HandleViolation(session, word, "IC");
            return true;
        }

        _cacheDone = true;

        return false;
    }

    public bool CheckOOCMessage(string message, ICommonSession session)
    {
        if (!_protectionEnabled || string.IsNullOrEmpty(message))
            return false;

        if (_admin.IsAdmin(session, true))
            return false;

        if (!_cacheDone)
            CachePrototypes();

        foreach (var word in _oocWords)
        {
            if (!message.Contains(word, StringComparison.OrdinalIgnoreCase))
                continue;

            HandleViolation(session, word, "OOC");
            return true;
        }

        _cacheDone = true;

        return false;
    }

    private bool TryGetSession(EntityUid player, out ICommonSession? session)
    {
        session = null;
        return _playerManager.TryGetSessionByEntity(player, out session);
    }

    private void HandleViolation(ICommonSession player, string word, string channel)
    {
        var banReason = Loc.GetString("chat-protection-ban-reason", ("word", word), ("channel", channel));
        var kickReason = Loc.GetString("chat-protection-kick-reason", ("word", word), ("channel", channel));
        _log.Info($"{player.Name} ({player.UserId}) использовал запрещённое слово: '{word}' в {channel}");

        if (_deleteMessagesEnabled)
            _punishment.DeleteMessages(player);

        if (channel == "IC" && _eraseEnabled)
            _punishment.EraseCharacter(player);

        if (_banEnabled)
        {
            _punishment.SendAdminAlert(Loc.GetString("chat-protection-admin-announcement-ban-reason",
                ("player", player.Name),
                ("word", word),
                ("channel", channel)));
            _punishment.ApplyBan(player, banReason, _banDuration);
        }
        else if (_kickEnabled)
        {
            _punishment.SendAdminAlert(Loc.GetString("chat-protection-admin-announcement-kick-reason",
                ("player", player.Name),
                ("word", word),
                ("channel", channel)));
            _punishment.KickPlayer(player, kickReason);
        }
    }
}
