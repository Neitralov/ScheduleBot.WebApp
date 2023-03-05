namespace ScheduleBot;

public class ScheduleFinder
{
    private readonly Notifier _notifier;
    private readonly CloudConvertAPI _xlsxConverter;
    private readonly CancellationTokenSource _cts;
    
    private static string GetOldTablePath(Corps corps) => Environment.CurrentDirectory + $"/Data/Schedule{(int)corps}.xlsx";
    private static string GetNewTablePath(Corps corps) => Environment.CurrentDirectory + $"/Data/Schedule{(int)corps}(new).xlsx";

    public ScheduleFinder(Notifier notifier, CloudConvertAPI xlsxConverter, CancellationTokenSource cts)
    {
        _notifier = notifier;
        _xlsxConverter = xlsxConverter;
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
    
    public async Task CheckForCachedScheduleForAllCorpsAsync()
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
    
    private async Task CheckForCachedScheduleAsync(Corps corps)
    {
        if (File.Exists(GetOldTablePath(corps)) == false)
        {
            if (!await TryLoadScheduleAsync(corps))
                throw new Exception("Не удалось скачать расписание с сайта.");
            
            File.Move(GetNewTablePath(corps), GetOldTablePath(corps));
            await GetSchedulePictureAsync(corps);
        }
    }
    
    private static async Task<bool> TryLoadScheduleAsync(Corps corps)
    {
        Directory.CreateDirectory(Environment.CurrentDirectory + "/Data");
        
        var clientHandler = new HttpClientHandler();
        clientHandler.ServerCertificateCustomValidationCallback = 
            (sender, cert, chain, sslPolicyErrors) => true;
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
            
            await File.WriteAllBytesAsync(GetNewTablePath(corps),
                await httpClient.GetByteArrayAsync(schedulePath));
        }
        catch (Exception e)
        {
            LogError("Не удается скачать расписание с сайта. " + e.Message);
            Bot.GetInstance()?.Kill();
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
            File.Move(GetNewTablePath(corps), GetOldTablePath(corps), true);
            return Task.FromResult(true);
        }

        File.Delete(GetNewTablePath(corps));
        return Task.FromResult(false);
    }
    
    private async Task GetSchedulePictureAsync(Corps corps)
    {
        var schedulePath = corps switch
        {
            Corps.First => Environment.GetEnvironmentVariable("FirstCorpsSchedulePath"),
            Corps.Second => Environment.GetEnvironmentVariable("SecondCorpsSchedulePath"),
            Corps.Third => Environment.GetEnvironmentVariable("ThirdCorpsSchedulePath"),
            Corps.Fourth => Environment.GetEnvironmentVariable("FourthCorpsSchedulePath"),
            _ => throw new Exception("Такого корпуса не существует")
        };
        
        var job = await _xlsxConverter.CreateJobAsync(new JobCreateRequest()
        {
            Tasks = new
            {
                import_it = new ImportUrlCreateRequest()
                {
                    Url = schedulePath
                },
                convert = new ConvertCreateRequest()
                {
                    Input = "import_it",
                    Input_Format = "xlsx",
                    Output_Format = "jpg"
                },
                export_it = new ExportUrlCreateRequest()
                {
                    Input = "convert"
                }
            }
        });

        job = await _xlsxConverter.WaitJobAsync(job.Data.Id);
        var exportTask = job.Data.Tasks.FirstOrDefault(t => t.Name == "export_it");
        var fileExport = exportTask?.Result.Files.FirstOrDefault();
        
        using var httpClient = new HttpClient();

        await File.WriteAllBytesAsync(Bot.GetSchedulePicturePath(corps),
            await httpClient.GetByteArrayAsync(fileExport!.Url));
    }
}