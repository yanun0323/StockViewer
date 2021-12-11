﻿namespace mApp.Model;
public class Stock
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public SortedDictionary<DateTime, TradingData> TradingData { get; set; }
    public Stock() => TradingData = new();
    public Stock(string id, string name) {
        Id = id;
        Name = name;
        TradingData = new();
    }
    public void AddTradingDatas(Stock? stock)
    {
        if (stock == null) {
            Trace.WriteLine("Stock is null");
            return;
        }

        Id = stock.Id;
        Name = stock.Name;
        foreach (var data in stock.TradingData)
        {
            if(!TradingData.ContainsKey(data.Key))
                TradingData.Add(data.Key, data.Value);
        }
    }
    public void CheckTradingDatas()
    {
        List<DateTime> list = new();
        foreach (var data in TradingData)
        {
            if (data.Key.Hour != UpdateTime.Beginning.Hour)
                list.Add(data.Key);
        }
        list.ForEach(x => TradingData.Remove(x));
    }
    public static Stock LoadLocalData(string dataPath, string stockId)
    {
        Stock? stock = new();
        string stockPath = Path.Combine(dataPath, stockId);
        DirectoryInfo path = new(stockPath);

        foreach (FileInfo file in path.EnumerateFiles("*"))
        {
            Stock? source = Json.LoadJsonData<Stock>(stockPath, file.Name);
            stock.AddTradingDatas(source);
        }
        return stock;
    }
    public override string ToString()
    {
        return $"Stock: {Id} {Name} {TradingData.Last().Key:yyyyMMdd}";
    }
}
public struct TradingData
{
    public string Volume { get; init; }
    public string VolumeMoney { get; init; }
    public string Start { get; init; }
    public string Max { get; init; }
    public string Min { get; init; }
    public string End { get; init; }
    public string Grade { get; init; }
    public string Spread { get; init; }
    public string Turnover { get; init; }

    public int mVolume { get => int.Parse(Volume.Replace(",", "")); }
    public long mVolumeMoney { get => long.Parse(VolumeMoney.Replace(",", "")); }
    public double mStart { get => double.Parse(Start.Replace(",", "")); }
    public double mMax { get => double.Parse(Max.Replace(",", "")); }
    public double mMin { get => double.Parse(Min.Replace(",", "")); }
    public double mEnd { get => double.Parse(End.Replace(",", "")); }
    public string mGrade { get => Grade; }
    public double mSpread { get => double.Parse(Spread.Replace(",", "")); }
    public int mTurnover { get => int.Parse(Turnover.Replace(",", "")); }
}

