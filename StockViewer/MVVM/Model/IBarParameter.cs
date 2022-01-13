
namespace StockViewer.MVVM.Model;
public interface IBarParameter
{
    int Start { get; set; }
    int Count { get; set; }
    int MinCount { get; set; }
    double Width { get; set; }
    double MinWidth { get; set; }
}
