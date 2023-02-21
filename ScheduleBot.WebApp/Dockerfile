FROM mcr.microsoft.com/dotnet/aspnet:7.0 
WORKDIR /app
EXPOSE 80
EXPOSE 443
COPY ./bin/Release/net7.0/publish .
ENTRYPOINT ["dotnet", "ScheduleBot.WebApp.dll"]
