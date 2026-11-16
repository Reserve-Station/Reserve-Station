## Base actions

alerts-vampire-blood-name = Blood Drunk
alerts-vampire-blood-desc = Shows how much blood you've drunk. Extend your fangs and left-click a target to drink.

alerts-vampire-fed-name = Blood Fullness
alerts-vampire-fed-desc = Your current blood fullness. Drink blood to stay fed.
alerts-vampire-fangs-name = Fangs Extended
alerts-vampire-fangs-desc = Your fangs are extended. Left-click a target to drink their blood.
vampire-fangs-extended = You extend your fangs.
vampire-fangs-extended-others = {CAPITALIZE(THE($vampire))} bares sharp fangs.
vampire-fangs-retracted = You retract your fangs.

roles-antag-vampire-name = Vampire
roles-antag-vampire-description = Feed on the crew. Extend your fangs and drink their blood.
roles-antag-vampire-thrall-name = Vampire Thrall
roles-antag-vampire-thrall-description = Obey your vampire master.
role-subtype-vampire = Vampire
role-subtype-vampire-thrall = Thrall
vampire-thrall-admin-note = master: { $master }
vampire-thrall-admin-note-other-roles = { $note }, also: { $roles }

vampire-roundend-name = vampire

vampire-drink-start = You sink your fangs into {CAPITALIZE(THE($target))}.
vampire-drink-start-target = {CAPITALIZE(THE($vampire))} sinks their fangs into your neck!
vampire-drink-start-others = {CAPITALIZE(THE($vampire))} sinks their fangs into {THE($target)}'s neck!
vampire-drink-no-progression = This blood carries no mind. It sates your hunger, but won't make you stronger.

vampire-not-enough-blood = Not enough blood.

vampire-mouth-covered = Your mouth is covered!
vampire-drink-invalid-target = You cannot drink blood from vampires or their thralls.
vampire-target-protected-by-faith = This person is protected by their faith!
vampire-drink-target-maxed = You have already drunk { $amount } units of blood from this target.
vampire-drink-target-hard-max = You have drunk the maximum amount of blood from this target ({ $amount } units).
vampire-full-power-achieved = Your vampiric essence surges full power achieved!
vampire-umbrae-full-power-fov = The shadows bend to your will. You can now see through walls!

vampire-role-greeting = You are a vampire!
    Your blood thirst compels you to feed on crew members. Use your abilities to turn other crew.
    Your fangs allow you to suck blood from humans. Blood will regenerate health and give you new abilities.
    Find something to accomplish during this shift!

# Objectives
objective-issuer-vampire = [color=crimson]Vampire[/color]

objective-condition-drain-title = Drain {$count} units of blood
objective-condition-drain-description = Drink {$count} units of blood from crew members using your fangs.

objective-vampire-thrall-obey-master-title = Obey your master, {$targetName}.

# Class selection action
action-vampire-class-select = Select vampire class
action-vampire-class-select-desc = Choose your vampire subclass

# Round end statistics
roundend-prepend-vampire-drained-low = The vampires barely fed this shift, draining only {$blood} units of blood.
roundend-prepend-vampire-drained-medium = The vampires had a decent meal, draining {$blood} units of blood.
roundend-prepend-vampire-drained-high = The vampires had a blood feast, draining {$blood} units of blood!
roundend-prepend-vampire-drained-critical = The vampires went on a feeding frenzy, draining a staggering {$blood} units of blood!

roundend-prepend-vampire-drained = No vampires managed to drain any significant amount of blood this round.
roundend-prepend-vampire-drained-named = {$name} was the most bloodthirsty vampire, draining {$number} units of blood total.

# Vampire class selection tooltips
vampire-class-hemomancer-tooltip = Hemomancer
    Focuses on blood magic and the manipulation of blood around you

vampire-class-umbrae-tooltip = Umbrae
    Focuses on darkness, stealth ambushing and mobility

vampire-class-gargantua-tooltip = Gargantua
    Focuses on tenacity and melee damage

vampire-class-dantalion-tooltip = Dantalion
    Focuses on thralling and illusions

# Hemomancer abilities
action-vampire-hemomancer-tendrils-wrong-place = Cannot cast there.

action-vampire-blood-barrier-wrong-place = Cannot place barriers there.

action-vampire-sanguine-pool-already-in = You are already in sanguine pool form!
action-vampire-sanguine-pool-invalid-tile = You cannot become a blood pool here.
action-vampire-sanguine-pool-enter = You transform into a pool of blood!
action-vampire-sanguine-pool-exit = You reform from the blood pool!
vampire-space-burn-warning = The harsh void light scorches your undead flesh!

