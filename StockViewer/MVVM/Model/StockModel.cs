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


