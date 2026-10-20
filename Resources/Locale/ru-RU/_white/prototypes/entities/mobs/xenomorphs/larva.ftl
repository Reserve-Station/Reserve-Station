ent-BaseMobXenomorphSentinel = личинка ксеноморфа
    .desc = Ранняя стадия развития, характеризующаяся небольшими размерами, червеобразной формой тела и отсутствием выраженных конечностей.
    .suffix = { ent-BaseMobXenomorphAdult.suffix }

ent-MobXenomorphLarva = { ent-BaseMobXenomorphSentinel }
    .desc = { ent-BaseMobXenomorphSentinel.desc }
    .suffix = { ent-BaseMobXenomorphSentinel.suffix }

ent-MobXenomorphLarvaDungeon = { ent-BaseMobXenomorphSentinel }
    .desc = { ent-BaseMobXenomorphSentinel.desc }
    .suffix = { ent-BaseMobXenomorphDungeon.suffix }

ent-RoundStartMobXenomorphLarva = { ent-MobXenomorphLarva }
    .desc = { ent-MobXenomorphLarva.desc }
    .suffix = { ent-MobXenomorphLarva.suffix }, Раунд-старт
