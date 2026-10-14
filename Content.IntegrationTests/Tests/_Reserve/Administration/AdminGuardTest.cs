// SPDX-License-Identifier: AGPL-3.0-or-later

using System.Collections.Generic;
using Content.Server._Reserve.Discord.AdminGuard;
using Content.Server._Reserve.Discord.Logger;
using Content.Server.Database;
using Content.Shared.Administration;
using Content.Shared.CCVar;
using Robust.Shared.Configuration;
using Robust.Shared.Console;
using Robust.Shared.Log;
using Robust.Shared.Network;

namespace Content.IntegrationTests.Tests.Administration;

/// <summary>
///     Reserve Red - AdminGuard: события админов должны уходить в Discord независимо от настроек
///     и не ломать сами действия. Любая ошибка внутри логгера пишется как Error и валит тест.
/// </summary>
[TestFixture]
public sealed class AdminGuardTest
{
    private const string BrokenWebhook = "http://127.0.0.1:1/api/webhooks/1/broken";

    [Test]
    [TestCase(false)]
    [TestCase(true)]
    public async Task AdminEventsWorkWithDiscordConfig(bool brokenConfig)
    {
        await using var pair = await PoolManager.GetServerClient(new PoolSettings { Connected = true, Destructive = true });
        var server = pair.Server;

        var cfg = server.ResolveDependency<IConfigurationManager>();
        var db = server.ResolveDependency<IServerDbManager>();
        var guard = server.ResolveDependency<IAdminGuardManager>();
        var console = server.ResolveDependency<IConsoleHost>();
        var logManager = server.ResolveDependency<ILogManager>();

        var player = pair.Player!;
        await PoolManager.WaitUntil(server, async () => await db.GetPlayerRecordByUserId(player.UserId) != null);

        await server.WaitPost(() =>
        {
            if (brokenConfig)
            {
                // Ожидаемые ошибки отправки в Discord не должны валить тест
                logManager.GetSawmill("DISCORD").Level = LogLevel.Fatal;
                logManager.GetSawmill("discord.logger").Level = LogLevel.Fatal;
                cfg.SetCVar(CCVars.DiscordAdminLogsWebhook, BrokenWebhook);
                cfg.SetCVar(CCVars.DiscordAdminchatWebhook, BrokenWebhook);
                cfg.SetCVar(CCVars.AdminChatDiscordChannelId, "not-a-channel-id");
                cfg.SetCVar(CCVars.AdminLogsDiscordChannelId, "not-a-channel-id");
            }
            else
            {
                logManager.GetSawmill("discord.logger").Level = LogLevel.Debug;
            }
        });

        // Реальные команды: наигранное время и баланс
        foreach (var command in new[]
                 {
                     $"playtime_addoverall {player.Name} 10",
                     $"playtime_addrole {player.Name} Passenger 5",
                     $"balance:add {player.Name} 5",
                     $"balance:rem {player.Name} 2",
                     $"balance:set {player.Name} 7",
                     "kick NoSuchPlayerForAdminGuard test",
                 })
        {
            await server.WaitPost(() => console.ExecuteCommand(command));
            await Task.Delay(300);
        }

        // Остальные события напрямую через AdminGuard
        var actor = DiscordLogActor.From(player);
        var perms = new AdminPermsData("Title", "Rank", AdminFlags.Ban, AdminFlags.Debug, false);

        await server.WaitPost(() =>
        {
            guard.PlaytimeChanged(player.UserId, [("Overall", TimeSpan.FromMinutes(30)), ("Passenger", TimeSpan.FromMinutes(-5))], true, actor);

            guard.AdminRankAdded(new AdminRankData("Rank", AdminFlags.Ban), actor);
            guard.AdminRankUpdated(new AdminRankData("Rank", AdminFlags.Ban), new AdminRankData("Rank2", AdminFlags.Ban | AdminFlags.Moderator), actor);
            guard.AdminRankRemoved(new AdminRankData("Rank2", AdminFlags.Ban | AdminFlags.Moderator), actor);
            guard.AdminPermsAdded(player.UserId, perms, actor);
            guard.AdminPermsUpdated(player.UserId, perms, perms with { Title = null, Suspended = true }, actor);
            guard.AdminPermsRemoved(player.UserId, actor);

            guard.DiscordAccountLinked(player.UserId, 999999999999UL);
            guard.DiscordAccountUnlinked(null, 999999999999UL);
            guard.CurrencyTransferred(player.UserId, new NetUserId(Guid.NewGuid()), "5 coins");

            guard.PlayerKicked(actor, "reason", DiscordLogActor.Server);
            guard.PlayerFrozen(actor, AdminFreezeAction.Freeze, actor);
            guard.PlayerFrozen(actor, AdminFreezeAction.FreezeAndMute, actor);
            guard.PlayerFrozen(actor, AdminFreezeAction.Unfreeze, actor);

            guard.AdminStatusChanged(AdminStatusChange.LoggedIn, player.Name, "Title");
            guard.AdminStatusChanged(AdminStatusChange.LoggedOut, player.Name);
            guard.AdminStatusChanged(AdminStatusChange.Deadmined, player.Name);
            guard.AdminStatusChanged(AdminStatusChange.Readmined, player.Name);

            guard.WatchlistPlayerJoined(player.UserId, new AdminWatchlistRecord(
                1, null, null, TimeSpan.Zero, "watch message", null, DateTimeOffset.Now, null, null, null, false, null, null));
            guard.UserIdMismatch("Name", "Main");
        });

        // Даём фоновым отправкам отработать
        await pair.RunTicksSync(30);
        await Task.Delay(1500);

        await pair.CleanReturnAsync();
    }

    /// <summary>
    ///     Регрессия: при настроенном вебхуке отправка внутри Guarded не должна зависать (повторный захват очереди)
    ///     и очередь не должна блокироваться для следующих сообщений.
    /// </summary>
    [Test]
    public async Task GuardedSendCompletesWithConfiguredTarget()
    {
        await using var pair = await PoolManager.GetServerClient(new PoolSettings { Destructive = true });
        var server = pair.Server;

        var cfg = server.ResolveDependency<IConfigurationManager>();
        var logger = server.ResolveDependency<IDiscordLogger>();
        var logManager = server.ResolveDependency<ILogManager>();

        await server.WaitPost(() =>
        {
            logManager.GetSawmill("DISCORD").Level = LogLevel.Fatal;
            logManager.GetSawmill("discord.logger").Level = LogLevel.Fatal;
            // Невалидный URL падает мгновенно, без сетевых таймаутов
            cfg.SetCVar(CCVars.DiscordAdminchatWebhook, "not-a-url");
            cfg.SetCVar(CCVars.DiscordBansWebhook, "not-a-url");
        });

        Assert.That(logger.UsesBot(DiscordLogChannel.AdminChat), Is.False);

        for (var i = 0; i < 2; i++)
        {
            var task = logger.Guarded(() => logger.SendText("test", DiscordLogChannel.AdminChat | DiscordLogChannel.Bans));
            await task.WaitAsync(TimeSpan.FromSeconds(30));
        }

        await pair.CleanReturnAsync();
    }
}
