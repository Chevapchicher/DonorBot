namespace BaseBot.Models;

public class Report
{
    public List<BloodType> Groups { get; set; } = new();

    public string GetReportText()
    {
        string text = string.Empty;

        foreach (var group in Groups)
        {
            text += $"{group.Color} {group.Name}\r\n";
        }

        return text;
    }
}

public class BloodType
{
    public TypeRezus TypeRezus { get; set; }
    public string TypeName => TypeRezus.Type.ToString();
    public string RezusName => TypeRezus.RezusEnum == RezusEnum.Pos 
        ? "+" 
        : "-";

    public string Name => $"{TypeName}{RezusName}";

    public NeedForBlood NeedForBlood { get; set; }

    public string Color
    {
        get
        {
            switch (NeedForBlood)
            {
                case NeedForBlood.High:
                    return GREEN;
                case NeedForBlood.Middle:
                    return YELLOW;
                case NeedForBlood.Low:
                    return RED;
            }

            return string.Empty;
        }
    }

    private static string GREEN = "🟢";
    private static string RED = "🔴";   
    private static string YELLOW = "🟡";
}

public class TypeRezus
{
    public BloodTypeEnum Type { get; set; }
    public RezusEnum RezusEnum { get; set; }
}

public enum NeedForBlood
{
    Low,
    Middle,
    High,
}

public enum RezusEnum
{
    Pos = 1,
    Neg
}

public enum BloodTypeEnum
{
    I = 1,
    II,
    III,
    IV
}
