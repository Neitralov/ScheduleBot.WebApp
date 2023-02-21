namespace ScheduleBot.WebApp.Models;

public class StatisticsViewModel
{
    public int[]? SubscribersInCorps { get; init; }
    public int[]? ChatSubscribersInCorps { get; init; }
    public int[]? GroupSubscribersInCorps { get; init; }
    public int TotalSubscribers { get; init; }
}