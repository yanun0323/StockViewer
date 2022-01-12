
using System.Collections.ObjectModel;

using System.Windows.Shapes;

namespace StockViewer.MVVM.ViewModel;
public class MainChartViewModel : ObservableObject
{
    public static Thickness CandleMargin = new(1, 0, 1, 0);
    public static double GridWidth = 7;

    IStockModel? _StockModel;
    Grid? _ChartCanvas = new() { Width = 300, Height = 300 };
    ObservableCollection<Rectangle>? _Rectangles = new();

    public ICommand? MouseUpCommand { get; set; }
    public ICommand? MouseMoveCommand { get; set; }
    public ICommand? SizeChangedCommand { get; set; }
    public ICommand? LoadedCommand { get; set; }
    public ObservableCollection<Rectangle>? Rectangles { get => _Rectangles; set { _Rectangles = value; OnPropertyChanged(); } }



    public MainChartViewModel(IStockModel stockModel)
    {
        _StockModel = stockModel;
        Refresh();

        MouseUpCommand = new RelayCommand<MouseButtonEventArgs>(e => 
        {
        });

        MouseMoveCommand = new RelayCommand<MouseEventArgs>(e =>
        {
        });

        SizeChangedCommand = new RelayCommand<SizeChangedEventArgs>(args =>
        {
        });

        LoadedCommand = new RelayCommand<Grid>(grid => 
        {
            _ChartCanvas = grid;
            Refresh();
        });
    }

    public void Update(IStockModel stockModel)
    {
        _StockModel = stockModel;
        Refresh();
    }



    void Refresh()
    {
        _Rectangles = null;
        _Rectangles = new();

        double max = _StockModel!.PriceData.Max(x => x.Value.mMax);
        double min = _StockModel!.PriceData.Min(x => x.Value.mMin);

        int i = _StockModel!.PriceData.Count();
        double width = 7;
        double ratio = _ChartCanvas!.Height / (max - min);
        foreach ((DateTime date, Price price) in _StockModel!.PriceData)
        {
            _Rectangles.Add(CreateThick(price, i, max, min, width, ratio));
            _Rectangles.Add(CreateThin(price, i, max, min, width, ratio));

            i--;
        }
        Rectangles = _Rectangles;

        static Rectangle CreateThick(Price price, int i, double max, double min, double width, double ratio) 
        {
            double high = price.mStart > price.mEnd ? price.mStart : price.mEnd;
            double low = price.mStart < price.mEnd ? price.mStart : price.mEnd;
            double top = ratio * (max - high);
            double left = i * (width + 2);

            Rectangle rect = new();
            rect.Width = width;
            rect.Height = ratio * (high - low);
            rect.Fill = price.mStart > price.mEnd ? iColor.Green : iColor.Red;

            Canvas.SetTop(rect, top);
            Canvas.SetLeft(rect, left);

            return rect;
        }
        static Rectangle CreateThin(Price price, int i, double max, double min, double width, double ratio)
        {
            double high = price.mMax;
            double low = price.mMin;
            double top = ratio * (max - high);
            double left = i * (width + 2) + (width / 2);

            Rectangle rect = new();
            rect.Width = 1;
            rect.Height = ratio * (high - low);
            rect.Fill = price.mStart > price.mEnd ? iColor.Green : iColor.Red;

            Canvas.SetTop(rect, top);
            Canvas.SetLeft(rect, left);

            return rect;
        }
    }
}
