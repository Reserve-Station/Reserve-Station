ent-TraitorFluffCodewordsObjective = Use the codewords in your speech for the whole shift.
    .desc = Do not slip into ordinary speech.

ent-TraitorFluffInfiltrateObjective = Spend the shift getting into other departments.
    .desc = Do it however you like.

ent-TraitorFluffImpersonateObjective = Spend the shift pretending to be someone else.
    .desc = Do not let anyone expose you.

ent-TraitorFluffTrustPersonObjective = Win this person's trust.
    .desc = By the end of the shift this person should consider you an ally.

objective-fluff-trust-person-title = Win the trust of { $targetName }, { CAPITALIZE($job) }.

ent-TraitorFluffRumorPersonObjective = Spread a rumor that this person is a Syndicate agent.
    .desc = Do not name yourself in that rumor.

objective-fluff-rumor-person-title = Spread a rumor that { $targetName }, { CAPITALIZE($job) }, is a Syndicate agent.

ent-TraitorFluffDistractPersonObjective = Do not let this person work in peace.
    .desc = Distract them, confuse them, waste their time, but do not kill them.

objective-fluff-distract-person-title = Do not let { $targetName }, { CAPITALIZE($job) }, work in peace for the whole shift.

ent-TraitorFluffFramePersonObjective = Frame this person.
    .desc = By the end of the shift the suspicion should point only at them. Do not let yourself be exposed.

objective-fluff-frame-person-title = Frame { $targetName }, { CAPITALIZE($job) }.

ent-TraitorFluffSabotageDepartmentObjective = Sabotage a department for the whole shift.
    .desc = The department should end the shift worse than it started.

objective-fluff-sabotage-department-title =
    { $department ->
        [security] Sabotage Security for the whole shift.
        [medical] Sabotage the medical department for the whole shift.
        [engineering] Sabotage the engineering department for the whole shift.
        [science] Sabotage the science department for the whole shift.
        [cargo] Sabotage cargo for the whole shift.
        [service] Sabotage the service department for the whole shift.
       *[command] Sabotage Command for the whole shift.
    }

ent-TraitorFluffBlameDepartmentObjective = Make a department blame the others for its troubles.
    .desc = The suspicion must not fall on you.

objective-fluff-blame-department-title =
    { $department ->
        [security] Get Security to blame the other departments for its troubles.
        [medical] Get the medical department to blame the other departments for its troubles.
        [engineering] Get the engineering department to blame the other departments for its troubles.
        [science] Get the science department to blame the other departments for its troubles.
        [cargo] Get cargo to blame the other departments for its troubles.
        [service] Get the service department to blame the other departments for its troubles.
       *[command] Get Command to blame the other departments for its troubles.
    }

ent-TraitorFluffFakeHelpDepartmentObjective = Pretend to help a department while doing nothing.
    .desc = Leave as soon as they start checking on you.

objective-fluff-fake-help-department-title =
    { $department ->
        [security] Deceive Security with your help while actually doing nothing.
        [medical] Deceive the medical department with your help while actually doing nothing.
        [engineering] Deceive the engineering department with your help while actually doing nothing.
        [science] Deceive the science department with your help while actually doing nothing.
        [cargo] Deceive cargo with your help while actually doing nothing.
        [service] Deceive the service department with your help while actually doing nothing.
       *[command] Deceive Command with your help while actually doing nothing.
    }

ent-TraitorFluffPanicDepartmentObjective = Spread panic through a department with a lie.
    .desc = Keep the lie alive until the end of the shift.

objective-fluff-panic-department-title =
    { $department ->
        [security] Spread panic in Security with your lie.
        [medical] Spread panic in the medical department with your lie.
        [engineering] Spread panic in the engineering department with your lie.
        [science] Spread panic in the science department with your lie.
        [cargo] Spread panic in cargo with your lie.
        [service] Spread panic in the service department with your lie.
       *[command] Spread panic in Command with your lie.
    }

ent-TraitorFluffRumorsObjective = Become the station's main source of rumors.
    .desc = Appear in every major rumor on the station.

ent-TraitorFluffNanoChatObjective = Answer NanoChat all shift as if you were someone else.
    .desc = Do not break character.

ent-TraitorFluffOtherHandsObjective = Work only through other people's hands.
    .desc = None of your actions should be done in your own name.

ent-TraitorFluffNicknameObjective = By the end of the shift the station must have an unknown saboteur with any nickname.
    .desc = That nickname must not give you away.

ent-TraitorFluffThreatsDepartmentObjective = Keep a department's workers under constant threat.
    .desc = Do not give yourself away.

objective-fluff-threats-department-title =
    { $department ->
        [security] Make sure there are constant threats to Security's workers.
        [medical] Make sure there are constant threats to the medical department's workers.
        [engineering] Make sure there are constant threats to the engineering department's workers.
        [science] Make sure there are constant threats to the science department's workers.
        [cargo] Make sure there are constant threats to cargo's workers.
        [service] Make sure there are constant threats to the service department's workers.
       *[command] Make sure there are constant threats to Command's workers.
    }
