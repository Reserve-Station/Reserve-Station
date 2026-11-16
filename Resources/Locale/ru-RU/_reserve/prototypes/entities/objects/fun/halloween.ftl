# SPDX-FileCopyrightText: Made by Ceterai on GitHub.
#
# SPDX-License-Identifier: AGPL-3.0-or-later
#
# Title: Halloween on Reserve Station

ent-MobBatAngry = { ent-MobBat }
    .desc = Эта летучая мышь выглядит очень кровожадно...
    .suffix = Хэллоуин, Злой, AI
ent-SpawnMobBatAngry = спавнер - { ent-MobBatAngry }
    .desc = { ent-MarkerBase.desc }
    .suffix = Хэллоуин, Злой

ent-MobBatBlack = { ent-MobBat }
    .desc = Она подгорела или это её естественный цвет?
    .suffix = Хэллоуин, Чёрный, AI
ent-SpawnMobBatBlack = спавнер - { ent-MobBatBlack }
    .desc = { ent-MarkerBase.desc }
    .suffix = Хэллоуин, Чёрный

ent-MobBatBlackAngry = { ent-MobBatBlack }
    .desc = { ent-MobBatBlack.desc }
    .suffix = Хэллоуин, Чёрный, Злой, AI
ent-SpawnMobBatBlackAngry = спавнер - { ent-MobBatBlackAngry }
    .desc = { ent-MarkerBase.desc }
    .suffix = Хэллоуин, Чёрный, Злой

ent-MobBatBloodRed = кроваво-красная { ent-MobBat }
    .desc = Она покрыта кровью или это её естественный цвет?
    .suffix = Хэллоуин, Много крови, AI
ent-SpawnMobBatBloodRed = спавнер - { ent-MobBatBloodRed }
    .desc = { ent-MarkerBase.desc }
    .suffix = Хэллоуин, Много крови
ent-MobBatBloodRedAngry = { ent-MobBatBloodRed }
    .desc = { ent-MobBatBloodRed.desc }
    .suffix = Хэллоуин, Много крови, Злой, AI
ent-SpawnMobBatBloodRedAngry = спавнер - { ent-MobBatBloodRedAngry }
    .desc = { ent-MarkerBase.desc }
    .suffix = Хэллоуин, Много крови, Злой

ent-MobBatVampire = { ent-MobBat }
    .desc = Кажется, она намного крупнее обычной...
    .suffix = Хэллоуин, Вампир, AI
ent-SpawnMobBatVampire = спавнер - { ent-MobBatVampire }
    .desc = { ent-MarkerBase.desc }
    .suffix = Хэллоуин, Вампир
ent-MobBatVampireAngry = { ent-MobBatVampire }
    .desc = { ent-MobBatVampire.desc }
    .suffix = Хэллоуин, Вампир, Злой, AI
ent-SpawnMobBatVampireAngry = спавнер - { ent-MobBatVampireAngry }
    .desc = { ent-MarkerBase.desc }
    .suffix = Хэллоуин, Вампир, Злой

ent-SpawnMobBatAny = спавнер - { ent-MobBat }
    .desc = { ent-MarkerBase.desc }
    .suffix = Хэллоуин, Любая
ent-SpawnMobBatAnyAngry = спавнер - { ent-MobBatAngry }
    .desc = { ent-MarkerBase.desc }
    .suffix = Хэллоуин, Любая, Злой

ent-CandyBucketFilled = { ent-CandyBucket }
    .desc = { ent-CandyBucket.desc }
    .suffix = Хэллоуин, Тыква, Заполненное
ent-CandyBucketFilledMystery = { ent-CandyBucketFilled }
    .desc = { ent-CandyBucketFilled.desc }
    .suffix = Хэллоуин, Тыква, Заполненное, Таинственное
ent-CandyBucketFilledMysteryEvil = { ent-CandyBucketFilled }
    .desc = { ent-CandyBucketFilled.desc }
    .suffix = Хэллоуин, Тыква, Заполненное, Таинственное, Злое
ent-CandyBucketFilledAny = { ent-CandyBucketFilled }
    .desc = { ent-CandyBucketFilled.desc }
    .suffix = Хэллоуин, Тыква, Заполненное, Любое
ent-CandyBucketLarge = вместительное { ent-CandyBucket }
    .desc = { ent-CandyBucketFilled.desc }
    .suffix = Хэллоуин, Тыква, Заполненное, Большое
