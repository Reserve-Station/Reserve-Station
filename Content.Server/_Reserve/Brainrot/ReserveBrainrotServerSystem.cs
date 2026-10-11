using System.Threading.Tasks;
using Content.Server.Database;
using Content.Server.GameTicking;
using Content.Shared._Reserve.Brainrot;
using Robust.Shared.Asynchronous;

namespace Content.Server._Reserve.Brainrot;

public sealed class ReserveBrainrotServerSystem : EntitySystem
{
    [Dependency] private readonly IServerDbManager _db = default!;
    [Dependency] private readonly ITaskManager _taskManager = default!;

    private readonly List<string> _customTriggers = new();
    private readonly object _lock = new();

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
                lock (_lock)
                {
                    _customTriggers.Clear();
                    _customTriggers.AddRange(triggers);
                }
                RaiseNetworkEvent(new SyncBrainrotTriggersMessage(new List<string>(_customTriggers)));
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

    public async Task AddTrigger(string trigger)
    {
        var normalized = trigger.Trim().ToLowerInvariant();
        if (string.IsNullOrEmpty(normalized) || normalized.Length > 512)
            return;

        lock (_lock)
        {
            if (_customTriggers.Contains(normalized))
                return;
        }

        try
        {
            await _db.AddBrainrotTriggerAsync(normalized);
            _taskManager.RunOnMainThread(() =>
            {
                lock (_lock)
                {
                    if (!_customTriggers.Contains(normalized))
                        _customTriggers.Add(normalized);
                }
                RaiseNetworkEvent(new SyncBrainrotTriggersMessage(new List<string>(_customTriggers)));
                OnTriggersChanged?.Invoke();
            });
        }
        catch (Exception e)
        {
            Log.Error($"Failed to add custom brainrot trigger '{normalized}' to DB: {e}");
        }
    }

    public async Task RemoveTrigger(string trigger)
    {
        var normalized = trigger.Trim().ToLowerInvariant();
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
                lock (_lock)
                {
                    _customTriggers.Remove(normalized);
                }
                RaiseNetworkEvent(new SyncBrainrotTriggersMessage(new List<string>(_customTriggers)));
                OnTriggersChanged?.Invoke();
            });
        }
        catch (Exception e)
        {
            Log.Error($"Failed to remove custom brainrot trigger '{normalized}' from DB: {e}");
        }
    }
}
