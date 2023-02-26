using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScheduleBot.WebApp.Models;
using static System.IO.File;

namespace ScheduleBot.WebApp.Controllers;

public class LogsController : Controller
{
    [HttpGet]
    [Authorize]
    [Route("/logs")]
    public IActionResult Logs()
    {
        var viewModels = new List<LogsViewModel>();
        
        if (Exists(Environment.CurrentDirectory + "/Logs/log.txt") is false)
            return View(viewModels);
        
        var logFile = new FileInfo(Environment.CurrentDirectory + "/Logs/log.txt");
        var logs = ReadAllLines(logFile.FullName).Reverse();
       
        foreach (var log in logs)
        {
            var logParts = log.Split('|');
            
            if (logParts.Length != 3)
                continue;
            
            var viewModel = new LogsViewModel()
            {
                DateTime = logParts[0],
                Status = logParts[1],
                Message = logParts[2]
            };
            
            viewModels.Add(viewModel);
        }
        
        return View(viewModels);
    }
}