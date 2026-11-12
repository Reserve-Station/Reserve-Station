// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Shared.CCVar;
using Content.Shared.GameTicking;
using Content.Shared._Reserve.Overlays;
using Robust.Client.Graphics;
using Robust.Shared.Configuration;
using Robust.Shared.Player;

namespace Content.Client._Reserve.Overlays;

public sealed class SaturationScaleSystem : EntitySystem
{
    [Dependency] private readonly IOverlayManager _overlayMan = default!;
    [Dependency] private readonly ISharedPlayerManager _playerMan = default!;
    [Dependency] private readonly IConfigurationManager _cfgMan = default!;

    private SaturationScaleOverlay _overlay = default!;
    private bool _moodEffectsEnabled;

    public override void Initialize()
    {
        base.Initialize();

        _overlay = new();
        _moodEffectsEnabled = _cfgMan.GetCVar(CCVars.MoodVisualEffects);
        _cfgMan.OnValueChanged(CCVars.MoodVisualEffects, HandleMoodEffectsUpdated);

        SubscribeLocalEvent<SaturationScaleOverlayComponent, ComponentInit>(OnInit);
        SubscribeLocalEvent<SaturationScaleOverlayComponent, ComponentShutdown>(OnShutdown);

        SubscribeLocalEvent<SaturationScaleOverlayComponent, PlayerAttachedEvent>(OnPlayerAttached);
        SubscribeLocalEvent<SaturationScaleOverlayComponent, PlayerDetachedEvent>(OnPlayerDetached);

        SubscribeNetworkEvent<RoundRestartCleanupEvent>(RoundRestartCleanup);
    }

    public override void Shutdown()
    {
        _cfgMan.UnsubValueChanged(CCVars.MoodVisualEffects, HandleMoodEffectsUpdated);

        base.Shutdown();
    }

    private void HandleMoodEffectsUpdated(bool moodEffectsEnabled)
    {
        _moodEffectsEnabled = moodEffectsEnabled;

        // The CVar can be toggled mid-round, so re-evaluate the overlay instead of only removing it.
        UpdateOverlayVisibility();
    }

    private void RoundRestartCleanup(RoundRestartCleanupEvent ev)
    {
        RemoveOverlay();
    }

    private void OnPlayerDetached(EntityUid uid, SaturationScaleOverlayComponent component, PlayerDetachedEvent args)
    {
        RemoveOverlay();
    }

    private void OnPlayerAttached(EntityUid uid, SaturationScaleOverlayComponent component, PlayerAttachedEvent args)
    {
        AddOverlay();
    }

    private void OnShutdown(EntityUid uid, SaturationScaleOverlayComponent component, ComponentShutdown args)
    {
        if (uid != _playerMan.LocalEntity)
            return;

        RemoveOverlay();
    }

    private void OnInit(EntityUid uid, SaturationScaleOverlayComponent component, ComponentInit args)
    {
        if (uid != _playerMan.LocalEntity)
            return;

        AddOverlay();
    }

    /// <summary>
    ///     Brings the overlay in line with the current mood effects CVar and the local player state.
    /// </summary>
    private void UpdateOverlayVisibility()
    {
        if (_moodEffectsEnabled
            && _playerMan.LocalEntity is { } player
            && HasComp<SaturationScaleOverlayComponent>(player))
        {
            AddOverlay();
            return;
        }

        RemoveOverlay();
    }

    private void AddOverlay()
    {
        if (!_moodEffectsEnabled || _overlayMan.HasOverlay<SaturationScaleOverlay>())
            return;

        _overlayMan.AddOverlay(_overlay);
    }

    private void RemoveOverlay()
    {
        if (!_overlayMan.HasOverlay<SaturationScaleOverlay>())
            return;

        _overlayMan.RemoveOverlay(_overlay);
    }
}
