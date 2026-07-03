// SPDX-FileCopyrightText: 2025 GoobBot <uristmchands@proton.me>
// SPDX-FileCopyrightText: 2025 Roudenn <romabond091@gmail.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Goobstation.Shared.SpecialAnimation;
using Robust.Client.GameObjects;
using Robust.Client.Graphics;
using Robust.Client.Player;
using Robust.Client.ResourceManagement;
using Robust.Client.UserInterface;
using Robust.Shared;
using Robust.Shared.Configuration;
using Robust.Shared.Timing;
using Robust.Shared.Utility;
using System.Numerics;  // Reserve edit: Fix special animation for cards

namespace Content.Goobstation.Client.Overlays;

public sealed class SpecialAnimationOverlay : Overlay
{
    [Dependency] private readonly IPlayerManager _player = default!;
    [Dependency] private readonly IResourceCache _cache = default!;
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly IClyde _clyde = default!;
    [Dependency] private readonly IConfigurationManager _configManager = default!;
    [Dependency] private readonly IUserInterfaceManager _uiManager = default!;
    [Dependency] private readonly IEntityManager _entity = default!;  // Reserve edit: Fix special animation for cards

    private readonly SpriteSystem _sprite;

    public Queue<SpecialAnimationData> AnimationQueue = new();

    private SpecialAnimationData? _currentAnimation;

    private IRenderTexture? _target;  // Reserve edit: Fix special animation for cards

    private (Font Font, string Path, int Size)? _font;

    public SpecialAnimationOverlay(SpriteSystem spriteSys)
    {
        IoCManager.InjectDependencies(this);

        var path = SpecialAnimationData.DefaultAnimation.TextFontPath;
        var size = SpecialAnimationData.DefaultAnimation.TextFontSize;

        _font = (new VectorFont(_cache.GetResource<FontResource>(path), size), path, size);
        _sprite = spriteSys;
        ZIndex = 102;
    }

    protected override void Draw(in OverlayDrawArgs args)
    {
        if (_player.LocalEntity == null)
            return;

        // Set current animation if we don't have any
        if (_currentAnimation is null)
        {
            if (!AnimationQueue.TryDequeue(out _currentAnimation))
                return;

            if (!StartupAnimation(_currentAnimation))
                return;
        }

        var curTime = _timing.RealTime;

        DebugTools.Assert(_currentAnimation.TotalDuration > _currentAnimation.FadeInDuration + _currentAnimation.FadeOutDuration);

        var endTime = _currentAnimation.StartTime + TimeSpan.FromSeconds(_currentAnimation.TotalDuration);

        // The animation is over, kill it
        if (endTime < curTime)
        {
            _currentAnimation = null;
            return;
        }

        CalculateAnimation(_currentAnimation);

        // Draw everything on screen
        var screen = args.ScreenHandle;
        var center = _clyde.ScreenSize / 2;
        var uiScale = _configManager.GetCVar(CVars.DisplayUIScale);

        if (uiScale == 0f)
            uiScale = _uiManager.DefaultUIScale;

        var opacity = _currentAnimation.Opacity;
        // Reserve edit start: Reserve cards
        var anime = GetAnimationEntity(_currentAnimation);

        if (anime != null)
        {
            EntityUid a = (EntityUid) anime;
            var targetSize = args.Viewport.RenderTarget.Size;
            if (_target?.Size != targetSize)
            {
                _target = _clyde
                    .CreateRenderTarget(targetSize,
                        new RenderTargetFormatParameters(RenderTargetColorFormat.Rgba8Srgb),
                        name: nameof(SpecialAnimationOverlay));
            }

            screen.RenderInRenderTarget(_target,
                () =>
            {
                screen.DrawEntity(
                    a,
                    center + _currentAnimation.Position,
                    Vector2.One * uiScale * _currentAnimation.Scale,
                    Angle.Zero,
                    Angle.Zero,
                    Direction.South,
                    _entity.GetComponent<SpriteComponent>(a));
            },
                Color.Transparent);
            screen.DrawTexture(_target.Texture, Vector2.Zero, Color.White.WithAlpha(opacity));
        }
        else
        {
            var texture = _sprite.Frame0(_currentAnimation.Sprite);
            var box = UIBox2.FromDimensions(center + _currentAnimation.Position, texture.Size * uiScale * _currentAnimation.Scale);
            screen.DrawTextureRect(texture, box, Color.White.WithAlpha(opacity));
        }
        // Reserve edit end: Reserve cards

        // Render text
        if (_currentAnimation.Text == null)
            return;

        // Change our font if required
        if (_font == null ||
            _font.Value.Path != _currentAnimation.TextFontPath ||
            _font.Value.Size != _currentAnimation.TextFontSize)
        {
            var path = _currentAnimation.TextFontPath;
            var size = _currentAnimation.TextFontSize;
            _font = (new VectorFont(_cache.GetResource<FontResource>(path), size), path, size);
        }

        screen.DrawString(
            _font.Value.Font,
            center + _currentAnimation.TextPosition,
            _currentAnimation.Text,
            _currentAnimation.TextOverrideColor.WithAlpha(opacity));
    }

    private bool StartupAnimation(SpecialAnimationData animation)
    {
        animation.Position = animation.StartPosition;
        animation.StartTime = _timing.RealTime;
        animation.LastTime = _timing.RealTime;
        animation.Opacity = 0f;
        return true;
    }

    private void CalculateAnimation(SpecialAnimationData animation)
    {
        var curTime = _timing.RealTime;
        var frameTime = (float) (curTime - animation.LastTime).TotalSeconds;
        var fadeInEndTime = animation.StartTime + TimeSpan.FromSeconds(animation.FadeInDuration);
        var fadeOutStartTime = animation.StartTime + TimeSpan.FromSeconds(animation.TotalDuration) - TimeSpan.FromSeconds(animation.FadeOutDuration);

        // Move the sprite
        var distanceVec = animation.EndPosition - animation.StartPosition;
        var moveAmount = distanceVec * frameTime / animation.TotalDuration;
        animation.Position += moveAmount;

        // Fade-in
        if (fadeInEndTime > curTime)
        {
            var fadeInOpacityChange = animation.MaxOpacity / animation.FadeInDuration;
            animation.Opacity += fadeInOpacityChange * frameTime;
            animation.Opacity = MathF.Min(animation.Opacity, animation.MaxOpacity);
        }

        // Fade-out
        if (fadeOutStartTime < curTime)
        {
            var fadeOutOpacityChange = animation.MaxOpacity / animation.FadeOutDuration;
            animation.Opacity -= fadeOutOpacityChange * frameTime;
            animation.Opacity = MathF.Max(animation.Opacity, 0f);
        }

        animation.LastTime = curTime;
    }

    // Reserve edit start: Reserve cards
    private EntityUid? GetAnimationEntity(SpecialAnimationData animation)
    {
        if (animation.Source == null)
            return null;

        var source = _entity.GetEntity(animation.Source);

        if (!_entity.TryGetComponent<SpriteComponent>(source, out var sourceSprite))
            return null;

        // Copy the sprite component from source to the dummy entity.
        var dummyEnt = _entity.Spawn();
        var dummySprite = _entity.EnsureComponent<SpriteComponent>(dummyEnt);
        dummySprite.CopyFrom(sourceSprite);
        return dummyEnt;
    }
    // Reserve edit end: Reserve cards
}
