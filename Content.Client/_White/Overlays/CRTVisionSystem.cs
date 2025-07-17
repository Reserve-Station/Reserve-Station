// SPDX-FileCopyrightText: 2025 Kutosss <162154227+Kutosss@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 ReserveBot <211949879+ReserveBot@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Spatison <137375981+Spatison@users.noreply.github.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Shared._White.Overlays;
using Content.Shared.Damage;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Content.Shared.Mobs.Systems;
using Content.Shared.Stunnable;
using Content.Shared.Weapons.Melee.Events;
using Robust.Client.Graphics;
using Robust.Shared.Player;

namespace Content.Client._White.Overlays;

// ReSharper disable once InconsistentNaming
public sealed class CRTVisionSystem : EntitySystem
{
    [Dependency] private readonly IOverlayManager _overlayMan = default!;
    [Dependency] private readonly ISharedPlayerManager _playerMan = default!;
    [Dependency] private readonly IEntityManager _entityManager = default!;

    private CRTVisionOverlay _overlay = default!;

    // For health tracking
    private float _healthPercentage = 1.0f;

    // Reserve edit start - magic numbers have been converted to constants
    // Constants for glitch effects
    private const float DamageGlitchIntensityFactor = 0.016f; // 0.8f / 50.0f
    private const float DamageGlitchDuration = 0.4f;

    // Constants for different types of effects
    private const float CriticalStateGlitchIntensity = 1.5f;
    private const float CriticalStateGlitchDuration = 0.5f;

    private const float ThresholdGlitchIntensity = 0.5f;
    private const float ThresholdGlitchDuration = 0.3f;

    private const float AttackGlitchIntensity = 0.5f;
    private const float AttackGlitchDuration = 0.35f;

    private const float StunGlitchIntensity = 0.7f;
    private const float StunGlitchDuration = 0.4f;
    // Reserve edit end

    public override void Initialize()
    {
        base.Initialize();

        _overlay = new();

        SubscribeLocalEvent<CRTVisionOverlayComponent, ComponentInit>(OnComponentChange);
        SubscribeLocalEvent<CRTVisionOverlayComponent, ComponentShutdown>(OnComponentChange);
        SubscribeLocalEvent<CRTVisionOverlayComponent, LocalPlayerAttachedEvent>(OnPlayerStateChange);
        SubscribeLocalEvent<CRTVisionOverlayComponent, LocalPlayerDetachedEvent>(OnPlayerStateChange);

        // Subscribe to damage events
        SubscribeLocalEvent<CRTVisionOverlayComponent, DamageChangedEvent>(OnDamageChanged);

        // Subscribe to mob state change events
        SubscribeLocalEvent<CRTVisionOverlayComponent, MobStateChangedEvent>(OnMobStateChanged);

        // Subscribe to health threshold events
        SubscribeLocalEvent<CRTVisionOverlayComponent, MobThresholdChecked>(OnThresholdChecked);

        // Subscribe to attack and stun events
        SubscribeLocalEvent<CRTVisionOverlayComponent, AttackedEvent>(OnAttacked);
        SubscribeLocalEvent<CRTVisionOverlayComponent, StunnedEvent>(OnStunned);
    }

    private void OnComponentChange<T>(EntityUid uid, T component, EntityEventArgs args) where T: IComponent
    {
        if (uid == _playerMan.LocalEntity)
            UpdateOverlayState();
    }

    private void OnPlayerStateChange<T>(EntityUid uid, CRTVisionOverlayComponent component, T args)
    {
        UpdateOverlayState();
    }

    private void UpdateOverlayState()
    {
        var player = _playerMan.LocalEntity;
        if (player == null || !EntityManager.HasComponent<CRTVisionOverlayComponent>(player))
        {
            _overlayMan.RemoveOverlay(_overlay);
            return;
        }

        UpdateHealthPercentage(player.Value);


        if (!_overlayMan.HasOverlay<CRTVisionOverlay>())
            _overlayMan.AddOverlay(_overlay);
    }