action-vampire-blood-eruption-activated = You cause blood to erupt in spikes around you!

action-vampire-blood-bringers-rite-not-enough-power = You lack full vampiric power (need above 1000 total blood & 8 unique victims)
action-vampire-blood-brighters-rite-not-enough-blood = Not enough blood to activate blood bringers rite
action-vampire-blood-bringers-rite-start = Blood Bringers Rite activated!
action-vampire-blood-bringers-rite-stop = Blood bringers rite deactivated
action-vampire-blood-bringers-rite-stop-blood = Blood Bringers Rite deactivated - not enough blood

vampire-locate-result = Your senses trace { $target } to { $location }.
vampire-locate-not-same-sector = That person is not on your sector.
vampire-locate-unknown = Unknown area
vampire-locate-no-targets = No prey can be sensed on this sector.

predator-sense-title = Predator Sense
vampire-locate-search-placeholder = Search...

vampiric-claws-remove-popup = You make claws disappear.

# Umbrae abilities
action-vampire-cloak-of-darkness-start = You blend into the shadows!
action-vampire-cloak-of-darkness-stop = You step out of the shadows.

action-vampire-shadow-snare-placed = You set a shadow snare trap.
action-vampire-shadow-snare-wrong-place = You can't place a trap here.
action-vampire-shadow-snare-scatter = You scattered the shadow trap.
vampire-shadow-snare-oldest-removed = Your old shadow snare dissipates.
ent-shadow-snare-ensnare = shadow snare

action-vampire-shadow-anchor-returned = You returned to the shadow anchor
action-vampire-shadow-anchor-installed = You've secured a spot in the shadows

action-vampire-shadow-boxing-start = You begin shadow boxing.
action-vampire-shadow-boxing-stop = Shadow boxing has been stoped.
action-vampire-shadow-boxing-ends = Shadow boxing ends.

action-vampire-dark-passage-wrong-place = The darkness here is impenetrable...
action-vampire-dark-passage-activated = You slipped through the darkness...

action-vampire-extinguish-activated = You absorbed the light around you...({$count})

action-vampire-eternal-darkness-not-enough-blood = You have run out of blood to sustain eternal darkness.
action-vampire-eternal-darkness-start = You conjured eternal darkness...
action-vampire-eternal-darkness-stop = The eternal darkness has dissipated...

# Dantalion

vampire-enthrall-start = You reach into {CAPITALIZE(THE($target))}'s mind...
vampire-enthrall-success = {CAPITALIZE(THE($target))} bends the knee and becomes your thrall.
vampire-enthrall-target = Your mind is overwhelmed by vampiric domination!
vampire-enthrall-limit = You cannot control any more thralls.
vampire-enthrall-invalid = That target cannot be enthralled.
vampire-thrall-released = The vampiric hold over you fades.

vampire-pacify-invalid = That target cannot be pacified.
vampire-pacify-success = {CAPITALIZE(THE($target))} succumbs to your overwhelming serenity.
vampire-pacify-target = A crushing calm drowns your will to fight!

vampire-subspace-swap-thrall = You cannot subspace swap with your thralls.
vampire-subspace-swap-dead = That mind is beyond your reach.
vampire-subspace-swap-failed = The subspace rift fizzles uselessly.
vampire-subspace-swap-success = Space twists as you trade places with {CAPITALIZE(THE($target))}!
vampire-subspace-swap-target = Reality warps and you are torn into a new position!

vampire-rally-thralls-success = {$count ->
    [one] Your call rallies a thrall back to your side!
    *[other] Your call rallies {$count} thralls back to your side!
}
vampire-rally-thralls-none = None of your thralls can answer the call.
vampire-thrall-holy-water-freed = The holy water purges the vampires hold on your mind!

vampire-blood-bond-start = Rivers of blood knit you to your thralls.
vampire-blood-bond-stop = You let the blood bond fall slack.
vampire-blood-bond-no-thralls = You have no enthralled servants to bond with.
vampire-blood-bond-stop-blood = The bond shreds itself; you lack the blood to sustain it.

action-vampire-not-enough-power = Your power is insufficient (need >1000 total blood & 8 unique victims).

# Gargantua
vampire-blood-swell-start = Your muscles swell with unholy power
vampire-blood-swell-end = The blood rage subsides.

