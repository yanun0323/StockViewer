namespace mApp.Model; 
public class TitleStock
{
    public TradingData tradingData { get; set; }
    public string SpreadWithSymbol { get; set; }
    public SolidColorBrush Spread_Color { get; set; }

    public TitleStock(TradingData mTD)
    {
        tradingData = mTD;
        SpreadWithSymbol = GetSpreadWithSymbol(mTD.mGrade, mTD.mSpread);
        Spread_Color = GetColorFromPrice(mTD.mGrade);
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
    private static string GetSpreadWithSymbol(string grade, double spread)
    {
        if (grade.Contains("red"))
        {
            return "▲" + $"{spread}";
        }
        else if (grade.Contains("green"))
        {
            return "▼" + $"{spread}";
        }
        else
        {
            return "-";
        }
    }

}