ent-CandyBucketBluespace = бездонное { ent-CandyBucket }
    .desc = { ent-CandyBucketFilled.desc }
    .suffix = Хэллоуин, Тыква, Заполненное, Блюспейс
ent-SpawnCandyBucket = спавнер - любое { ent-CandyBucket }
    .desc = { ent-MarkerBase.desc }
    .suffix = Хэллоуин, Тыква, Случайное

ent-FoodDonutJellyPumpkin = желейный тыквенный пончик
    .desc = Вкуснее всего с тыквенным пряным латте.
    .suffix = Хэллоуин

ent-HalloweenLightTube = { ent-LedLightTube }
    .desc = { ent-LedLightTube.desc }
    .suffix = Хэллоуин
ent-PoweredlightHalloween = { ent-PoweredlightLED }
    .desc = { ent-PoweredlightLED.desc }
    .suffix = Хэллоуин
ent-HalloweenLightBulb = { ent-DimLightBulb }
    .desc = { ent-DimLightBulb.desc }
    .suffix = Хэллоуин
ent-PoweredSmallLightHalloween = { ent-PoweredDimSmallLight }
    .desc = { ent-PoweredDimSmallLight.desc }
    .suffix = Хэллоуин

ent-CandleOrangeInfinite = волшебная оранжевая свеча
    .desc = { ent-CandleInfinite.desc }
    .suffix = Хэллоуин
ent-CandleOrangeSmallInfinite = маленькая волшебная оранжевая свеча
    .desc = { ent-CandleInfinite.desc }
    .suffix = Хэллоуин

ent-RandomHalloweenCandleBundleSpawner = спавнер - случайный набор хэллоуинских свечей
    .desc = Создаёт случайный набор хэллоуинских свечей.
    .suffix = Хэллоуин
ent-RandomHalloweenDecorSpawner = спавнер - случайный хэллоуинский декор
    .desc = Создаёт случайный хэллоуинский декор (тыквы, фонари и прочее).
    .suffix = Хэллоуин
ent-RandomHalloweenTableDecorSpawner = спавнер - случайный хэллоуинский декор для стола
    .desc = Создаёт случайный декор для стола (свечи, угощения и прочее).
    .suffix = Хэллоуин
ent-RandomHalloweenCrateSpawner = спавнер - случайный хэллоуинский ящик
    .desc = Создаёт случайный хэллоуинский ящик (с паутиной или гроб).
    .suffix = Хэллоуин
ent-RandomPosterSpooky = спавнер - случайный жуткий плакат
    .desc = Создаёт случайный жуткий плакат на тему Хэллоуина.
    .suffix = Хэллоуин

ent-BloodCooler = кулер с кровью
    .desc = { ent-WaterCooler.desc } Жидкость внутри подозрительно красная.
    .suffix = Хэллоуин
ent-BloodCoolerFakeCherryJuice = { ent-BloodCooler }
    .desc = { ent-BloodCooler.desc }
    .suffix = Хэллоуин, Вишнёвый сок
ent-BloodCoolerFakeTomatoJuice = { ent-BloodCooler }
    .desc = { ent-BloodCooler.desc }
    .suffix = Хэллоуин, Томатный сок
ent-RandomBloodCooler = спавнер - случайный { ent-BloodCooler }
    .desc = Создаёт случайный кулер с ненастоящей кровью.
    .suffix = Хэллоуин, В основном ненастоящая

ent-MobTomatoKillerMutant = помидор-мутант
    .desc = Такого на руки не возьмёшь...
    .suffix = Хэллоуин, Адмем, Злой, AI

ent-MobFlyingCutlass = летучая сабля
    .desc = Абордажная сабля с призрачными крыльями.
    .suffix = Хэллоуин, Адмем, AI
ent-MobFlyingCutlassHostile = { ent-MobFlyingCutlass }
    .desc = Летающая абордажная сабля, атакующая всех поблизости.
    .suffix = Хэллоуин, Адмем, Злой, AI
ent-MobFlyingClaymore = летучий клеймор
    .desc = Клеймор с собственной волей, порхающий в воздухе.
    .suffix = Хэллоуин, Адмем, AI
ent-MobFlyingClaymoreHostile = { ent-MobFlyingClaymore }
    .desc = Летающий клеймор, преследующий свои цели в воздухе.
    .suffix = Хэллоуин, Адмем, Злой, AI
