using System.Text.Json.Serialization;

namespace StockViewer.MVVM.Model;
public class Stock
{
    [JsonInclude]
    public string Id { get; set; } = "";
    [JsonInclude]
    public string Name { get; set; } = "";
    [JsonInclude]
    public SortedDictionary<DateTime, TradingData> TradingData { get; set; }


    public Stock() => TradingData = new();
    public Stock(string id, string name) {
        Id = id;
        Name = name;
        TradingData = new();
    }
    public DateTime GetLastDate() => TradingData.Last().Key;
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
            if(TradingData.ContainsKey(data.Key))
                TradingData[data.Key] = data.Value;
            else
                TradingData.Add(data.Key, data.Value);
        }
    }
    public static Stock LoadLocalData(string dataPath, string stockId)
    {
        Stock? stock = new();
        string stockPath = Path.Combine(dataPath, stockId);
        DirectoryInfo path = new(stockPath);

        foreach (FileInfo file in path.EnumerateFiles("*"))
        {
            Stock? source = FileManagement.LoadJson<Stock?>(stockPath, file.Name);
            stock.AddTradingDatas(source);
        }
        return stock;
    }
    public override string ToString() => $"Stock: {Id} {Name} {TradingData.Last().Key:yyyyMMdd}";
}
public struct TradingData
{
    [JsonInclude]
    public string Volume { get; init; }
    [JsonInclude]
    public string VolumeMoney { get; init; }
    [JsonInclude]
    public string Start { get; init; }
    [JsonInclude]
    public string Max { get; init; }
    [JsonInclude]
    public string Min { get; init; }
    public string End { get; init; }
    [JsonInclude]
    public string Grade { get; init; }
    [JsonInclude]
    public string Spread { get; init; }
    [JsonInclude]
    public string Turnover { get; init; }
    [JsonIgnore]
    public int mVolume { get => int.Parse(Volume.Replace(",", "")) / 1000; }
    [JsonIgnore]
    public long mVolumeMoney { get => long.Parse(VolumeMoney.Replace(",", "")); }
    [JsonIgnore]
    public double mStart { get => double.Parse(Start.Replace(",", "")); }
    [JsonIgnore]
    public double mMax { get => double.Parse(Max.Replace(",", "")); }
    [JsonIgnore]
    public double mMin { get => double.Parse(Min.Replace(",", "")); }
    [JsonIgnore]
    public double mEnd { get => double.Parse(End.Replace(",", "")); }
    [JsonIgnore]
    public string mGrade { get => Grade; }
    [JsonIgnore]
    public double mSpread 
    { 
        get 
        {
            double num = double.Parse(Spread.Replace(",", ""));
            if (num != 0.0 && Grade.Contains("green"))
                return -num;
            else
                return num;
        }
    }
    [JsonIgnore]
    public int mTurnover { get => int.Parse(Turnover.Replace(",", "")); }
    [JsonIgnore]
    public string mRatio 
    {
        get
        {
            double lastEnd = mEnd + mSpread;
            return $"{Math.Round(100 * mSpread / lastEnd, 2)} %";
        }
    }
    [JsonIgnore]
    public SolidColorBrush mColor
    {
        get
        {
            string grade = mGrade;
            if (grade.Contains("red"))
                return iColor.Red; 
            if (grade.Contains("green"))
                return iColor.Green;
            return iColor.Gray;
        }
    }
    [JsonIgnore]
    public string mSpreadSymbol
    {
        get
        {
            string grade = mGrade;
            if (grade.Contains("red"))
                return $"▲ {Spread}";

            if (grade.Contains("green"))
                return $"▼ {Spread}";

            return "-";
        }
    }
}

