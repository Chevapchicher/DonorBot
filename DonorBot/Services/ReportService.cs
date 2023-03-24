using AngleSharp.Html.Parser;
using BaseBot;
using BaseBot.Models;
using DonorBot.Configs;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Timer = System.Timers.Timer;

namespace DonorBot.Services;

public class ReportService : BackgroundService
{
    private readonly TelegramBot _tgBot;
    private readonly AppConfig _config;
    private readonly LogService _logService;
    private readonly PeriodicTimer _timer;

    public ReportService(TelegramBot tgBot,
        IOptions<AppConfig> config,
        LogService logService)
    {
        _timer = new PeriodicTimer(config.Value.Period);
        _tgBot = tgBot;
        _config = config.Value;
        _logService = logService;
    }

    private async Task Process(CancellationToken token)
    {
        try
        {
            HttpClient client = new();
            var content = await (await client.GetAsync(_config.ParsingUrl, token)).Content
                .ReadAsStringAsync(token);

            var parser = new HtmlParser();
            var doc = await parser.ParseDocumentAsync(content, token);

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

    protected override async Task ExecuteAsync(CancellationToken token)
    {
        do
        {
            try
            {
                await Process(token);
            }
            catch (Exception ex)
            {
                await _tgBot.SendTextMessage($"Произошла какая-то хуйня: {ex}");
            }

        } while (await _timer.WaitForNextTickAsync(token) || !token.IsCancellationRequested);
    }
}