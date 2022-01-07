using System.Threading.Tasks;

namespace StockViewer.Library.CrawlerConverter;
public class InstitutionConverter
{
    public List<List<string?>>? data { get; set; }
    [JsonIgnore]
    private static DateTime _switch = new(2017, 12, 18);
    public static void Run(DateTime start, Dictionary<string, StockModel> stockModelCollection)
    {
        if (start.Year < InstitutionCrawler.Begin.Year)
            return;

        int year = start.Year;
        DateTime target = (start< InstitutionCrawler.Begin) ? InstitutionCrawler.Begin : start;

        if (target.AddHours(17) > DateTime.Now)
            return;

        while (target.AddHours(17) < DateTime.Now && target.Year == year)
        {
            Catch(target, stockModelCollection);
            target = target.AddDays(1);
        }
    }
    public static void Catch(DateTime target, Dictionary<string, StockModel> stockModelCollection)
    {
        string Name = $"{target:yyyyMMdd}";
        Trace.WriteLine($"InstitutionConverter - {Name}");
        InstitutionConverter? source = FileManagement.LoadJson<InstitutionConverter?>(FilePath.Path_Raw_Institution, Name, true);

        if (source == null || source.data == null)
            return;

        List<List<string?>> datalist = source.data;

        foreach (var data in datalist)
        {
            Institution institution = (target < _switch) ? new()
            {
                ForeignBuy = data[2] ?? "0",
                ForeignSell = data[3] ?? "0",
                ForeignSuper = data[4] ?? "0",
                ForeignDealerBuy = data[2] ?? "0",
                ForeignDealerSell = data[3] ?? "0",
                ForeignDealerSuper = data[4] ?? "0",
                TrustBuy = data[5] ?? "0",
                TrustSell = data[6] ?? "0",
                TrustSuper = data[7] ?? "0",
                DealerSuper = data[8] ?? "0",
                InstitutionSuper = data.Last() ?? "0",
            }
            : new()
            {
                ForeignBuy = data[2] ?? "0",
                ForeignSell = data[3] ?? "0",
                ForeignSuper = data[4] ?? "0",
                ForeignDealerBuy = data[5] ?? "0",
                ForeignDealerSell = data[6] ?? "0",
                ForeignDealerSuper = data[7] ?? "0",
                TrustBuy = data[8] ?? "0",
                TrustSell = data[9] ?? "0",
                TrustSuper = data[10] ?? "0",
                DealerSuper = data[11] ?? "0",
                InstitutionSuper = data.Last() ?? "0",
            };
            string id = data[0].Trim();
            string name = data[1].Trim();

            if (!stockModelCollection.ContainsKey(id))
                stockModelCollection.Add(id, new(id, name));

            stockModelCollection[id].InstitutionData.TryAdd(target, institution);
        }
    }
}
