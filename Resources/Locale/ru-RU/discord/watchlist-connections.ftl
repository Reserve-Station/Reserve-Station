discord-watchlist-connection-header =
    { $players ->
        [one] {$players} игрок из списка наблюдения подключился
       *[other] {$players} игроков из списка наблюдения подключились
    } к {$serverName}
discord-watchlist-connection-entry = - {$playerName} с сообщением «{$message}»{ $expiry ->
        *[other] {" "}(истекает <t:{$expiry}:R>)
    }{ $otherWatchlists ->
        [0] {""}
        [one] {" "}и ещё {$otherWatchlists} запись в списке наблюдения
        *[other] {" "}и ещё {$otherWatchlists} записей в списке наблюдения
    }
