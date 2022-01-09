using System.Collections.ObjectModel;

namespace StockViewer.MVVM.ViewModel;

public class ChartGridViewModel:ObservableObject
{
    private ObservableCollection<Line> _ChartLines = new();
    private ObservableCollection<Label> _ChartLabels = new();
    private Thickness _Margin;
    private Size _ChartSize;
    private double _Ratio;
    private double _HighestPrice;
    private double _LowestPrice;
    private double HeightRatio = CandleViewModel.CandleHeightRatio;


    public ObservableCollection<Line> ChartLines { get => _ChartLines; set { _ChartLines = value; } }
    public ObservableCollection<Label> ChartLabels { get => _ChartLabels; set { _ChartLabels = value; } }
    public Thickness Margin { get => _Margin; set => _Margin = value; }

    public ChartGridViewModel(Size chart, double highestPrice, double lowestPrice) 
    {
        Draw(chart, highestPrice, lowestPrice,MainChartViewModel.CandleMargin);
    }

    public void Resize(Size chart, double? highestPrice = null, double? lowestPrice = null, Thickness? margin = null) => Draw(chart, highestPrice ?? _HighestPrice, lowestPrice ?? _LowestPrice, margin ?? Margin);

    private void Draw(Size chart, double highestPrice, double lowestPrice, Thickness margin)
    {
        _ChartSize = chart;
        _Margin = margin;
        _HighestPrice = highestPrice; 
        _LowestPrice = lowestPrice;
        _Ratio = (chart.Height * HeightRatio) / (_HighestPrice - _LowestPrice);

        double priceInterval = _HighestPrice - _LowestPrice;
        int offset = 1;
        while (priceInterval / offset > 15)
        {
            offset *= 5;
        }
        double price = (int)(_LowestPrice / offset) * offset + offset;

        _ChartLines = new();
        _ChartLabels = new();
        _ChartLines.Add(CreatLine(_LowestPrice));
        _ChartLabels.Add(CreatLabel(_LowestPrice));
        while (price < _HighestPrice)
        {
            _ChartLines.Add(CreatLine(price));
            _ChartLabels.Add(CreatLabel(price));
            price += offset;
        }
        _ChartLines.Add(CreatLine(_HighestPrice));
        _ChartLabels.Add(CreatLabel(_HighestPrice));
        ChartLines = _ChartLines;
    }

    private Line CreatLine(double price) { 
        double top = (_HighestPrice - price) * _Ratio;
        Line result = new Line() {
            Stroke = Brushes.LightGray,
            StrokeThickness = 1,
            X1 = 0,
            Y1 = 0,
            X2 = _ChartSize.Width,
            Y2 = 0
        };
        Canvas.SetTop(result, top);
        Canvas.SetLeft(result, 0);

        return result;
    }
    private Label CreatLabel(double price)
    {
        double top = (_HighestPrice - price) * _Ratio;
        double width =  40;
        Label result = new Label()
        {
            Width = width,
            FontSize = 11,
            Foreground = Brushes.LightGray,
            Content = Math.Round(price, 2),
            VerticalContentAlignment = VerticalAlignment.Center,
            HorizontalContentAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Top,
            HorizontalAlignment = HorizontalAlignment.Left,
        };
        Canvas.SetTop(result, top - 5);
        Canvas.SetLeft(result, _ChartSize.Width - MainChartViewModel.GridWidth);

        return result;
    }
}
