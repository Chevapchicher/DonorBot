using Telegram.Bot;
using Telegram.Bot.Types;

namespace BaseBot;
public partial class TelegramBot
{
    private async Task ProcessTextMessage(Message mes, string message)
    {

    }

    private async Task ProcessCallbackQuery(CallbackQuery callbackQuery)
    {

    }

    public async Task SendTextMessage(string text, bool disableNotification = true)
    {
        foreach (var subscriber in _config.Subscribers)
        {
            try
            {
                await _tgBotClient.SendTextMessageAsync(subscriber, text, disableNotification: disableNotification);
            }
            catch { }
        }
    }
}
