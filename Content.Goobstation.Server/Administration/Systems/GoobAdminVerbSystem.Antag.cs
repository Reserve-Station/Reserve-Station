// SPDX-License-Identifier: AGPL-3.0-or-later

using System.Diagnostics.CodeAnalysis;
using Content.Goobstation.Common.Blob;
using Content.Goobstation.Server.Changeling.GameTicking.Rules;
using Content.Goobstation.Server.Devil.GameTicking.Rules;
using Content.Goobstation.Server.Shadowling.Rules;
using Content.Goobstation.Server.Slasher.Components; // Reserve edit: Fix antag verbs
using Content.Server.Administration.Managers;
using Content.Server.Antag;
using Content.Shared._EinsteinEngines.Silicon.Components;
using Content.Shared.Administration;
using Content.Server.Clothing.Systems; // Reserve edit: Fix antag verbs
using Content.Shared.Database;
using Content.Shared.Mind.Components;
using Content.Shared.Verbs;
using Robust.Shared.Player;
using Robust.Shared.Prototypes; // Reserve edit: Fix antag verbs
using Robust.Shared.Utility;

namespace Content.Goobstation.Server.Administration.Systems;

public sealed partial class GoobAdminVerbSystem
{
    [Dependency] private readonly AntagSelectionSystem _antag = default!;
    [Dependency] private readonly IAdminManager _admin = default!;
    [Dependency] private readonly OutfitSystem _outfit = default!; // Reserve edit: Fix antag verbs

    private static readonly EntProtoId DefaultSlasherRule = "TokenSlasherSpawn"; // Reserve edit: Fix antag verbs

    private void AddAntagVerbs(GetVerbsEvent<Verb> args)
    {
        if (!AntagVerbAllowed(args, out var targetPlayer))
            return;

        // Changelings
        Verb ling = new()
        {
            Text = "063. " + Loc.GetString("admin-verb-text-make-changeling"), // Reserve edit: Fix antag verbs
            Category = VerbCategory.Antag,
            Icon = new SpriteSpecifier.Rsi(new ResPath("/Textures/Interface/Misc/job_icons.rsi"), "Changeling"), // Reserve edit: Fix antag verbs
            Act = () =>
            {
                if (!HasComp<SiliconComponent>(args.Target))
                    _antag.ForceMakeAntag<ChangelingRuleComponent>(targetPlayer, "Changeling");
            },
            Impact = LogImpact.High,
            Message = string.Join(": ", Loc.GetString("admin-verb-text-make-changeling"), Loc.GetString("admin-verb-make-changeling")), // Reserve edit: Fix antag verbs
        };
        if (!HasComp<SiliconComponent>(args.Target))
            args.Verbs.Add(ling);

        // Blob
        Verb blobAntag = new()
        {
            Text = "061. " + Loc.GetString("admin-verb-text-make-blob"), // Reserve edit: Fix antag verbs
            Category = VerbCategory.Antag,
            Icon = new SpriteSpecifier.Rsi(new("/Textures/Interface/Misc/job_icons.rsi"), "Blob"), // Reserve edit: Fix antag verbs
            Act = () =>
            {
                EnsureComp<BlobCarrierComponent>(args.Target).HasMind = HasComp<ActorComponent>(args.Target);
            },
            Impact = LogImpact.High,
            Message = string.Join(": ", Loc.GetString("admin-verb-text-make-blob"), Loc.GetString("admin-verb-make-blob")), // Reserve edit: Fix antag verbs
        };
        if (!HasComp<SiliconComponent>(args.Target))
            args.Verbs.Add(blobAntag);

        // Devil
        Verb devilAntag = new()
        {
            Text = "004. " + Loc.GetString("admin-verb-text-make-devil"), // Reserve edit: Fix antag verbs
            Category = VerbCategory.Antag,
            Icon = new SpriteSpecifier.Rsi(new("/Textures/Interface/Misc/job_icons.rsi"), "Devil"), // Reserve edit: Fix antag verbs
            Act = () =>
            {
                _antag.ForceMakeAntag<DevilRuleComponent>(targetPlayer, "Devil");
            },
            Impact = LogImpact.High,
            Message = string.Join(": ", Loc.GetString("admin-verb-text-make-devil"), Loc.GetString("admin-verb-make-devil")), // Reserve edit: Fix antag verbs
        };
        args.Verbs.Add(devilAntag);

        // Einstein Engines - Shadowlings
        Verb shadowling = new()
        {
            Text = "005. " + Loc.GetString("admin-verb-text-make-shadowling"), // Reserve edit: Fix antag verbs
            Category = VerbCategory.Antag,
            Icon = new SpriteSpecifier.Rsi(
                new("/Textures/Interface/Misc/job_icons.rsi"), "Shadowling"), // Reserve edit: Fix antag verbs
            Act = () =>
            {
                _antag.ForceMakeAntag<ShadowlingRuleComponent>(targetPlayer, "Shadowling");
            },
            Impact = LogImpact.High,
            Message = string.Join(": ", Loc.GetString("admin-verb-text-make-shadowling"), Loc.GetString("admin-verb-make-shadowling")), // Reserve edit: Fix antag verbs
        };
        args.Verbs.Add(shadowling);

        // Reserve start
        var slasherName = Loc.GetString("admin-verb-text-make-slasher");
        Verb slasher = new()
        {
            Text = "006. " + slasherName,
            Category = VerbCategory.Antag,
            Icon = new SpriteSpecifier.Rsi(new("/Textures/Interface/Misc/job_icons.rsi"), "Slasher"),
            Act = () =>
            {
                _outfit.SetOutfit(args.Target, "EmptyNudeGear");
                _antag.ForceMakeAntag<AntagLockerSpawnComponent>(targetPlayer, DefaultSlasherRule);
            },
            Impact = LogImpact.High,
            Message = string.Join(": ", slasherName, Loc.GetString("admin-verb-make-slasher")),
        };
        args.Verbs.Add(slasher);
        // Reserve end
    }

    public bool AntagVerbAllowed(GetVerbsEvent<Verb> args, [NotNullWhen(true)] out ICommonSession? target)
    {
        target = null;

        if (!TryComp<ActorComponent>(args.User, out var actor))
            return false;

        var player = actor.PlayerSession;

        if (!_admin.HasAdminFlag(player, AdminFlags.Fun))
            return false;

        if (!HasComp<MindContainerComponent>(args.Target) || !TryComp<ActorComponent>(args.Target, out var targetActor))
            return false;

        target = targetActor.PlayerSession;
        return true;
    }
}
