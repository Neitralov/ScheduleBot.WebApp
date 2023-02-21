using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScheduleBot.WebApp.Models;

namespace ScheduleBot.WebApp.Controllers;

public class AuthorizationController : Controller
{
    [HttpGet]
    [Route("/login")]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    [Route("/login")]
    public async Task<IActionResult> Login(AuthorizationViewModel viewModel)
    {
        if (!ModelState.IsValid)
            return View();

        var password = Environment.GetEnvironmentVariable("PASSWORD");
        
        if (viewModel.Password != password)
        {
            ModelState.AddModelError("Password", "Неправильный пароль");
            return View();    
        }
        
        var claims = new List<Claim> { new(ClaimTypes.Name, viewModel.Password!) };

        // создаем объект ClaimsIdentity
        var claimsIdentity = new ClaimsIdentity(claims, "Cookies");

        // установка аутентификационных куки
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity));

        return Redirect("/");
    }

    [HttpGet]
    [Authorize]
    [Route("/logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login");
    }
}