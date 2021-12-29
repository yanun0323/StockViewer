namespace mApp.MVVM.Model; 
public class TitleStock
{
    public string Name { get; set; }
    public TradingData TradingData { get; set; }

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
    }

}


