namespace ScheduleBot.WebApp.Pages;

[Authorize]
[IgnoreAntiforgeryToken]
public class DatabaseModel : PageModel
{
    public IActionResult OnGet()
    {
        return Page();
    }
    
    public async Task<IActionResult> OnPostBackup(IFormFile? file)
    {
        if (file is null)
            return Page();

        var databaseFilesPath = Environment.CurrentDirectory + "/Database";
        var archivePath = Environment.CurrentDirectory + "/" + file.FileName;

        await using (var stream = Create(archivePath))
        {
            await file.CopyToAsync(stream);   
        }
        
        ZipFile.ExtractToDirectory(
            sourceArchiveFileName: archivePath,
            destinationDirectoryName: databaseFilesPath,
            overwriteFiles: true
        );

        Delete(archivePath);

        return Page();
    }
    
    public IActionResult OnGetBackup()
    {
        var databaseFilesPath = Environment.CurrentDirectory + "/Database";
        var archivePath = Environment.CurrentDirectory + "/wwwroot/backup.zip";

        if (Exists(archivePath))
            Delete(archivePath);

        ZipFile.CreateFromDirectory(
            sourceDirectoryName: databaseFilesPath,
            destinationArchiveFileName: archivePath
        );

        return File("backup.zip", "application/zip", "backup.zip");
    }    
}