ent-MobFlyingLaserGun = летучая лазерная пушка
    .desc = Лазерная пушка, зависшая в воздухе.
    .suffix = Хэллоуин, Адмем, AI
ent-MobFlyingLaserGunHostile = { ent-MobFlyingLaserGun }
    .desc = Летающая лазерная пушка, преследующая цели и стреляющая по ним.
    .suffix = Хэллоуин, Адмем, Злой, AI

ent-MobFlyingGhostBurger = { ent-FoodBurgerGhost }
    .desc = Слишком жуткий, чтобы оставаться на тарелке, но всё ещё съедобный.
    .suffix = Хэллоуин, Адмем, AI
ent-MobFlyingGhostBurgerHostile = { ent-MobFlyingGhostBurger }
    .desc = Тебе не спрятаться...
    .suffix = Хэллоуин, Адмем, Злой, AI

ent-SpiritCandleDecor = { ent-SpiritCandle }
    .desc = Декоративная свеча для проведения ритуалов.
    .suffix = Хэллоуин, Адмем, Декоративная

ent-PlushiePumpkin = плюшевая { ent-FoodPumpkin }
    .desc = Мягкая игрушка в форме тыквы.
    .suffix = Хэллоуин, { ent-BasePlushie.suffix }
ent-PlushieGhostBurger = плюшевый { ent-FoodBurgerGhost }
    .desc = Мягкая игрушка в форме призрачного бургера.
    .suffix = Хэллоуин, { ent-BasePlushie.suffix }

ent-MobPlushieCeteraiOrchiDeaMimeEvil = злобная { ent-PlushieCeteraiOrchiDeaMime }
    .desc = Похоже, она любит тебя слишком сильно...
    .suffix = { ent-PlushieCeteraiOrchiDeaMime.suffix }, Хэллоуин, Адмем, Злая, AI

ent-PumpkinBucket = тыкведро
    .desc = Ведро в виде вырезанной тыквы. Жутковато, но мыть пол им всё ещё удобно.
    .suffix = Хэллоуин, Тыква
ent-PumpkinBucketSurreal = странное { ent-PumpkinBucket }
    .desc = { ent-PumpkinBucket.desc }
    .suffix = { ent-PumpkinBucket.suffix }
ent-RandomPumpkinBucket = спавнер - случайное { ent-PumpkinBucket }
    .desc = { ent-PumpkinBucket.desc }
    .suffix = { ent-PumpkinBucket.suffix }

ent-SteelBenchWebbed = { ent-SteelBench } в паутине
    .desc = Обычная стальная скамейка, покрытая пылью и паутиной.
    .suffix = Хэллоуин

ent-StatueReaper = статуя жнеца
    .desc = Мрачный каменный жнец с тыквой-фонарём.
    .suffix = Хэллоуин, Светильник Джека
ent-StatueReaperSurreal = странная { ent-StatueReaper }
    .desc = { ent-StatueReaper.desc }
    .suffix = { ent-StatueReaper.suffix }
ent-RandomStatueReaper = спавнер - случайная { ent-StatueReaper }
    .desc = { ent-StatueReaper.desc }
    .suffix = { ent-StatueReaper.suffix }

ent-PumpkinKillerSeeds = пакет семян (тыква-убийца)
    .desc = { ent-SeedBase.desc }
    .suffix = Хэллоуин
ent-PumpkinKillerSeedsSurreal = пакет семян (странная тыква-убийца)
    .desc = { ent-PumpkinKillerSeeds.desc }
    .suffix = { ent-PumpkinKillerSeeds.suffix }

ent-MobPumpkinKiller = тыква-убийца
    .desc = Злобная тыква, охотящаяся на живых.
    .suffix = Хэллоуин, Злой, AI
ent-MobPumpkinKillerSurreal = странная { ent-MobPumpkinKiller }
    .desc = { ent-MobPumpkinKiller.desc }
    .suffix = { ent-MobPumpkinKiller.suffix }
ent-MobPumpkinKillerMutant = тыква-мутант
    .desc = { ent-MobPumpkinKiller.desc }
    .suffix = Хэллоуин, Адмем, Босс, Злой, AI

ent-MobCthulhuEyeSmall = злобный глаз
    .desc = Влажный немигающий глаз, парящий в воздухе.
    .suffix = Хэллоуин, Ктулху, Адмем, Злой, AI
