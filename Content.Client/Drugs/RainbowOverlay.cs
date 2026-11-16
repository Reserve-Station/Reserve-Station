// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Shared.CCVar;
using Content.Shared.Drugs;
using Content.Shared.StatusEffectNew;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Shared.Configuration;
using Robust.Shared.Enums;
using Robust.Shared.Prototypes;
using Robust.Shared.Timing;

namespace Content.Client.Drugs;

public sealed class RainbowOverlay : Overlay
{
    private static readonly ProtoId<ShaderPrototype> Shader = "Rainbow";

    [Dependency] private readonly IConfigurationManager _config = default!;
    [Dependency] private readonly IEntityManager _entityManager = default!;
    [Dependency] private readonly IPrototypeManager _prototypeManager = default!;
    [Dependency] private readonly IPlayerManager _playerManager = default!;
    [Dependency] private readonly IEntitySystemManager _sysMan = default!;
    [Dependency] private readonly IGameTiming _timing = default!;
    private readonly StatusEffectsSystem _statusEffects = default!;

    public override OverlaySpace Space => OverlaySpace.WorldSpace;
    public override bool RequestScreenTexture => true;
    private readonly ShaderInstance _rainbowShader;

    public float Intoxication = 0.0f;
    public float Phase = 0.0f;

    // Reserve edit start: smooth rainbow intensity
    /// <summary>
    ///     Remaining effect duration (in seconds) at which the overlay reaches full strength.
    /// </summary>
    private const float PowerDivisor = 120.0f;

    /// <summary>
    ///     How fast (in seconds) the intensity catches up with the remaining effect duration.
    /// </summary>
    private const float RampTime = 4.0f;
    // Reserve edit end: smooth rainbow intensity

    private float _timeScale = 0.0f;
    private float _warpScale = 0.0f;

    private float EffectScale => Math.Clamp(Intoxication / PowerDivisor, 0.0f, 1.0f); // Reserve edit: smooth rainbow intensity

    public RainbowOverlay()
    {
        IoCManager.InjectDependencies(this);

        _statusEffects = _sysMan.GetEntitySystem<StatusEffectsSystem>();

        _rainbowShader = _prototypeManager.Index(Shader).InstanceUnique();
        _config.OnValueChanged(CCVars.ReducedMotion, OnReducedMotionChanged, invokeImmediately: true);
    }

    private void OnReducedMotionChanged(bool reducedMotion)
    {
        _timeScale = reducedMotion ? 0.0f : 1.0f;
        _warpScale = reducedMotion ? 0.0f : 1.0f;
    }

    protected override void FrameUpdate(FrameEventArgs args)
    {
        var playerEntity = _playerManager.LocalEntity;

        if (playerEntity == null)
            return;

        if (!_statusEffects.TryGetEffectsEndTimeWithComp<SeeingRainbowsStatusEffectComponent>(playerEntity, out var endTime))
            return;

        // Reserve edit start: smooth rainbow intensity
        // Intensity smoothly follows the remaining duration: ramps up on start/new doses and fades out at the end.
        var timeLeft = endTime == null
            ? PowerDivisor
            : (float)(endTime.Value - _timing.CurTime).TotalSeconds;

        var target = Math.Clamp(timeLeft, 0.0f, PowerDivisor);
        Intoxication += (target - Intoxication) * Math.Min(1.0f, args.DeltaSeconds / RampTime);
        // Reserve edit end: smooth rainbow intensity
    }

    protected override bool BeforeDraw(in OverlayDrawArgs args)
    {
        if (!_entityManager.TryGetComponent(_playerManager.LocalEntity, out EyeComponent? eyeComp))
            return false;

        if (args.Viewport.Eye != eyeComp.Eye)
            return false;

        return EffectScale > 0;
    }

    protected override void Draw(in OverlayDrawArgs args)
    {
        if (ScreenTexture == null)
            return;

        var handle = args.WorldHandle;
        _rainbowShader.SetParameter("SCREEN_TEXTURE", ScreenTexture);
        _rainbowShader.SetParameter("colorScale", EffectScale);
        _rainbowShader.SetParameter("timeScale", _timeScale);
        _rainbowShader.SetParameter("warpScale", _warpScale * EffectScale);
        _rainbowShader.SetParameter("phase", Phase);
        handle.UseShader(_rainbowShader);
        handle.DrawRect(args.WorldBounds, Color.White);
        handle.UseShader(null);
    }
}