using Telegram.Bot;
using Telegram.Bot.Types;

namespace DonorBot.Services.TelegramBot;
public partial class TelegramBot
{
    private async Task ProcessTextMessage(Message mes, string message)
    {
        switch (message)
        {
            case "/offnot":
                _notificationService.SetNotification(mes.Chat.Id, false);
                await _tgBotClient.SendTextMessageAsync(mes.Chat.Id, "Оповещения выключены");
                return;

            case "/onnot":
                _notificationService.SetNotification(mes.Chat.Id, true);
                await _tgBotClient.SendTextMessageAsync(mes.Chat.Id, "Оповещения включены");
                return;
        }
    }

    private async Task ProcessCallbackQuery(CallbackQuery callbackQuery)
    {

    }

    public async Task SendTextMessage(string text, bool disableNotification = true)
    {
        foreach (var subscriber in _notificationService.GetIdsForNotifications())
        {
            try
            {
                await _tgBotClient.SendTextMessageAsync(subscriber, text, disableNotification: disableNotification);
            }
            catch { }
        }
    }
}
