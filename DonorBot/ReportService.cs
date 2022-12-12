using AngleSharp.Html.Parser;
using BaseBot.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Timer = System.Timers.Timer;

namespace BaseBot;

public class ReportService
{
    private readonly TelegramBot _tgBot;
    private readonly AppConfig _config;
    private readonly LogService _logService;

    public ReportService(TelegramBot tgBot,
        IOptions<AppConfig> config,
        LogService logService)
    {
        _tgBot = tgBot;
        _config = config.Value;
        _logService = logService;
    }

    public void Start()
    {
        var timer = new Timer(_config.PeriodHours * 60 * 60 * 1000);
        timer.Elapsed += async (_, _) => await PrepareReport();
        timer.Start();

        PrepareReport().ConfigureAwait(false);
    }

    private async Task PrepareReport()
    {
        try
        {
            HttpClient client = new();
            var content = await (await client.GetAsync(_config.ParsingUrl)).Content
                .ReadAsStringAsync();

            var parser = new HtmlParser();
            var doc = parser.ParseDocument(content);

            var items = doc.GetElementsByClassName("spk-lights__item");

            var report = new Report();

            foreach (var item in items)
            {
                var rezuses = item
                    .GetElementsByClassName("spk-lights__group")
                    .SelectMany(x => x.Children);

                foreach (var rezusItem in rezuses)
                {
                    var groupStr = item.Children.FirstOrDefault(x => x.ClassName == "spk-lights__head")?.TextContent;

                    var group = new BloodType
                    {
                        TypeRezus = new TypeRezus
                        {
                            Type = groupStr switch
                            {
                                " 0 (I) " => BloodTypeEnum.I,
                                " A (II) " => BloodTypeEnum.II,
                                " B (III) " => BloodTypeEnum.III,
                                " AB (IV) " => BloodTypeEnum.IV
                            },

                            RezusEnum = rezusItem.TextContent switch
                            {
                                " Rh+ " => RezusEnum.Pos,
                                " Rh- " => RezusEnum.Neg
                            }
                        }
                    };

                    switch (rezusItem.ClassName)
                    {
                        case "spk-lights__group-item spk-lights__group-item--max":
                            group.NeedForBlood = NeedForBlood.Low;
                            break;

                        case "spk-lights__group-item spk-lights__group-item--middle":
                            group.NeedForBlood = NeedForBlood.Middle;
                            break;

                        case "spk-lights__group-item spk-lights__group-item--min":
                            group.NeedForBlood = NeedForBlood.High;
                            break;
                    }

                    report.Groups.Add(group);
                }
            }

            await _tgBot.SendTextMessage(report.GetReportText());
        }
        catch (Exception exception)
        {
            _logService.AddLog(exception.ToString());
        }
    }
}


