using System.Numerics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Maths;

namespace Content.Client._Reserve.NewPlayerGuide;

/// <summary>
///     Arranges visible children into equal-width columns so wide content wraps instead of starving siblings.
/// </summary>
public sealed class EqualColumnsContainer : Container
{
    private int _separation = 8;

    public int Separation
    {
        get => _separation;
        set
        {
            if (_separation == value)
                return;

            _separation = value;
            InvalidateMeasure();
        }
    }

    protected override Vector2 MeasureOverride(Vector2 availableSize)
    {
        var visibleCount = 0;
        foreach (var child in Children)
        {
            if (child.Visible)
                visibleCount++;
        }

        if (visibleCount == 0)
            return Vector2.Zero;

        var totalSeparation = Math.Max(0, _separation) * (visibleCount - 1);
        var colWidth = float.IsFinite(availableSize.X)
            ? Math.Max(0f, (availableSize.X - totalSeparation) / visibleCount)
            : float.PositiveInfinity;
        var childAvailable = new Vector2(colWidth, availableSize.Y);

        var maxChildWidth = 0f;
        var maxChildHeight = 0f;
        foreach (var child in Children)
        {
            if (!child.Visible)
                continue;

            child.Measure(childAvailable);
            maxChildWidth = Math.Max(maxChildWidth, child.DesiredSize.X);
            maxChildHeight = Math.Max(maxChildHeight, child.DesiredSize.Y);
        }

        return new Vector2(maxChildWidth * visibleCount + totalSeparation, maxChildHeight);
    }

    protected override Vector2 ArrangeOverride(Vector2 finalSize)
    {
        var visibleCount = 0;
        foreach (var child in Children)
        {
            if (child.Visible)
                visibleCount++;
        }

        if (visibleCount == 0)
            return finalSize;

        var separation = Math.Max(0, _separation);
        var totalSeparation = separation * (visibleCount - 1);
        var colWidth = Math.Max(0f, (finalSize.X - totalSeparation) / visibleCount);

        var col = 0;
        foreach (var child in Children)
        {
            if (!child.Visible)
                continue;

            var x = col * (colWidth + separation);
            var width = col == visibleCount - 1
                ? Math.Max(0f, finalSize.X - x)
                : colWidth;

            child.Arrange(UIBox2.FromDimensions(x, 0f, width, finalSize.Y));
            col++;
        }

        return finalSize;
    }
}
