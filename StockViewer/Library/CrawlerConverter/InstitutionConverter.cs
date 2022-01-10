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

        Trace.WriteLine($"Strat InstitutionConverter:{year}");
        while (target.AddHours(17) < DateTime.Now && target.Year == year)
        {
            Catch(target, stockModelCollection);
            target = target.AddDays(1);
        }
    }
    public static void Catch(DateTime target, Dictionary<string, StockModel> stockModelCollection)
    {
        string Name = $"{target:yyyyMMdd}";
        InstitutionConverter? source = FileManagement.LoadJson<InstitutionConverter?>(FilePath.Path_Raw_Institution, Name, true);

        if (source == null || source.data == null)
            return;

        List<List<string?>> datalist = source.data;

        foreach (var data in datalist)
        {
            string id = data[0] == null ? "" : data[0]!.Trim();
            string name = data[1] == null ? "" : data[1]!.Trim();
            if (id == "" || name == "" || !stockModelCollection.ContainsKey(id))
                continue;

            Institution institution = (target < _switch) ? new()
            {
                ForeignSuper = data[4] ?? "0",
                ForeignDealerSuper = data[4] ?? "0",
                TrustSuper = data[7] ?? "0",
                DealerSuper = data[8] ?? "0",
                InstitutionSuper = data.Last() ?? "0",
            }
            : new()
            {
                ForeignSuper = data[4] ?? "0",
                ForeignDealerSuper = data[7] ?? "0",
                TrustSuper = data[10] ?? "0",
                DealerSuper = data[11] ?? "0",
                InstitutionSuper = data.Last() ?? "0",
            };

            stockModelCollection[id].InstitutionData.TryAdd(target, institution);
        }
    }
}
