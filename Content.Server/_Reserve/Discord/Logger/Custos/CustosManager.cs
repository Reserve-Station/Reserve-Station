using System.Linq;
using Content.Server._Reserve.Discord.Logger;

namespace Content.Server._Reserve.Discord.Custos;

public sealed class CustosManager : ICustosManager, IPostInjectInit
{
    [Dependency] private readonly IDiscordLogger _logger = default!;
    [Dependency] private readonly ILogManager _logManager = default!;

    public const int ColorTriggerAdded = 0x2ecc71;
    public const int ColorTriggerRemoved = 0xe74c3c;
    public const int ColorSuspicious = 0xf39c12;
    public const int ColorCritical = 0x9b59b6;

    private const DiscordLogChannel Channel = DiscordLogChannel.Custos;

    private ISawmill _sawmill = default!;

    void IPostInjectInit.PostInject()
    {
        _sawmill = _logManager.GetSawmill("custos");
    }

    public void TriggerAdded(string trigger, DiscordLogActor actor, bool suspicious = false, string? suspiciousReason = null)
    {
        _sawmill.Debug($"Custos: trigger '{trigger}' added by {actor.Name ?? "System"} (suspicious: {suspicious})");

        _ = _logger.Guarded(async () =>
        {
            var titleKey = suspicious ? "custos-title-trigger-suspicious" : "custos-title-trigger-added";
            var color = suspicious ? ColorSuspicious : ColorTriggerAdded;

            var msg = _logger.NewMessage(titleKey, color);
            msg.Add("custos-trigger", ("trigger", trigger));
            msg.Add("custos-by", ("admin", await _logger.ActorLabel(actor)));

            if (suspicious && !string.IsNullOrWhiteSpace(suspiciousReason))
            {
                msg.Add("custos-reason", ("reason", suspiciousReason));
            }

            await _logger.Send(msg, Channel);
        });
    }

    public void TriggerRejected(string trigger, DiscordLogActor actor, string? reason = null)
    {
        _sawmill.Debug($"Custos: rejected trigger '{trigger}' from {actor.Name ?? "System"}: {reason}");

        _ = _logger.Guarded(async () =>
        {
            var msg = _logger.NewMessage("custos-title-trigger-rejected", ColorSuspicious);
            msg.Add("custos-trigger", ("trigger", trigger));
            msg.Add("custos-by", ("admin", await _logger.ActorLabel(actor)));

            if (!string.IsNullOrWhiteSpace(reason))
            {
                msg.Add("custos-reason", ("reason", reason));
            }

            await _logger.Send(msg, Channel);
        });
    }

    public void TriggerRemoved(string trigger, DiscordLogActor actor)
    {
        _sawmill.Debug($"Custos: trigger '{trigger}' removed by {actor.Name ?? "System"}");

        _ = _logger.Guarded(async () =>
        {
            var msg = _logger.NewMessage("custos-title-trigger-removed", ColorTriggerRemoved);
            msg.Add("custos-trigger", ("trigger", trigger));
            msg.Add("custos-by", ("admin", await _logger.ActorLabel(actor)));

            await _logger.Send(msg, Channel);
        });
    }

    public void TriggerMassRemovalAlert(int count, TimeSpan window, DiscordLogActor actor, IReadOnlyList<string>? recentTriggers = null)
    {
        _sawmill.Debug($"Custos: mass trigger removal detected by {actor.Name ?? "System"}: {count} in {window.TotalSeconds}s");

        _ = _logger.Guarded(async () =>
        {
            var msg = _logger.NewMessage("custos-title-trigger-mass-removed", ColorCritical);
            msg.Add("custos-by", ("admin", await _logger.ActorLabel(actor)));
            msg.Add("custos-count", ("count", count.ToString()), ("seconds", Math.Round(window.TotalSeconds).ToString()));

            if (recentTriggers is { Count: > 0 })
            {
                var formatted = recentTriggers.Select(t => $"`{t.Replace('`', '\'')}`");
                var listStr = string.Join(", ", formatted);
                if (listStr.Length > 800)
                    listStr = listStr[..797] + "…";
                msg.Add("custos-recent-list", ("triggers", listStr));
            }

            await _logger.Send(msg, Channel);
        });
    }
}
