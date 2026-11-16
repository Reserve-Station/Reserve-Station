using System.Numerics;
using Content.Client.Graphics;
using Content.Shared.Cuffs.Components;
using Content.Shared.Item;
using Content.Shared.Trigger.Components.Triggers;
using Content.Shared.Weapons.Ranged.Components;
using Robust.Client.Animations;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Shared.Animations;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Client.Revenant;

public sealed partial class RevenantAnimatedSystem : EntitySystem
{
    [Dependency] private SharedPointLightSystem _lights = default!;
    [Dependency] private AnimationPlayerSystem _animation = default!; // Reserve edit
    [Dependency] private SpriteSystem _sprite = default!; // Reserve edit

    [Dependency] private IPrototypeManager _prototype = default!; // Reserve edit

    // Reserve edit start - outline and sparkles
    private const string FloatAnimationKey = "revenant-animate-float";
    private const string OutlineShaderId = "revenant-animate-outline";
    private const string SparklesLayerKey = "revenant-animate-sparkles";
    private static readonly ProtoId<ShaderPrototype> OutlineShader = "SelectionOutline";
    private static readonly ResPath SparklesRsi = new("_Reserve/Effects/revenant_animate.rsi");
    // Reserve edit end

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<RevenantAnimatedComponent, ComponentStartup>(OnStartup);
        SubscribeLocalEvent<RevenantAnimatedComponent, ComponentShutdown>(OnShutdown);
        SubscribeLocalEvent<RevenantAnimatedComponent, AnimationCompletedEvent>(OnAnimationCompleted); // Reserve edit
        SubscribeLocalEvent<RevenantAnimatedComponent, GettingPickedUpAttemptEvent>(OnPickupAttempt); // Reserve edit
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var enumerator = EntityQueryEnumerator<RevenantAnimatedComponent>();

        while (enumerator.MoveNext(out var uid, out var comp))
        {
            if (comp.LightOverlay == null)
                continue;
            comp.Accumulator += frameTime;
            // Reserve edit start - the glow never fully fades out, the outline pulses with it
            var pulse = Math.Abs((float) Math.Sin(0.25 * Math.PI * comp.Accumulator));
            _lights.SetEnergy(comp.LightOverlay.Value.Owner, 0.7f + 1.5f * pulse, comp.LightOverlay.Value.Comp);
            comp.Outline?.SetParameter("outline_color", comp.OutlineColor.WithAlpha(0.5f + 0.5f * pulse));
            // Reserve edit end
        }
    }

    private void OnStartup(EntityUid uid, RevenantAnimatedComponent comp, ComponentStartup args)
    {
        var lightEnt = Spawn(null, new EntityCoordinates(uid, 0, 0));
        var light = AddComp<PointLightComponent>(lightEnt);

        comp.LightOverlay = (lightEnt, light);

        _lights.SetEnabled(lightEnt, true, light);
        _lights.SetColor(lightEnt, comp.LightColor, light);
        _lights.SetRadius(lightEnt, comp.LightRadius, light);

        PlayFloatAnimation(uid, comp); // Reserve edit
        AddOutlineAndSparkles(uid, comp); // Reserve edit
    }

    private void OnShutdown(EntityUid uid, RevenantAnimatedComponent comp, ComponentShutdown args)
    {
        if (comp.LightOverlay != null)
            Del(comp.LightOverlay);

        // Reserve edit start - put the item back on the floor
        _animation.Stop(uid, FloatAnimationKey);
        if (TryComp<SpriteComponent>(uid, out var sprite))
        {
            _sprite.SetOffset((uid, sprite), Vector2.Zero);
            _sprite.RemovePostShader((uid, sprite), OutlineShaderId);
            if (_sprite.LayerMapTryGet((uid, sprite), SparklesLayerKey, out var layer, false))
                _sprite.RemoveLayer((uid, sprite), layer);
        }
        comp.Outline = null;
        // Reserve edit end
    }

    // Reserve edit start - outline and sparkles
    /// <summary>
    /// Draws a purple outline around the item and puts twinkling stars around it.
    /// </summary>
    private void AddOutlineAndSparkles(EntityUid uid, RevenantAnimatedComponent comp)
    {
        if (!TryComp<SpriteComponent>(uid, out var sprite))
            return;

        comp.Outline = _prototype.Index(OutlineShader).InstanceUnique();
        comp.Outline.SetParameter("outline_width", comp.OutlineWidth);
        comp.Outline.SetParameter("outline_color", comp.OutlineColor);
        comp.Outline.SetParameter("outline_fullbright", true);

        // Drawn before the hover outline, so hovering the item still shows the usual interaction outline on top.
        _sprite.SetPostShader((uid, sprite), new SpriteComponent.PostShaderArgs(OutlineShaderId, comp.Outline)
        {
            Before = ContentPostShaderIds.BeforeOutlines,
            After = ContentPostShaderIds.AfterBaseEffects,
        });

        if (_sprite.LayerMapTryGet((uid, sprite), SparklesLayerKey, out _, false))
            return;

        var layer = _sprite.AddLayer((uid, sprite), new PrototypeLayerData
        {
            RsiPath = SparklesRsi.ToString(),
            State = "sparkles",
            Shader = "unshaded",
        }, null);
        _sprite.LayerMapSet((uid, sprite), SparklesLayerKey, layer);
    }
    // Reserve edit end

    // Reserve edit start - hover animation and pickup block
    /// <summary>
    /// Restarts the hover animation so it loops for as long as the item is animated.
    /// </summary>
    private void OnAnimationCompleted(Entity<RevenantAnimatedComponent> ent, ref AnimationCompletedEvent args)
    {
        if (args.Key != FloatAnimationKey || ent.Comp.LifeStage > ComponentLifeStage.Running)
            return;

        PlayFloatAnimation(ent, ent.Comp);
    }

    /// <summary>
    /// Predicts the server's pickup block so the item doesn't flicker into the player's hand.
    /// </summary>
    private void OnPickupAttempt(Entity<RevenantAnimatedComponent> ent, ref GettingPickedUpAttemptEvent args)
    {
        // Must match the server: handcuffs and grenades jump into hands and pockets on their own.
        if (!HasComp<GunComponent>(ent) && (HasComp<HandcuffComponent>(ent) || HasComp<TriggerOnUseComponent>(ent)))
            return;

        args.Cancel();
    }

    /// <summary>
    /// Makes the item bob up and down above the floor.
    /// </summary>
    private void PlayFloatAnimation(EntityUid uid, RevenantAnimatedComponent comp)
    {
        if (_animation.HasRunningAnimation(uid, FloatAnimationKey))
            return;

        var low = new Vector2(0, comp.FloatHeight);
        var high = new Vector2(0, comp.FloatHeight + comp.FloatAmplitude);

        var animation = new Animation
        {
            Length = TimeSpan.FromSeconds(comp.FloatTime * 2),
            AnimationTracks =
            {
                new AnimationTrackComponentProperty
                {
                    ComponentType = typeof(SpriteComponent),
                    Property = nameof(SpriteComponent.Offset),
                    InterpolationMode = AnimationInterpolationMode.Cubic,
                    KeyFrames =
                    {
                        new AnimationTrackProperty.KeyFrame(low, 0f),
                        new AnimationTrackProperty.KeyFrame(high, comp.FloatTime),
                        new AnimationTrackProperty.KeyFrame(low, comp.FloatTime),
                    },
                },
            },
        };

        _animation.Play(uid, animation, FloatAnimationKey);
    }
    // Reserve edit end
}
