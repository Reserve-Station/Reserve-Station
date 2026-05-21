### Interaction Messages

# System

## When trying to ingest without the required utensil... but you gotta hold it
ingestion-you-need-to-hold-utensil = Вам нужно иметь {$utensil} чтобы это есть!

ingestion-try-use-is-empty = {CAPITALIZE($entity)} пуст!
ingestion-try-use-wrong-utensil = Вы не можете {$verb} {$food} с помощью {$utensil}.

ingestion-remove-mask = Сначала снимите {$entity}.

## Failed Ingestion

ingestion-you-cannot-ingest-any-more = Вы не можете {$verb} еще больше!
ingestion-other-cannot-ingest-any-more = {CAPITALIZE(SUBJECT($target))} не может {$verb} еще больше!

ingestion-cant-digest = Вы не можете переварить {$entity}!
ingestion-cant-digest-other = {CAPITALIZE(SUBJECT($target))} не может переварить {$entity}!

## Action Verbs, not to be confused with Verbs

ingestion-verb-food = есть
ingestion-verb-drink = пить

# Edible Component

edible-nom = Ням. {$flavors}
edible-nom-other = Ням.
edible-slurp = Сёрб. {$flavors}
edible-slurp-other = Сёрб.
edible-swallow = Вы глотаете { $food }.
edible-gulp = Глоть. {$flavors}
edible-gulp-other = Глоть.

edible-has-used-storage = Вы не можете {$verb} { $food } пока внутри что-то есть.

## Nouns

edible-noun-edible = съедобное
edible-noun-food = еда
edible-noun-drink = напиток
edible-noun-pill = таблетка

## Verbs

edible-verb-edible = глотать
edible-verb-food = есть
edible-verb-drink = пить
edible-verb-pill = проглотить

## Force feeding

edible-force-feed = {CAPITALIZE($user)} пытается заставить вас {$verb} что-то!
edible-force-feed-success = {CAPITALIZE($user)} заставляет вас {$verb} что-то! {$flavors}
edible-force-feed-success-user = Вы успешно накормили {$target}.
