namespace ScheduleBot;

public class BotHandler
{
    private readonly TelegramBotClient _botClient;
    private readonly AdminTools _adminTools;
    private readonly Notifier _notifier;
    private readonly long _adminId;
    private readonly CancellationTokenSource _cts;
    
    public BotHandler(TelegramBotClient botClient, AdminTools adminTools, Notifier notifier, long adminId, CancellationTokenSource cts)
    {
        _botClient = botClient;
        _adminTools = adminTools;
        _notifier = notifier;
        _adminId = adminId;
        _cts = cts;
    }
    
    public async Task BotProcessingAsync()
    {
        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = Array.Empty<UpdateType>()
        };

        _botClient.StartReceiving(HandleUpdateAsync, HandlePollingErrorAsync, receiverOptions, _cts.Token);

        var me = await _botClient.GetMeAsync(_cts.Token);

        LogInfo($"Начато прослушивание бота @{me.Username}");
        Console.ReadLine();
    }

    private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update,
        CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
            return;
        
        if (update.Message is not { Text: { } messageText } message)
            return;

        var chatId = message.Chat.Id;
        
        LogInfo(chatId == _adminId
            ? $"Получено сообщение '{messageText}' из чата ADMIN."
            : $"Получено сообщение '{messageText}' из чата {chatId}.");

        var command = messageText.Split('@')[0];
        await ProcessCommandAsync(command, chatId);
    }

    private async Task ProcessCommandAsync(string command, long chatId)
    {
        const string startMessage = "Это бот, который отправляет расписание БГК.\n\n" +
                                    "Получите изображение с актуальным расписанием!\n" +
                                    "/get1 | Первый корпус.\n" +
                                    "/get2 | Второй корпус.\n" +
                                    "/get3 | Третий корпус.\n" +
                                    "/get4 | Четвертый корпус.\n\n" +
                                    "Подпишитесь на рассылку новых расписаний!\n" +
                                    "/subscribe1 | Первый корпус.\n" +
                                    "/subscribe2 | Второй корпус.\n" +
                                    "/subscribe3 | Третий корпус.\n" +
                                    "/subscribe4 | Четвертый корпус.\n\n" +
                                    "/unsubscribe | Отписаться от всех подписок.";

        const string unknownCommandMessage =
            "Такой команды не существует. Выполните /start, чтобы получить список доступных команд.";

        var task = command switch
        {
            "/start"       => _botClient.SendTextMessageAsync(chatId, startMessage),
            "/get1"        => _notifier.SendSchedulePictureAsync(chatId, Corps.First),
            "/get2"        => _notifier.SendSchedulePictureAsync(chatId, Corps.Second),
            "/get3"        => _notifier.SendSchedulePictureAsync(chatId, Corps.Third),
            "/get4"        => _notifier.SendSchedulePictureAsync(chatId, Corps.Fourth),
            "/subscribe1"  => _notifier.SubscribeToScheduleNewsletter(chatId, Corps.First),
            "/subscribe2"  => _notifier.SubscribeToScheduleNewsletter(chatId, Corps.Second),
            "/subscribe3"  => _notifier.SubscribeToScheduleNewsletter(chatId, Corps.Third),
            "/subscribe4"  => _notifier.SubscribeToScheduleNewsletter(chatId, Corps.Fourth),
            "/unsubscribe" => _notifier.UnsubscribeToScheduleNewsletter(chatId),
            "/statistics"  => _adminTools.GetNumberOfBotSubscribersAsync(chatId),
            "/logs"        => _adminTools.GetLogsArchiveAsync(chatId),
            _              => _botClient.SendTextMessageAsync(chatId, unknownCommandMessage)
        };

        await task;    
    }

    private static Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception,
        CancellationToken cancellationToken)
    {
        var errorMessage = exception switch
        {
            ApiRequestException apiRequestException
                => $"Telegram API Error: [{apiRequestException.ErrorCode}] {apiRequestException.Message}",
            _ => exception.ToString()
        };

        var requestException = exception as ApiRequestException;

        Action? action = requestException?.ErrorCode switch
        {
            401 => () => Bot.GetInstance()?.Kill(),
            409 => () => Bot.GetInstance()?.Kill(),
            _ => null
        };
        action?.Invoke();
        
        LogError(errorMessage);
        return Task.CompletedTask;
    }
}