<p align="center"> <img alt="Reserve Station 14" width="100%" src="https://i.imgur.com/yagb8UO.png" loading="lazy" /></p>

<p align="center">
  <a href="https://discord.gg/WXZvqzZ2Fc"><img src="https://img.shields.io/badge/Discord-060e11?style=for-the-badge&logo=Discord&logoColor=c23c2d" alt="Discord Community & Administration Contact" loading="lazy" /></a>
  <a href="https://boosty.to/reservestation"><img src="https://img.shields.io/badge/Boosty-060e11?style=for-the-badge&logo=Boosty&logoColor=c23c2d" alt="Boosty Support & Maecenas Content" loading="lazy" /></a>
  <a href="https://ccdn.reserve-station.space/fork/reserve/"><img src="https://img.shields.io/badge/CCDN-060e11?style=for-the-badge&logo=dotnet&logoColor=c23c2d" alt="Download Compiled Builds" loading="lazy" /></a>
  <a href="CONTRIBUTING.md"><img src="https://img.shields.io/badge/Внесение вклада-060e11?style=for-the-badge&logo=github&logoColor=c23c2d" alt="Contribution Guide" loading="lazy" /></a>
  <a href="SECURITY.md"><img src="https://img.shields.io/badge/Безопасность-060e11?style=for-the-badge&logo=letsencrypt&logoColor=c23c2d" alt="Report Security Issues" loading="lazy" /></a>
  <a href="#лицензия"><img src="https://img.shields.io/badge/Лицензия-060e11?style=for-the-badge&logo=spdx&logoColor=c23c2d" alt="Licensing" loading="lazy" /></a>
</p>

<p align="center">
  <img src="https://img.shields.io/badge/dynamic/json?url=http%3A%2F%2Freserve-station.space%3A1212%2Fstatus&query=%24.name&style=for-the-badge&label=%D0%A1%D1%82%D0%B0%D1%82%D1%83%D1%81&labelColor=060e11&color=c23c2d" alt="Server Status" loading="lazy" />
  <img src="https://img.shields.io/badge/dynamic/json?url=http%3A%2F%2Freserve-station.space%3A1212%2Fstatus&query=%24.round_id&style=for-the-badge&label=%D0%A0%D0%B0%D1%83%D0%BD%D0%B4&labelColor=060e11&color=8f0303" alt="Round ID" loading="lazy" />
  <img src="https://img.shields.io/badge/dynamic/json?url=http%3A%2F%2Freserve-station.space%3A1212%2Fstatus&query=%24.map&style=for-the-badge&label=%D0%A1%D1%82%D0%B0%D0%BD%D1%86%D0%B8%D1%8F&labelColor=060e11&color=8f0303" alt="Station Map" loading="lazy" />
  <img src="https://img.shields.io/badge/dynamic/json?url=http%3A%2F%2Freserve-station.space%3A1212%2Fstatus&query=%24.preset&style=for-the-badge&label=%D0%A0%D0%B5%D0%B6%D0%B8%D0%BC&labelColor=060e11&color=8f0303" alt="Gamemode" loading="lazy" />
</p>

---

**Резерв** - это некоммерческий проект с комфортным уровнем РП. Мы нацелены на настоящую РП составляющую игры, которая **не будет** навязываться чересчур строгими правилами, заставляя игроков отыгрывать по стандартному шаблону.

Билд сервера - это сильно модифицированный форк [Goob Station](https://github.com/Goob-Station/Goob-Station), который, в свою очередь, является форком Space Station 14.

**Space Station 14** - это ремейк SS13, который работает на собственном движке [Robust Toolbox](https://github.com/space-wizards/RobustToolbox), написанном на C#.
Больше про текущую сборку Robust Toolbox, используемую Reserve Station, можно узнать в [Robust Toolbox README](https://github.com/red-wing-ss14/redbox?tab=readme-ov-file).

Билд дополнен многочисленными **уникальными механиками**, **контентом** и **лучшими портами**! [**Кровные Братья**](https://github.com/Reserve-Station/Reserve-Station/pull/264), [**Заговорщики**](https://github.com/Reserve-Station/Reserve-Station/pull/157), [**Горящие жидкости**](https://github.com/Reserve-Station/Reserve-Station/pull/345), [**Бармания Орхидеи**](https://github.com/Reserve-Station/Reserve-Station/pull/278), [**Сад Орхидеи**](https://github.com/Reserve-Station/Reserve-Station/pull/285), [**Прокачанный удобный интерфейс**](https://github.com/Reserve-Station/Reserve-Station/pull/303), [**Система меценатов**](https://github.com/Reserve-Station/Reserve-Station/pull/388), [**Расширенная кастомизация**](https://github.com/Reserve-Station/Reserve-Station/pull/327), [**Своя стилистика**](https://github.com/Reserve-Station/Reserve-Station/pull/362), [**Удобный Дискорд-бот**](https://github.com/Reserve-Station/Reserve-Station/pull/369), [**Королевская битва**](https://github.com/Reserve-Station/Reserve-Station/pull/73) и многое, многое другое!

Кроме того, билд полностью переведён на русский язык, и переводы регулярно обновляются с появлением нового контента.

## Сборка

Следуйте [гайду от Space Wizards](https://docs.spacestation14.com/en/general-development/setup/setting-up-a-development-environment.html) по настройке рабочей среды, но учитывайте, что наши репозитории отличаются и некоторые вещи могут работать по-другому.
Примерный процесс сборки указан ниже.  
Кроме того, мы предлагаем [несколько скриптов](#полезные-скрипты), чтобы облегчить работу.

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

## Администрация

- <img src="https://github.com/echotry-ss14.png" width="16" height="16" alt="echotry" style="border-radius:50%;" loading="lazy"/> [**echotry**](https://github.com/echotry-ss14) - создатель проекта и его владелец, а также контент-мейкер.
- <img src="https://github.com/Ceterai.png" width="16" height="16" alt="Ceterai" style="border-radius:50%;" loading="lazy"/> [**Орхидея**](https://github.com/Ceterai) - лидер проекта, хост инфраструктуры, администратор медиаресурсов и главная разработчица.

Связываться по любым вопросам лучше всего через Дискорд, по вопросам подписки или поддержки также можно через Бусти.

## Лицензия

Содержимое, добавленное в этот репозиторий после коммита [8270907bdc509a3fb5ecfecde8cc14e5845ede36](https://github.com/Reserve-Station/Reserve-Station/commit/8270907bdc509a3fb5ecfecde8cc14e5845ede36), распространяется по лицензии GNU Affero General Public License версии 3.0, если не указано иное. См. [AGPL-3.0-or-later.txt](LICENSES/AGPL-3.0-or-later.txt). Содержимое, внесённое в этот репозиторий до этого коммита, лицензируется по лицензии MIT, если не указано иное. См. [MIT.txt](LICENSES/MIT.txt).

Большинство ассетов лицензировано под [CC-BY-SA 3.0](https://creativecommons.org/licenses/by-sa/3.0/), если не указано иное. Лицензия и авторские права на ассеты указаны в файле метаданных. [Пример](Resources/Textures/Objects/Tools/crowbar.rsi/meta.json).

Обратите внимание, что некоторые ассеты лицензированы под некоммерческой [CC-BY-NC-SA 3.0](https://creativecommons.org/licenses/by-nc-sa/3.0/) или аналогичной некоммерческой лицензией и должны быть удалены, если вы хотите использовать этот проект в коммерческих целях.
