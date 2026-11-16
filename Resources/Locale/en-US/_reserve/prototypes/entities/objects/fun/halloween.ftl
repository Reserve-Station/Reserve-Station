# SPDX-FileCopyrightText: Made by Ceterai on GitHub.
#
# SPDX-License-Identifier: AGPL-3.0-or-later
#
# Title: Halloween on Reserve Station

ent-MobBatAngry = { ent-MobBat }
    .desc = This bat looks very bloodthirsty...
    .suffix = Halloween, Evil, AI
ent-SpawnMobBatAngry = { ent-MobBatAngry } spawner
    .desc = { ent-MarkerBase.desc }
    .suffix = Halloween, Evil

ent-MobBatBlack = { ent-MobBat }
    .desc = Did it get burned, or is that its natural color?
    .suffix = Halloween, Black, AI
ent-SpawnMobBatBlack = { ent-MobBatBlack } spawner
    .desc = { ent-MarkerBase.desc }
    .suffix = Halloween, Black

ent-MobBatBlackAngry = { ent-MobBatBlack }
    .desc = { ent-MobBatBlack.desc }
    .suffix = Halloween, Black, Evil, AI
ent-SpawnMobBatBlackAngry = { ent-MobBatBlackAngry } spawner
    .desc = { ent-MarkerBase.desc }
    .suffix = Halloween, Black, Evil

ent-MobBatBloodRed = blood-red bat
    .desc = Is it covered in blood or is this its natural color?
    .suffix = Halloween, Lots of Blood, AI
ent-SpawnMobBatBloodRed = { ent-MobBatBloodRed } spawner
    .desc = { ent-MarkerBase.desc }
    .suffix = Halloween, Lots of Blood
ent-MobBatBloodRedAngry = { ent-MobBatBloodRed }
    .desc = { ent-MobBatBloodRed.desc }
    .suffix = Halloween, Lots of Blood, Evil, AI
ent-SpawnMobBatBloodRedAngry = { ent-MobBatBloodRedAngry } spawner
    .desc = { ent-MarkerBase.desc }
    .suffix = Halloween, Lots of Blood, Evil

ent-MobBatVampire = { ent-MobBat }
    .desc = It seems much larger than usual...
    .suffix = Halloween, Vampire, AI
ent-SpawnMobBatVampire = { ent-MobBatVampire } spawner
    .desc = { ent-MarkerBase.desc }
    .suffix = Halloween, Vampire
ent-MobBatVampireAngry = { ent-MobBatVampire }
    .desc = { ent-MobBatVampire.desc }
    .suffix = Halloween, Vampire, Evil, AI
ent-SpawnMobBatVampireAngry = { ent-MobBatVampireAngry } spawner
    .desc = { ent-MarkerBase.desc }
    .suffix = Halloween, Vampire, Evil

ent-SpawnMobBatAny = { ent-MobBat } spawner
    .desc = { ent-MarkerBase.desc }
    .suffix = Halloween, Any
ent-SpawnMobBatAnyAngry = { ent-MobBatAngry } spawner
    .desc = { ent-MarkerBase.desc }
    .suffix = Halloween, Any

ent-CandyBucketFilled = { ent-CandyBucket }
    .desc = { ent-CandyBucket.desc }
    .suffix = Halloween, Pumpkin, Filled
ent-CandyBucketFilledMystery = { ent-CandyBucketFilled }
    .desc = { ent-CandyBucketFilled.desc }
    .suffix = Halloween, Pumpkin, Filled, Mystery
ent-CandyBucketFilledMysteryEvil = { ent-CandyBucketFilled }
    .desc = { ent-CandyBucketFilled.desc }
    .suffix = Halloween, Pumpkin, Filled, Mystery, Evil
ent-CandyBucketFilledAny = { ent-CandyBucketFilled }
    .desc = { ent-CandyBucketFilled.desc }
    .suffix = Halloween, Pumpkin, Filled, Any
