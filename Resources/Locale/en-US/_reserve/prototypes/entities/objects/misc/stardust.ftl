# SPDX-FileCopyrightText: Made by Ceterai on GitHub.
#
# SPDX-License-Identifier: AGPL-3.0-or-later
#
# Title: Stardust

stardust-shared-suffix = Orchi Dea, { entity-category-suffix-donotmap }

seeds-stardust-name = stardust orchid
seeds-stardust-display-name = stardust orchid
flavor-complex-stardust = like stardust
reagent-name-stardust = stardust
reagent-desc-stardust = It feels like it's whispering something to you...
reagent-name-stardust-shared-suffix = Stardust, { stardust-shared-suffix }
reagent-name-stardust-goddess = goddess cocktail
reagent-desc-stardust-goddess = It feels like Orchi is whispering something to you...
reagent-name-stardust-pill = 10u
reagent-name-stardust-label = smile~
reagent-name-stardust-goddess-shared-suffix = Goddess cocktail, { reagent-name-stardust-shared-suffix }
mood-effect-stardust = I feel stardust fueling me with life!

ent-StardustSeeds = packet of stardust seeds
    .desc = { ent-SeedBase.desc }
    .suffix = { stardust-shared-suffix }
ent-MaterialStardust = stardust pile
    .desc = A pile of stardust, somehow quite unrest. Can be placed in any container using a reagent grinder.
    .suffix = { stardust-shared-suffix }
ent-LeavesStardust = stardust orchid petals
    .desc = They glow faintly in the dark... Can be grinded into dust, or ground into a pile. Or you can put them on your head like a wreath. Or on your arm like a bracelet. Or on your neck like a necklace. Or on your ears like clips. Or on your eyes like lenses.
    .suffix = { stardust-shared-suffix }

ent-JointStardust = stardust roll
    .desc = Slow and safe, just don't litter and don't bother others.
    .suffix = { reagent-name-stardust-shared-suffix }, Joint
ent-PillCanisterStardust = happy bottle
    .desc = You hear faint whispers coming from it...
    .suffix = { reagent-name-stardust-shared-suffix }
ent-PillStardust = stardust pill
    .desc = A small, shiny pill. What is it made of?..
    .suffix = { stardust-shared-suffix }
ent-FoodSnackStardust = stardust candy
    .desc = A small, sweet shiny dragee. What is it made of?..
    .suffix = { reagent-name-stardust-shared-suffix }
ent-FoodSnackLollypopStardust = stardust lollypop
    .desc = A lollypop with the taste of stardust. And, possibly, not just the taste...
    .suffix = Loadout, Trinkets, { stardust-shared-suffix }

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

ent-FoodSnackStardustGoddess = goddess candy
    .desc = It seems like this dragee was made by Orchi Dea goddess herself.
    .suffix = { reagent-name-stardust-goddess-shared-suffix }
ent-FoodSnackLollypopStardustGoddess = goddess lollypop
    .desc = A lollypop with the taste of the goddess cocktail. And, possibly, not just the taste...
    .suffix = Loadout, Trinkets, { reagent-name-stardust-goddess-shared-suffix }
