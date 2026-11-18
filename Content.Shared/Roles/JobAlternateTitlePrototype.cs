// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Shared.CCVar;
using Content.Shared.Preferences;
using Robust.Shared.Configuration;
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

public static class JobAlternateTitleExtensions
{
    /// <summary>
    /// Gets the localized alternate title the profile selected for this job,
    /// or null if none is selected or alternate titles are disabled.
    /// Used for ID cards, PDAs and station records (crew manifest).
    /// </summary>
    public static string? GetLocalizedAlternateTitle(
        this HumanoidCharacterProfile? profile,
        ProtoId<JobPrototype> job,
        IPrototypeManager prototypes,
        IConfigurationManager cfg)
    {
        if (profile == null || !cfg.GetCVar(CCVars.ICAlternateJobTitlesEnable))
            return null;

        if (!profile.JobAlternateTitles.TryGetValue(job, out var altTitleId)
            || !prototypes.TryIndex(altTitleId, out var altTitle))
            return null;

        return altTitle.LocalizedName(profile.Gender);
    }
}
