
namespace StockViewer.MVVM.Model;
public class StockModel : IStockModel
{
    public string Id { get; set; }
    public string Name { get; set; }
    public SortedDictionary<DateTime, Price> PriceData { get; set; } = new();
    public SortedDictionary<DateTime, Institution> InstitutionData { get; set; } = new();
    [JsonIgnore]
    public string IdName { get => string.Join(" ", Id, Name); }
    [JsonIgnore]
    public DateTime? LastDate { get => PriceData.Any() ? PriceData.Last().Key : null; }
    [JsonIgnore]
    public KeyValuePair<DateTime, Price> LastPrice { get => PriceData.Any() ? PriceData.Last() : new(PriceCrawler.Begin , Price.Deafult()) ; }
    [JsonIgnore]
    public KeyValuePair<DateTime, Institution> LastInstitution { get => InstitutionData.Any() ? InstitutionData.Last() : new(InstitutionCrawler.Begin, Institution.Deafult()); }


    [JsonIgnore]
    private string mPath { get => Path.Combine(FilePath.Path_Stock, Id); }

    public StockModel(string id = "", string name = "")
    {
        Id = id;
        Name = name;
    }
    public void Refresh(string? id = null) {
        if ( Id == id)
            return;

        Id = id ?? Id;

        if (Id == "")
            return;
        PriceData = new();
        InstitutionData = new();
        DirectoryInfo path = new(mPath);
        foreach (FileInfo file in path.EnumerateFiles("*"))
        {
            StockModel? load = FileManagement.LoadJson<StockModel?>(mPath, file.Name);
            if(load == null)
                continue;

            if(Name != load.Name)
                Name = load.Name;

            foreach (var item in load.PriceData)
            {
                PriceData.TryAdd(item.Key, item.Value);
            }

            foreach (var item in load.InstitutionData)
            {
                InstitutionData.TryAdd(item.Key, item.Value);
            }
        }
    }
    public void CorrectPriceData() 
    {
        foreach ( (DateTime date, Price price) in PriceData)
        {

        }
    }
    public override string ToString() => $"Stock: {Id} {Name} {LastDate:yyyyMMdd}";
}


