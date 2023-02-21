# ScheduleBot.WebApp
Телеграм бот в веб интерфейсом, который отправляет расписание БГК и следит за его обновлением.  
Опробуйте бота прямо сейчас: https://t.me/BSC_ScheduleBot

## Как собрать и запустить (Сборка для Linux):
__Для сборки бота необходимо иметь Podman и .NET 7 SDK.__
1. Скачиваем исходники.
2. Переходим в папку проекта `cd ScheduleBot.WebApp/ScheduleBot.WebApp/`
3. Создаем папки `mkdir tmp/Data tmp/Database tmp/Logs`
4. Выполняем скрипт `/bin/bash/ ./build-and-run.sh`  

Запускается контейнер с именем `test`. Web интерфейс доступен на порту `8000`. Пароль `123`.


## Как развернуть бота на сервере:
В процессе написания...

## Используемые прямые зависимости
- Microsoft.EntityFrameworkCore 6.0.13
- Microsoft.EntityFrameworkCore.Sqlite 6.0.13
- CloudConvert.API 1.2.0
- NLog 5.1.1
- Telegram.Bot 18.0.0
