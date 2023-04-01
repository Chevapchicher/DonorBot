using DonorBot.Models;

namespace DonorBot.Configs;

public class AppConfig
{
    public static string ConfigKey = "app";

    public string BotToken { get; set; }
    public long[] Subscribers { get; set; }
    public TypeRezus[] WatchingGroups { get; set; }
    public TimeSpan Period { get; set; }
    public string ParsingUrl { get; set; }
}