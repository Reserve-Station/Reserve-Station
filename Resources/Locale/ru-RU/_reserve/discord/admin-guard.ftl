# Reserve Red - AdminGuard: действия админов и разовые события в Discord

admin-guard-title-playtime = Изменено наигранное время
admin-guard-title-rank-added = Создана группа прав
admin-guard-title-rank-updated = Изменена группа прав
admin-guard-title-rank-removed = Удалена группа прав
admin-guard-title-perms-added = Выданы права админа
admin-guard-title-perms-updated = Изменены права админа
admin-guard-title-perms-removed = Сняты права админа
admin-guard-title-discord-linked = Привязан Discord-аккаунт
admin-guard-title-discord-unlinked = Отвязан Discord-аккаунт
admin-guard-title-currency-added = Админ начислил монеты
admin-guard-title-currency-removed = Админ списал монеты
admin-guard-title-currency-set = Админ установил баланс
admin-guard-title-currency-transfer = Перевод монет между игроками
admin-guard-title-perms-violation = :rotating_light: Заблокирована попытка изменить права
admin-guard-title-session-login = Начата админская сессия
admin-guard-title-session-reload = Изменены права в админской сессии
admin-guard-title-host-promoted = :warning: Использован promotehost

admin-guard-target = **Игрок:** { $target }
admin-guard-by = **Администратор:** { $admin }
admin-guard-field = **{ $field }:** { $value }
admin-guard-field-changed = **{ $field }:** { $old } → { $new }
admin-guard-field-name = Название
admin-guard-field-title = Титул
admin-guard-field-rank = Группа
admin-guard-field-flags = Флаги
admin-guard-field-suspended = Приостановлено
admin-guard-field-discord = Discord
admin-guard-field-amount = Сумма
admin-guard-field-balance = Баланс
admin-guard-field-from = Отправитель
admin-guard-field-to = Получатель
admin-guard-field-reason = Причина
admin-guard-field-suppressed = Повторы
admin-guard-field-source = Источник
admin-guard-field-active = Активны
admin-guard-field-stealth = Стелс
admin-guard-actor = **Кто:** { $actor }
admin-guard-field-address = Адрес
admin-guard-source-db = база данных
admin-guard-source-localhost = :warning: хост - подключение с localhost (console.loginlocal)
admin-guard-source-hostuser = :warning: хост - ник совпадает с console.login_host_user
admin-guard-source-promotehost = :warning: хост - команда promotehost
admin-guard-violation-higher-rank = пытался изменить группу прав выше своих полномочий
admin-guard-violation-higher-admin = пытался изменить админа выше своих полномочий
admin-guard-violation-flags-above = пытался выдать флаги, которых у него нет
admin-guard-violation-rank-above = пытался назначить группу прав выше своих полномочий
admin-guard-violation-overlap = отправил одни и те же флаги как выданные и снятые (модифицированный клиент)
admin-guard-perms-violation-alert = :rotating_light: { $actor } **заблокирован** при изменении админских прав: { $reason }
admin-guard-perms-violation-suppressed = ещё { $count } { $count ->
    [one] раз
    [few] раза
   *[other] раз
} за предыдущие { $seconds } с
admin-guard-host-login-alert = :warning: { $name } получил **хост**-права (все флаги, в обход базы данных). Источник: { $source }
admin-guard-playtime-added = - { $tracker }: **{ $minutes }** мин.
admin-guard-playtime-set = - { $tracker }: установлено **{ $minutes }** мин.
admin-guard-none = *нет*
admin-guard-never = *никогда*
admin-guard-unknown = *неизвестно*
admin-guard-no-reason = не указана

admin-guard-kick = { $admin } кикнул игрока { $target }. Причина: { $reason }
admin-guard-freeze = { $admin } заморозил игрока { $target }.
admin-guard-unfreeze = { $admin } разморозил игрока { $target }.
admin-guard-freeze-and-mute = { $admin } заморозил и заглушил игрока { $target }.

admin-guard-admin-login = Администратор **зашёл**: { $name }
admin-guard-admin-logout = Администратор **вышел**: { $name }
admin-guard-admin-deadmin = { $name } **снял** с себя админ-права.
admin-guard-admin-readmin = { $name } **вернул** себе админ-права.

admin-guard-watchlist-joined = **За игроком установлено наблюдение:** { $player } зашёл на сервер. ID { $id }
admin-guard-watchlist-message = **Текст:** { $message }
admin-guard-watchlist-expiry = **Истекает:** { $expiry }
admin-guard-userid-mismatch = { $name } пытается подключиться с UserID игрока { $main }.