ent-CandyBucketLarge = roomy { ent-CandyBucket }
    .desc = { ent-CandyBucketFilled.desc }
    .suffix = Halloween, Pumpkin, Filled, Large
ent-CandyBucketBluespace = bottomless { ent-CandyBucket }
    .desc = { ent-CandyBucketFilled.desc }
    .suffix = Halloween, Pumpkin, Filled, Bluespace
ent-SpawnCandyBucket = spawner for any { ent-CandyBucket }
    .desc = { ent-MarkerBase.desc }
    .suffix = Halloween, Pumpkin, Random

ent-FoodDonutJellyPumpkin = pumpkin jelly-donut
    .desc = Goes great with pumpkin spice latte.
    .suffix = Halloween

ent-HalloweenLightTube = { ent-LedLightTube }
    .desc = { ent-LedLightTube.desc }
    .suffix = Halloween
ent-PoweredlightHalloween = { ent-PoweredlightLED }
    .desc = { ent-PoweredlightLED.desc }
    .suffix = Halloween
ent-HalloweenLightBulb = { ent-DimLightBulb }
    .desc = { ent-DimLightBulb.desc }
    .suffix = Halloween
ent-PoweredSmallLightHalloween = { ent-PoweredDimSmallLight }
    .desc = { ent-PoweredDimSmallLight.desc }
    .suffix = Halloween

ent-CandleOrangeInfinite = magic orange candle
    .desc = { ent-CandleInfinite.desc }
    .suffix = Halloween
ent-CandleOrangeSmallInfinite = small magic orange candle
    .desc = { ent-CandleInfinite.desc }
    .suffix = Halloween

ent-RandomHalloweenCandleBundleSpawner = random Halloween candle bundle spawner
    .desc = Spawns a random assortment of Halloween candles.
    .suffix = Halloween
ent-RandomHalloweenDecorSpawner = random Halloween decor spawner
    .desc = Spawns random Halloween decor (pumpkins, lanterns, and more).
    .suffix = Halloween
ent-RandomHalloweenTableDecorSpawner = random Halloween table decor spawner
    .desc = Spawns random table decor (candles, treats, and more).
    .suffix = Halloween
ent-RandomHalloweenCrateSpawner = random Halloween crate spawner
    .desc = Spawns a random Halloween crate (with a web or a coffin).
    .suffix = Halloween
ent-RandomPosterSpooky = random spooky poster spawner
    .desc = Spawns a random spooky Halloween-themed poster.
    .suffix = Halloween

ent-BloodCooler = blood cooler
    .desc = { ent-WaterCooler.desc } The liquid inside looks suspiciously red.
    .suffix = Halloween
ent-BloodCoolerFakeCherryJuice = { ent-BloodCooler }
    .desc = { ent-BloodCooler.desc }
    .suffix = Halloween, Cherry juice
ent-BloodCoolerFakeTomatoJuice = { ent-BloodCooler }
    .desc = { ent-BloodCooler.desc }
    .suffix = Halloween, Tomato juice
ent-RandomBloodCooler = random blood cooler spawner
    .desc = Spawns a random cooler filled with fake blood.
    .suffix = Halloween, Mostly fake

ent-MobTomatoKillerMutant = mutant killer tomato
    .desc = You won't be able to pick this one up...
    .suffix = Halloween, Admeme, Hostile, AI

ent-MobFlyingCutlass = flying cutlass
    .desc = A boarding cutlass with spectral wings.
    .suffix = Halloween, Admeme, AI
ent-MobFlyingCutlassHostile = { ent-MobFlyingCutlass }
    .desc = A flying cutlass that attacks anyone nearby.
    .suffix = Halloween, Admeme, Hostile, AI
ent-MobFlyingClaymore = flying claymore
    .desc = A claymore with a will of its own, flitting through the air.
    .suffix = Halloween, Admeme, AI
ent-MobFlyingClaymoreHostile = { ent-MobFlyingClaymore }
    .desc = A flying claymore that pursues its targets.
    .suffix = Halloween, Admeme, Hostile, AI
