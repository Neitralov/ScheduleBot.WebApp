namespace ScheduleBot;

public class AdminTools
{
    private readonly TelegramBotClient _botClient;
    private readonly long _adminId;

    public AdminTools(TelegramBotClient botClient, long adminId)
    {
        _botClient = botClient;
        _adminId = adminId;
    }
    
    public async Task GetNumberOfBotSubscribersAsync(long chatId)
    {
        if (await HasAdminAccess(chatId))
        {
            await using var db = new DataBaseProvider();

            var subscribers = db.Subscribers.ToArray();
            
            var totalSubscribers = subscribers.Length;

            var chatSubscribersInCorps = new int[4];
            var groupSubscribersInCorps = new int[4];
            var subscribersInCorps = new int[4];
            
            for (var index = 0; index < 4; index++)
            {
                chatSubscribersInCorps[index] = 
                    subscribers.Count(x => (x.TelegramId >= 0) && (x.Corps == index + 1));
                groupSubscribersInCorps[index] = 
                    subscribers.Count(x => (x.TelegramId < 0) && (x.Corps == index + 1));
                
                subscribersInCorps[index] = chatSubscribersInCorps[index] + groupSubscribersInCorps[index];
            }

            await _botClient.SendTextMessageAsync(
                chatId: chatId,
                text: $"Количество подписчиков по корпусам:\n\n" +
                      $"[{subscribersInCorps[0]}] - Первый корпус.\n" +
                      $"Из них: [{chatSubscribersInCorps[0]}/{groupSubscribersInCorps[0]}] - Чатов/Групп.\n" +
                      $"[{subscribersInCorps[1]}] - Второй корпус.\n" +
                      $"Из них: [{chatSubscribersInCorps[1]}/{groupSubscribersInCorps[1]}] - Чатов/Групп.\n" +
                      $"[{subscribersInCorps[2]}] - Третий корпус.\n" +
                      $"Из них: [{chatSubscribersInCorps[2]}/{groupSubscribersInCorps[2]}] - Чатов/Групп.\n" +
                      $"[{subscribersInCorps[3]}] - Четвертый корпус.\n" +
                      $"Из них: [{chatSubscribersInCorps[3]}/{groupSubscribersInCorps[3]}] - Чатов/Групп.\n\n" +
                      $"[{totalSubscribers}] - Всего подписчиков.");
        }
        else
        {
            const string feedbackMessage = "У вас недостаточно прав для выполнения этой команды";
            await _botClient.SendTextMessageAsync(chatId, feedbackMessage);
        }
    }

    public async Task GetLogsArchiveAsync(long chatId)
    {
        if (await HasAdminAccess(chatId))
        {
            var sourceDirectoryName = Environment.CurrentDirectory + "/Logs";
            var destinationArchiveFileName = Environment.CurrentDirectory + "/logs.zip";
            ZipFile.CreateFromDirectory(sourceDirectoryName, destinationArchiveFileName); 
            
            await using var stream = File.OpenRead(destinationArchiveFileName);
            var inputOnlineFile = new InputOnlineFile(stream, "Logs.zip");
            await _botClient.SendDocumentAsync(chatId, inputOnlineFile);
            
            File.Delete(destinationArchiveFileName);
        }
        else
        {
            const string feedbackMessage = "У вас недостаточно прав для выполнения этой команды";
            await _botClient.SendTextMessageAsync(chatId, feedbackMessage);
        }
    }

    private Task<bool> HasAdminAccess(long chatId)
    {
        return Task.FromResult(_adminId == chatId);
    }
}