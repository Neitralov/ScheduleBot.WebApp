namespace ScheduleBot.WebApp.Pages;

[IgnoreAntiforgeryToken]
public class LoginModel : PageModel
{
    [BindProperty]
    [Required(ErrorMessage = "Ошибка - поле с паролем не может быть пустым")]
    public string? Password { get; set; }

    public IActionResult OnGet()
    {
        if (User.Identity is { IsAuthenticated: true })
            return RedirectToPage("StartBot");

        return Page();
    }

    public async Task<IActionResult> OnPost()
    {
        if (!ModelState.IsValid)
            return Page();

        var password = Environment.GetEnvironmentVariable("PASSWORD");

        if (Password != password)
        {
            ModelState.AddModelError("Password", "Неправильный пароль");
            return Page();
        }

        var claims = new List<Claim> { new(ClaimTypes.Name, Password!) };

        // создаем объект ClaimsIdentity
        var claimsIdentity = new ClaimsIdentity(claims, "Cookies");

        // установка аутентификационных куки
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity));

        LogInfo($"Произведен вход в систему.");

        return RedirectToPage("StartBot");
    }

    public async Task<IActionResult> OnGetLogout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToPage("Login");
    }
}