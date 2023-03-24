using AngleSharp;
using AngleSharp.Html.Parser;
using BaseBot;
using BaseBot.Models;
using Timer = System.Timers.Timer;

namespace DonorBot.Services;

public class BotHost
{

    public BotHost(TelegramBot tgBot)
    {
        tgBot.Start();
    }
}