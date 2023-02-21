using LogLevel = NLog.LogLevel;

namespace ScheduleBot.WebApp;

public static class NLogLogger
{
    private static readonly Logger Logger;

    static NLogLogger()
    {
        Logger = LogManager.GetCurrentClassLogger();
        InitNLog();
    }

    private static void InitNLog()
    {
        LogManager.Setup().LoadConfiguration(builder =>
        {
            builder.ForLogger().FilterMinLevel(LogLevel.Info).WriteToConsole();
            builder.ForLogger().FilterMinLevel(LogLevel.Debug).WriteToFile(
                fileName: $"Logs/log.txt",
                layout: "${date} | ${level:uppercase=true} | ${message}");
        });
    }
    
    public static void LogInfo(string message) => Logger.Info(message);
    public static void LogError(string message) => Logger.Error(message);
}