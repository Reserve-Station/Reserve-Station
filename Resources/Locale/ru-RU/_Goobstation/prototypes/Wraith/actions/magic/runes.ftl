ent-BaseRune = руна
    .desc = { "" }

ent-CollideRune = { ent-BaseRune } столкновения
    .desc = { ent-BaseRune.desc }
ent-ActivateRune = { ent-BaseRune } активации
    .desc = { ent-CollideRune.desc }
ent-ExplosionRune = { ent-BaseRune } взрыва
    .desc = { ent-CollideRune.desc }
ent-StunRune = { ent-BaseRune } оглушения
    .desc = { ent-CollideRune.desc }
ent-IgniteRune = { ent-BaseRune } зажигания
    .desc = { ent-CollideRune.desc }
ent-FlashRune = { ent-BaseRune } вспышки
    .desc = { ent-ActivateRune.desc }

ent-CollideTimerRune = { ent-CollideRune } с задержкой
    .desc = { ent-CollideRune.desc }
ent-ExplosionTimedRune = { ent-ExplosionRune } с задержкой
    .desc = { ent-CollideTimerRune.desc }
ent-ExplosionActivateRune = { ent-ActivateRune } с задержкой
    .desc = { ent-ActivateRune.desc }
ent-FlashRuneTimer = { ent-FlashRune } с задержкой
    .desc = { ent-CollideTimerRune.desc }
