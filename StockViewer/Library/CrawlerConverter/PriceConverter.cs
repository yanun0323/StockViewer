using System.Threading.Tasks;

namespace StockViewer.Library.CrawlerConverter;
public class PriceConverter
{
    public List<List<string>>? data8 { get; set; }
    public List<List<string>>? data9 { get; set; }

    public static void Run(Dictionary<string, StockModel> stockModelCollection)
    {

        DateTime? loadTime = FileManagement.LoadJson<DateTime?>(FilePath.Path_Root, FilePath.Name_UpdateTime_Price);
        DateTime target = loadTime ?? PriceCrawler.Begin;

        Task task = new(() => Trace.WriteLine($"Task!"));
        while (target.AddHours(14) < DateTime.Now) 
        {
            task = Catch(target, stockModelCollection);
            task.Wait();

            target.SaveJson(FilePath.Path_Root, FilePath.Name_UpdateTime_Price);
            target = target.AddDays(1);
        }
        Trace.WriteLine($"Done!");
    }
    public static Task Catch(DateTime target,Dictionary<string, StockModel> stockModelCollection)
    {
        return Task.Run(() =>
        {
            DateTime copy = target;

            string Name = $"{copy:yyyyMMdd}";
            Trace.WriteLine($"PriceConverter - {Name}");
            PriceConverter? source = FileManagement.LoadJson<PriceConverter?>(FilePath.Path_Raw_Price, Name);

            List<List<string>>? datalist = source == null ? null : copy.IsBeforeSwitchDay() ? source.data8 : source.data9;

            if (datalist != null)
            {

                if (datalist.Count() < 100)
                    Trace.WriteLine($"datalist.Count(): {datalist.Count()}");



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

                    stockModelCollection[id].PriceData.TryAdd(copy, price);
                }
            }
        });
    }
}
