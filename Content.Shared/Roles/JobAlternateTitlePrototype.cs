// SPDX-License-Identifier: AGPL-3.0-or-later

using Robust.Shared.Enums;
using Robust.Shared.Prototypes;

namespace Content.Shared.Roles;

[Prototype]
public sealed partial class JobAlternateTitlePrototype : IPrototype
{
    [IdDataField]
    public string ID { get; private set; } = default!;

    [DataField]
    public string Name = default!;

    [DataField]
    public string? FemaleName = default!;

    [DataField]
    public string? MaleName = default!;

    public string LocalizedName(Gender? gender)
    {
        switch (gender)
        {
            case Gender.Female:
                return Loc.GetString(FemaleName ?? Name);
            case Gender.Male:
                return Loc.GetString(MaleName ?? Name);
            default:
                return Loc.GetString(Name);
        }
    }

    [DataField]
    public HashSet<JobRequirement>? Requirements;
}
