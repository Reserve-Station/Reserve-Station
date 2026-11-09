// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Shared.Objectives.Components;
using Robust.Shared.Random;

namespace Content.Server._Reserve.Objectives;

/// <summary>
/// Fills a fluff objective title with a random department.
/// </summary>
public sealed class PickObjectiveDepartmentSystem : EntitySystem
{
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly MetaDataSystem _metaData = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<PickObjectiveDepartmentComponent, ObjectiveAssignedEvent>(OnAssigned);
        SubscribeLocalEvent<PickObjectiveDepartmentComponent, ObjectiveAfterAssignEvent>(OnAfterAssign);
    }

    private void OnAssigned(Entity<PickObjectiveDepartmentComponent> ent, ref ObjectiveAssignedEvent args)
    {
        if (ent.Comp.Departments.Count == 0)
            args.Cancelled = true;
    }

    private void OnAfterAssign(Entity<PickObjectiveDepartmentComponent> ent, ref ObjectiveAfterAssignEvent args)
    {
        if (ent.Comp.Departments.Count == 0)
            return;

        var department = _random.Pick(ent.Comp.Departments);
        var title = Loc.GetString(ent.Comp.Title, ("department", department));
        _metaData.SetEntityName(ent.Owner, title, args.Meta);
    }
}
