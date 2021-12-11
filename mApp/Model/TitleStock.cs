namespace mApp.Model; 
public class TitleStock
{
    public string Name { get; set; }
    public TradingData TradingData { get; set; }
    public string SpreadWithSymbol { get; set; }
    public SolidColorBrush Spread_Color { get; set; }

    public TitleStock(Stock stock)
    {
        Name = string.Join(" ",stock.Id, stock.Name);
        if (stock!.TradingData.Count != 0)
            TradingData = stock!.TradingData.Last().Value;
        else
            TradingData = new()
            {
                Volume = "--",
                VolumeMoney = "--",
                Start = "--",
                Max = "--",
                Min = "--",
                End = "--",
                Grade = "--",
                Spread = "--",
                Turnover = "--"
            };
        SpreadWithSymbol = GetSpreadWithSymbol(TradingData.Grade, TradingData.Spread);
        Spread_Color = GetColorFromPrice(TradingData.Grade);
    }
    private static SolidColorBrush GetColorFromPrice(string grade)
    {
        if (grade.Contains("red"))
        {
            return Color.Red;
        }
        else if (grade.Contains("green"))
        {
            return Color.Green;
        }
        else
        {
            return Color.Gray;
        }
    }
    private static string GetSpreadWithSymbol(string grade, string spread)
    {
        if (grade.Contains("red"))
        {
            return "▲" + spread;
        }
        else if (grade.Contains("green"))
        {
            return "▼" + spread;
        }
        else
        {
            return "-";
        }
    }

}


