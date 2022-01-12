
namespace StockViewer.MVVM.Model;
public interface IStockModel
{
    string IdName { get; }
    string Id { get; set; }
    string Name { get; set; }
    DateTime? LastDate { get; }
    KeyValuePair<DateTime, Price> LastPrice { get; }
    SortedDictionary<DateTime, Price> PriceData { get; set; }
    KeyValuePair<DateTime, Institution> LastInstitution { get; }
    SortedDictionary<DateTime, Institution> InstitutionData { get; set; }

    void Refresh(string? id);
}
