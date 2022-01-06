namespace StockViewer.Library.CrawlerConverter;
public class InstitutionConverter
{
    public List<List<string>>? data { get; set; }
    [JsonIgnore]
    private static DateTime _switch = new(2017, 12, 18);
    public static void Run(Dictionary<string, StockModel> stockModelCollection)
    {

        DateTime? loadTime = FileManagement.LoadJson<DateTime?>(FilePath.Path_Root, FilePath.Name_UpdateTimeName_Institution);
        DateTime target = loadTime ?? InstitutionCrawler.Begin;
        DateTime now= DateTime.Now;
        Trace.WriteLine($"{target}");
        Trace.WriteLine($"{target.AddHours(17) < now}");

        while (target.AddHours(17) < now)
        {
            string Name = $"{target:yyyyMMdd}";
            Trace.WriteLine($"InstitutionConverter - {Name}");
            InstitutionConverter? source = FileManagement.LoadJson<InstitutionConverter?>(FilePath.Path_Raw_Institution, Name, true);

            if (source != null && source.data != null)
            {
                List<List<string>> datalist = source.data;

                foreach (var data in datalist)
                {
                    Institution institution = (target < _switch) ? new()
                    {
                        ForeignBuy = data[2],
                        ForeignSell = data[3],
                        ForeignSuper = data[4],
                        ForeignDealerBuy = data[2],
                        ForeignDealerSell = data[3],
                        ForeignDealerSuper = data[4],
                        TrustBuy = data[5],
                        TrustSell = data[6],
                        TrustSuper = data[7],
                        DealerSuper = data[8],
                        InstitutionSuper = data.Last(),
                    }
                    : new()
                    {
                        ForeignBuy = data[2],
                        ForeignSell = data[3],
                        ForeignSuper = data[4],
                        ForeignDealerBuy = data[5],
                        ForeignDealerSell = data[6],
                        ForeignDealerSuper = data[7],
                        TrustBuy = data[8],
                        TrustSell = data[9],
                        TrustSuper = data[10],
                        DealerSuper = data[11],
                        InstitutionSuper = data.Last(),
                    };
                    string id = data[0].Trim();
                    string name = data[1].Trim();

                    if (!stockModelCollection.ContainsKey(id))
                        stockModelCollection.Add(id, new(id, name));

                    stockModelCollection[id].InstitutionData.TryAdd(target, institution);
                }
            }
            target.SaveJson(FilePath.Path_Root, FilePath.Name_UpdateTimeName_Institution);
            target = target.AddDays(1);
        }
    }
}
