using AngleSharp;
using AngleSharp.Html.Parser;
using Timer = System.Timers.Timer;

namespace DonorBot.Services;

public class BotHost
{

    public BotHost(TelegramBot.TelegramBot tgBot)
    {
        tgBot.Start();
    }
}