using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;

namespace BaseBot;

public partial class TelegramBot
{
    private readonly AppConfig _config;
    private readonly TelegramBotClient _tgBotClient;
    private readonly LogService _logService;

    public TelegramBot(IOptions<AppConfig> config,
        LogService logService)
    {
        _config = config.Value;
        _logService = logService;
        _tgBotClient = new TelegramBotClient(config.Value.BotToken);
    }

    public void Start()
    {
        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = { } // receive all update types
        };

        _tgBotClient.StartReceiving(HandleUpdateAsync, HandleErrorAsync, receiverOptions);
    }

    private static Task HandleErrorAsync(ITelegramBotClient client, Exception ex, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        var newThread = new Thread(async () =>
        {
            try
            {
                if (update.Message is { Text: { } message } mes)
                {
                    await ProcessTextMessage(mes, message);
                }

                if (update.CallbackQuery is { } callbackQuery)
                {
                    await ProcessCallbackQuery(callbackQuery);
                }
            }
            catch (Exception ex)
            {
                _logService.AddLog(ex.ToString());
                await _tgBotClient.SendTextMessageAsync(update.Message.Chat.Id, "Произошла ошибка!", cancellationToken: cancellationToken);
            }
        });

        newThread.Start();
        return Task.CompletedTask;
    }
}