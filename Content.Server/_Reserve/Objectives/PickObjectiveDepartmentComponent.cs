// SPDX-License-Identifier: AGPL-3.0-or-later

namespace Content.Server._Reserve.Objectives;

/// <summary>
/// Picks a department id and writes it into the objective title.
/// The title locale must branch on <c>department</c>, because Russian department names change case.
/// </summary>
[RegisterComponent, Access(typeof(PickObjectiveDepartmentSystem))]
public sealed partial class PickObjectiveDepartmentComponent : Component
{
    /// <summary>
    /// Locale id for the objective title. Passed <c>department</c> with one of <see cref="Departments"/>.
    /// </summary>
    [DataField(required: true)]
    public LocId Title;

    /// <summary>
    /// Department ids passed to the title locale as <c>department</c>.
    /// </summary>
    [DataField]
    public List<string> Departments = new()
    {
        "security",
        "medical",
        "engineering",
        "science",
        "cargo",
        "service",
        "command",
    };
}
