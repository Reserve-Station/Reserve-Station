using Content.Server.EUI;
using Content.Server.Administration.Managers;
using Content.Shared.Administration;
using Content.Shared.Eui;
using Content.Shared._Reserve.ChatTrigger;

namespace Content.Server._Reserve.ChatTrigger;

public sealed class BrainrotPanelEui : BaseEui
{
    [Dependency] private readonly IEntityManager _entManager = default!;
    [Dependency] private readonly IAdminManager _adminManager = default!;

    private readonly ReserveBrainrotServerSystem _brainrotSystem;

    public BrainrotPanelEui()
    {
        IoCManager.InjectDependencies(this);
        _brainrotSystem = _entManager.System<ReserveBrainrotServerSystem>();
    }

    public override void Opened()
    {
        base.Opened();

        if (!_adminManager.HasAdminFlag(Player, AdminFlags.Admin))
        {
            Close();
            return;
        }

        _brainrotSystem.OnTriggersChanged += OnTriggersChanged;
        StateDirty();
    }

    public override void Closed()
    {
        base.Closed();
        _brainrotSystem.OnTriggersChanged -= OnTriggersChanged;
    }

    private void OnTriggersChanged()
    {
        StateDirty();
    }

    public override EuiStateBase GetNewState()
    {
        return new BrainrotPanelEuiState(new List<string>(_brainrotSystem.CustomTriggers));
    }

    public override async void HandleMessage(EuiMessageBase msg)
    {
        base.HandleMessage(msg);

        if (!_adminManager.HasAdminFlag(Player, AdminFlags.Admin))
            return;

        try
        {
            switch (msg)
            {
                case AddBrainrotTriggerMsg addMsg:
                    await _brainrotSystem.AddTrigger(addMsg.Trigger, Player);
                    break;
                case RemoveBrainrotTriggerMsg removeMsg:
                    await _brainrotSystem.RemoveTrigger(removeMsg.Trigger, Player);
                    break;
            }
        }
        catch (Exception e)
        {
            Logger.GetSawmill("brainrot.eui").Error($"Error processing brainrot trigger message from {Player.Name}: {e}");
        }
    }
}
