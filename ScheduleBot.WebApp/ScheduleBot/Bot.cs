namespace ScheduleBot;

public class Bot
{
    private static Bot? _instance;

    private readonly TelegramBotClient _botClient;
    private readonly CancellationTokenSource _cts;

    public bool IsRunning { get; private set; }

    public static string GetSchedulePicturePath(Corps corps) => Environment.CurrentDirectory + $"/Data/Schedule{(int)corps}.jpg";

    private Bot(string botClientApiToken)
    {
        _botClient = new TelegramBotClient(botClientApiToken);
        _cts = new CancellationTokenSource();
    }

    public static void CreateInstance(string botClientApiToken)
    {
        _instance = new Bot(botClientApiToken);
    }

    public static Bot? GetInstance()
    {
        return _instance;
    }

    public void Kill()
    {
        _cts.Cancel();
        _cts.Dispose();
        IsRunning = false;
        _instance = null;
    }

    public async Task Run(int scheduleCheckTimeStart, int scheduleCheckTimeEnd, int timeBetweenChecks, long adminId)
    {
        if (IsRunning)
            return;
        
        var notifier = new Notifier(_botClient, adminId);
        var scheduleFinder = new ScheduleFinder(notifier, _cts);
        
        await ScheduleFinder.CheckForCachedScheduleForAllCorpsAsync();
        
        var scheduleCheckTimeRange = new HoursRange(scheduleCheckTimeStart, scheduleCheckTimeEnd);
        
        var adminTools = new AdminTools(_botClient, adminId);
        var botHandler = new BotHandler(_botClient, adminTools, notifier, adminId, _cts);
        
        var tasks = new[]
        {
            botHandler.BotProcessingAsync(),
            scheduleFinder.ScheduleSearchAsync(scheduleCheckTimeRange, timeBetweenChecks)
        };

        IsRunning = true;

        await Task.WhenAll(tasks);
    }
}