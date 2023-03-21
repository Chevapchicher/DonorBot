using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using BaseBot;
using DonorBot.Configs;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace DonorBot.Services;

public class AppointmentCheckerService : BackgroundService
{
    private readonly PeriodicTimer _timer;
    private readonly TelegramBot _tgBot;

    public AppointmentCheckerService(TelegramBot tgBot, IOptions<AppointmentConfig> config)
    {
        _timer = new PeriodicTimer(config.Value.Period);
        _tgBot = tgBot;
    }
    protected override async Task ExecuteAsync(CancellationToken token)
    {
        do
        {
            using var client = new HttpClient();

            var weeks = new List<int> { Week.ThisWeek, Week.NextWeek, Week.TwoNextWeek };

            foreach (var week in weeks)
            {
                var response = await client.PostAsync("http://fdo.1spbgmu.ru/index.php",
                    new FormUrlEncodedContent(CreateFormBody(week)), token);

                var content = await response.Content.ReadAsStringAsync(token);

                var parser = new HtmlParser();
                var doc = await parser.ParseDocumentAsync(content, token);

                var days = doc.QuerySelectorAll<IHtmlDivElement>("div.col-md-1-7");

                foreach (var day in days)
                {
                    var times = day.QuerySelectorAll<IHtmlDivElement>("div.one_day_week.cells_week div.recept.recpublish1");

                    if (IsFreeDay(times))
                    {
                        var weekOf = week switch
                        {
                            Week.ThisWeek => "на этой неделе",
                            Week.NextWeek => "на следующей неделе",
                            Week.TwoNextWeek => "через две недели"
                        };

                        var dayStr = day.TextContent.Split().FirstOrDefault(x => !string.IsNullOrWhiteSpace(x));

                        await _tgBot.SendTextMessage($"Есть место {weekOf}! {dayStr}");
                    }
                }
            }
            

        } while (await _timer.WaitForNextTickAsync(token) || !token.IsCancellationRequested);
    }

    private static bool IsFreeDay(IEnumerable<IHtmlDivElement> times)
    {
        return times.Any(x => x.TextContent.Contains("из"));
    }

    private Dictionary<string, string> CreateFormBody(int numberOfWeek)
    {
        return new Dictionary<string, string>
        {
            { "option", "com_ttfsp" },
            { "view", "detail" },
            { "format", "raw" },
            { "id", "28" },
            { "cdate", DateTime.Now.NextDayOfWeek(DayOfWeek.Monday)
                .AddDays(7 * numberOfWeek - 1)
                .ToEpoch()
                .ToString()
            }

        };
    }

    private class Week
    {
        public const int ThisWeek = -1;
        public const int NextWeek = 0;
        public const int TwoNextWeek = 1;
    }
}