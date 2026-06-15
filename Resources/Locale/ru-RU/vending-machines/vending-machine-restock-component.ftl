vending-machine-restock-invalid-inventory = { CAPITALIZE($this) } не подходит для того, чтобы пополнить { $target }.
vending-machine-restock-needs-panel-open = Техническая панель { CAPITALIZE($target) } должна быть открыта.
vending-machine-restock-start = { $user } начинает пополнять { $target }.
vending-machine-restock-done =
    { $user } { GENDER($user) ->
        [male] закончил
        [female] закончила
        [epicene] закончили
       *[neuter] закончило
    } пополнять { $target }.

vending-machine-restock-start-self = Вы начинаете пополнять запасы { THE($target) }.

vending-machine-restock-start-others = { CAPITALIZE(THE($user)) } начинает пополнение запасов { THE($target) }.

vending-machine-restock-done-self = Вы завершаете пополнение запасов { THE($target) }.

vending-machine-restock-done-others = { CAPITALIZE(THE($user)) } завершает пополнение запасов { THE($target) }.
