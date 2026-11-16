# SPDX-FileCopyrightText: Made by Ceterai on GitHub.
#
# SPDX-License-Identifier: AGPL-3.0-or-later
#
# Title: Stardust

stardust-shared-suffix = Орхи Дея, { entity-category-suffix-donotmap }

seeds-stardust-name = звёздная орхидея
seeds-stardust-display-name = звёздная орхидея
flavor-complex-stardust = как звёздная пыль
reagent-name-stardust = звёздная пыль
reagent-desc-stardust = Кажется, будто она что-то тебе шепчет...
reagent-name-stardust-shared-suffix = Звёздная пыль, { stardust-shared-suffix }
reagent-name-stardust-goddess = коктейль богини
reagent-desc-stardust-goddess = Кажется, будто Орхи что-то тебе шепчет...
reagent-name-stardust-pill = 10 ед.
reagent-name-stardust-label = улыбнись~
reagent-name-stardust-goddess-shared-suffix = Коктейль Богини, { reagent-name-stardust-shared-suffix }
mood-effect-stardust = Я чувствую, как звёздная пыль наполняет меня жизнью!

ent-StardustSeeds = пакет семян (звёздная орхидея)
    .desc = { ent-SeedBase.desc }
    .suffix = { stardust-shared-suffix }
ent-MaterialStardust = горка звёздной пыли
    .desc = Кучка звёздной пыли, почему-то беспокойная. Можно поместить в любую ёмкость с помощью измельчителя.
    .suffix = { stardust-shared-suffix }
ent-LeavesStardust = лепестки звёздной орхидеи
    .desc = Они слабо светятся в темноте... Могут быть измельчены в пыль. Либо можно надеть их на голову, как венок. Либо на руку, как браслет. Или на шею, как ожерелье. Или на уши, как клипсы. Или на глаза, как линзы.
    .suffix = { stardust-shared-suffix }

ent-JointStardust = звёздная скрутка
    .desc = Медленно и безопасно, главное не мусорить и не мешать другим.
    .suffix = { reagent-name-stardust-shared-suffix }, Косяк
ent-PillCanisterStardust = баночка счастья
    .desc = Кажется, из неё доносится слабый шёпот...
    .suffix = { reagent-name-stardust-shared-suffix }
ent-PillStardust = звёздная пыль
    .desc = Маленькая, блестящая таблетка. Из чего она сделана?..
    .suffix = Таблетка, { stardust-shared-suffix }
ent-FoodSnackStardust = звёздное драже
    .desc = Маленькая, сладкая блестящая конфетка. Из чего она сделана?..
    .suffix = { reagent-name-stardust-shared-suffix }
ent-FoodSnackLollypopStardust = леденец со звёздной пылью
    .desc = Леденец со вкусом звёздной пыли. И, возможно, не только вкусом...
    .suffix = Лодаут, Безделушки, { stardust-shared-suffix }

ent-DrinkStardustGlass = { ent-DrinkGlass }
    .desc = { reagent-desc-stardust }
    .suffix = { reagent-name-stardust-shared-suffix }
ent-DrinkStardustJug = { ent-CustomDrinkJug }
    .desc = { reagent-desc-stardust }
    .suffix = { reagent-name-stardust-shared-suffix }
ent-ChemistryBottleStardust = { ent-BaseChemistryBottleFilled }
    .desc = { reagent-desc-stardust }
    .suffix = { reagent-name-stardust-shared-suffix }
ent-SyringeStardust = { ent-PrefilledSyringe }
    .desc = { ent-PrefilledSyringe.desc }
    .suffix = { reagent-name-stardust-shared-suffix }
ent-JugStardust = { ent-Jug }
    .desc = { reagent-desc-stardust }
    .suffix = { reagent-name-stardust-shared-suffix }
ent-BucketStardust = { ent-Bucket }
    .desc = { reagent-desc-stardust }
    .suffix = { reagent-name-stardust-shared-suffix }
ent-DrinkStardustBottleFull = { ent-DrinkBottle }
    .desc = { reagent-desc-stardust }
    .suffix = { reagent-name-stardust-shared-suffix }
ent-DrinkStardustCarton = { ent-DrinkCarton }
    .desc = { reagent-desc-stardust }
    .suffix = { reagent-name-stardust-shared-suffix }
ent-FoodCondimentBottleStardust = { ent-DrinkBottleTall }
    .desc = { reagent-desc-stardust }
    .suffix = { reagent-name-stardust-shared-suffix }
ent-DrinkStardustCup = { ent-DrinkCup }
    .desc = { reagent-desc-stardust }
    .suffix = { reagent-name-stardust-shared-suffix }

ent-DrinkStardustGoddessGlass = { ent-DrinkGlass }
    .desc = { reagent-desc-stardust-goddess }
    .suffix = { reagent-name-stardust-goddess-shared-suffix }
ent-DrinkStardustGoddessJug = { ent-CustomDrinkJug }
    .desc = { reagent-desc-stardust-goddess }
    .suffix = { reagent-name-stardust-goddess-shared-suffix }
ent-ChemistryBottleStardustGoddess = { ent-BaseChemistryBottleFilled }
    .desc = { reagent-desc-stardust-goddess }
    .suffix = { reagent-name-stardust-goddess-shared-suffix }
ent-SyringeStardustGoddess = { ent-PrefilledSyringe }
    .desc = { ent-PrefilledSyringe.desc }
    .suffix = { reagent-name-stardust-goddess-shared-suffix }
ent-JugStardustGoddess = { ent-Jug }
    .desc = { reagent-desc-stardust-goddess }
    .suffix = { reagent-name-stardust-goddess-shared-suffix }
ent-BucketStardustGoddess = { ent-Bucket }
    .desc = { reagent-desc-stardust-goddess }
    .suffix = { reagent-name-stardust-goddess-shared-suffix }
ent-DrinkStardustGoddessBottleFull = { ent-DrinkBottle }
    .desc = { reagent-desc-stardust-goddess }
    .suffix = { reagent-name-stardust-goddess-shared-suffix }
ent-DrinkStardustGoddessCarton = { ent-DrinkCarton }
    .desc = { reagent-desc-stardust-goddess }
    .suffix = { reagent-name-stardust-goddess-shared-suffix }
ent-FoodCondimentBottleStardustGoddess = { ent-DrinkBottleTall }
    .desc = { reagent-desc-stardust-goddess }
    .suffix = { reagent-name-stardust-goddess-shared-suffix }
ent-DrinkStardustGoddessCup = { ent-DrinkCup }
    .desc = { reagent-desc-stardust-goddess }
    .suffix = { reagent-name-stardust-goddess-shared-suffix }

ent-FoodSnackStardustGoddess = драже богини
    .desc = Кажется, будто эту конфету приготовила богиня Орхи Дея.
    .suffix = { reagent-name-stardust-goddess-shared-suffix }
ent-FoodSnackLollypopStardustGoddess = леденец богини
    .desc = Леденец со вкусом коктейля богини. И, возможно, не только вкусом...
    .suffix = Лодаут, Безделушки, { reagent-name-stardust-goddess-shared-suffix }
