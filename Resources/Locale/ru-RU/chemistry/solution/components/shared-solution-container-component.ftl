shared-solution-container-component-on-examine-empty-container = Не содержит вещества.
shared-solution-container-component-on-examine-main-text = Содержит [color={$color}]{$desc}[/color] { $chemCount ->
    [1] вещество.
   *[other] вещество-смесь.
    }
shared-solution-container-component-on-examine-worded-amount-one-reagent = вещество.
shared-solution-container-component-on-examine-worded-amount-multiple-reagents = вещество-смесь.
examinable-solution-has-recognizable-chemicals = В этом растворе вы можете распознать { $recognizedString }.
examinable-solution-recognized-first = [color={ $color }]{ $chemical }[/color]
examinable-solution-recognized-next = , [color={ $color }]{ $chemical }[/color]
examinable-solution-recognized-last = и [color={ $color }]{ $chemical }[/color]
examinable-solution-on-examine-volume = Емкость { $fillLevel ->
    [exact] заполнена на [color=white]{$current}/{$max}[/color] ед.
   *[other] [bold]{ -solution-vague-fill-level(fillLevel: $fillLevel) }[/bold].
}

examinable-solution-on-examine-volume-no-max = Емкость { $fillLevel ->
    [exact] заполнена на [color=white]{$current}[/color] ед.
   *[other] [bold]{ -solution-vague-fill-level(fillLevel: $fillLevel) }[/bold].
}

examinable-solution-on-examine-volume-puddle = Лужа { $fillLevel ->
    [exact] [color=white]{$current}[/color] ед.
    [full] течет через края!
    [mostlyfull] течет через края!
    [halffull] глубокая и течет.
    [halfempty] глубокая.
   *[mostlyempty] небольшая.
    [empty] еле заметна.
}

-solution-vague-fill-level =
    { $fillLevel ->
        [full] [color=white]полная[/color]
        [mostlyfull] [color=#DFDFDF]почти полная[/color]
        [halffull] [color=#C8C8C8]заполнена больше чем наполовину[/color]
        [halfempty] [color=#C8C8C8]заполнена почти наполовину[/color]
        [mostlyempty] [color=#A4A4A4]почти пустая[/color]
       *[empty] [color=gray]пустая[/color]
    }
