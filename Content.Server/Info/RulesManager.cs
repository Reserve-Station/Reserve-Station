// SPDX-License-Identifier: AGPL-3.0-or-later

using System.Net;
using Content.Server.Database;
using Content.Shared.CCVar;
using Content.Shared.Info;
using Robust.Shared.Configuration;
using Robust.Shared.Log;
using Robust.Shared.Network;

namespace Content.Server.Info;

public sealed class RulesManager
{
    [Dependency] private readonly IServerDbManager _dbManager = default!;
    [Dependency] private readonly INetManager _netManager = default!;
    [Dependency] private readonly IConfigurationManager _cfg = default!;

    private readonly ISawmill _sawmill = Logger.GetSawmill("rules"); // Reserve: new player guide

    private static DateTime LastValidReadTime => DateTime.UtcNow - TimeSpan.FromDays(60);

    public void Initialize()
    {
        _netManager.Connected += OnConnected;
        _netManager.RegisterNetMessage<SendRulesInformationMessage>();
        _netManager.RegisterNetMessage<RulesAcceptedMessage>(OnRulesAccepted);
    }

    private async void OnConnected(object? sender, NetChannelArgs e)
    {
         var isLocalhost = IPAddress.IsLoopback(e.Channel.RemoteEndPoint.Address) &&
                               _cfg.GetCVar(CCVars.RulesExemptLocal);

        // Reserve start: new player guide
        var hasCooldown = false;         
        var isFirstConnection = false;
        if (!isLocalhost)
        {
            var lastRead = await _dbManager.GetLastReadRules(e.Channel.UserId);
            hasCooldown = lastRead > LastValidReadTime;
            if (lastRead == null)
            {
                try
                {
                    var playTimes = await _dbManager.GetPlayTimes(e.Channel.UserId.UserId);
                    isFirstConnection = playTimes.Count == 0;
                }
                catch (Exception exception)
                {
                    _sawmill.Error($"Failed to check playtime for {e.Channel.UserId}: {exception}");
                }
            }
        }
        // Reserve end

        var showRulesMessage = new SendRulesInformationMessage
        {
            PopupTime = _cfg.GetCVar(CCVars.RulesWaitTime),
            CoreRules = _cfg.GetCVar(CCVars.RulesFile),
            ShouldShowRules = !isLocalhost && !hasCooldown,
            ShouldShowNewPlayerGuide = isFirstConnection, // Reserve: new player guide
        };
        _netManager.ServerSendMessage(showRulesMessage, e.Channel);
    }

    private async void OnRulesAccepted(RulesAcceptedMessage message)
    {
        var date = DateTime.UtcNow;
        await _dbManager.SetLastReadRules(message.MsgChannel.UserId, date);
    }
}