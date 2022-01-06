using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace StockViewer.MVVM.Model;
public class StockModel
{
    public string Id { get; set; }
    public string Name { get; set; }
    public SortedDictionary<DateTime, Price> PriceData { get; set; } = new();
    public SortedDictionary<DateTime, Institution> InstitutionData { get; set; } = new();
    [JsonIgnore]
    public DateTime? LastDate { get => PriceData.Any() ? null : PriceData.Last().Key; }
    [JsonIgnore]
    public KeyValuePair<DateTime, Price>? LastPrice { get => PriceData.Any() ? null : PriceData.Last(); }
    [JsonIgnore]
    public KeyValuePair<DateTime, Institution>? LastInstitution { get => InstitutionData.Any() ? null : InstitutionData.Last(); }


    [JsonIgnore]
    private string mPath { get => Path.Combine(FilePath.Path_Stock, Id); }

    public StockModel(string id = "", string name = "")
    {
        Id = id;
        Name = name;
    }
    public override string ToString() => $"Stock: {Id} {Name} {LastDate:yyyyMMdd}";
}


