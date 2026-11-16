# Трубка-светильник

ent-Poweredlight = светильник
    .desc = Осветительный прибор. Потребляет энергию и излучает свет, если оснащён лампой-трубкой.

ent-PoweredlightEmpty = { ent-Poweredlight }
    .desc = { ent-Poweredlight.desc }
    .suffix = Пустой
ent-AlwaysPoweredWallLight = { ent-Poweredlight }
    .desc = Всегда работающий светильник.
    .suffix = Всегда запитанный

ent-PoweredlightLED = { ent-Poweredlight }
    .desc = { ent-Poweredlight.desc }
    .suffix = LED, Светодиод
ent-AlwaysPoweredLightLED = { ent-AlwaysPoweredWallLight }
    .desc = { ent-AlwaysPoweredWallLight.desc }
    .suffix = Всегда запитанный, LED, Светодиод

ent-PoweredlightExterior = { ent-Poweredlight }
    .desc = { ent-Poweredlight.desc }
    .suffix = Экстерьер
ent-AlwaysPoweredLightExterior = { ent-AlwaysPoweredWallLight }
    .desc = { ent-AlwaysPoweredWallLight.desc }
    .suffix = Всегда запитанный, Экстерьер

ent-PoweredlightSodium = { ent-Poweredlight }
    .desc = { ent-Poweredlight.desc }
    .suffix = Натриевый
ent-AlwaysPoweredLightSodium = { ent-AlwaysPoweredWallLight }
    .desc = { ent-AlwaysPoweredWallLight.desc }
    .suffix = Всегда запитанный, Натриевый

# Маленький светильник

ent-PoweredSmallLight = маленький светильник
    .desc = Осветительный прибор. Потребляет энергию и излучает свет, если оснащён лампой-трубкой.

ent-PoweredSmallLightEmpty = { ent-PoweredSmallLight }
    .desc = { ent-PoweredSmallLight.desc }
    .suffix = Пустой
ent-SmallLight = { ent-PoweredSmallLight }
    .desc = Всегда работающий светильник.
    .suffix = Всегда запитанный

ent-PoweredLEDSmallLight = { ent-PoweredSmallLightEmpty }
    .suffix = LED, Светодиод
    .desc = { ent-PoweredSmallLightEmpty.desc }
ent-PoweredDimSmallLight = { ent-PoweredSmallLightEmpty }
    .suffix = Тусклый
    .desc = { ent-PoweredSmallLightEmpty.desc }
ent-PoweredWarmSmallLight = { ent-PoweredSmallLightEmpty }
    .desc = { ent-PoweredSmallLightEmpty.desc }
    .suffix = Тёплый

ent-EmergencyLight = аварийная лампа
    .desc = Маленькая лампочка с внутренним аккумулятором, которая включается, как только перестаёт получать питание извне. Технология NanoTrasen позволяет ей адаптировать свой цвет для оповещения экипажа об обстановке на станции.

ent-PoweredlightCyan = { ent-Poweredlight }
    .suffix = Голубой
    .desc = { ent-Poweredlight.desc }
ent-AlwaysPoweredlightCyan = { ent-AlwaysPoweredWallLight }
    .suffix = Всегда запитанный, Голубой
    .desc = { ent-AlwaysPoweredWallLight.desc }

ent-PoweredlightBlue = { ent-Poweredlight }
    .suffix = Синий
    .desc = { ent-Poweredlight.desc }
ent-AlwaysPoweredlightBlue = { ent-AlwaysPoweredWallLight }
    .suffix = Всегда запитанный, Синий
    .desc = { ent-AlwaysPoweredWallLight.desc }

ent-PoweredlightYellow = { ent-Poweredlight }
    .suffix = Жёлтый
    .desc = { ent-Poweredlight.desc }
ent-AlwaysPoweredlightYellow = { ent-AlwaysPoweredWallLight }
    .suffix = Всегда запитанный, Жёлтый
    .desc = { ent-AlwaysPoweredWallLight.desc }

ent-PoweredlightPink = { ent-Poweredlight }
    .suffix = Розовый
    .desc = { ent-Poweredlight.desc }
ent-AlwaysPoweredlightPink = { ent-AlwaysPoweredWallLight }
    .suffix = Всегда запитанный, Розовый
    .desc = { ent-AlwaysPoweredWallLight.desc }

ent-PoweredlightOrange = { ent-Poweredlight }
    .suffix = Оранжевый
    .desc = { ent-Poweredlight.desc }
ent-AlwaysPoweredlightOrange = { ent-AlwaysPoweredWallLight }
    .suffix = Всегда запитанный, Оранжевый
    .desc = { ent-AlwaysPoweredWallLight.desc }

ent-PoweredlightBlack = { ent-Poweredlight }
    .suffix = Чёрный
    .desc = { ent-Poweredlight.desc }
ent-AlwaysPoweredlightBlack = { ent-AlwaysPoweredWallLight }
    .suffix = Всегда запитанный, Чёрный
    .desc = { ent-AlwaysPoweredWallLight.desc }

ent-PoweredlightRed = { ent-Poweredlight }
    .suffix = Красный
    .desc = { ent-Poweredlight.desc }
ent-AlwaysPoweredlightRed = { ent-AlwaysPoweredWallLight }
    .suffix = Всегда запитанный, Красный
    .desc = { ent-AlwaysPoweredWallLight.desc }

ent-PoweredlightGreen = { ent-Poweredlight }
    .suffix = Зелёный
    .desc = { ent-Poweredlight.desc }
ent-AlwaysPoweredlightGreen = { ent-AlwaysPoweredWallLight }
    .suffix = Всегда запитанный, Зелёный
    .desc = { ent-AlwaysPoweredWallLight.desc }

# Напольный светильник

ent-PoweredFloorLight = напольный светильник
    .desc = Осветительный прибор, встроенный в пол. Потребляет электроэнергию и светит при использовании лампочки.

ent-AlwaysPoweredFloorLight = { ent-PoweredFloorLight }
    .desc = Всегда работающий напольный светильник.
    .suffix = Всегда запитанный
ent-PoweredFloorLightEmpty = { ent-PoweredFloorLight }
    .desc = { ent-PoweredFloorLight.desc }
    .suffix = Пустой
