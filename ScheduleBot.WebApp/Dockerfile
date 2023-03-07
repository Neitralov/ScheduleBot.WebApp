FROM mcr.microsoft.com/dotnet/aspnet:7.0 
WORKDIR /app
EXPOSE 80
EXPOSE 443
RUN apt-get update
RUN apt-get install -y libreoffice
COPY ./bin/Release/net7.0/publish .
ENTRYPOINT ["dotnet", "ScheduleBot.WebApp.dll"]