ent-MobCthulhuEye = Глаз Ктулху
    .desc = Огромный злобный глаз, поражающий добычу раскалёнными лазерами.
    .suffix = Хэллоуин, Ктулху, Адмем, Злой, Босс, AI
ent-MobCthulhuEyeSmallSurreal = странный { ent-MobCthulhuEyeSmall }
    .desc = { ent-MobCthulhuEyeSmall.desc }
    .suffix = { ent-MobCthulhuEyeSmall.suffix }
ent-MobCthulhuEyeSurreal = странный { ent-MobCthulhuEye }
    .desc = { ent-MobCthulhuEye.desc }
    .suffix = { ent-MobCthulhuEye.suffix }

ent-VendingMachineHalloweenPumpkin = Тыквенные вкусности
    .desc = Автомат в форме тыквы, полный сезонных угощений.
    .suffix = Хэллоуин
ent-VendingMachineHalloweenSpooky = Ноктюрн
    .desc = Мрачный торговый автомат с жуткими угощениями.
    .suffix = Хэллоуин
ent-VendingMachineHalloweenPumpkinSurreal = Странные { ent-VendingMachineHalloweenPumpkin }
    .desc = { ent-VendingMachineHalloweenPumpkin.desc }
    .suffix = { ent-VendingMachineHalloweenPumpkin.suffix }
ent-VendingMachineHalloweenSpookySurreal = Странный { ent-VendingMachineHalloweenSpooky }
    .desc = { ent-VendingMachineHalloweenSpooky.desc }
    .suffix = { ent-VendingMachineHalloweenSpooky.suffix }
ent-RandomVendingMachineHalloween = спавнер - случайный хэллоуинский автомат
    .desc = Случайный автомат с хэллоуинскими угощениями.
    .suffix = Хэллоуин

seeds-pumpkin-killer-name = тыква-убийца
seeds-pumpkin-killer-display-name = тыквенное растение-убийца

ghost-role-information-tomato-mutant-name = Помидор-мутант
ghost-role-information-tomato-mutant-description = Вы — необычайно крупный и крепкий томат-убийца. Охотьтесь на живых.
ghost-role-information-flying-cutlass-name = Летучая абордажная сабля
ghost-role-information-flying-cutlass-description = Вы — летающая абордажная сабля. Взлетайте и сами решайте, сражаться или отступить.
ghost-role-information-flying-cutlass-hostile-name = Зловещая абордажная сабля
ghost-role-information-flying-cutlass-hostile-description = Охотьтесь на людей поблизости с помощью своего клинка.
ghost-role-information-flying-claymore-name = Летучий клеймор
ghost-role-information-flying-claymore-description = Вы — летающий клеймор. Взлетайте и сами решайте, сражаться или отступить.
ghost-role-information-flying-claymore-hostile-name = Зловещий клеймор
ghost-role-information-flying-claymore-hostile-description = Охотьтесь на людей поблизости с помощью своего клинка.
ghost-role-information-flying-laser-gun-name = Летучая лазерная пушка
ghost-role-information-flying-laser-gun-description = Вы — летающая лазерная пушка. Взлетайте и сами решайте, сражаться или отступить.
ghost-role-information-flying-laser-gun-hostile-name = Зловещая лазерная пушка
ghost-role-information-flying-laser-gun-hostile-description = Охотьтесь на людей поблизости и стреляйте в них из лазера.
ghost-role-information-flying-ghost-burger-name = Летучий призрачный бургер
ghost-role-information-flying-ghost-burger-description = Вы — съедобный призрачный бургер, пролетающий сквозь любые преграды. Время для приключений!
ghost-role-information-flying-ghost-burger-hostile-name = Зловещий призрачный бургер
ghost-role-information-flying-ghost-burger-hostile-description = Летайте сквозь преграды и охотьтесь на живых.
ghost-role-information-orchi-dea-mime-plushie-name = Злобная плюшевая Орхи
ghost-role-information-orchi-dea-mime-plushie-description = Пришло время наказаний~
ghost-role-information-pumpkin-killer-name = Тыква-убийца
ghost-role-information-pumpkin-killer-description = Вы — злобное тыквенное растение. Охотьтесь на живых.
ghost-role-information-cthulhu-eye-boss-name = Гигантский парящий глаз
ghost-role-information-cthulhu-eye-boss-description = Вы — колоссальный парящий глаз. Охотьтесь на добычу раскалёнными лазерами.
