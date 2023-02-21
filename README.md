# ScheduleBot.WebApp
Телеграм бот в веб интерфейсом, который отправляет расписание БГК и следит за его обновлением.  
Опробуйте бота прямо сейчас: https://t.me/BSC_ScheduleBot

![image](https://user-images.githubusercontent.com/109409226/220381400-ccd9e399-e9f1-4ffb-b2b4-1dcf38232747.png)

## Как собрать и запустить из исходников (Сборка для Linux):
__Для сборки бота необходимо иметь Podman и .NET 7 SDK.__
1. Скачиваем исходники.
2. Переходим в папку проекта `cd ScheduleBot.WebApp/ScheduleBot.WebApp/`
3. Создаем папки `mkdir tmp/Data tmp/Database tmp/Logs`
4. Выполняем скрипт `/bin/bash/ ./build-and-run.sh`  

Запускается контейнер с именем `test`. Web интерфейс доступен на порту `8000`. Пароль `123`.

## Как запустить:
В процессе написания...

## Используемые прямые зависимости
- Microsoft.EntityFrameworkCore 6.0.13
- Microsoft.EntityFrameworkCore.Sqlite 6.0.13
- CloudConvert.API 1.2.0
- NLog 5.1.1
- Telegram.Bot 18.0.0
