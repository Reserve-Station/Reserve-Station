cqc-fail-notself = Вы не можете никого обучить с помощью { CAPITALIZE(THE($manual)) }.

cqc-fail-changeling = Нам это ни к чему.
cqc-fail-knowanother = Вы уже владеете другим боевым искусством.
cqc-fail-already = Вы уже знаете все о боевом искусстве.
cqc-success-unblocked = Ваши навыки кулинарного ближнего боя больше не привязаны к кухне.
cqc-success-learned = Вы выучили CQC.
capoeira-success-learned = Вы изучили капоэйра. Руководство сгорает в ваших руках...
dragon-success-learned = Вы изучили драконье кунг-фу. Инструкция сгорает у вас в руках...
ninjutsu-success-learned = Вы изучили ниндзюцу. Свиток сгорает у вас в руках...
hellrip-success-learned = Вы изучили адское разрывание. Свиток сгорает в ваших руках...

carp-scroll-waiting = Путешествие в тысячу миль начинается с одного шага, и путь мудрости прокладывается медленно, по одному уроку за раз.
carp-scroll-advance = Вы стали на один шаг ближе к тому, чтобы стать мастером Пути Спящего Карпа.
carp-scroll-complete = Теперь вы мастер Пути Спящего Карпа.

carp-saying-huah = ХУА!
carv-vaying-hya = ХЬЯ!
carp-saying-choo = ЧУО!
carp-saying-wuo = ВУО!
carp-saying-kya = КЬЯ!
carp-saying-huh = ХЙАХ!
carp-saying-hiyoh = ХИЙОХ!
carp-saying-strike = УДАР КАРПА!
carp-saying-bite = УКУС КАРПА!

carp-saying-banzai = БАНЗАИИИ!
carp-saying-kiya = КЬЯЯЯЯ!
carp-saying-omae = ОМАЕ ВО МОУ ШИНДЕИРУ!
carp-saying-see = ТЫ МЕНЯ НЕ ВИДИШЬ!
carp-saying-time = МОЕ ВРЕМЯ ПРИШЛО!!
carp-saying-cowabunga = КОВАБУНГА!

krav-maga-ready = Вы готовы к { $action }

martial-arts-action-sender =
    { GENDER($user) ->
        [male] Вы ударили
        [female] Вы ударили
        [epicene] Вы ударили
       *[neuter] Вы ударили
    } { $name } { $move }.
martial-arts-action-receiver =
    { $name } { GENDER($name) ->
        [male] ударил
        [female] ударила
        [epicene] ударили
       *[neuter] ударило
    } тебя { $move }.

martial-arts-fail-prone = Вы не можете использовать этот приём лёжа!
martial-arts-fail-target-down = Вы не можете использовать этот приём против сбитых целей!
martial-arts-fail-target-standing = Вы не можете использовать этот приём против стоящих целей!
capoeira-fail-low-velocity = Вы слишком медлительны, чтобы выполнить этот приём!
ninjutsu-fail-loss-of-surprise = Ваши намерения известны! Не удаётся выполнить это движение!

# Capoeira
martial-arts-combo-PushKick = толчковым ударом ногой
martial-arts-combo-CircleKick = круговым ударом ногой
martial-arts-combo-SweepKick = ударом-сметанием
martial-arts-combo-SpinKick = ударом с разворота
martial-arts-combo-KickUp = ударом с подъёма
# SleepingCarp
martial-arts-combo-SleepingCarpGnashingTeeth = скрежетом зубов
martial-arts-combo-SleepingCarpKneeHaul = киль-холом
martial-arts-combo-SleepingCarpCrashingWaves = ударом разбивающих волн
# CQC
martial-arts-combo-CQCSlam = ударом о землю
martial-arts-combo-CQCKick = пинком
martial-arts-combo-CQCRestrain = удержанием
martial-arts-combo-CQCPressure = давлением
martial-arts-combo-CQCConsecutive = серией ударов
martial-arts-combo-NeckSnap = переломом шеи
martial-arts-combo-LegSweep = подсечкой
# KungFuDragon
martial-arts-combo-DragonClaw = когтем дракона
martial-arts-combo-DragonTail = хвостом дракона
martial-arts-combo-DragonStrike = ударом дракона
# HellRip
martial-arts-combo-DropKick = ударом с разворота
martial-arts-combo-HeadRip = отвалом бошки
martial-arts-combo-TearDown = срывом
martial-arts-combo-Slam = адским ударом
# CorporateJudo
martial-arts-combo-JudoDiscombobulate = оглушением
martial-arts-combo-JudoEyePoke = ударом пальцами в глаза
martial-arts-combo-JudoThrow = броском дзюдо
martial-arts-combo-JudoArmbar = захватом руки рычагом
martial-arts-combo-JudoWheelThrow = колесом
# Ninjutsu
martial-arts-combo-BiteTheDust = канув в пыли
martial-arts-combo-DirtyKill = грязным убийством
martial-arts-combo-Assassinate = умерщвлением
martial-arts-combo-Ninjutsu-Takedown = захватом ниндзюцу

alerts-dragon-power-name = Сила дракона
alerts-dragon-power-desc = Уделите минутку размышлениям о прошлых и грядущих битвах. Это понимание защитит вас от вреда в будущем.

alerts-sneak-attack-name = Внезапная атака
alerts-sneak-attack-desc = Для истинного шиноби первый удар и последний - это одно и то же.

alerts-loss-of-surprise-name = Потеря внезапности
alerts-loss-of-surprise-desc = Ваши намерения известны! Вам потребуется несколько мгновений, чтобы снова напасть исподтишка.
