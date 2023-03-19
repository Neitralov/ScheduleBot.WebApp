namespace ScheduleBot.WebApp.Pages;

[Authorize]
[IgnoreAntiforgeryToken]
public class StartBotModel : PageModel
{
    [BindProperty]
    [Required(ErrorMessage = "Ошибка - поле не может быть пустым")]
    public string? BotTokenApi { get; set; }
    
    [BindProperty]
    [Required(ErrorMessage = "Ошибка - поле не может быть пустым")]
    public string? AdminId { get; set; }

    [BindProperty]
    [Required(ErrorMessage = "Ошибка - поле не может быть пустым")]
    public string? TimeBetweenChecks { get; set; }
    
    [BindProperty]
    [Required(ErrorMessage = "Ошибка - поле не может быть пустым")]
    public string? CheckTimeStart { get; set; }
    
    [BindProperty]
    [Required(ErrorMessage = "Ошибка - поле не может быть пустым")]
    public string? CheckTimeEnd { get; set; }
    
    [BindProperty]
    [Required(ErrorMessage = "Ошибка - поле не может быть пустым")]
    public string? FirstCorpsSchedulePath { get; set; }
    
    [BindProperty]
    [Required(ErrorMessage = "Ошибка - поле не может быть пустым")]
    public string? SecondCorpsSchedulePath { get; set; }
    
    [BindProperty]
    [Required(ErrorMessage = "Ошибка - поле не может быть пустым")]
    public string? ThirdCorpsSchedulePath { get; set; }
    
    [BindProperty]
    [Required(ErrorMessage = "Ошибка - поле не может быть пустым")]
    public string? FourthCorpsSchedulePath { get; set; }
    
    public IActionResult OnGet()
    {
        CheckingIfBotIsRunning();

        BotTokenApi = Environment.GetEnvironmentVariable("BotTokenApi");
        AdminId = Environment.GetEnvironmentVariable("AdminId");
        TimeBetweenChecks = Environment.GetEnvironmentVariable("TimeBetweenChecks");
        CheckTimeStart = Environment.GetEnvironmentVariable("CheckTimeStart");
        CheckTimeEnd = Environment.GetEnvironmentVariable("CheckTimeEnd");
        FirstCorpsSchedulePath = Environment.GetEnvironmentVariable("FirstCorpsSchedulePath");
        SecondCorpsSchedulePath = Environment.GetEnvironmentVariable("SecondCorpsSchedulePath");
        ThirdCorpsSchedulePath = Environment.GetEnvironmentVariable("ThirdCorpsSchedulePath");
        FourthCorpsSchedulePath = Environment.GetEnvironmentVariable("FourthCorpsSchedulePath");
        
        return Page();
    }
    
    public IActionResult OnPost()
    {
        if(Bot.GetInstance()?.IsRunning is true)
            return Page();
        
        if (!int.TryParse(CheckTimeStart, out var checkTimeStart))
            ModelState.AddModelError("CheckTimeStart", "Время начала проверки указано некорректно");
        
        if (!int.TryParse(CheckTimeEnd, out var checkTimeEnd))
            ModelState.AddModelError("CheckTimeEnd", "Время конца проверки указано некорректно");
        
        if (!int.TryParse(TimeBetweenChecks, out var timeBetweenChecksInSeconds))
            ModelState.AddModelError("TimeBetweenChecks", "Время между проверками указано некорректно");
        
        if (!long.TryParse(AdminId, out var adminId))
            ModelState.AddModelError("AdminId", "ID админа указано некорректно");
        
        CheckingIfBotIsRunning();
        if (!ModelState.IsValid)
            return Page();
        
        Environment.SetEnvironmentVariable("BotTokenApi", BotTokenApi);
        Environment.SetEnvironmentVariable("AdminId", AdminId);
        Environment.SetEnvironmentVariable("TimeBetweenChecks", TimeBetweenChecks);
        Environment.SetEnvironmentVariable("CheckTimeStart", CheckTimeStart);
        Environment.SetEnvironmentVariable("CheckTimeEnd", CheckTimeEnd);
        
        Environment.SetEnvironmentVariable("FirstCorpsSchedulePath", FirstCorpsSchedulePath);
        Environment.SetEnvironmentVariable("SecondCorpsSchedulePath", SecondCorpsSchedulePath);
        Environment.SetEnvironmentVariable("ThirdCorpsSchedulePath", ThirdCorpsSchedulePath);
        Environment.SetEnvironmentVariable("FourthCorpsSchedulePath", FourthCorpsSchedulePath);
        
        Bot.CreateInstance(BotTokenApi!);

        var timeBetweenChecksInMilliseconds = timeBetweenChecksInSeconds * 1000;
        
        Bot.GetInstance()?.Run(checkTimeStart, checkTimeEnd, timeBetweenChecksInMilliseconds, adminId);
        
        CheckingIfBotIsRunning();
        return RedirectToPage("StartBot");
    }
    
    private void CheckingIfBotIsRunning()
    {
        var bot = Bot.GetInstance();
        if (bot is null || bot.IsRunning == false)
            ViewData["IsRunning"] = false;
        else
            ViewData["IsRunning"] = true;
    }
    
    public IActionResult OnGetShutdown()
    {
        Bot.GetInstance()?.Kill();
        CheckingIfBotIsRunning();
        LogInfo("Бот выключен.");
        return RedirectToPage("Login");
    }
}