vampire-blood-rush-start = Blood surges through your limbs!
vampire-blood-rush-end = Your supernatural speed fades.

vampire-seismic-stomp-activate = The ground shudders beneath your fury!

vampire-overwhelming-force-start = Your presence becomes immovable.
vampire-overwhelming-force-stop = You relax your iron grip.
vampire-overwhelming-force-too-heavy = This object is far too heavy to move!
vampire-overwhelming-force-door-pried = You wrench the door open with brute strength.

vampire-demonic-grasp-hit = A demonic claw seizes you!
vampire-demonic-grasp-pull = The claw drags you toward the vampire!

vampire-charge-start = You barrel forward with unstoppable force!
vampire-charge-impact = You crash into {CAPITALIZE(THE($target))} with devastating force!


vampire-blood-swell-cancel-shoot = Your fingers don`t fit in the trigger guard!!

vampire-holy-place-burn = The sacred ground sears your unholy flesh!

alerts-vampire-blood-swell-name = Blood Swell
alerts-vampire-blood-swell-desc = Your muscles surge with unholy power.
alerts-vampire-blood-rush-name = Blood Rush
alerts-vampire-blood-rush-desc = Supernatural speed courses through your limbs.

# region Admin antag verb

admin-verb-text-make-vampire = Make a Vampire
admin-verb-make-vampire = Turn the target into a vampire.

# endregion

# region Guidebook entries

guide-entry-vampire = Vampire
guide-entry-vampire-progression = Vampire Progression
guide-entry-vampire-classes = Vampire Classes
guide-entry-vampire-counterplay = Countering Vampires

mind-role-vampire-name = Vampire Role
objective-vampire-survive-name = Survive
objective-vampire-survive-desc = I must survive at all costs.
objective-vampire-escape-name = Escape to CentComm alive and unrestrained.
objective-vampire-escape-desc = I need to escape on the evacuation shuttle without being captured.
objective-vampire-kill-random-desc = Do it however you like, just make sure they do not reach CentComm.
objective-vampire-thrall-obey-name = Obey your master
objective-vampire-thrall-obey-desc = You are enthralled. Follow your master's commands.

# endregion

# region Entities

ent-VampiricClawsItem = vampiric claws
    .desc = Blood-forged claws that siphon vitae on hit. They dissipate after 15 swings, or if dispelled.
ent-VampireDecoyEntity = vampire decoy
ent-MobVampireSanguinePool = sanguine pool
    .desc = A sentient puddle of vampiric blood.

# endregion

# region Actions

ent-ActionVampireToggleFangs = Toggle Fangs (Toggle)
    .desc = Extend or retract your fangs to drink the blood of your victims.
ent-ActionVampireGlare = Glare (Free)
    .desc = Paralyzes and silences nearby targets, dealing them stamina damage over time.
ent-ActionVampireRejuvenateI = Rejuvenate (Free)
    .desc = Instantly removes stun and restores 100 stamina damage.
ent-ActionVampireRejuvenateII = Rejuvenate (Free)
    .desc = Instantly removes stun, restores 100 stamina, clears harmful reagents (10u), and heals damage.
ent-ActionClassSelectId = Select Vampire Class
    .desc = Choose your vampire subclass.

# Hemomancer

ent-ActionVampireHemomancerClaws = Vampiric Claws
    .desc = Creates non-dispellable blood-forged claws. Each hit grants +5 blood. 15 hits. Use in hand to dispel.
ent-ActionVampireSanguinePool = Sanguine Pool
    .desc = Transforms you into a pool of blood for 8 seconds, allowing you to pass through doors and windows.
ent-ActionVampireHemomancerTendrils = Blood Tendrils
    .desc = After a short delay, tendrils erupt in a 3x3 area, poisoning and heavily slowing victims.
ent-ActionVampireBloodBarrier = Blood Barrier
    .desc = Creates 3 blood barriers at the selected location. Vampires can pass through them.
ent-ActionVampirePredatorSense = Predator Sense
    .desc = Track your prey; they have nowhere to hide...
ent-ActionVampireBloodEruption = Blood Eruption (100)
    .desc = Any blood within a 4-tile radius erupts, dealing 50 blunt damage to those standing in it.
ent-ActionVampireBloodBringersRite = Bloodbringer's Rite (Toggle)
    .desc = When activated, everyone nearby starts bleeding heavily. You drink their blood and heal.

# Umbrae

ent-ActionVampireCloakOfDarkness = Cloak of Darkness (Toggle)
    .desc = Invisibility and a speed bonus depending on lighting. Stronger in the dark, weaker in bright light.
ent-ActionVampireShadowSnare = Shadow Snare (20)
    .desc = Places a fragile shadow trap. Deals damage, blinds (20s), and heavily slows non-vampire humanoids.
ent-ActionVampireShadowAnchor = Shadow Anchor (20)
    .desc = The first use places a beacon (2 min). Reusing it instantly returns you to it and consumes the beacon.
ent-ActionVampireShadowBoxing = Shadow Boxing (50)
    .desc = Command shadow bats to beat the target. You must remain within 4 tiles.
ent-ActionVampireDarkPassage = Dark Passage (20)
    .desc = Teleports to the selected point through the shadows.
ent-ActionVampireExtinguish = Extinguish Light (0)
    .desc = Destroys light sources within a 3-tile radius, granting 5 blood for each.
ent-ActionVampireEternalDarkness = Eternal Darkness (Toggle)
    .desc = Envelops the area around you in darkness and gradually lowers the temperature of nearby bodies.

# Dantalion

ent-ActionVampireEnthrall = Enthrall (150)
    .desc = Channel for 15 seconds on a humanoid to bend them to your will. Breaks if either of you moves.
ent-ActionVampirePacify = Pacify (30)
    .desc = Fills the target's mind with bliss, pacifying them for 40 seconds.
ent-ActionVampireSubspaceSwap = Subspace Swap (30)
    .desc = Select a target within a 7-tile radius, swap places with them, and slow them for 4 seconds.
ent-ActionVampireDecoy = Decoy (30)
    .desc = Leaves a fragile copy that blinds attackers when damaged while you hide in invisibility.
ent-ActionVampireRallyThralls = Rally Thralls (100)
    .desc = Commands thralls within a 7-tile radius to remove stuns, wake up, and restore stamina.
ent-ActionVampireBloodBond = Blood Bond (Toggle)
    .desc = Toggles a blood bond with nearby thralls, redistributing damage at the cost of 2.5 blood per second.
ent-ActionVampireMassHysteria = Mass Hysteria (70)
    .desc = Instills terror in all nearby minds (except thralls), blinding and causing hallucinations for 30 seconds.

# Gargantua

ent-ActionVampireBloodSwell = Blood Swell (30)
    .desc = Reduces incoming damage and shortens stuns for 30 seconds. Weapons do not work, but melee grows.
ent-ActionVampireBloodRush = Blood Rush (30)
    .desc = Doubles your movement speed for 10 seconds.
ent-ActionVampireSeismicStomp = Seismic Stomp (30)
    .desc = Strikes the ground, knocking down and pushing creatures within a 3-tile radius. Destroys floor tiles.
ent-ActionVampireOverwhelmingForce = Overwhelming Force (Toggle)
    .desc = Automatically breaches powerless doors. While active, you cannot be pushed or pulled. Costs 5 blood per door.
ent-ActionVampireDemonicGrasp = Demonic Grasp (20)
    .desc = Launches a demonic hand up to 15 tiles. Immobilizes the target for 5 seconds and can pull in combat mode.
ent-ActionVampireCharge = Charge (30)
    .desc = Rushes in a direction until hitting an obstacle. Creatures take 60 crushing damage and are knocked back 5 tiles. Structures take 150 damage.

# endregion

# region Effects

ent-VampireBloodTendrilVisual = blood tendrils
ent-VampireShadowBoxingPunch = shadow punch
ent-VampireBloodBarrier = blood barrier
    .desc = A barrier made of solidified blood that blocks movement.
ent-VampireSanguinePoolOut = vampire transformation out
ent-VampireSanguinePoolIn = vampire transformation in
ent-VampireBloodEruptionVisual = blood eruption
ent-VampireDrainBeam = drain beam
    .desc = A crimson beam of life-draining energy.
ent-VampireDrainBeamVisual = drain beam visual
    .desc = A smooth client-side vampire drain beam.
ent-VampireBloodBondBeamVisual = blood bond beam visual
    .desc = A smooth client-side vampire blood bond beam.
ent-VampireShadowAnchorBeacon = shadow anchor
    .desc = A pulsing knot of shadow you can return to.
ent-VampireShadowSnare = shadow snare
    .desc = A nearly invisible trap made of condensed shadows.
ent-VampireShadowSnareEnsnare = shadow tendrils
    .desc = Dark tendrils binding your legs.
