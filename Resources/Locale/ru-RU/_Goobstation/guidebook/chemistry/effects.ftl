reagent-effect-guidebook-deal-stamina-damage =
    { $chance ->
        [1]
            { $deltasign ->
                [1] сделки
               *[-1] исцеляет
            }
       *[other]
            { $deltasign ->
                [1] сделка
               *[-1] исцеление
            }
    } { $amount } { $immediate ->
        [true] немедленно
       *[false] сверхурочные
    } урон выносливости

reagent-effect-guidebook-stealth-entities = Маскирует живых существ поблизости.

reagent-effect-guidebook-change-faction = Изменяет фракцию существа на {$faction}.

reagent-effect-guidebook-mutate-plants-nearby = Случайным образом мутирует близлежащие растения.

reagent-effect-guidebook-dnascramble = Изменяет ДНК гуманоида.

reagent-effect-guidebook-change-species = Меняет расу цели на {$species}.

reagent-effect-guidebook-change-species-random = Меняет расу цели на случайную.

reagent-effect-guidebook-sex-change = Меняет пол гуманоида.

reagent-effect-guidebook-immunity-modifier =
    { $chance ->
        [1] Изменяет
        *[other] изменяют
    } скорость усиления иммунитета на {NATURALFIXED($gainrate, 5)}, силу на {NATURALFIXED($strength, 5)} как минимум на {NATURALFIXED($time, 3)} {MANY("секунд", $time)}

reagent-effect-guidebook-disease-progress-change =
    { $chance ->
        [1] Изменяет
        *[other] изменяют
    } прогрессию {$type} болезней на {NATURALFIXED($amount, 5)}

reagent-effect-guidebook-disease-mutate = Мутирует болезнь на {NATURALFIXED($amount, 4)}
