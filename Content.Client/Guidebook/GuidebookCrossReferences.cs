// SPDX-FileCopyrightText: 2026 Space Station 14 Contributors
//
// SPDX-License-Identifier: AGPL-3.0-or-later

using System;
using System.Linq;
using System.Numerics;
using Content.Client.UserInterface.ControlExtensions;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Prototypes;

namespace Content.Client.Guidebook;

public interface IGuidebookEntryAnchor
{
    IPrototype? AnchorPrototype { get; }
}

public interface IGuidebookCrossReference
{
    IPrototype? TargetPrototype { get; }

    void ActivateCrossReference();
}

public interface IGuidebookCrossReferenceNavigator
{
    void NavigateToCrossReferenceTarget(IGuidebookCrossReference reference);
}

public sealed class GuidebookCrossReferenceIndex
{
    private const float ScrollTopPadding = 40f;

    private readonly Dictionary<IPrototype, List<Control>> _entryAnchors = new();

    public void BuildFromPage(Control pageRoot)
    {
        _entryAnchors.Clear();

        var pendingReferences = new List<IGuidebookCrossReference>();
        ScanControlTree(pageRoot, pendingReferences);

        var reachable = new HashSet<IPrototype>(_entryAnchors.Keys);
        foreach (var reference in pendingReferences)
        {
            if (reference.TargetPrototype is { } target && reachable.Contains(target))
                reference.ActivateCrossReference();
        }
    }

    public bool TryRevealAndScroll(IPrototype target, ScrollContainer scroll, IUserInterfaceManager uiManager)
    {
        if (!_entryAnchors.TryGetValue(target, out var anchors) || anchors.Count == 0)
            return false;

        var anchor = anchors.FirstOrDefault(a => a is Controls.GuideFoodIngredientEmbed)
            ?? anchors.FirstOrDefault(a => a is Controls.GuideMicrowaveEmbed)
            ?? anchors[0];

        if (!anchor.Visible)
            anchor.Visible = true;

        uiManager.DeferAction(() =>
        {
            if (anchor.GetControlScrollPosition() is not { } position)
                return;

            scroll.HScrollTarget = position.X;
            scroll.VScrollTarget = Math.Max(0, position.Y - ScrollTopPadding);
        });

        return true;
    }

    public void Clear()
    {
        _entryAnchors.Clear();
    }

    private void ScanControlTree(Control node, List<IGuidebookCrossReference> pendingReferences)
    {
        if (node is IGuidebookEntryAnchor { AnchorPrototype: { } anchorProto })
        {
            if (!_entryAnchors.TryGetValue(anchorProto, out var list))
            {
                list = new List<Control>();
                _entryAnchors[anchorProto] = list;
            }

            list.Add(node);
        }

        if (node is IGuidebookCrossReference crossRef)
            pendingReferences.Add(crossRef);

        foreach (var child in node.Children)
            ScanControlTree(child, pendingReferences);
    }
}
