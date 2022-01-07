using System.Threading.Tasks;

namespace StockViewer.Library.CrawlerConverter;
public class PriceConverter
{
    public List<List<string>>? data8 { get; set; }
    public List<List<string>>? data9 { get; set; }

    public static void Run(DateTime start, Dictionary<string, StockModel> stockModelCollection)
    {
        int year = start.Year;
        DateTime target = start;
        if (target.AddHours(14) > DateTime.Now)
            return;

        while (target.AddHours(14) < DateTime.Now && target.Year == year) 
        {
            Catch(target, stockModelCollection);
            target = target.AddDays(1);
        }
        return;

    }
    public static void Catch(DateTime target,Dictionary<string, StockModel> stockModelCollection)
    {
        string Name = $"{target:yyyyMMdd}";
        Trace.WriteLine($"PriceConverter - {Name}");
        PriceConverter? source = FileManagement.LoadJson<PriceConverter?>(FilePath.Path_Raw_Price, Name);

        List<List<string>>? datalist = source == null ? null : target.IsBeforeSwitchDay() ? source.data8 :  source.data9;

        if (datalist == null || datalist.Count() < 100)
            return;

        foreach (var data in datalist)
        {
            if (data[5].Trim() == "--")
                continue;

            Price price = new()
            {
                Volume = data[2].Trim(),
                VolumeMoney = data[4].Trim(),
                Start = data[5].Trim(),
                Max = data[6].Trim(),
                Min = data[7].Trim(),
                End = data[8].Trim(),
                Grade = data[9].Trim(),
                Spread = data[10].Trim(),
                Per = data[15].Trim(),
            };

            string id = data[0].Trim();
            string name = data[1].Trim();

            if (!stockModelCollection.ContainsKey(id))
                stockModelCollection.Add(id, new(id, name));

            stockModelCollection[id].PriceData.TryAdd(target, price);
        }
    }
}
