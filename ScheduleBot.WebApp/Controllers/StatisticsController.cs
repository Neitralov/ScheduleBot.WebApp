using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScheduleBot.WebApp.Models;

namespace ScheduleBot.WebApp.Controllers;

public class StatisticsController : Controller
{
    [HttpGet]
    [Authorize]
    [Route("/statistics")]
    public IActionResult Statistics(DataBaseProvider db)
    {
        var subscribers = db.Subscribers.ToArray();
        
        var totalSubscribers = subscribers.Length;

        var chatSubscribersInCorps = new int[4];
        var groupSubscribersInCorps = new int[4];
        var subscribersInCorps = new int[4];
            
        for (var index = 0; index < 4; index++)
        {
            chatSubscribersInCorps[index] = 
                subscribers.Count(x => (x.TelegramId >= 0) && (x.Corps == index + 1));
            groupSubscribersInCorps[index] = 
                subscribers.Count(x => (x.TelegramId < 0) && (x.Corps == index + 1));
                
            subscribersInCorps[index] = chatSubscribersInCorps[index] + groupSubscribersInCorps[index];
        }

        var viewModel = new StatisticsViewModel()
        {
            SubscribersInCorps = subscribersInCorps,
            ChatSubscribersInCorps = chatSubscribersInCorps,
            GroupSubscribersInCorps = groupSubscribersInCorps,
            TotalSubscribers = totalSubscribers
        };
        
        return View(viewModel);
    }
}