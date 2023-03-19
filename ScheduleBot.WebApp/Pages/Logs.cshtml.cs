namespace ScheduleBot.WebApp.Pages;

[Authorize]
[IgnoreAntiforgeryToken]
public class LogsModel : PageModel
{
    [BindProperty] 
    public List<Log> Logs { get; set; } = new(); 

    public IActionResult OnGet()
    {
        if (Exists(Environment.CurrentDirectory + "/Logs/log.txt") is false)
            return Page();
        
        var logFile = new FileInfo(Environment.CurrentDirectory + "/Logs/log.txt");
        var lines = ReadAllLines(logFile.FullName).Reverse();
       
        foreach (var line in lines)
        {
            var logParts = line.Split('|');
            
            if (logParts.Length != 3)
                continue;
            
            var log = new Log
            {
                DateTime = logParts[0],
                Status = logParts[1],
                Message = logParts[2]
            };
            
            Logs.Add(log);
        }
        
        return Page();
    }    
}