// SPDX-License-Identifier: MIT

using System.Numerics;
using Content.Client.Gameplay;
using Content.Goobstation.Common.CCVar;
using Content.Shared.Popups;
using Robust.Client.Graphics;
using Robust.Client.ResourceManagement;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controllers;
using Robust.Shared.Configuration;

namespace Content.Client.Popups;

/// <summary>
/// Handles screens-space popups. World popups are handled via PopupOverlay.
/// </summary>
public sealed class PopupUIController : UIController, IOnStateEntered<GameplayState>, IOnStateExited<GameplayState>
{
    [UISystemDependency] private readonly PopupSystem? _popup = default!;

    private IConfigurationManager _configManager = default!;  // Reserve edit: Upstream 20261020
    private Font _smallFont = default!;
    private Font _mediumFont = default!;
    private Font _largeFont = default!;
    private Font _runechatSmallFont = default!;  // Reserve edit: Upstream 20261020
    private Font _runechatMediumFont = default!;  // Reserve edit: Upstream 20261020
    private Font _runechatLargeFont = default!;  // Reserve edit: Upstream 20261020

    private PopupRootControl? _popupControl;

    public override void Initialize()
    {
        base.Initialize();
        var cache = IoCManager.Resolve<IResourceCache>();
        // Reserve edit start: Upstream 20261020 - Popup font should check the Runechat setting
        // _smallFont = new VectorFont(cache.GetResource<FontResource>("/Fonts/_ES/Grand9KPixelRegular.ttf"), 10); // Goob
        // _mediumFont = new VectorFont(cache.GetResource<FontResource>("/Fonts/_ES/Grand9KPixelRegular.ttf"), 12); // Goob
        // _largeFont = new VectorFont(cache.GetResource<FontResource>("/Fonts/_ES/Grand9KPixelRegular.ttf"), 14); // Goob
        _configManager = IoCManager.Resolve<IConfigurationManager>();

        var defaultFont = cache.GetResource<FontResource>("/Fonts/NotoSans/NotoSans-Regular.ttf");
        var runechatFont = cache.GetResource<FontResource>("/Fonts/_ES/Grand9KPixelRegular.ttf");
        _smallFont = new VectorFont(defaultFont, 10);
        _mediumFont = new VectorFont(defaultFont, 12);
        _largeFont = new VectorFont(defaultFont, 14);
        _runechatSmallFont = new VectorFont(runechatFont, 10);
        _runechatMediumFont = new VectorFont(runechatFont, 12);
        _runechatLargeFont = new VectorFont(runechatFont, 14);
        // Reserve edit end: Upstream 20261020 - Popup font should check the Runechat setting
    }

    public void OnStateEntered(GameplayState state)
    {
        _popupControl = new PopupRootControl(_popup, this);

        UIManager.RootControl.AddChild(_popupControl);
    }

    public void OnStateExited(GameplayState state)
    {
        if (_popupControl == null)
            return;

        UIManager.RootControl.RemoveChild(_popupControl);
        _popupControl = null;
    }

    public void DrawPopup(PopupSystem.PopupLabel popup, DrawingHandleScreen handle, Vector2 position, float scale)
    {
        var lifetime = PopupSystem.GetPopupLifetime(popup);

        // Keep alpha at 1 until TotalTime passes half its lifetime, then gradually decrease to 0.
        var alpha = MathF.Min(1f, 1f - MathF.Max(0f, popup.TotalTime - lifetime / 2) * 2 / lifetime);

        var updatedPosition = position - new Vector2(0f, MathF.Min(8f, 12f * (popup.TotalTime * popup.TotalTime + popup.TotalTime)));
        // var font = _smallFont;  // Reserve edit: Upstream 20261020
        var useRunechat = _configManager.GetCVar(GoobCVars.ChatRunechat);  // Reserve edit: Upstream 20261020
        var font = useRunechat ? _runechatSmallFont : _smallFont;  // Reserve edit: Upstream 20261020
        var color = Color.White.WithAlpha(alpha);
        var outline = new TextOutline(2f, Color.Black.WithAlpha(alpha)); // Goob

        switch (popup.Type)
        {
            case PopupType.SmallCaution:
                color = Color.Red;
                break;
            case PopupType.Medium:
                font = useRunechat ? _runechatMediumFont : _mediumFont;  // Reserve edit: Upstream 20261020
                color = Color.LightGray;
                break;
            case PopupType.MediumCaution:
                font = useRunechat ? _runechatMediumFont : _mediumFont;  // Reserve edit: Upstream 20261020
                color = Color.Red;
                break;
            case PopupType.Large:
                font = useRunechat ? _runechatLargeFont : _largeFont;  // Reserve edit: Upstream 20261020
                color = Color.LightGray;
                break;
            case PopupType.LargeCaution:
                font = useRunechat ? _runechatLargeFont : _largeFont;  // Reserve edit: Upstream 20261020
                color = Color.Red;
                break;
        }

        var dimensions = handle.GetDimensions(font, popup.Text, scale);
        handle.DrawString(font, updatedPosition - dimensions / 2f + popup.RandomOffset, popup.Text, scale, color.WithAlpha(alpha), useRunechat ? outline : null); // Goob  // Reserve edit: Upstream 20261020
    }

    /// <summary>
    /// Handles drawing all screen popups.
    /// </summary>
    private sealed class PopupRootControl : Control
    {
        private readonly PopupSystem? _popup;
        private readonly PopupUIController _controller;

        public PopupRootControl(PopupSystem? system, PopupUIController controller)
        {
            _popup = system;
            _controller = controller;
        }

        protected override void Draw(DrawingHandleScreen handle)
        {
            base.Draw(handle);

            if (_popup == null)
                return;

            // Different window
            var windowId = UserInterfaceManager.RootControl.Window.Id;

            foreach (var popup in _popup.CursorLabels)
            {
                if (popup.InitialPos.Window != windowId)
                    continue;

                _controller.DrawPopup(popup, handle, popup.InitialPos.Position, UIScale);
            }
        }
    }
}
