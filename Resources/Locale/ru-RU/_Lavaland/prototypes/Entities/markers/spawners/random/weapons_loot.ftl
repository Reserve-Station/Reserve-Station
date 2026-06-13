ent-LavalandWeaponLootSpawner = спавнер лут оружие
    .suffix = Лаваленд
    .desc = { ent-MarkerBase.desc }

ent-CrusherWeaponLootSpawner = { ent-LavalandWeaponLootSpawner }-крушитель
    .suffix = { ent-LavalandWeaponLootSpawner.suffix }
    .desc = { ent-MarkerBase.desc }
ent-CrusherUpgradeLootSpawner = { ent-CrusherWeaponLootSpawner }
    .suffix = { ent-LavalandWeaponLootSpawner.suffix }, Улучшенный
    .desc = { ent-MarkerBase.desc }

ent-ProtoKineticWeaponLootSpawner = { ent-LavalandWeaponLootSpawner } прото-кинетическое
    .suffix = { ent-LavalandWeaponLootSpawner.suffix }
    .desc = { ent-MarkerBase.desc }
ent-ProtoKineticWeaponSpaceLootSpawner = { ent-ProtoKineticWeaponLootSpawner }
    .suffix = { ent-ProtoKineticWeaponLootSpawner.suffix }, Улучшенный
    .desc = { ent-MarkerBase.desc }
ent-ProtoKineticUpgradeLootSpawner = { ent-ProtoKineticWeaponLootSpawner }
    .suffix = { ent-ProtoKineticWeaponLootSpawner.suffix }, Улучшение
    .desc = { ent-MarkerBase.desc }
ent-SecondaryKineticWeaponLootSpawner = { ent-ProtoKineticWeaponLootSpawner }
    .suffix = { ent-ProtoKineticWeaponLootSpawner.suffix }, Вторичное
    .desc = { ent-MarkerBase.desc }
