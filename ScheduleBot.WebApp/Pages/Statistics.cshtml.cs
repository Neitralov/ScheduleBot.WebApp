namespace ScheduleBot.WebApp.Pages;

[Authorize]
[IgnoreAntiforgeryToken]
public class StatisticsModel : PageModel
{
    [BindProperty] public int[] SubscribersInCorps { get; set; } = new int[4];
    [BindProperty] public int[] ChatSubscribersInCorps { get; set; } = new int[4];
    [BindProperty] public int[] GroupSubscribersInCorps { get; set; } = new int[4];
    [BindProperty] public int TotalSubscribers { get; set; }

    public IActionResult OnGet(DataBaseProvider db)
    {
        var subscribers = db.Subscribers.ToArray();
        
        TotalSubscribers = subscribers.Length;
        
        for (var index = 0; index < 4; index++)
        {
            ChatSubscribersInCorps[index] = 
                subscribers.Count(x => (x.TelegramId >= 0) && (x.Corps == index + 1));
            GroupSubscribersInCorps[index] = 
                subscribers.Count(x => (x.TelegramId < 0) && (x.Corps == index + 1));
                
            SubscribersInCorps[index] = ChatSubscribersInCorps[index] + GroupSubscribersInCorps[index];
        }
        
        return Page();
    }    
}