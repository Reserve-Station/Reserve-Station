ent-RandomHumanoidSpawnerDeathSquad = роль призрака - оперативник эскадрона смерти
    .suffix = ОБР, Эскадрон смерти
    .desc = { "" }

ent-RandomHumanoidSpawnerERTLeader = роль призрака - лидер ОБР
    .suffix = ОБР, Базовый
    .desc = { "" }
ent-RandomHumanoidSpawnerERTLeaderEVA = { ent-RandomHumanoidSpawnerERTLeader }
    .suffix = ОБР, Бронескафандр
    .desc = { ent-RandomHumanoidSpawnerERTLeader.desc }
ent-RandomHumanoidSpawnerERTLeaderEVALecter = { ent-RandomHumanoidSpawnerERTLeaderEVA }
    .suffix = ОБР, Лектер, Бронескафандр
    .desc = { ent-RandomHumanoidSpawnerERTLeaderEVA.desc }
ent-RandomHumanoidSpawnerERTLeaderArmed = { ent-RandomHumanoidSpawnerERTLeaderEVA }
    .suffix = ОБР, Вооружен, Бронескафандр
    .desc = Вооружен XL8, 4 запасных магазина разного типа.

ent-RandomHumanoidSpawnerERTChaplain = роль призрака - священник ОБР
    .suffix = ОБР, Базовый
    .desc = { ent-RandomHumanoidSpawnerERTLeader.desc }
ent-RandomHumanoidSpawnerERTChaplainEVA = { ent-RandomHumanoidSpawnerERTChaplain }
    .suffix = ОБР, Бронескафандр
    .desc = { ent-RandomHumanoidSpawnerERTChaplain.desc }

ent-RandomHumanoidSpawnerERTJanitor = роль призрака - уборщик ОБР
    .suffix = ОБР, Базовый
    .desc = { ent-RandomHumanoidSpawnerERTLeader.desc }
ent-RandomHumanoidSpawnerERTJanitorEVA = { ent-RandomHumanoidSpawnerERTJanitor }
    .suffix = ОБР, Бронескафандр
    .desc = { ent-RandomHumanoidSpawnerERTJanitor.desc }

ent-RandomHumanoidSpawnerERTEngineer = роль призрака - инженер ОБР
    .suffix = ОБР, Базовый
    .desc = { ent-RandomHumanoidSpawnerERTLeader.desc }
ent-RandomHumanoidSpawnerERTEngineerEVA = { ent-RandomHumanoidSpawnerERTEngineer }
    .suffix = ОБР, Бронескафандр
    .desc = { ent-RandomHumanoidSpawnerERTEngineer.desc }
ent-RandomHumanoidSpawnerERTEngineerArmed = { ent-RandomHumanoidSpawnerERTEngineer }
    .suffix = ОБР, Вооружен, Бронескафандр
    .desc = Вооружен Силовиком, имеет детонационный шнур и коробку детонаторов.

ent-RandomHumanoidSpawnerERTSecurity = роль призрака - офицер ОБР
    .suffix = ОБР, Базовый
    .desc = { ent-RandomHumanoidSpawnerERTLeader.desc }
ent-RandomHumanoidSpawnerERTSecurityEVA = { ent-RandomHumanoidSpawnerERTSecurity }
    .suffix = ОБР, Бронескафандр
    .desc = { ent-RandomHumanoidSpawnerERTSecurity.desc }
ent-RandomHumanoidSpawnerERTSecurityEVALecter = { ent-RandomHumanoidSpawnerERTSecurityEVA }
    .suffix = ОБР, Лектер, Бронескафандр
    .desc = { ent-RandomHumanoidSpawnerERTSecurityEVA.desc }
ent-RandomHumanoidSpawnerERTSecurityArmedRifle = { ent-RandomHumanoidSpawnerERTSecurityEVA }, стрелок
    .suffix = ОБР, Винтовка, Бронескафандр
    .desc = Вооружен Лектером, 4 запасных магазина различного типа, Лазерная пушка и переносной зарядник.
ent-RandomHumanoidSpawnerERTSecurityArmedGrenade = { ent-RandomHumanoidSpawnerERTSecurityEVA }, гренадёр
    .suffix = ОБР, Гранаты, Бронескафандр
    .desc = Вооружен Гидрой с осколочными снарядами, имеет в запасе 6 фугасных, 3 ЭМИ и светошумовых снаряда.
ent-RandomHumanoidSpawnerERTSecurityArmedVanguard = { ent-RandomHumanoidSpawnerERTSecurityEVA }, авангард
    .suffix = ОБР, Авангард, Бронескафандр
    .desc = Вооружен WT550, 4 запасных магазина, 3 телескопических щита.
ent-RandomHumanoidSpawnerERTSecurityArmedShotgun = { ent-RandomHumanoidSpawnerERTSecurityEVA }, сапёр
    .suffix = ОБР, Дробовик, Бронескафандр
    .desc = Вооружен Силовиком, 3 коробки различной дроби, осколочной гранатой, детонационным шнуром и коробкой детонаторов.

ent-RandomHumanoidSpawnerERTMedical = роль призрака - медик ОБР
    .suffix = ОБР, Базовый
    .desc = { ent-RandomHumanoidSpawnerERTLeader.desc }
ent-RandomHumanoidSpawnerERTMedicalEVA = { ent-RandomHumanoidSpawnerERTMedical }
    .suffix = ОБР, Бронескафандр
    .desc = { ent-RandomHumanoidSpawnerERTMedical.desc }
ent-RandomHumanoidSpawnerERTMedicalArmed = { ent-RandomHumanoidSpawnerERTMedical }
    .suffix = ОБР, Вооружен, ВКД
    .desc = Вооружен Лектером, 4 запасных магазина разного типа.

ent-RandomHumanoidSpawnerCBURNUnit = роль призрака - агент РХБЗЗ
    .desc = { "" }
    .suffix = ОБР

ent-RandomHumanoidSpawnerCentcomOfficial = роль призрака - представитель ЦентКом
    .desc = { "" }

ent-RandomHumanoidSpawnerSyndicateAgent = роль призрака - агент Синдиката
    .desc = { "" }
ent-RandomHumanoidSpawnerNukeOp = роль призрака - ядерный оперативник
    .desc = { "" }
    .suffix = Синдикат

ent-RandomHumanoidSpawnerCluwne = роль призрака - клувень
    .desc = { "" }
    .suffix = Клувень
