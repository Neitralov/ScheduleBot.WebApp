using System.ComponentModel.DataAnnotations;

namespace ScheduleBot.WebApp.Models;

public class StartBotViewModel
{
    [Required(ErrorMessage = "Ошибка - поле не может быть пустым")]
    public string? BotTokenApi { get; init; }
    
    [Required(ErrorMessage = "Ошибка - поле не может быть пустым")]
    public string? AdminId { get; init; }
    
    [Required(ErrorMessage = "Ошибка - поле не может быть пустым")]
    public string? CloudConvertTokenApi { get; init; }
    
    [Required(ErrorMessage = "Ошибка - поле не может быть пустым")]
    public string? TimeBetweenChecks { get; init; }
    
    [Required(ErrorMessage = "Ошибка - поле не может быть пустым")]
    public string? CheckTimeStart { get; init; }
    
    [Required(ErrorMessage = "Ошибка - поле не может быть пустым")]
    public string? CheckTimeEnd { get; init; }
    
    [Required(ErrorMessage = "Ошибка - поле не может быть пустым")]
    public string? FirstCorpsSchedulePath { get; init; }
    
    [Required(ErrorMessage = "Ошибка - поле не может быть пустым")]
    public string? SecondCorpsSchedulePath { get; init; }
    
    [Required(ErrorMessage = "Ошибка - поле не может быть пустым")]
    public string? ThirdCorpsSchedulePath { get; init; }
    
    [Required(ErrorMessage = "Ошибка - поле не может быть пустым")]
    public string? FourthCorpsSchedulePath { get; init; }
}