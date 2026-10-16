using Content.Server._Reserve.Discord.Logger;

namespace Content.Server._Reserve.Discord.Custos;

public interface ICustosManager
{
    void TriggerAdded(string trigger, DiscordLogActor actor, bool suspicious = false, string? suspiciousReason = null);

    void TriggerRejected(string trigger, DiscordLogActor actor, string? reason = null);

    void TriggerRemoved(string trigger, DiscordLogActor actor);

    void TriggerMassRemovalAlert(int count, TimeSpan window, DiscordLogActor actor, IReadOnlyList<string>? recentTriggers = null);
}
