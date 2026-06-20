// SPDX-FileCopyrightText: 2026 Space Station 14 Contributors
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Client.Guidebook.RichText;
using Content.Client.UserInterface.ControlExtensions;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Input;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;

namespace Content.Client.Guidebook.Controls;

public sealed class GuidebookCrossRefLabel : Control, IGuidebookCrossReference
{
    private static readonly Color HoverColor = Color.LightSkyBlue;

    private bool _isActive;
    private FormattedMessage? _message;
    private readonly RichTextLabel _label;

    public IPrototype? TargetPrototype { get; set; }

    public GuidebookCrossRefLabel()
    {
        MouseFilter = MouseFilterMode.Pass;
        OnKeyBindDown += OnClick;
        OnMouseEntered += _ => RefreshAppearance(hovered: true);
        OnMouseExited += _ => RefreshAppearance(hovered: false);

        _label = new RichTextLabel();
        AddChild(_label);
    }

    public void SetMessage(FormattedMessage message)
    {
        _message = message;
        RefreshAppearance(hovered: false);
    }

    public void ActivateCrossReference()
    {
        if (_message == null)
            return;

        _isActive = true;
        MouseFilter = MouseFilterMode.Stop;
        DefaultCursorShape = CursorShape.Hand;
        RefreshAppearance(hovered: false);
    }

    private void RefreshAppearance(bool hovered)
    {
        if (_message == null)
            return;

        if (!_isActive)
        {
            _label.SetMessage(_message, tagsAllowed: null);
            return;
        }

        _label.SetMessage(_message, null, hovered ? HoverColor : TextLinkTag.LinkColor);
    }

    private void OnClick(GUIBoundKeyEventArgs args)
    {
        if (!_isActive || args.Function != EngineKeyFunctions.UIClick)
            return;

        if (this.TryGetParentHandler<IGuidebookCrossReferenceNavigator>(out var navigator))
        {
            navigator.NavigateToCrossReferenceTarget(this);
            args.Handle();
            return;
        }

        Logger.Warning("Guidebook cross-reference click had no navigator in the visual tree.");
    }
}
