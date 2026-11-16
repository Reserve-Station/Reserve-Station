// SPDX-License-Identifier: AGPL-3.0-or-later

using System.Collections.Generic;
using System.Linq;
using Content.Server.Administration.Managers;
using Content.Server.Administration.Notes;
using Content.Server.Database;
using Content.Shared.CCVar;
using Content.Shared.Database;
using Content.Shared.Roles;
using Robust.Shared.Configuration;
using Robust.Shared.Console;
using Robust.Shared.Log;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;

#pragma warning disable CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
namespace Content.IntegrationTests.Tests.Administration;

/// <summary>
///     Reserve Red - BanGuard: баны, разбаны и заметки должны работать независимо от настроек Discord.
///     Любая ошибка внутри BanGuard логируется как Error и валит тест (FailureLogLevel = Warning).
/// </summary>
/// <remarks>
///     Тест пишет в БД (баны, заметки, привязку Discord) и меняет CVar-ы, поэтому пара помечается Dirty,
///     иначе другие тесты (например, PardonTest, ожидающий пустую таблицу банов) увидят чужие данные.
/// </remarks>
[TestFixture]
public sealed class BanGuardTest
{
    private const string BrokenWebhook = "http://127.0.0.1:1/api/webhooks/1/broken";

    [Test]
    [TestCase(false)]
    [TestCase(true)]
    public async Task BansAndNotesWorkWithDiscordConfig(bool brokenConfig)
    {
        await using var pair = await PoolManager.GetServerClient(new PoolSettings { Connected = true, Destructive = true });
        var server = pair.Server;

        var cfg = server.ResolveDependency<IConfigurationManager>();
        var banManager = server.ResolveDependency<IBanManager>();
        var db = server.ResolveDependency<IServerDbManager>();
        var notes = server.ResolveDependency<IAdminNotesManager>();
        var logManager = server.ResolveDependency<ILogManager>();

        var admin = pair.Player!;
        var targetId = new NetUserId(Guid.NewGuid());

        await server.WaitPost(() =>
        {
            if (brokenConfig)
            {
                // Ожидаемые ошибки отправки в Discord не должны валить тест
                logManager.GetSawmill("DISCORD").Level = LogLevel.Fatal;
                logManager.GetSawmill("discord.ban_guard").Level = LogLevel.Fatal;
                logManager.GetSawmill("discord.logger").Level = LogLevel.Fatal;
                cfg.SetCVar(CCVars.DiscordBansWebhook, BrokenWebhook);
                cfg.SetCVar(CCVars.DiscordAdminLogsWebhook, BrokenWebhook);
                cfg.SetCVar(CCVars.AdminChatDiscordChannelId, "not-a-channel-id");
                cfg.SetCVar(CCVars.AdminLogsDiscordChannelId, "not-a-channel-id");
            }
            else
            {
                logManager.GetSawmill("discord.ban_guard").Level = LogLevel.Debug;
                logManager.GetSawmill("discord.logger").Level = LogLevel.Debug;
            }
        });

        // Привязанный Discord админа должен отображаться упоминанием во всех сообщениях
        await PoolManager.WaitUntil(server, async () => await db.GetPlayerRecordByUserId(admin.UserId) != null);
        var linkCode = Guid.NewGuid();
        await db.SetLinkingCode(admin.UserId, linkCode);
        await db.ConsumeLinkingCode(linkCode, 999999999999UL, default);

        // Бан
        await server.WaitPost(() =>
        {
            var info = new CreateServerBanInfo("test ban");
            info.AddUser(targetId, "BanGuardTarget");
            info.WithBanningAdmin(admin.UserId);
            info.WithMinutes(60);
            banManager.CreateServerBan(info);
        });

        await PoolManager.WaitUntil(server, async () =>
            (await db.GetBansAsync(null, targetId, null, null, true, BanType.Server)).Count == 1);

        // Роль-бан
        await server.WaitPost(() =>
        {
            var info = new CreateRoleBanInfo("test role ban");
            info.AddUser(targetId, "BanGuardTarget");
            info.WithBanningAdmin(admin.UserId);
            info.AddJob(new ProtoId<JobPrototype>("Passenger"));
            banManager.CreateRoleBan(info);
        });

        List<BanDef> roleBans = [];
        await PoolManager.WaitUntil(server, async () =>
        {
            roleBans = await db.GetBansAsync(null, targetId, null, null, true, BanType.Role);
            return roleBans.Count == 1;
        });

        // Разбан роль-бана
        Task<string>? pardon = null;
        await server.WaitPost(() => pardon = banManager.PardonRoleBan(roleBans.Single().Id!.Value, admin.UserId, DateTimeOffset.Now));
        Assert.That(await pardon!, Does.Contain("Pardoned"));

        // Заметка: добавление, изменение срока, удаление
        Task? add = null;
        await server.WaitPost(() => add = notes.AddAdminRemark(admin, admin.UserId, NoteType.Note, "test note", NoteSeverity.Minor, false, null));
        await add!;

        var note = (await notes.GetAllAdminRemarks(admin.UserId)).Single();

        Task? edit = null;
        await server.WaitPost(() => edit = notes.ModifyAdminRemark(note.Id, NoteType.Note, admin, "test note", NoteSeverity.Minor, true, DateTime.UtcNow.AddDays(1)));
        await edit!;

        Task? delete = null;
        await server.WaitPost(() => delete = notes.DeleteAdminRemark(note.Id, NoteType.Note, admin));
        await delete!;

        // Исключения из банов и вайтлисты ролей (консольные команды, ник админа есть в БД)
        var console = server.ResolveDependency<IConsoleHost>();
        foreach (var command in new[]
                 {
                     $"ban_exemption_update {admin.Name} IP",
                     $"ban_exemption_update {admin.Name} IP Datacenter",
                     $"jobwhitelistadd {admin.Name} Passenger",
                     $"jobwhitelistremove {admin.Name} Passenger",
                 })
        {
            await server.WaitPost(() => console.ExecuteCommand(command));
            await Task.Delay(500);
        }
        // Даём фоновым отправкам BanGuard отработать
        await pair.RunTicksSync(30);
        await Task.Delay(1000);

        await pair.CleanReturnAsync();
    }
}
#pragma warning restore CS8632 // The annotation for nullable reference types should only be used in code within a '#nullable' annotations context.
