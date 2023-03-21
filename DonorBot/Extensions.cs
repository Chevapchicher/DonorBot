namespace BaseBot;

public static class Extensions
{
    public static DateTime NextDayOfWeek(this DateTime date, DayOfWeek day)
    {
        int diff = ((int)day - (int)date.DayOfWeek + 6) % 7;
        return date.AddDays(diff + 1);
    }

    public static int ToEpoch(this DateTime date)
    {
        var t = date - new DateTime(1970, 1, 1);
        return (int)t.TotalSeconds;
    }
}
