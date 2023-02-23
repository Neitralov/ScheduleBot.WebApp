namespace ScheduleBot;

public class Notifier
{
    private readonly TelegramBotClient _botClient;
    private readonly long _adminId;

    public Notifier(TelegramBotClient botClient, long adminId)
    {
        _botClient = botClient;
        _adminId = adminId;
    }
    
    public async Task NotifySubscribersAsync(Corps corps)
    {
        await using var db = new DataBaseProvider();
        var subscribers = db.Subscribers.Where(x => x.Corps == (int)corps).ToArray();

        var tasks = new List<Task>();
        
        foreach (var subscriber in subscribers)
        {
            tasks.Add(SendSchedulePictureAsync(subscriber.TelegramId, corps));
            await Task.Delay(35); // Ограничение телеграма: не более 30 сообщений в секунду.        
        }
        
        await Task.WhenAll(tasks);
        
        LogInfo($"Подписчики корпуса №{(int)corps} были оповещены.");
    }

    public async Task SubscribeToScheduleNewsletterAsync(long chatId, Corps corps)
    {
        await using var db = new DataBaseProvider();
        var isThereSameSubscriber =
            db.Subscribers.FirstOrDefault(x => (x.TelegramId == chatId) && (x.Corps == (int)corps)) != null;

        if (isThereSameSubscriber)
        {
            const string feedbackMessage = "Вы уже подписаны на обновление расписания этого корпуса.";
            await _botClient.SendTextMessageAsync(chatId, feedbackMessage);
        }
        else
        {
            const string feedbackMessage = "Вы подписались на обновление расписания корпуса.";

            var tasks = new[]
            {
                _botClient.SendTextMessageAsync(chatId, feedbackMessage),
                AddSubscriberAsync(chatId, corps)
            };

            await Task.WhenAll(tasks);
        }
    }

    private static async Task AddSubscriberAsync(long chatId, Corps corps)
    {
        await using var db = new DataBaseProvider();
        var subscriber = new Subscriber { TelegramId = chatId, Corps = (int)corps };
        await db.Subscribers.AddAsync(subscriber);
        await db.SaveChangesAsync();
    }

    public async Task UnsubscribeToScheduleNewsletterAsync(long chatId)
    {
        await using var db = new DataBaseProvider();
        var isThereThisSubscriber = db.Subscribers.Any(x => x.TelegramId == chatId);

        if (isThereThisSubscriber)
        {
            const string feedbackMessage = "Вы отписались от получения обновлений всех расписаний.";

            var tasks = new[]
            {
                _botClient.SendTextMessageAsync(chatId, feedbackMessage),
                RemoveSubscriberAsync(chatId)
            };

            await Task.WhenAll(tasks);
        }
        else
        {
            const string feedbackMessage = "Вы не были подписаны на обновление какого-либо расписания.";
            await _botClient.SendTextMessageAsync(chatId, feedbackMessage);
        }
    }

    private async Task RemoveSubscriberAsync(long chatId)
    {
        await using var db = new DataBaseProvider();
        var allSubscriberRecords = db.Subscribers.Where(x => x.TelegramId == chatId).ToArray();
        db.Subscribers.RemoveRange(allSubscriberRecords);
        await db.SaveChangesAsync();
        
        LogInfo(chatId == _adminId
            ? $"Пользователь ADMIN отписан. Было удалено записей: {allSubscriberRecords.Length}."
            : $"Пользователь {chatId} отписан. Было удалено записей: {allSubscriberRecords.Length}.");
    }
    
    public async Task SendSchedulePictureAsync(long chatId, Corps corps)
    {
        await using var stream = File.OpenRead(Bot.GetSchedulePicturePath(corps));
        var inputOnlineFile = new InputOnlineFile(stream, $"Расписание корпуса #{(int)corps}.jpg");

        try
        {
            await _botClient.SendDocumentAsync(chatId, inputOnlineFile);
        }
        catch
        {
            LogInfo(chatId == _adminId
                ? $"Пользователь ADMIN заблокировал бота. Производится удаление."
                : $"Пользователь {chatId} заблокировал бота. Производится удаление.");
            
            await RemoveSubscriberAsync(chatId);
        }
    }
}