ent-MobFlyingLaserGun = flying laser gun
    .desc = A laser gun hovering in the air.
    .suffix = Halloween, Admeme, AI
ent-MobFlyingLaserGunHostile = { ent-MobFlyingLaserGun }
    .desc = A flying laser gun that pursues and shoots its targets.
    .suffix = Halloween, Admeme, Hostile, AI

ent-MobFlyingGhostBurger = { ent-FoodBurgerGhost }
    .desc = Too spooky to stay on the plate, but still edible.
    .suffix = Halloween, Admeme, AI
ent-MobFlyingGhostBurgerHostile = { ent-MobFlyingGhostBurger }
    .desc = You can't hide...
    .suffix = Halloween, Admeme, Hostile, AI

ent-MobPlushieCeteraiOrchiDeaMimeEvil = evil { ent-PlushieCeteraiOrchiDeaMime }
    .desc = It seems like she loves you a little too much...
    .suffix = { ent-PlushieCeteraiOrchiDeaMime.suffix }, Halloween, Admeme, Hostile, AI

ent-SpiritCandleDecor = { ent-SpiritCandle }
    .desc = A decorative candle for ritual gatherings.
    .suffix = Halloween, Admeme, Decorative

ent-PlushiePumpkin = pumpkin plushie
    .desc = A stuffed toy shaped like a pumpkin.
    .suffix = Halloween, { ent-BasePlushie.suffix }
ent-PlushieGhostBurger = ghost burger plushie
    .desc = A stuffed toy shaped like a ghost burger.
    .suffix = Halloween, { ent-BasePlushie.suffix }

ent-BucketPumpket = pumpket
    .desc = A pumpkin-themed bucket for collecting candy.
    .suffix = Halloween
ent-PumpkinBucketSurreal = surreal { ent-PumpkinBucket }
    .desc = { ent-PumpkinBucket.desc }
    .suffix = { ent-PumpkinBucket.suffix }
ent-RandomPumpkinBucket = random { ent-PumpkinBucket } spawner
    .desc = { ent-PumpkinBucket.desc }
    .suffix = { ent-PumpkinBucket.suffix }

ent-SteelBenchWebbed = web-covered steel bench
    .desc = An ordinary steel bench, abandoned to a dusting of cobwebs.
    .suffix = Halloween

ent-StatueReaper = reaper statue
    .desc = A solemn stone reaper with a jack-o'-lantern.
    .suffix = Halloween, Jack-o'-lantern
ent-StatueReaperSurreal = surreal { ent-StatueReaper }
    .desc = { ent-StatueReaper.desc }
    .suffix = { ent-StatueReaper.suffix }
ent-RandomStatueReaper = random { ent-StatueReaper } spawner
    .desc = { ent-StatueReaper.desc }
    .suffix = { ent-StatueReaper.suffix }

ent-PumpkinKillerSeeds = packet of pumpkin killer seeds
    .desc = { ent-SeedBase.desc }
    .suffix = Halloween
ent-PumpkinKillerSeedsSurreal = packet of surreal pumpkin killer seeds
    .desc = { ent-PumpkinKillerSeeds.desc }
    .suffix = { ent-PumpkinKillerSeeds.suffix }

ent-MobPumpkinKiller = pumpkin killer
    .desc = A vicious pumpkin with a taste for the living.
    .suffix = Halloween, Evil, AI
ent-MobPumpkinKillerSurreal = surreal { ent-MobPumpkinKiller }
    .desc = { ent-MobPumpkinKiller.desc }
    .suffix = { ent-MobPumpkinKiller.suffix }
ent-MobPumpkinKillerMutant = pumpkin mutant
    .desc = { ent-MobPumpkinKiller.desc }
    .suffix = Halloween, Admeme, Boss, Evil, AI

ent-MobCthulhuEye = floating eye
    .desc = A wet, unblinking eye that drifts through the air.
    .suffix = Halloween, Admeme, Evil, AI
