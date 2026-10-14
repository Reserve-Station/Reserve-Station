// SPDX-License-Identifier: AGPL-3.0-or-later

using System.Threading;
using System.Threading.Tasks;
using Content.Server.Database;
using Content.Server.Discord;
using Content.Server.Discord.DiscordLink;
using Content.Server.GameTicking;
using Content.Shared.CCVar;
using Robust.Shared.Configuration;
using Robust.Shared.Network;

namespace Content.Server._Reserve.Discord.Logger;

public sealed partial class DiscordLogger : IDiscordLogger, IPostInjectInit
{
    [Dependency] private IConfigurationManager _cfg = default!;
    [Dependency] private IServerDbManager _db = default!;
    [Dependency] private IEntitySystemManager _systems = default!;
    [Dependency] private ILogManager _logManager = default!;
    [Dependency] private DiscordLink _link = default!;
    [Dependency] private DiscordWebhook _webhook = default!;

    private const int TextLimit = 1900;

    private ISawmill _sawmill = default!;

    void IPostInjectInit.PostInject()
    {
        _sawmill = _logManager.GetSawmill("discord.logger");
    }

    // Сообщения собираются по очереди: БД в тестах синхронная и не терпит параллельных запросов
    private readonly SemaphoreSlim _gate = new(1, 1);

    public async Task Guarded(Func<Task> action)
    {
        // Ничего не настроено - не тратим запросы к БД на сборку сообщения, которое некуда отправить
        if (!HasAnyTarget())
            return;

        await _gate.WaitAsync();
        try
        {
            await Safe(action);
        }
        finally
        {
            _gate.Release();
        }
    }

    // Без очереди: вызывается изнутри Guarded, повторный захват очереди зависал бы навсегда
    private async Task Safe(Func<Task> action)
    {
        try
        {
            await action();
        }
        catch (Exception e)
        {
            _sawmill.Error($"Discord logging failed: {e}");
        }
    }

    public bool UsesBot(DiscordLogChannel channel)
    {
        if (!_link.IsConnected)
            return false;

        return channel switch
        {
            DiscordLogChannel.AdminChat => GetChannelId(CCVars.AdminChatDiscordChannelId) != null,
            DiscordLogChannel.AdminLogs => (GetChannelId(CCVars.AdminLogsDiscordChannelId) ?? GetChannelId(CCVars.AdminChatDiscordChannelId)) != null,
            _ => false,
        };
    }

    private bool HasAnyTarget()
    {
        if (_link.IsConnected)
            return true;

        return !string.IsNullOrWhiteSpace(_cfg.GetCVar(CCVars.DiscordBansWebhook))
               || !string.IsNullOrWhiteSpace(_cfg.GetCVar(CCVars.DiscordAdminLogsWebhook))
               || !string.IsNullOrWhiteSpace(_cfg.GetCVar(CCVars.DiscordAdminchatWebhook));
    }
    public DiscordLogMessage NewMessage(string titleKey, int color, int? id = null, params (string, object)[] titleArgs)
    {
        var args = new (string, object)[titleArgs.Length + 1];
        args[0] = ("id", id?.ToString() ?? "-");
        titleArgs.CopyTo(args, 1);

        return new DiscordLogMessage(Loc.GetString(titleKey, args), color, Footer());
    }

    private string Footer()
    {
        var server = _cfg.GetCVar(CCVars.GameHostName);
        var round = string.Empty;

        if (_systems.TryGetEntitySystem<GameTicker>(out var ticker))
        {
            round = ticker.RunLevel switch
            {
                GameRunLevel.PreRoundLobby => ticker.RoundId == 0
                    ? Loc.GetString("discord-logger-round-lobby-restart")
                    : Loc.GetString("discord-logger-round-lobby", ("id", (ticker.RoundId + 1).ToString())),
                GameRunLevel.InRound => Loc.GetString("discord-logger-round-in-round", ("id", ticker.RoundId.ToString())),
                GameRunLevel.PostRound => Loc.GetString("discord-logger-round-post", ("id", ticker.RoundId.ToString())),
                _ => string.Empty,
            };
        }

        return $"{server} ({round})";
    }

    #region Labels

    public async Task<string> PlayerName(NetUserId id)
    {
        return (await _db.GetPlayerRecordByUserId(id))?.LastSeenUserName ?? id.ToString();
    }

