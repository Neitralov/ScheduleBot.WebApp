using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ScheduleBot.WebApp.Controllers;

public class DatabaseController : Controller
{
    [HttpGet]
    [Authorize]
    [Route("/database")]
    public IActionResult Database()
    {
        return View();
    }

    [HttpPost]
    [Authorize]
    [Route("/database/import")]
    public async Task<IActionResult> DatabaseImport(IFormFile? file)
    {
        if (file is null)
            return View("Database");

        var databaseFilesPath = Environment.CurrentDirectory + "/Database";
        var archivePath = Environment.CurrentDirectory + "/" + file.FileName;

        await using (var stream = System.IO.File.Create(archivePath))
        {
            await file.CopyToAsync(stream);   
        }
        
        ZipFile.ExtractToDirectory(
            sourceArchiveFileName: archivePath,
            destinationDirectoryName: databaseFilesPath,
            overwriteFiles: true
        );

        System.IO.File.Delete(archivePath);

        return View("Database");
    }

    [HttpGet]
    [Authorize]
    [Route("/database/export")]
    public IActionResult DatabaseExport()
    {
        var databaseFilesPath = Environment.CurrentDirectory + "/Database";
        var archivePath = Environment.CurrentDirectory + "/wwwroot/backup.zip";

        if (System.IO.File.Exists(archivePath))
            System.IO.File.Delete(archivePath);

        ZipFile.CreateFromDirectory(
            sourceDirectoryName: databaseFilesPath,
            destinationArchiveFileName: archivePath
        );

        return File("backup.zip", "application/zip", "backup.zip");
    }
}