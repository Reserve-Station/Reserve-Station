// SPDX-License-Identifier: AGPL-3.0-or-later

using Content.Goobstation.Shared.Pirates.Roles; // Reserve edit: Fix antag verbs
using Content.Server._Goobstation.Wizard.Components;
using Content.Server._DV.CosmicCult.Components; // DeltaV
using Content.Server._Harmony.GameTicking.Rules.Components; // Harmony
using Content.Server.Antag;
using Content.Server.GameTicking;
using Content.Server.GameTicking.Rules.Components;
using Content.Server.Zombies;
using Content.Shared.Administration;
using Content.Server.Clothing.Systems;
using Content.Shared.Database;
using Content.Shared.Humanoid;
using Content.Shared.Mind.Components;
using Content.Shared.Roles;
using Content.Shared.Verbs;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Utility;
using Content.Shared.Roles.Components;

namespace Content.Server.Administration.Systems;

public sealed partial class AdminVerbSystem
{
    [Dependency] private readonly AntagSelectionSystem _antag = default!;
    [Dependency] private readonly ZombieSystem _zombie = default!;
    [Dependency] private readonly GameTicker _gameTicker = default!;
    [Dependency] private readonly OutfitSystem _outfit = default!;

    private static readonly EntProtoId DefaultTraitorRule = "Traitor";
    private static readonly EntProtoId DefaultInitialInfectedRule = "Zombie";
    private static readonly EntProtoId DefaultNukeOpRule = "LoneOpsSpawn";
    private static readonly EntProtoId DefaultRevsRule = "Revolutionary";
    private static readonly EntProtoId DefaultThiefRule = "Thief";
    private static readonly EntProtoId DefaultChangelingRule = "Changeling";
    private static readonly EntProtoId ParadoxCloneRuleId = "ParadoxCloneSpawn";
    private static readonly EntProtoId DefaultWizardRule = "Wizard";
    private static readonly EntProtoId DefaultNinjaRule = "NinjaSpawn";
    private static readonly ProtoId<StartingGearPrototype> PirateGearId = "PirateGear";
    // Harmony start
    private static readonly EntProtoId DefaultConspiratorRule = "Conspirators";
    private static readonly EntProtoId DefaultBloodBrotherRule = "BloodBrothers";
    // Harmony end
    // Reserve edit start: Fix antag verbs
    private static readonly EntProtoId DefaultCorporateAgentRule = "CorporateAgent";
    private static readonly EntProtoId DefaultWizardApprenticeRule = "ApprenticeRule";
    private static readonly EntProtoId DefaultPirateCaptainRule = "PiratesSpawn";
    private static readonly EntProtoId DefaultPirateRule = "Pirate";
    private static readonly EntProtoId DefaultNukeOpCommandRule = "Nukeops";
    private static readonly EntProtoId DefaultNukeOpHonkRule = "Honkops";
    private static readonly EntProtoId DefaultContractorRule = "ContractorSpawnMidround";
    private static readonly EntProtoId DefaultMimeAssassinRule = "MimeAssassinMidround";
    private static readonly EntProtoId DefaultTunnelClownRule = "TunnelClownMidround";
    private static readonly EntProtoId DefaultAbductorRule = "LoneAbductorSpawn";
    private static readonly EntProtoId DefaultAbductorVictimRule = "AbductorVictim";
    private static readonly EntProtoId DefaultDarkPriestRule = "DarkPriestMidround";
    // Reserve edit end: Fix antag verbs

