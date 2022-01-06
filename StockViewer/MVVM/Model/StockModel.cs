using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace StockViewer.MVVM.Model;
public class StockModel
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public SortedDictionary<DateTime, Price> PriceData { get; set; } = new();
    public SortedDictionary<DateTime, Price> InstitutionData { get; set; } = new();

    public SortedDictionary<DateTime, Price> InvestmentTrust { get; set; } = new();
    public SortedDictionary<DateTime, Price> InvestmentForeign { get; set; } = new();
    public SortedDictionary<DateTime, Price> Dealer { get; set; } = new();
    
    public StockModel() {}
    public StockModel(string id, string name)
    {
        Id = id;
        Name = name;
    }



    public static StockModel? LoadLocalData(string dataPath, string stockId)
    {
        if(!ContainData(dataPath, stockId))
            return null;

        StockModel? stock = new();
        string stockPath = Path.Combine(dataPath, stockId);

        DirectoryInfo path = new(stockPath);
        foreach (FileInfo file in path.EnumerateFiles("*"))
        {
            StockModel? source = Extention.LoadJson<StockModel?>(stockPath, file.Name);
            stock.AddTradingDatas(source);
        }
        return stock;
    }
    public static bool ContainData(string dataPath, string stockId) => Directory.Exists(Path.Combine(dataPath, stockId));



    public DateTime GetLastDate() => PriceData.Last().Key;
    public void AddTradingDatas(StockModel? stock)
    {
        if (stock == null)
        {
            Trace.WriteLine("Stock is null");
            return;
        }

        Id = stock.Id;
        Name = stock.Name;
        foreach (var data in stock.PriceData)
        {
            if (PriceData.ContainsKey(data.Key))
                PriceData[data.Key] = data.Value;
            else
                PriceData.Add(data.Key, data.Value);
        }
    }
    public override string ToString() => $"Stock: {Id} {Name} {PriceData.Last().Key:yyyyMMdd}";
}
public struct Price
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


