ent-BaseMobAsteroid = { ent-BaseMob }
    .desc = { ent-BaseMob.desc }
    .suffix = Астероиды, Злой, AI

ent-MobGoliath = голиаф
    .desc = Массивное чудовище, использующее свои длинные щупальца для ловли добычи. Не рекомендуется угрожать им ни при каких условиях.
    .suffix = { ent-BaseMobAsteroid.suffix }
ent-ActionGoliathTentacle = [color=red]Удар щупальцем[/color]
    .desc = Используйте свои щупальца, чтобы схватить и оглушить игрока!
ent-GoliathTentacle = щупальце
    .desc = Не дайте ему добраться до вас!
    .suffix = { ent-BaseMobAsteroid.suffix }
ent-BaseEffectGoliathTentacleSpawn = щупальце
    .desc = { ent-EffectBase.desc }
ent-EffectGoliathTentacleSpawn = щупальце
    .desc = { ent-BaseEffectGoliathTentacleSpawn.desc }
ent-EffectGoliathTentacleRetract = { ent-BaseEffectGoliathTentacleSpawn }
    .desc = { ent-BaseEffectGoliathTentacleSpawn.desc }

ent-MobHivelord = повелитель роя
    .desc = Воистину инопланетное существо, представляющее собой массу неизвестного органического, постоянно колышущегося материала. Во время атаки его части отделяются и атакуют совместно с оригиналом.
    .suffix = { ent-BaseMobAsteroid.suffix }
ent-MobHivelordBrood = отпрыск повелителя роя
    .desc = Осколок оригинального повелителя роя, поддерживающий своего оригинала. Один особой угрозы не представляет, но...
    .suffix = { ent-BaseMobAsteroid.suffix }
ent-FoodHivelordRemains = остатки повелителя роя
    .desc = Это всё, что осталось от повелителя роя, и, похоже, именно это позволяет ему безвредно отделять от себя части... Его целебные свойства скоро утратят силу, если не воспользоваться ими поскорее. Постарайтесь не думать о том, что именно вы едите.
ent-FoodHivelordRemainsInert = инертные остатки повелителя роя
    .desc = Это всё, что осталось от повелителя роя... Теперь точно всё.

ent-MobBasilisk = василиск
    .desc = Территориальное чудовище, покрытое толстым панцирем, поглощающим энергию. Его взгляд заставляет жертв замерзать изнутри.
    .suffix = { ent-BaseMobAsteroid.suffix }