    // All antag verbs have names so invokeverb works.
    private void AddAntagVerbs(GetVerbsEvent<Verb> args)
    {
        if (!TryComp<ActorComponent>(args.User, out var actor))
            return;

        var player = actor.PlayerSession;

        if (!_adminManager.HasAdminFlag(player, AdminFlags.Fun))
            return;

        if (!HasComp<MindContainerComponent>(args.Target) || !TryComp<ActorComponent>(args.Target, out var targetActor))
            return;

        var targetPlayer = targetActor.PlayerSession;

        var traitorName = Loc.GetString("admin-verb-text-make-traitor");
        Verb traitor = new()
        {
            Text = "011. " + traitorName, // Reserve edit: Fix antag verbs
            Category = VerbCategory.Antag,
            Icon = new SpriteSpecifier.Rsi(new ResPath("/Textures/Interface/Misc/job_icons.rsi"), "Syndicate"),
            Act = () =>
            {
                _antag.ForceMakeAntag<TraitorRuleComponent>(targetPlayer, DefaultTraitorRule);
            },
            Impact = LogImpact.High,
            Message = string.Join(": ", traitorName, Loc.GetString("admin-verb-make-traitor")),
        };
        args.Verbs.Add(traitor);

        var initialInfectedName = Loc.GetString("admin-verb-text-make-initial-infected");
        Verb initialInfected = new()
        {
            Text = "054. " + initialInfectedName, // Reserve edit: Fix antag verbs
            Category = VerbCategory.Antag,
            Icon = new SpriteSpecifier.Rsi(new("/Textures/Interface/Misc/job_icons.rsi"), "InitialInfected"),
            Act = () =>
            {
                _antag.ForceMakeAntag<ZombieRuleComponent>(targetPlayer, DefaultInitialInfectedRule);
            },
            Impact = LogImpact.High,
            Message = string.Join(": ", initialInfectedName, Loc.GetString("admin-verb-make-initial-infected")),
        };
        args.Verbs.Add(initialInfected);

        var zombieName = Loc.GetString("admin-verb-text-make-zombie");
        Verb zombie = new()
        {
            Text = "055. " + zombieName, // Reserve edit: Fix antag verbs
            Category = VerbCategory.Antag,
            Icon = new SpriteSpecifier.Rsi(new("/Textures/Interface/Misc/job_icons.rsi"), "Zombie"),
            Act = () =>
            {
                _zombie.ZombifyEntity(args.Target);
            },
            Impact = LogImpact.High,
            Message = string.Join(": ", zombieName, Loc.GetString("admin-verb-make-zombie")),
        };
        args.Verbs.Add(zombie);

        var nukeOpName = Loc.GetString("admin-verb-text-make-nuclear-operative");
        Verb nukeOp = new()
        {
            Text = "013. " + nukeOpName, // Reserve edit: Fix antag verbs
            Category = VerbCategory.Antag,
            Icon = new SpriteSpecifier.Rsi(new("/Textures/Interface/Misc/job_icons.rsi"), "NukeOps"), // Reserve edit: Fix antag verbs
            Act = () =>
            {
                _antag.ForceMakeAntag<NukeopsRuleComponent>(targetPlayer, DefaultNukeOpRule);
            },
            Impact = LogImpact.High,
            Message = string.Join(": ", nukeOpName, Loc.GetString("admin-verb-make-nuclear-operative")),
        };
        args.Verbs.Add(nukeOp);

        var pirateName = Loc.GetString("admin-verb-text-make-pirate");
        Verb pirate = new()
        {
            Text = "052. " + pirateName, // Reserve edit: Fix antag verbs
            Category = VerbCategory.Antag,
            Icon = new SpriteSpecifier.Rsi(new("/Textures/Interface/Misc/job_icons.rsi"), "Pirate"), // Reserve edit: Fix antag verbs
            Act = () =>
            {
                // // pirates just get an outfit because they don't really have logic associated with them
                // _outfit.SetOutfit(args.Target, PirateGearId);
                _antag.ForceMakeAntag<PirateRoleComponent>(targetPlayer, DefaultPirateRule);
            },
            Impact = LogImpact.High,
            Message = string.Join(": ", pirateName, Loc.GetString("admin-verb-make-pirate")),
        };
        args.Verbs.Add(pirate);

        var headRevName = Loc.GetString("admin-verb-text-make-head-rev");
        Verb headRev = new()
        {
            Text = "020. " + headRevName, // Reserve edit: Fix antag verbs
            Category = VerbCategory.Antag,
            Icon = new SpriteSpecifier.Rsi(new("/Textures/Interface/Misc/job_icons.rsi"), "HeadRevolutionary"),
            Act = () =>
            {
                _antag.ForceMakeAntag<RevolutionaryRuleComponent>(targetPlayer, DefaultRevsRule);
            },
            Impact = LogImpact.High,
            Message = string.Join(": ", headRevName, Loc.GetString("admin-verb-make-head-rev")),
        };
        args.Verbs.Add(headRev);

        var thiefName = Loc.GetString("admin-verb-text-make-thief");
        Verb thief = new()
        {
            Text = "001. " + thiefName, // Reserve edit: Fix antag verbs
            Category = VerbCategory.Antag,
            Icon = new SpriteSpecifier.Rsi(new ResPath("/Textures/Interface/Misc/job_icons.rsi"), "Thief"), // Reserve edit: Fix antag verbs
            Act = () =>
            {
                _antag.ForceMakeAntag<ThiefRuleComponent>(targetPlayer, DefaultThiefRule);
            },
            Impact = LogImpact.High,
            Message = string.Join(": ", thiefName, Loc.GetString("admin-verb-make-thief")),
        };
        args.Verbs.Add(thief);

        var paradoxCloneName = Loc.GetString("admin-verb-text-make-paradox-clone");
        Verb paradox = new()
        {
            Text = "016. " + paradoxCloneName, // Reserve edit: Fix antag verbs
            Category = VerbCategory.Antag,
            Icon = new SpriteSpecifier.Rsi(new("/Textures/Interface/Misc/job_icons.rsi"), "ParadoxClone"),
            Act = () =>
            {
                var ruleEnt = _gameTicker.AddGameRule(ParadoxCloneRuleId);

                if (!TryComp<ParadoxCloneRuleComponent>(ruleEnt, out var paradoxCloneRuleComp))
                    return;

                paradoxCloneRuleComp.OriginalBody = args.Target; // override the target player

                _gameTicker.StartGameRule(ruleEnt);
            },
            Impact = LogImpact.High,
            Message = string.Join(": ", paradoxCloneName, Loc.GetString("admin-verb-make-paradox-clone")),
        };
        /* Goobwizard
        var wizardName = Loc.GetString("admin-verb-text-make-wizard");
        Verb wizard = new()
        {
            Text = wizardName,
            Category = VerbCategory.Antag,
            Icon = new SpriteSpecifier.Rsi(new("/Textures/Interface/Misc/job_icons.rsi"), "Wizard"),
            Act = () =>
            {
                // Wizard has no rule components as of writing, but I gotta put something here to satisfy the machine so just make it wizard mind rule :)
                _antag.ForceMakeAntag<WizardRoleComponent>(targetPlayer, DefaultWizardRule);
            },
            Impact = LogImpact.High,
            Message = string.Join(": ", wizardName, Loc.GetString("admin-verb-make-wizard")),
        };
        args.Verbs.Add(wizard);
        */

        var ninjaName = Loc.GetString("admin-verb-text-make-space-ninja");
        Verb ninja = new()
        {
            Text = "002. " + ninjaName, // Reserve edit: Fix antag verbs
            Category = VerbCategory.Antag,
            Icon = new SpriteSpecifier.Rsi(new("/Textures/Interface/Misc/job_icons.rsi"), "Ninja"), // Reserve edit: Fix antag verbs
            Act = () =>
            {
                _antag.ForceMakeAntag<NinjaRoleComponent>(targetPlayer, DefaultNinjaRule);
            },
            Impact = LogImpact.High,
            Message = string.Join(": ", ninjaName, Loc.GetString("admin-verb-make-space-ninja")),
        };
        args.Verbs.Add(ninja);

        if (HasComp<HumanoidAppearanceComponent>(args.Target)) // only humanoids can be cloned
            args.Verbs.Add(paradox);

        // goobstation - heretics
        var hereticName = Loc.GetString("admin-verb-text-make-heretic");
        Verb heretic = new()
        {
            Text = "003. " + hereticName, // Reserve edit: Fix antag verbs
            Category = VerbCategory.Antag,
            Icon = new SpriteSpecifier.Rsi(new ResPath("/Textures/Interface/Misc/job_icons.rsi"), "Heretic"), // Reserve edit: Fix antag verbs
            Act = () =>
            {
                _antag.ForceMakeAntag<HereticRuleComponent>(targetPlayer, "Heretic");
            },
            Impact = LogImpact.High,
            Message = string.Join(": ", hereticName, Loc.GetString("admin-verb-make-heretic")),
        };
        args.Verbs.Add(heretic);

        // Goobstation - Wizard
        var wizardName = Loc.GetString("admin-verb-text-make-wizard");
        Verb wizard = new()
        {
            Text = "044. " + wizardName, // Reserve edit: Fix antag verbs
            Category = VerbCategory.Antag,
            Icon = new SpriteSpecifier.Rsi(new ResPath("/Textures/_Goobstation/Wizard/StatusIcons/10x10.rsi"), "wizard"), // Reserve edit: Fix antag verbs
            Act = () =>
            {
                _antag.ForceMakeAntag<WizardRuleComponent>(targetPlayer, "Wizard");
            },
            Impact = LogImpact.High,
            Message = string.Join(": ", wizardName, Loc.GetString("admin-verb-make-wizard")),
        };
        args.Verbs.Add(wizard);

        // Begin DeltaV Additions
        var cosmicCultName = Loc.GetString("admin-verb-text-make-cosmiccultist");
        Verb cosmiccult = new()
        {
            Text = "062. " + cosmicCultName, // Reserve edit: Fix antag verbs
            Category = VerbCategory.Antag,
            Icon = new SpriteSpecifier.Rsi(new("/Textures/_DV/CosmicCult/Icons/antag_icons.rsi"), "CosmicCult"),
            Act = () =>
            {
                _antag.ForceMakeAntag<CosmicCultRuleComponent>(targetPlayer, "CosmicCult");
            },
            Impact = LogImpact.High,
            Message = string.Join(": ", cosmicCultName, Loc.GetString("admin-verb-make-cosmiccultist")),
        };
        args.Verbs.Add(cosmiccult);
        // End DeltaV Additions
        // Harmony start
        var conspiratorName = Loc.GetString("admin-verb-text-make-conspirator");
        Verb conspirator = new()
        {
            Text = "017. " + conspiratorName, // Reserve edit: Fix antag verbs
            Category = VerbCategory.Antag,
            Icon = new SpriteSpecifier.Rsi(new("/Textures/_Harmony/Interface/Misc/job_icons.rsi"), "Conspirator"),
            Act = () =>
            {
                _antag.ForceMakeAntag<ConspiratorRuleComponent>(targetPlayer, DefaultConspiratorRule);
            },
            Impact = LogImpact.High,
            Message = string.Join(": ", conspiratorName, Loc.GetString("admin-verb-make-conspirator")),
        };
        args.Verbs.Add(conspirator);

        var bloodBrotherName = Loc.GetString("admin-verb-text-make-blood-brother");
        Verb bloodBrother = new()
        {
            Text = "018. " + bloodBrotherName, // Reserve edit: Fix antag verbs
            Category = VerbCategory.Antag,
            Icon = new SpriteSpecifier.Rsi(new("/Textures/_Harmony/Interface/Misc/job_icons.rsi"), "BloodBrother"),
            Act = () =>
            {
                _antag.ForceMakeAntag<BloodBrotherRuleComponent>(targetPlayer, DefaultBloodBrotherRule);
            },
            Impact = LogImpact.High,
            Message = string.Join(": ", bloodBrotherName, Loc.GetString("admin-verb-make-blood-brother")),
        };
        args.Verbs.Add(bloodBrother);
        // Harmony end
        // Reserve start
        var corporateAgentName = Loc.GetString("admin-verb-text-make-corporate-agent");
        Verb corporateAgent = new()
        {
            Text = "019. " + corporateAgentName,
            Category = VerbCategory.Antag,
            Icon = new SpriteSpecifier.Rsi(new("/Textures/Interface/Misc/job_icons.rsi"), "CorporateAgent"),
            Act = () =>
            {
                _antag.ForceMakeAntag<TraitorRuleComponent>(targetPlayer, DefaultCorporateAgentRule);
            },
            Impact = LogImpact.High,
            Message = string.Join(": ", corporateAgentName, Loc.GetString("admin-verb-make-corporate-agent")),
        };
        args.Verbs.Add(corporateAgent);

        var wizardApprenticeName = Loc.GetString("admin-verb-text-make-wizard-apprentice");
        Verb wizardApprentice = new()
        {
            Text = "045. " + wizardApprenticeName,
            Category = VerbCategory.Antag,
            Icon = new SpriteSpecifier.Rsi(new ResPath("/Textures/_Goobstation/Wizard/StatusIcons/10x10.rsi"), "apprentice"),
            Act = () =>
            {
                _antag.ForceMakeAntag<WizardRuleComponent>(targetPlayer, DefaultWizardApprenticeRule);
            },
            Impact = LogImpact.High,
            Message = string.Join(": ", wizardApprenticeName, Loc.GetString("admin-verb-make-wizard-apprentice")),
        };
        args.Verbs.Add(wizardApprentice);

        var contractorName = Loc.GetString("admin-verb-text-make-contractor");
        Verb contractor = new()
        {
            Text = "012. " + contractorName,
            Category = VerbCategory.Antag,
            Icon = new SpriteSpecifier.Rsi(new("/Textures/Interface/Misc/job_icons.rsi"), "Contractor"),
            Act = () =>
            {
                _outfit.SetOutfit(args.Target, "EmptyNudeGear");
                _antag.ForceMakeAntag<NukeopsRuleComponent>(targetPlayer, DefaultContractorRule);
            },
            Impact = LogImpact.High,
            Message = string.Join(": ", contractorName, Loc.GetString("admin-verb-make-contractor")),
        };
        args.Verbs.Add(contractor);

        var nukeOpCommandName = Loc.GetString("admin-verb-text-make-nuclear-operative-command");
        Verb nukeOpCommand = new()
        {
            Text = "014. " + nukeOpCommandName,
            Category = VerbCategory.Antag,
            Icon = new SpriteSpecifier.Rsi(new("/Textures/Interface/Misc/job_icons.rsi"), "NukeOpsCommander"),
            Act = () =>
            {
                _outfit.SetOutfit(args.Target, "EmptyNudeGear");
                _antag.ForceMakeAntag<NukeopsRuleComponent>(targetPlayer, DefaultNukeOpCommandRule);
            },
            Impact = LogImpact.High,
            Message = string.Join(": ", nukeOpCommandName, Loc.GetString("admin-verb-make-nuclear-operative-command")),
        };
        args.Verbs.Add(nukeOpCommand);

        var nukeOpHonkName = Loc.GetString("admin-verb-text-make-nuclear-operative-command-honk");
        Verb nukeOpHonk = new()
        {
            Text = "015. " + nukeOpHonkName,
            Category = VerbCategory.Antag,
            Icon = new SpriteSpecifier.Rsi(new("/Textures/Interface/Misc/job_icons.rsi"), "NukeOpsClown"),
            Act = () =>
            {
                _outfit.SetOutfit(args.Target, "EmptyNudeGear");
                _antag.ForceMakeAntag<NukeopsRuleComponent>(targetPlayer, DefaultNukeOpHonkRule);
            },
            Impact = LogImpact.High,
            Message = string.Join(": ", nukeOpHonkName, Loc.GetString("admin-verb-make-nuclear-operative-command-honk")),
        };
        args.Verbs.Add(nukeOpHonk);

        var pirateCaptainName = Loc.GetString("admin-verb-text-make-pirate-captain");
        Verb pirateCaptain = new()
        {
            Text = "051. " + pirateCaptainName,
            Category = VerbCategory.Antag,
            Icon = new SpriteSpecifier.Rsi(new("/Textures/Interface/Misc/job_icons.rsi"), "PirateCaptain"),
            Act = () =>
            {
                _outfit.SetOutfit(args.Target, "EmptyNudeGear");
                _antag.ForceMakeAntag<PirateRoleComponent>(targetPlayer, DefaultPirateCaptainRule);
            },
            Impact = LogImpact.High,
            Message = string.Join(": ", pirateCaptainName, Loc.GetString("admin-verb-make-pirate-captain")),
        };
        args.Verbs.Add(pirateCaptain);

        var mimeAssassinName = Loc.GetString("admin-verb-text-make-mime-assassin");
        Verb mimeAssassin = new()
        {
            Text = "007. " + mimeAssassinName,
            Category = VerbCategory.Antag,
            Icon = new SpriteSpecifier.Rsi(new("/Textures/Interface/Misc/job_icons.rsi"), "MimeAssassin"),
            Act = () =>
            {
                _outfit.SetOutfit(args.Target, "MimeAssassinGear");
                _antag.ForceMakeAntag<NukeopsRuleComponent>(targetPlayer, DefaultMimeAssassinRule);
            },
            Impact = LogImpact.High,
            Message = string.Join(": ", mimeAssassinName, Loc.GetString("admin-verb-make-mime-assassin")),
        };
        args.Verbs.Add(mimeAssassin);

        var tunnelClownName = Loc.GetString("admin-verb-text-make-tunnel-clown");
        Verb tunnelClown = new()
        {
            Text = "008. " + tunnelClownName,
            Category = VerbCategory.Antag,
            Icon = new SpriteSpecifier.Rsi(new("/Textures/Interface/Misc/job_icons.rsi"), "ClownTunnel"),
            Act = () =>
            {
                _outfit.SetOutfit(args.Target, "TunnelClownAntagGear");
                _antag.ForceMakeAntag<NukeopsRuleComponent>(targetPlayer, DefaultTunnelClownRule);
            },
            Impact = LogImpact.High,
            Message = string.Join(": ", tunnelClownName, Loc.GetString("admin-verb-make-tunnel-clown")),
        };
        args.Verbs.Add(tunnelClown);

        var darkPriestName = Loc.GetString("admin-verb-text-make-dark-priest");
        Verb darkPriest = new()
        {
            Text = "050. " + darkPriestName,
            Category = VerbCategory.Antag,
            Icon = new SpriteSpecifier.Rsi(new("/Textures/Interface/Misc/job_icons.rsi"), "ChaplainDark"),
            Act = () =>
            {
                _outfit.SetOutfit(args.Target, "DarkPriestAntagGear");
                _antag.ForceMakeAntag<NukeopsRuleComponent>(targetPlayer, DefaultDarkPriestRule);
            },
            Impact = LogImpact.High,
            Message = string.Join(": ", darkPriestName, Loc.GetString("admin-verb-make-dark-priest")),
        };
        args.Verbs.Add(darkPriest);

        var abductorName = Loc.GetString("admin-verb-text-make-abductor");
        Verb abductor = new()
        {
            Text = "009. " + abductorName,
            Category = VerbCategory.Antag,
            Icon = new SpriteSpecifier.Rsi(new("/Textures/Interface/Misc/job_icons.rsi"), "Abductor"),
            Act = () =>
            {
                _outfit.SetOutfit(args.Target, "EmptyNudeGear");
                _antag.ForceMakeAntag<NukeopsRuleComponent>(targetPlayer, DefaultAbductorRule);
            },
            Impact = LogImpact.High,
            Message = string.Join(": ", abductorName, Loc.GetString("admin-verb-make-abductor")),
        };
        args.Verbs.Add(abductor);

        var abductorVictimName = Loc.GetString("admin-verb-text-make-abductor-victim");
        Verb abductorVictim = new()
        {
            Text = "010. " + abductorVictimName,
            Category = VerbCategory.Antag,
            Icon = new SpriteSpecifier.Rsi(new("/Textures/Interface/Misc/job_icons.rsi"), "AbductorVictim"),
            Act = () =>
            {
                _antag.ForceMakeAntag<NukeopsRuleComponent>(targetPlayer, DefaultAbductorVictimRule);
            },
            Impact = LogImpact.High,
            Message = string.Join(": ", abductorVictimName, Loc.GetString("admin-verb-make-abductor-victim")),
        };
        args.Verbs.Add(abductorVictim);
        // Reserve end
    }
}