ent-MobCthulhuEyeGiant = giant floating eye
    .desc = A colossal, malevolent eye that marks its prey with searing red lasers.
    .suffix = Halloween, Admeme, Boss, Evil, AI
ent-MobCthulhuEyeSurreal = surreal { ent-MobCthulhuEye }
    .desc = { ent-MobCthulhuEye.desc }
    .suffix = { ent-MobCthulhuEye.suffix }
ent-MobCthulhuEyeGiantSurreal = surreal { ent-MobCthulhuEyeGiant }
    .desc = { ent-MobCthulhuEyeGiant.desc }
    .suffix = { ent-MobCthulhuEyeGiant.suffix }

ent-VendingMachineHalloweenPumpkin = Pumpkin Treats
    .desc = A pumpkin-shaped machine full of seasonal treats.
    .suffix = Halloween
ent-VendingMachineHalloweenSpooky = Nocturne
    .desc = A shadowy vending machine stocked with unsettling treats.
    .suffix = Halloween
ent-VendingMachineHalloweenPumpkinSurreal = Surreal { ent-VendingMachineHalloweenPumpkin }
    .desc = { ent-VendingMachineHalloweenPumpkin.desc }
    .suffix = { ent-VendingMachineHalloweenPumpkin.suffix }
ent-VendingMachineHalloweenSpookySurreal = Surreal { ent-VendingMachineHalloweenSpooky }
    .desc = { ent-VendingMachineHalloweenSpooky.desc }
    .suffix = { ent-VendingMachineHalloweenSpooky.suffix }
ent-RandomVendingMachineHalloween = random Halloween vending machine spawner
    .desc = A random vending machine stocked with Halloween treats.
    .suffix = Halloween

seeds-pumpkin-killer-name = pumpkin killer
seeds-pumpkin-killer-display-name = pumpkin killer plant

ghost-role-information-tomato-mutant-name = Mutant Tomato
ghost-role-information-tomato-mutant-description = You are an unusually large and sturdy killer tomato. Hunt down the living.
ghost-role-information-flying-cutlass-name = Flying Cutlass
ghost-role-information-flying-cutlass-description = You are a flying cutlass. Take to the air and decide for yourself whether to fight or flee.
ghost-role-information-flying-cutlass-hostile-name = Sinister Cutlass
ghost-role-information-flying-cutlass-hostile-description = Hunt down nearby people with your blade.
ghost-role-information-flying-claymore-name = Flying Claymore
ghost-role-information-flying-claymore-description = You are a flying claymore. Take to the air and decide for yourself whether to fight or flee.
ghost-role-information-flying-claymore-hostile-name = Sinister Claymore
ghost-role-information-flying-claymore-hostile-description = Hunt down nearby people with your blade.
ghost-role-information-flying-laser-gun-name = Flying Laser Gun
ghost-role-information-flying-laser-gun-description = You are a flying laser gun. Take to the air and decide for yourself whether to fight or flee.
ghost-role-information-flying-laser-gun-hostile-name = Sinister Laser Gun
ghost-role-information-flying-laser-gun-hostile-description = Hunt down nearby people and shoot them with your laser.
ghost-role-information-flying-ghost-burger-name = Flying Ghost Burger
ghost-role-information-flying-ghost-burger-description = You are an edible ghost burger that can fly through anything. Have a bite-sized adventure.
ghost-role-information-flying-ghost-burger-hostile-name = Sinister Ghost Burger
ghost-role-information-flying-ghost-burger-hostile-description = Fly through anything and hunt down the living.
ghost-role-information-orchi-dea-mime-plushie-name = Evil Orchi Dea Plushie
ghost-role-information-orchi-dea-mime-plushie-description = It's time for punishments~
ghost-role-information-pumpkin-killer-name = Pumpkin Killer
ghost-role-information-pumpkin-killer-description = You are a vicious pumpkin plant. Hunt down the living.
ghost-role-information-cthulhu-eye-boss-name = Giant Floating Eye
ghost-role-information-cthulhu-eye-boss-description = You are a colossal floating eye. Hunt down your prey with searing lasers.
