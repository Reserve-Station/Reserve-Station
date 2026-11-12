// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Client.Administration.Managers; // Reserve edit
using Content.Client.Gameplay;
using Content.Client.Info;
using Content.Client._Reserve.NewPlayerGuide;
using Content.Shared.Administration; // Reserve edit
using Content.Shared.CCVar;
using Content.Shared.Guidebook;
using Content.Shared.Info;
using Content.Shared._Reserve.CCCVars;
using Robust.Client.Console;
using Robust.Client.UserInterface.Controllers;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Configuration;
using Robust.Shared.Network;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Client.UserInterface.Systems.Info;

public sealed class InfoUIController : UIController, IOnStateExited<GameplayState>
{
    [Dependency] private readonly IClientAdminManager _adminManager = default!; // Reserve edit
    [Dependency] private readonly IClientConsoleHost _consoleHost = default!;
    [Dependency] private readonly IConfigurationManager _cfg = default!; // Reserve: new player guide
    [Dependency] private readonly INetManager _netManager = default!;
    [Dependency] private readonly IPrototypeManager _prototype = default!;

    private RulesPopup? _rulesPopup;
    private RulesAndInfoWindow? _infoWindow;
    // Reserve start: new player guide
    private NewPlayerGuidePopup? _newPlayerGuidePopup;
    private bool _newPlayerGuideAutoShown;
    private float? _rulesPopupTime;
    // Reserve end

    private static readonly ProtoId<GuideEntryPrototype> DefaultRuleset = "DefaultRulesetReserve"; // Reserve edit

    public ProtoId<GuideEntryPrototype> RulesEntryId = DefaultRuleset;

    protected override string SawmillName => "rules";

    public override void Initialize()
    {
        base.Initialize();

        _netManager.RegisterNetMessage<RulesAcceptedMessage>();
        _netManager.RegisterNetMessage<SendRulesInformationMessage>(OnRulesInformationMessage);
        // Reserve start: new player guide
        _netManager.Disconnect += OnNetDisconnect;
        // Reserve end

        _consoleHost.RegisterCommand("fuckrules",
            "",
            "",
            (_, _, _) =>
            {
                if (!_adminManager.HasFlag(AdminFlags.Admin)) // Reserve edit: Only admins can skip reading rules.
                    return;

                OnAcceptPressed();
            });

        // Reserve start: new player guide
        _consoleHost.RegisterCommand("newplayerguide",
            "Opens the new player setup guide.",
            "newplayerguide",
            (_, _, _) =>
            {
                // Don't stack the guide over an already open rules popup, or over itself.
                if (_rulesPopup != null || _newPlayerGuidePopup != null)
                    return;

                ShowNewPlayerGuide(false, autoShown: false);
            });
        // Reserve end
    }

    private void OnRulesInformationMessage(SendRulesInformationMessage message)
    {
        RulesEntryId = message.CoreRules;
        // Reserve start: new player guide
        _rulesPopupTime = message.PopupTime;

        if (_newPlayerGuidePopup != null)
        {
            _newPlayerGuidePopup.ShowRulesAfter |= message.ShouldShowRules;
            return;
        }

        if (message.ShouldShowNewPlayerGuide && !_cfg.GetCVar(CCCVars.NewPlayerGuideSeen))
        {
            ShowNewPlayerGuide(message.ShouldShowRules, autoShown: true);
            return;
        }
        // Reserve end

        if (message.ShouldShowRules)
            ShowRules(message.PopupTime);
    }

    public void OnStateExited(GameplayState state)
    {
        // Reserve start: new player guide
        // Not closed here: the guide survives state changes and is only dropped on disconnect.
        // Reserve end

        if (_infoWindow == null)
            return;

        _infoWindow.Dispose();
        _infoWindow = null;
    }

    // Reserve start: new player guide
    private void ShowNewPlayerGuide(bool showRulesAfter, bool autoShown)
    {
        if (_newPlayerGuidePopup != null)
            return;

        _newPlayerGuideAutoShown = autoShown;
        _newPlayerGuidePopup = new NewPlayerGuidePopup
        {
            ShowRulesAfter = showRulesAfter,
        };
        _newPlayerGuidePopup.OnClosed += OnNewPlayerGuideClosed;
        UIManager.WindowRoot.AddChild(_newPlayerGuidePopup);
        LayoutContainer.SetAnchorPreset(_newPlayerGuidePopup, LayoutContainer.LayoutPreset.Wide);
    }

    private void OnNewPlayerGuideClosed()
    {
        if (_newPlayerGuidePopup == null)
            return;

        // Marking the guide as seen has to be persisted even when no setting was changed.
        var seen = _newPlayerGuideAutoShown;

        if (seen)
            _cfg.SetCVar(CCCVars.NewPlayerGuideSeen, true);

        CloseNewPlayerGuidePopup(forceSave: seen);
    }

    private void CloseNewPlayerGuidePopup(bool forceSave = false)
    {
        if (_newPlayerGuidePopup == null)
            return;

        var pendingRules = _newPlayerGuidePopup.ShowRulesAfter;
        var saveConfig = forceSave || _newPlayerGuidePopup.SettingsChanged;

        _newPlayerGuidePopup.OnClosed -= OnNewPlayerGuideClosed;
        _newPlayerGuidePopup.Orphan();
        _newPlayerGuidePopup = null;
        _newPlayerGuideAutoShown = false;

        if (saveConfig)
            _cfg.SaveToFile();

        if (pendingRules)
            ShowRulesDeferred();
    }

    private void OnNetDisconnect(object? sender, NetDisconnectedArgs args)
    {
        CloseNewPlayerGuidePopup();
    }

    private void ShowRulesDeferred()
    {
        Timer.Spawn(TimeSpan.Zero, () =>
        {
            if (!_netManager.IsConnected)
                return;

            ShowRules(GetRulesPopupTime());
        });
    }

    private float GetRulesPopupTime()
    {
        // The value from the server wins, including 0 (no countdown).
        return _rulesPopupTime ?? _cfg.GetCVar(CCVars.RulesWaitTime);
    }

    // Reserve end

    private void ShowRules(float time)
    {
        if (_rulesPopup != null)
            return;

        _rulesPopup = new RulesPopup
        {
            Timer = time
        };

        _rulesPopup.OnQuitPressed += OnQuitPressed;
        _rulesPopup.OnAcceptPressed += OnAcceptPressed;
        UIManager.WindowRoot.AddChild(_rulesPopup);
        LayoutContainer.SetAnchorPreset(_rulesPopup, LayoutContainer.LayoutPreset.Wide);
    }

    private void OnQuitPressed()
    {
        _consoleHost.ExecuteCommand("quit");
    }

    private void OnAcceptPressed()
    {
        _netManager.ClientSendMessage(new RulesAcceptedMessage());

        _rulesPopup?.Orphan();
        _rulesPopup = null;
    }

    public GuideEntryPrototype GetCoreRuleEntry()
    {
        if (!_prototype.TryIndex(RulesEntryId, out var guideEntryPrototype))
        {
            guideEntryPrototype = _prototype.Index(DefaultRuleset);
            Log.Error($"Couldn't find the following prototype: {RulesEntryId}. Falling back to {DefaultRuleset}, please check that the server has the rules set up correctly");
            return guideEntryPrototype;
        }

        return guideEntryPrototype;
    }

    public void OpenWindow()
    {
        if (_infoWindow == null || _infoWindow.Disposed)
            _infoWindow = UIManager.CreateWindow<RulesAndInfoWindow>();

        _infoWindow?.OpenCentered();
    }
}
