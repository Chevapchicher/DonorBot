using BaseBot.Models;

namespace BaseBot;

public class AppConfig
{
    public static string ConfigKey = "app";

    public string BotToken { get; set; }
    public long[] Subscribers { get; set; }
    public TypeRezus[] WatchingGroups { get; set; }
    public int PeriodHours { get; set; }
    public string ParsingUrl { get; set; }
}