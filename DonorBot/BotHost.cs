using AngleSharp;
using AngleSharp.Html.Parser;
using BaseBot.Models;
using Timer = System.Timers.Timer;

namespace BaseBot;

public class BotHost
{

    public BotHost(TelegramBot tgBot,
        ReportService reportService)
    {
        tgBot.Start();
        reportService.Start();
    }

   
}