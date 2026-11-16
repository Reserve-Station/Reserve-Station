// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Client.Guidebook.Controls;
using Content.Client.Guidebook.Richtext;
using Content.Goobstation.Shared.Factory;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Prototypes;
using System.Diagnostics.CodeAnalysis;

namespace Content.Goobstation.Client.Guidebook.Controls;

/// <summary>
/// Lists all entities with <see cref="AutomationSlotsComponent"/>.
/// </summary>
public sealed partial class GuideAutomationSlotsEmbed : IDocumentTag
{
    [Dependency] private IEntityManager _entMan = default!;
    [Dependency] private IPrototypeManager _proto = default!; // Reserve edit
    private readonly AutomationSystem _automation;

    // Reserve edit start: grid with captions instead of a narrow vertical list
    private const int Columns = 4;
    private const float CellWidth = 130f;
    // Reserve edit end

    public GuideAutomationSlotsEmbed()
    {
        IoCManager.InjectDependencies(this);

        _automation = _entMan.System<AutomationSystem>();
    }

    bool IDocumentTag.TryParseTag(Dictionary<string, string> args, [NotNullWhen(true)] out Control? control)
    {
        // Reserve edit start: grid with captions instead of a narrow vertical list
        var grid = new GridContainer()
        {
            Columns = Columns,
            HorizontalAlignment = Control.HAlignment.Center
        };
        foreach (var id in _automation.Automatable)
        {
            var name = _proto.TryIndex(id, out var proto) ? proto.Name : id.Id;
            var cell = new BoxContainer()
            {
                Orientation = BoxContainer.LayoutOrientation.Vertical,
                SetWidth = CellWidth,
                ToolTip = name
            };
            cell.AddChild(new GuideEntityEmbed(id, false, true) { HorizontalAlignment = Control.HAlignment.Center });
            cell.AddChild(new Label()
            {
                Text = name,
                ClipText = true,
                HorizontalAlignment = Control.HAlignment.Center
            });
            grid.AddChild(cell);
        }

        control = grid;
        // Reserve edit end
        return true;
    }
}
