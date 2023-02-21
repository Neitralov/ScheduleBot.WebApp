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
        var logsDirectory = new DirectoryInfo(Environment.CurrentDirectory + "/Logs");
        
        var logFiles = logsDirectory.GetFiles();
       
        var sortedLogFiles = logFiles.OrderBy(file => file.CreationTime).ToList();
        
        var viewModels = new List<LogsViewModel>();

        foreach (var logFile in sortedLogFiles)
        {
            var text = ReadAllLines(logFile.FullName).Reverse();  
            
            foreach (var line in text)
            {
                var log = line.Split('|');
            
                if (log.Length != 3)
                    continue;
            
                var viewModel = new LogsViewModel()
                {
                    DateTime = log[0],
                    Status = log[1],
                    Message = log[2]
                };
            
                viewModels.Add(viewModel);
            }
        }
        
        return View(viewModels);
    }
}