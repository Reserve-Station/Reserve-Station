// SPDX-License-Identifier: AGPL-3.0-or-later

using System.Text;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Timing;

namespace Content.Client._Reserve.UserInterface.Systems.Ghost.Controls;

/// <summary>
/// A button that scrolls its text content when hovered and the text is too long.
/// </summary>
public sealed class ScrollingTextButton : Button
{
    private const int ScrollThreshold = 17; // Start scrolling if text is longer than this
    private const float ScrollSpeed = 5f; // Characters per second

    private readonly StringBuilder _scrollBuilder = new();
    private string _originalText = string.Empty;
    private float _scrollOffset = 0f;
    private bool _isHovered = false;
    private bool _shouldScroll = false;
    private string _textWithSpacing = string.Empty;

    public ScrollingTextButton()
    {
        ClipText = true;

        OnMouseEntered += _ =>
        {
            _isHovered = true;
        };

        OnMouseExited += _ =>
        {
            _isHovered = false;
            _scrollOffset = 0f;
            base.Text = _originalText;
        };
    }

    public new string Text
    {
        get => _originalText;
        set
        {
            _originalText = value;
            _shouldScroll = value.Length >= ScrollThreshold;
            _textWithSpacing = value + "     ";
            base.Text = value;
        }
    }

    protected override void FrameUpdate(FrameEventArgs args)
    {
        base.FrameUpdate(args);

        if (!_isHovered || !_shouldScroll)
            return;

        // Character-based scrolling
        _scrollOffset += ScrollSpeed * args.DeltaSeconds;

        if (_scrollOffset >= _textWithSpacing.Length)
        {
            _scrollOffset -= _textWithSpacing.Length;
        }

        var offset = (int) _scrollOffset;
        _scrollBuilder.Clear();
        _scrollBuilder.Append(_textWithSpacing, offset, _textWithSpacing.Length - offset);
        _scrollBuilder.Append(_textWithSpacing, 0, offset);
        base.Text = _scrollBuilder.ToString();
    }
}