    // Process damage event
    private void OnDamageChanged(EntityUid uid, CRTVisionOverlayComponent component, DamageChangedEvent args)
    {
        if (uid != _playerMan.LocalEntity)
            return;

        // Check if it was damage and not healing
        if (args is { DamageIncreased: true, DamageDelta: not null, })
        {
            var damageAmount = (float) args.DamageDelta.GetTotal();
            TriggerImpactEffect(damageAmount);
        }

        // Update health percentage for glitch effects
        UpdateOverlayState();
    }

    // Update health percentage for glitch effects
    private void UpdateHealthPercentage(EntityUid uid)
    {
        if (!_entityManager.TryGetComponent<DamageableComponent>(uid, out var damageable) ||
            !_entityManager.HasComponent<MobThresholdsComponent>(uid))
            return;

        // Get critical threshold
        var mobThresholdSystem = _entityManager.System<MobThresholdSystem>();
        if (!mobThresholdSystem.TryGetIncapThreshold(uid, out var threshold))
            return;

        // Calculate health percentage (1.0 = full health, 0.0 = critical state)
        _healthPercentage = 1.0f - Math.Min(1.0f, (damageable.TotalDamage / threshold.Value).Float());

        // Pass health percentage to shader
        _overlay.SetHealthPercentage(_healthPercentage);
    }

    // Handle mob state change (e.g., transition from Normal to Critical)
    private void OnMobStateChanged(EntityUid uid, CRTVisionOverlayComponent component, MobStateChangedEvent args)
    {
        if (uid != _playerMan.LocalEntity)
            return;

        // If state worsened, show impact effect
        if (args.NewMobState > args.OldMobState)
            TriggerImpactEffect(20.0f); // Stronger effect on state change

        // Trigger a strong glitch effect when entering critical state
        if (args.NewMobState == MobState.Critical)
            _overlay.SetTemporaryGlitchEffect(CriticalStateGlitchIntensity, CriticalStateGlitchDuration); // Reserve edit

        // Update health percentage for glitch effects
        UpdateOverlayState();
    }

    // Handle health threshold check
    private void OnThresholdChecked(EntityUid uid, CRTVisionOverlayComponent component, MobThresholdChecked args)
    {
        if (uid != _playerMan.LocalEntity)
            return;

        // Trigger impact effect on health threshold check
        TriggerImpactEffect(15.0f);

        // When crossing a new threshold, trigger a medium glitch effect
        _overlay.SetTemporaryGlitchEffect(ThresholdGlitchIntensity, ThresholdGlitchDuration); // Reserve edit

        // Update health percentage for glitch effects
        UpdateOverlayState();
    }

    // Handle attack on player
    private void OnAttacked(EntityUid uid, CRTVisionOverlayComponent component, AttackedEvent args)
    {
        if (uid != _playerMan.LocalEntity)
            return;

        // Trigger a small glitch effect on every attack
        _overlay.SetTemporaryGlitchEffect(AttackGlitchIntensity, AttackGlitchDuration); // Reserve edit
    }

    // Handle stun event
    private void OnStunned(EntityUid uid, CRTVisionOverlayComponent component, StunnedEvent args)
    {
        if (uid != _playerMan.LocalEntity)
            return;

        // Trigger a medium glitch effect on stun
        _overlay.SetTemporaryGlitchEffect(StunGlitchIntensity, StunGlitchDuration); // Reserve edit
    }

    private void TriggerImpactEffect(float intensity)
    {
        var player = _playerMan.LocalEntity;
        if (player == null)
            return;

        // Trigger a temporary glitch effect proportional to damage
        var glitchIntensity = Math.Min(intensity * DamageGlitchIntensityFactor, 1.0f); // Reserve edit
        _overlay.SetTemporaryGlitchEffect(glitchIntensity, DamageGlitchDuration); // Reserve edit
    }
}
