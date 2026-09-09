// SPDX-License-Identifier: AGPL-3.0-or-later

using Robust.Shared.Prototypes;

namespace Content.Shared._Reserve.Antag;

[Prototype]
public sealed partial class AntagonistPrototype : IPrototype
{
    [IdDataField] public string ID { set; get; } = default!;

    /// <summary>
    ///     Name string to display in ghost teleport menu .
    /// </summary>
    [DataField(required: true)]
    public string Name = default!;

    /// <summary>
    ///     Description string to display in the character menu as an explanation of the department's function.
    /// </summary>
    [DataField(required: true)]
    public string Description = default!;

    /// <summary>
    ///     Description string to display in the character menu as an explanation of the department's function.
    /// </summary>
    [DataField(required: true)]
    public int Weight = default!;
}
