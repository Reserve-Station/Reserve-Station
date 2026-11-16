guide-entry-painting = Рисование

paint-window-title = Холст
paint-window-tools = Инструменты
paint-window-brush = Стиль кисти
paint-window-size = Размер
paint-window-size-value = {$size}px
paint-window-color = Цвет
paint-window-no-tool = Держите мелок или кисть, чтобы рисовать.
paint-window-undo = Отменить последний штрих (Ctrl+Z)
paint-window-finish = Завершить
paint-window-title-placeholder = Заголовок
paint-window-finish-button = Завершить и подписать
paint-window-finish-confirm = Подписать? После этого изменить нельзя.
paint-window-finished = [bold]{$title}[/bold]
    Автор: {$artist}

paint-tool-brush = Кисть
paint-tool-eraser = Ластик
paint-tool-line = Линия
paint-tool-rectangle = Прямоугольник
paint-tool-ellipse = Эллипс
paint-tool-picker = Пипетка

paint-style-round = Круглый
paint-style-square = Квадратный
paint-style-spray = Аэрозоль
paint-style-calligraphy = Каллиграфический

paint-tool-empty = {CAPITALIZE(THE($tool))} полностью использован.

paint-untitled = Без названия
paint-unknown-artist = неизвестным художником
paint-finished-name = картина "{$title}"
paint-copy-name = копия "{$title}"
paint-copy-artist = {$artist} (копия)
paint-finished-popup = Вы подписали "{$title}".
paint-examine-finished = Она называется [bold]{$title}[/bold], подписана {$artist}.
paint-fax-use-photocopier = Факсы не могут обрабатывать картины. Используйте ксерокс.

paint-slot-canvas = холст

paint-frame-unfinished = Завершите и подпишите картину перед её оформлением в рамку.
paint-frame-already-framed = Эта картина уже в рамке.
paint-frame-applied = Вы оформили {THE($painting)} в рамку.

paint-log-view-button = Просмотреть картину
paint-log-viewer-title = Картина {$token}
paint-log-viewer-missing = Эта картина не найдена. Возможно, она ещё не была сохранена в базе данных.

cmd-paintloadurl-desc = Загружает изображение и растягивает его на поверхность, пригодную для рисования, например, на холст.
cmd-paintloadurl-help = Использование: paintloadurl <uid сущности> <http(s) url>
cmd-paintloadurl-hint-target = <uid сущности>
cmd-paintloadurl-hint-url = <http(s) url>
cmd-paintloadurl-bad-target = {$target} не является поверхностью, пригодной для рисования.
cmd-paintloadurl-bad-url = URL должен быть абсолютным адресом http или https.
cmd-paintloadurl-downloading = Загрузка {$url}...
cmd-paintloadurl-too-large = Изображение больше 8 МБ.
cmd-paintloadurl-download-failed = Не удалось загрузить: {$error}
cmd-paintloadurl-bad-image = Не удалось прочитать изображение: {$error}
cmd-paintloadurl-success = Изображение загружено на {$target}.

reagent-desc-paint = Густая, ярко окрашенная краска. Налейте её в банку для краски, чтобы рисовать.
reagent-name-paint-red = красная краска
reagent-name-paint-yellow = жёлтая краска
reagent-name-paint-green = зелёная краска
reagent-name-paint-blue = синяя краска
reagent-name-paint-violet = фиолетовая краска
reagent-name-paint-black = чёрная краска
reagent-name-paint-white = белая краска

photocopier-verb-copy = Копировать картину
photocopier-busy = Она уже копирует.
photocopier-unpowered = У неё нет питания.
photocopier-needs-canvases = Сначала положите картину и пустой холст.
photocopier-nothing-to-copy = Оба холста пусты.
photocopier-needs-blank = Один из холстов должен быть пустым и неподписанным.
photocopier-size-mismatch = Пустой холст должен быть того же размера, что и картина.
