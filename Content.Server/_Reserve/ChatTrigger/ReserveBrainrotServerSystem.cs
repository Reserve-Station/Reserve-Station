using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Content.Server._Reserve.Discord.Custos;
using Content.Server._Reserve.Discord.Logger;
using Content.Server.Database;
using Content.Server.GameTicking;
using Content.Shared._Reserve.ChatTrigger;
using Robust.Shared.Asynchronous;
using Robust.Shared.Network;
using Robust.Shared.Player;

namespace Content.Server._Reserve.ChatTrigger;

public sealed class ReserveBrainrotServerSystem : EntitySystem
{
    [Dependency] private readonly IServerDbManager _db = default!;
    [Dependency] private readonly ITaskManager _taskManager = default!;
    [Dependency] private readonly ICustosManager _custos = default!;

    private readonly List<string> _customTriggers = new();
    private readonly object _lock = new();

    private readonly record struct RemovalRecord(DateTime Time, string Trigger);
    private readonly Dictionary<NetUserId, List<RemovalRecord>> _recentRemovals = new();
    private static readonly TimeSpan MassRemovalWindow = TimeSpan.FromSeconds(10);
    private const int MassRemovalThreshold = 5;

    public event Action? OnTriggersChanged;

    public IReadOnlyList<string> CustomTriggers
    {
        get
        {
            lock (_lock)
            {
                return _customTriggers.ToArray();
            }
        }
    }

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<PlayerJoinedLobbyEvent>(OnPlayerJoinedLobby);

        _ = LoadTriggersAsync();
    }

    private async Task LoadTriggersAsync()
    {
        try
        {
            var triggers = await _db.GetBrainrotTriggersAsync();
            _taskManager.RunOnMainThread(() =>
            {
                List<string> syncList;
                lock (_lock)
                {
                    _customTriggers.Clear();
                    _customTriggers.AddRange(triggers);
                    syncList = new List<string>(_customTriggers);
                }
                RaiseNetworkEvent(new SyncBrainrotTriggersMessage(syncList));
                OnTriggersChanged?.Invoke();
            });
        }
        catch (Exception e)
        {
            Log.Error($"Failed to load custom brainrot triggers from DB: {e}");
        }
    }

    private void OnPlayerJoinedLobby(PlayerJoinedLobbyEvent ev)
    {
        var session = ev.PlayerSession;
        List<string> syncList;
        lock (_lock)
        {
            syncList = new List<string>(_customTriggers);
        }
        RaiseNetworkEvent(new SyncBrainrotTriggersMessage(syncList), session);
    }

    public async Task AddTrigger(string trigger, ICommonSession? session = null)
    {
        await AddTrigger(trigger, DiscordLogActor.From(session));
    }

    public async Task AddTrigger(string trigger, DiscordLogActor actor)
    {
        if (!ChatTriggerHelper.IsValidTrigger(trigger, out var error))
        {
            Log.Warning($"Attempted to add invalid brainrot trigger '{trigger}': {error}");
            _custos.TriggerRejected(trigger, actor, error);
            return;
        }

        var normalized = ChatTriggerHelper.NormalizeTrigger(trigger);

        lock (_lock)
        {
            if (_customTriggers.Contains(normalized))
                return;
        }

        var isSuspicious = IsSuspiciousTrigger(normalized, out var suspiciousReason);

        try
        {
            await _db.AddBrainrotTriggerAsync(normalized);
            _taskManager.RunOnMainThread(() =>
            {
                List<string> syncList;
                lock (_lock)
                {
                    if (!_customTriggers.Contains(normalized))
                        _customTriggers.Add(normalized);
                    syncList = new List<string>(_customTriggers);
                }
                RaiseNetworkEvent(new SyncBrainrotTriggersMessage(syncList));
                OnTriggersChanged?.Invoke();
            });

            _custos.TriggerAdded(normalized, actor, isSuspicious, suspiciousReason);
        }
        catch (Exception e)
        {
            Log.Error($"Failed to add custom brainrot trigger '{normalized}' to DB: {e}");
        }
    }

    public async Task RemoveTrigger(string trigger, ICommonSession? session = null)
    {
        await RemoveTrigger(trigger, DiscordLogActor.From(session));
    }

    public async Task RemoveTrigger(string trigger, DiscordLogActor actor)
    {
        var normalized = ChatTriggerHelper.NormalizeTrigger(trigger);
        if (string.IsNullOrEmpty(normalized))
            return;

        lock (_lock)
        {
            if (!_customTriggers.Contains(normalized))
                return;
        }

        try
        {
            await _db.RemoveBrainrotTriggerAsync(normalized);
            _taskManager.RunOnMainThread(() =>
            {
                List<string> syncList;
                lock (_lock)
                {
                    _customTriggers.Remove(normalized);
                    syncList = new List<string>(_customTriggers);
                }
                RaiseNetworkEvent(new SyncBrainrotTriggersMessage(syncList));
                OnTriggersChanged?.Invoke();
            });

            var shouldAlertMassRemoval = false;
            var removalCount = 0;
            string[]? removedTriggers = null;

            lock (_lock)
            {
                if (actor.UserId is { } uid)
                {
                    var now = DateTime.UtcNow;
                    if (!_recentRemovals.TryGetValue(uid, out var list))
                    {
                        list = new List<RemovalRecord>();
                        _recentRemovals[uid] = list;
                    }

                    list.RemoveAll(r => now - r.Time > MassRemovalWindow);
                    list.Add(new RemovalRecord(now, normalized));

                    if (list.Count >= MassRemovalThreshold && list.Count % 5 == 0)
                    {
                        shouldAlertMassRemoval = true;
                        removalCount = list.Count;
                        removedTriggers = list.Select(r => r.Trigger).ToArray();
                    }

                    if (_recentRemovals.Count > 10)
                    {
                        var staleKeys = _recentRemovals
                            .Where(kvp => kvp.Value.All(r => now - r.Time > MassRemovalWindow))
                            .Select(kvp => kvp.Key)
                            .ToList();

                        foreach (var key in staleKeys)
                        {
                            _recentRemovals.Remove(key);
                        }
                    }
                }
            }

            if (shouldAlertMassRemoval)
            {
                _custos.TriggerMassRemovalAlert(removalCount, MassRemovalWindow, actor, removedTriggers);
            }
            else
            {
                _custos.TriggerRemoved(normalized, actor);
            }
        }
        catch (Exception e)
        {
            Log.Error($"Failed to remove custom brainrot trigger '{normalized}' from DB: {e}");
        }
    }

    private bool IsSuspiciousTrigger(string normalized, [NotNullWhen(true)] out string? reason)
    {
        reason = null;
        var variants = ChatTriggerHelper.ParseTriggerVariants(normalized);

        foreach (var variant in variants)
        {
            var clean = variant.Trim();
            if (ChatTriggerHelper.IsRegexTrigger(clean))
            {
                var pattern = clean.Substring(ChatTriggerHelper.RegexPrefix.Length).Trim();
                if (pattern is ".*" or ".+" or "^.*$" or "^.+$" or @"\w+" or @"\s+" or @"\S+" or @"[\s\S]*")
                {
                    reason = Loc.GetString("custos-suspicious-reason-broad-regex", ("pattern", pattern));
                    return true;
                }

                if (pattern.Length <= 2)
                {
                    reason = Loc.GetString("custos-suspicious-reason-short-regex", ("pattern", pattern));
                    return true;
                }
            }
            else if (clean.Length <= 2)
            {
                reason = Loc.GetString("custos-suspicious-reason-short-trigger", ("trigger", clean));
                return true;
            }
        }

        return false;
    }
}
