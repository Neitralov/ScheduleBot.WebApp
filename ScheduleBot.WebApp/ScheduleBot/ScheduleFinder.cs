namespace ScheduleBot;

public class ScheduleFinder
{
    private readonly Notifier _notifier;
    private readonly CancellationTokenSource _cts;
    
    private static string GetOldTablePath(Corps corps) => Environment.CurrentDirectory + $"/Data/Schedule{(int)corps}.xlsx";
    private static string GetNewTablePath(Corps corps) => Environment.CurrentDirectory + $"/Data/Schedule{(int)corps}(new).xlsx";

    public ScheduleFinder(Notifier notifier, CancellationTokenSource cts)
    {
        _notifier = notifier;
        _cts = cts;
    }
    
    public async Task ScheduleSearchAsync(HoursRange searchTime, int timeBetweenChecksInMilliseconds)
    {
        while (true)
        {
            if (_cts.IsCancellationRequested)
                return;
            
            if (searchTime == DateTime.Now.Hour)
                await CheckScheduleAvailabilityForAllCorpsAsync();

            await Task.Delay(timeBetweenChecksInMilliseconds);
        }
    }
    
    private async Task CheckScheduleAvailabilityForAllCorpsAsync()
    {
        var tasks = new[]
        {
            ScheduleSearchAsync(Corps.First),
            ScheduleSearchAsync(Corps.Second),
            ScheduleSearchAsync(Corps.Third),
            ScheduleSearchAsync(Corps.Fourth)
        };

        await Task.WhenAll(tasks);
    }
    
    private async Task ScheduleSearchAsync(Corps corps)
    {
        if (!await TryLoadScheduleAsync(corps))
            return;
        
        if (await IsNewScheduleAsync(corps))
        {
            LogInfo($"Обновлено расписание корпуса №{(int)corps}. Оповещаю подписчиков.");
            await GetSchedulePictureAsync(corps);
            await _notifier.NotifySubscribersAsync(corps);
        }
    }
    
    public static async Task CheckForCachedScheduleForAllCorpsAsync()
    {
        var tasks = new[]
        {
            CheckForCachedScheduleAsync(Corps.First),
            CheckForCachedScheduleAsync(Corps.Second),
            CheckForCachedScheduleAsync(Corps.Third),
            CheckForCachedScheduleAsync(Corps.Fourth)
        };
        
        await Task.WhenAll(tasks);
    }
    
    private static async Task CheckForCachedScheduleAsync(Corps corps)
    {
        if (Exists(GetOldTablePath(corps)) == false)
        {
            if (!await TryLoadScheduleAsync(corps))
            {
                Bot.GetInstance()?.Kill();
                return;
            }
            
            Move(GetNewTablePath(corps), GetOldTablePath(corps));
            await GetSchedulePictureAsync(corps);
        }
    }
    
    private static async Task<bool> TryLoadScheduleAsync(Corps corps)
    {
        Directory.CreateDirectory(Environment.CurrentDirectory + "/Data");
        
        var clientHandler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (_, _, _, _) => true
        };
       
        using var httpClient = new HttpClient(clientHandler);

        try
        {
            var schedulePath = corps switch
            {
                Corps.First => Environment.GetEnvironmentVariable("FirstCorpsSchedulePath"),
                Corps.Second => Environment.GetEnvironmentVariable("SecondCorpsSchedulePath"),
                Corps.Third => Environment.GetEnvironmentVariable("ThirdCorpsSchedulePath"),
                Corps.Fourth => Environment.GetEnvironmentVariable("FourthCorpsSchedulePath"),
                _ => throw new Exception("Такого корпуса не существует")
            };
            
            await WriteAllBytesAsync(GetNewTablePath(corps),
                await httpClient.GetByteArrayAsync(schedulePath));
        }
        catch (Exception e)
        {
            LogError($"Не удается скачать расписание с сайта для корпуса №{(int)corps}. {e.Message}.");
            return false;
        }

        return true;
    }
    
    private static Task<bool> IsNewScheduleAsync(Corps corps)
    {
        var newTable = new FileInfo(GetNewTablePath(corps));
        var oldTable = new FileInfo(GetOldTablePath(corps));

        if (newTable.Length != oldTable.Length)
        {
            Move(GetNewTablePath(corps), GetOldTablePath(corps), true);
            return Task.FromResult(true);
        }

        Delete(GetNewTablePath(corps));
        return Task.FromResult(false);
    }
    
    private static async Task GetSchedulePictureAsync(Corps corps)
    {
        using var process = new Process
        {
            StartInfo =
            {
                FileName = "/bin/bash",
                Arguments =
                    $"-c \"cd /app/Data ; libreoffice --convert-to jpg {GetOldTablePath(corps)} --headless\"",
                RedirectStandardInput = true,
                RedirectStandardOutput = true
            }
        };

        process.Start();
        await process.WaitForExitAsync(); 
    }
}