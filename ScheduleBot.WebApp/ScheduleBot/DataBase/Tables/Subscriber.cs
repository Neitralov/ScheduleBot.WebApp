namespace ScheduleBot.DataBase;

public class Subscriber
{
    public int Id { get; set; }
    public long TelegramId { get; init; }
    public int Corps { get; init; }

    public override string ToString() => $"{TelegramId} - {Corps}";
}