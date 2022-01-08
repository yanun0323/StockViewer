
namespace StockViewer.MVVM.Model;
public class Stock
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    [JsonIgnore]
    public string IdName { get => string.Join(" ", Id, Name); }
}
