using System.Collections.ObjectModel;

namespace StockViewer.MVVM.ViewModel;

public class ChartGridViewModel:ObservableObject
{
    private ObservableCollection<Line> _ChartLines = new();
    private ObservableCollection<Label> _ChartLabels = new();
    private ChartParameter _Parameter;
    private double _Ratio;


    public ObservableCollection<Line> ChartLines { get => _ChartLines; set { _ChartLines = value; } }
    public ObservableCollection<Label> ChartLabels { get => _ChartLabels; set { _ChartLabels = value; } }
    public Thickness Margin { get => MainChartViewModel.CandleMargin; }

    public ChartGridViewModel(ChartParameter parameter, bool custom = false, params double[] prices)
    {
        Draw(parameter, custom, prices);
    }

    public void Resize(Size chart)
    {
        _Parameter.Width = chart.Width;
        _Parameter.Height = chart.Height;
        Draw(_Parameter);
    }

    private void Draw(ChartParameter parameter, bool custom = false, params double[] prices)
    {
        _Parameter = parameter;
        double priceInterval = _Parameter.Highest - _Parameter.Lowest;
        _Ratio = (_Parameter.Height * CandleViewModel.CandleHeightRatio) / priceInterval;

        if (custom && prices != null)
        {
            foreach (double p in prices)
            {
                _ChartLines.Add(CreatLine(p));
                _ChartLabels.Add(CreatLabel(p));
            }
            ChartLines = _ChartLines;
            return;
        }


        int limitQuantity = (int)(_Parameter.Height / MainChartViewModel.ChartLineQuantityRatio);
        int offset = 1;
        while (priceInterval / offset > limitQuantity)
        {
            offset *= 5;
        }
        double price = (int)(_Parameter.Lowest / offset) * offset + offset;

        _ChartLines = new();
        _ChartLabels = new();
        _ChartLines.Add(CreatLine(_Parameter.Lowest));
        _ChartLabels.Add(CreatLabel(_Parameter.Lowest));
        while (price < _Parameter.Highest)
        {
            _ChartLines.Add(CreatLine(price));
            _ChartLabels.Add(CreatLabel(price));
            price += offset;
        }
        _ChartLines.Add(CreatLine(_Parameter.Highest));
        _ChartLabels.Add(CreatLabel(_Parameter.Highest));
        ChartLines = _ChartLines;
    }

    private Line CreatLine(double price) { 
        double top = (_Parameter.Highest - price) * _Ratio;
        Line result = new Line() {
            Stroke = Brushes.LightGray,
            StrokeThickness = 1,
            X1 = 0,
            Y1 = 0,
            X2 = _Parameter.Width,
            Y2 = 0
        };
        Canvas.SetTop(result, top);
        Canvas.SetLeft(result, 0);

        return result;
    }
    private Label CreatLabel(double price)
    {
        double top = (_Parameter.Highest - price) * _Ratio;
        int height = 30;
        Label result = new Label()
        {
            Width = MainChartViewModel.GridWidth,
            Height = height,
            FontSize = 11,
            Foreground = Brushes.LightGray,
            Content = Math.Round(price, 2),
            VerticalContentAlignment = VerticalAlignment.Center,
            HorizontalContentAlignment = HorizontalAlignment.Left,
            VerticalAlignment = VerticalAlignment.Top,
            HorizontalAlignment = HorizontalAlignment.Left,
        };
        Canvas.SetTop(result, top - height/2);
        Canvas.SetLeft(result, _Parameter.Width + 1);

        return result;
    }
}
