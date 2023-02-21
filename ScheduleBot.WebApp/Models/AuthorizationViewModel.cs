using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using ScheduleBot.WebApp.Controllers;

namespace ScheduleBot.WebApp.Models;

public class AuthorizationViewModel
{
    [Required(ErrorMessage = "Ошибка - поле с паролем не может быть пустым")]
    public string? Password { get; init; }
}