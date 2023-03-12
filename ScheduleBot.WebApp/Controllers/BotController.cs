using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ScheduleBot.WebApp.Models;

namespace ScheduleBot.WebApp.Controllers;

public class BotController : Controller
{
    [HttpGet]
    [Authorize]
    [Route("/")]
    [Route("/bot")]
    public IActionResult StartBot()
    {
        CheckingIfBotIsRunning();

        var viewModel = new StartBotViewModel
        {
            BotTokenApi = Environment.GetEnvironmentVariable("BotTokenApi"),
            AdminId = Environment.GetEnvironmentVariable("AdminId"),
            TimeBetweenChecks = Environment.GetEnvironmentVariable("TimeBetweenChecks"),
            CheckTimeStart = Environment.GetEnvironmentVariable("CheckTimeStart"),
            CheckTimeEnd = Environment.GetEnvironmentVariable("CheckTimeEnd"),
            FirstCorpsSchedulePath = Environment.GetEnvironmentVariable("FirstCorpsSchedulePath"),
            SecondCorpsSchedulePath = Environment.GetEnvironmentVariable("SecondCorpsSchedulePath"),
            ThirdCorpsSchedulePath = Environment.GetEnvironmentVariable("ThirdCorpsSchedulePath"),
            FourthCorpsSchedulePath = Environment.GetEnvironmentVariable("FourthCorpsSchedulePath")
        };
        
        return View(viewModel);
    }

    [HttpPost]
    [Authorize]
    [Route("/run")]
    public IActionResult StartBot(StartBotViewModel viewModel)
    {
        if(Bot.GetInstance()?.IsRunning is true)
            return View(viewModel);
        
        if (!int.TryParse(viewModel.CheckTimeStart, out var checkTimeStart))
            ModelState.AddModelError("CheckTimeStart", "Время начала проверки указано некорректно");
        
        if (!int.TryParse(viewModel.CheckTimeEnd, out var checkTimeEnd))
            ModelState.AddModelError("CheckTimeEnd", "Время конца проверки указано некорректно");
        
        if (!int.TryParse(viewModel.TimeBetweenChecks, out var timeBetweenChecksInSeconds))
            ModelState.AddModelError("TimeBetweenChecks", "Время между проверками указано некорректно");
        
        if (!long.TryParse(viewModel.AdminId, out var adminId))
            ModelState.AddModelError("AdminId", "ID админа указано некорректно");
        
        CheckingIfBotIsRunning();
        if (!ModelState.IsValid)
            return View(viewModel);
        
        Environment.SetEnvironmentVariable("BotTokenApi", viewModel.BotTokenApi);
        Environment.SetEnvironmentVariable("AdminId", viewModel.AdminId);
        Environment.SetEnvironmentVariable("TimeBetweenChecks", viewModel.TimeBetweenChecks);
        Environment.SetEnvironmentVariable("CheckTimeStart", viewModel.CheckTimeStart);
        Environment.SetEnvironmentVariable("CheckTimeEnd", viewModel.CheckTimeEnd);
        
        Environment.SetEnvironmentVariable("FirstCorpsSchedulePath", viewModel.FirstCorpsSchedulePath);
        Environment.SetEnvironmentVariable("SecondCorpsSchedulePath", viewModel.SecondCorpsSchedulePath);
        Environment.SetEnvironmentVariable("ThirdCorpsSchedulePath", viewModel.ThirdCorpsSchedulePath);
        Environment.SetEnvironmentVariable("FourthCorpsSchedulePath", viewModel.FourthCorpsSchedulePath);
        
        Bot.CreateInstance(viewModel.BotTokenApi!);

        var timeBetweenChecksInMilliseconds = timeBetweenChecksInSeconds * 1000;
        
        Bot.GetInstance()?.Run(checkTimeStart, checkTimeEnd, timeBetweenChecksInMilliseconds, adminId);
        
        CheckingIfBotIsRunning();
        return Redirect("/");
    }

    private void CheckingIfBotIsRunning()
    {
        var bot = Bot.GetInstance();
        if (bot is null || bot.IsRunning == false)
            ViewBag.IsRunning = false;
        else
            ViewBag.IsRunning = true;
    }

    [HttpGet]
    [Authorize]
    [Route("/shutdown")]
    public IActionResult ShutdownBot()
    {
        Bot.GetInstance()?.Kill();
        CheckingIfBotIsRunning();
        LogInfo("Бот выключен.");
        return Redirect("/bot");
    }
}