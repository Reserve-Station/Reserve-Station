entity-effect-guidebook-modify-disgust =
    { $chance ->
        [1] { $deltasign ->
                [1] увеличивается
                *[-1] снижается
            }
        *[other]
            { $deltasign ->
                [1] увеличение
                *[-1] снижение
            }
    } уровень отвращения по { $amount }
