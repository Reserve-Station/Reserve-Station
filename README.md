<p align="center"> <img alt="Reserve Station 14" width="100%" src="https://i.imgur.com/yagb8UO.png" /></p>

<div align="center" style="font-size: 150%;">
<a href="https://reserve-station.space/">САЙТ</a> | <a href="https://discord.gg/WXZvqzZ2Fc">ДИСКОРД</a> | <a href="https://boosty.to/reserve-station">БУСТИ</a>
</div>

---

**Резерв** - это русскоязычный форк [Goob Station](https://github.com/Goob-Station/Goob-Station), который, в свою очередь, является форком Space Station 14.

**Space Station 14** - это ремейк SS13, который работает на собственном движке [Robust Toolbox](https://github.com/space-wizards/RobustToolbox), написанном на C#.
Больше про текущую сборку Robust Toolbox, используемую Reserve Station, можно узнать в [Robust Toolbox README](/RobustToolbox/README.md).

## Сборка

Следуйте [гайду от Space Wizards](https://docs.spacestation14.com/en/general-development/setup/setting-up-a-development-environment.html) по настройке рабочей среды, но учитывайте, что наши репозитории отличаются и некоторые вещи могут отличаться.
Мы предлагаем несколько скриптов, показанных ниже, чтобы облегчить работу.

### Необходимые зависимости

> - Git
> - .NET SDK 10.0.100

### Windows

> 1. Склонируйте данный репозиторий
> 2. Запустите `git submodule update --init --recursive` в командной строке, чтобы скачать движок игры
> 3. Запускайте `Scripts/bat/buildAllDebug.bat` после любых изменений в коде проекта
> 4. Запустите `Scripts/bat/runQuickAll.bat`, чтобы запустить клиент и сервер
> 5. Подключитесь к локальному серверу и играйте

### Linux

> 1. Склонируйте данный репозиторий.
> 2. Запустите `git submodule update --init --recursive` в командной строке, чтобы скачать движок игры
> 3. Запускайте `Scripts/sh/buildAllDebug.sh` после любых изменений в коде проекта
> 4. Запустите `Scripts/sh/runQuickAll.sh`, чтобы запустить клиент и сервер
> 5. Подключитесь к локальному серверу и играйте

### MacOS

> Предположительно, также, как и на Линуксе.

### [Полезные скрипты](Scripts/README.md)

Скрипты для сборки и запуска игры можно найти тут: [`Scripts/README.md`](Scripts/README.md)

## Внесение вклада

> [!IMPORTANT]
> Подробную информацию о том, как внести вклад в проект, можно найти в [CONTRIBUTING.md](CONTRIBUTING.md).

> [!CAUTION]
> Сообщайте нам о любых проблемах безопасности через [SECURITY.md](SECURITY.md).

## Лицензия

Содержимое, добавленное в этот репозиторий после коммита [8270907bdc509a3fb5ecfecde8cc14e5845ede36](https://github.com/Goob-Station/Goob-Station/commit/8270907bdc509a3fb5ecfecde8cc14e5845ede36), распространяется по лицензии GNU Affero General Public License версии 3.0, если не указано иное. См. LICENSE-AGPLv3.txt. Содержимое, внесённое в этот репозиторий до коммита [8270907bdc509a3fb5ecfecde8cc14e5845ede36](https://github.com/Goob-Station/Goob-Station/commit/8270907bdc509a3fb5ecfecde8cc14e5845ede36), лицензируется по лицензии MIT, если не указано иное. См. LICENSE.txt.

Большинство ассетов лицензировано под [CC-BY-SA 3.0](https://creativecommons.org/licenses/by-sa/3.0/), если не указано иное. Лицензия и авторские права на ассеты указаны в файле метаданных. [Пример](https://github.com/space-wizards/space-station-14/blob/master/Resources/Textures/Objects/Tools/crowbar.rsi/meta.json).

Обратите внимание, что некоторые ассеты лицензированы под некоммерческой [CC-BY-NC-SA 3.0](https://creativecommons.org/licenses/by-nc-sa/3.0/) или аналогичной некоммерческой лицензией и должны быть удалены, если вы хотите использовать этот проект в коммерческих целях.
