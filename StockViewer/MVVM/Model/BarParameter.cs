
namespace StockViewer.MVVM.Model;
public class BarParameter : IBarParameter
{
    int _Count;
    int _Start = 0;
    public int Start { get => _Start; set { _Start = (value >= 0) ? value : 0; } }
    public int Count { get => _Count; set { _Count = (value > MinCount) ? _Count : value; }}
    public int MinCount { get; set; }
    public double Width { get; set; }
    public double MinWidth { get; set; }

    public BarParameter(int count, int minCount, double width, double minWidth)
    {
        _Count = count > minCount ? count : minCount;
        MinCount = minCount;
        Width = width > minWidth ? width : minWidth;
        MinWidth = minWidth;
    }
}