    public async Task<string> PlayerLabel(NetUserId id, string? name = null)
    {
        name ??= await PlayerName(id);
        var label = $"`{name.Replace('`', '\'')}`";

        try
        {
            if (await _db.GetLinkedDiscordId(id, default) is { } discordId)
                label += $" (<@{discordId}>)";
        }
        catch (Exception e)
        {
            // Упоминание - дополнительная информация, без него сообщение всё равно отправляется
            _sawmill.Info($"Failed to get linked Discord of {id}: {e}");
        }

        return label;
    }

    public async Task<List<string>> PlayerLabels(IEnumerable<NetUserId> ids)
    {
        var labels = new List<string>();
        foreach (var id in ids)
        {
            labels.Add(await PlayerLabel(id));
        }

        return labels;
    }

    public async Task<string> ActorLabel(DiscordLogActor actor)
    {
        if (actor.UserId is { } id)
            return await PlayerLabel(id, actor.Name);

        return actor.Name is { } name
            ? $"`{name.Replace('`', '\'')}`"
            : Loc.GetString("system-user");
    }

    public string DiscordTime(DateTime time)
    {
        // Unspecified приходит из БД и означает UTC
        var utc = time.Kind == DateTimeKind.Unspecified
            ? DateTime.SpecifyKind(time, DateTimeKind.Utc)
            : time.ToUniversalTime();

        return $"<t:{new DateTimeOffset(utc).ToUnixTimeSeconds()}:f>";
    }

    #endregion

    #region Sending

    public Task Send(DiscordLogMessage message, DiscordLogChannel channels)
    {
        _sawmill.Debug($"Discord log ({channels}):{'\n'}{message.ToPlainText()}");

        return SendTo(channels,
            () => new WebhookPayload { Embeds = [message.ToWebhookEmbed()] },
            (bot, channelId) => bot.SendEmbedAsync(channelId, message.Title, Truncate(message.Body, DiscordLogMessage.EmbedLimit), message.Color, message.Footer));
    }

    public Task SendText(string text, DiscordLogChannel channels)
    {
        text = Truncate(text, TextLimit);
        _sawmill.Debug($"Discord log ({channels}):{'\n'}{text}");

        return SendTo(channels,
            () => new WebhookPayload { Content = text },
            (bot, channelId) => bot.SendMessageAsync(channelId, text));
    }

    private Task SendTo(DiscordLogChannel channels, Func<WebhookPayload> payload, Func<DiscordLink, ulong, Task> viaBot)
    {
        var tasks = new List<Task>(3);

        if (channels.HasFlag(DiscordLogChannel.Bans))
            tasks.Add(Safe(() => PostWebhook(_cfg.GetCVar(CCVars.DiscordBansWebhook), payload())));

        if (channels.HasFlag(DiscordLogChannel.AdminLogs))
        {
            tasks.Add(Safe(() => SendVia(
                GetChannelId(CCVars.AdminLogsDiscordChannelId) ?? GetChannelId(CCVars.AdminChatDiscordChannelId),
                [CCVars.DiscordAdminLogsWebhook, CCVars.DiscordAdminchatWebhook],
                payload,
                viaBot)));
        }

        if (channels.HasFlag(DiscordLogChannel.AdminChat))
        {
            tasks.Add(Safe(() => SendVia(
                GetChannelId(CCVars.AdminChatDiscordChannelId),
                [CCVars.DiscordAdminchatWebhook],
                payload,
                viaBot)));
        }

        return Task.WhenAll(tasks);
    }

    /// <summary>
    ///     Бот, если он подключён и канал настроен; иначе первый настроенный вебхук из списка.
    /// </summary>
    private async Task SendVia(ulong? channelId, CVarDef<string>[] webhooks, Func<WebhookPayload> payload, Func<DiscordLink, ulong, Task> viaBot)
    {
        if (_link.IsConnected && channelId is { } id)
        {
            await viaBot(_link, id);
            return;
        }

        foreach (var cvar in webhooks)
        {
            var url = _cfg.GetCVar(cvar);
            if (string.IsNullOrWhiteSpace(url))
                continue;

            await PostWebhook(url, payload());
            return;
        }
    }

    private ulong? GetChannelId(CVarDef<string> cvar)
    {
        return ulong.TryParse(_cfg.GetCVar(cvar), out var id) && id != 0 ? id : null;
    }

    private async Task PostWebhook(string url, WebhookPayload payload)
    {
        if (string.IsNullOrWhiteSpace(url))
            return;

        if (await _webhook.GetWebhook(url) is not { } data)
            return;

        await _webhook.CreateMessage(data.ToIdentifier(), payload);
    }

    private static string Truncate(string text, int limit)
    {
        return text.Length <= limit ? text : text[..(limit - 1)] + "…";
    }

    #endregion
}
