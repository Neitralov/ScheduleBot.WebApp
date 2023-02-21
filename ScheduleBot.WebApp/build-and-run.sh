#!/bin/bash

dotnet publish -c Release 
cd ScheduleBot.WebApp || return 
podman stop test
podman build . -t webapptest

podman run \
-d \
--rm \
-p 8000:80 \
-v ./tmp/Data:/app/Data:Z \
-v ./tmp/Logs:/app/Logs:Z \
-v ./tmp/Database:/app/Database:Z \
--name test \
--env TZ=Asia/Barnaul \
--env PASSWORD=123 \
webapptest