
using System.Collections.ObjectModel;

using System.Windows.Shapes;

namespace StockViewer.MVVM.ViewModel;
public class MainChartViewModel : ObservableObject
{
    public static Thickness CandleMargin = new(1, 0, 1, 0);
    public static double GridWidth = 7;

    IStockModel? _StockModel;
    Size _Canvas = new(904, 356);
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
            Trace.WriteLine($"SizeChangedEventArgs {args.NewSize.Width} {args.NewSize.Height}");
            _Canvas = args.NewSize;
            Refresh();
        });

        LoadedCommand = new RelayCommand<Grid>(grid =>
        {
            //Trace.WriteLine($"grid {grid.Width} {grid.Height}");
            //_ChartCanvas = grid;
            //Refresh();
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

        int count = 100;
        int i = (_StockModel!.PriceData.Count() < count) ? _StockModel!.PriceData.Count() : count;

        double width = (_Canvas!.Width / i) - 2;

        double max = _StockModel!.PriceData.Take(i).Max(x => x.Value.mMax);
        double min = _StockModel!.PriceData.Take(i).Min(x => x.Value.mMin);

        double ratio = _Canvas!.Height / (max - min);
        i--;
        foreach ((DateTime date, Price price) in _StockModel!.PriceData)
        {
            _Rectangles.Add(CreateThick(price, i, max, min, width, ratio));
            _Rectangles.Add(CreateThin(price, i, max, min, width, ratio));
            if (i-- <= 0)
                break;
        }
        Trace.WriteLine($"_Rectangles.Count() {_Rectangles.Count()}");
        Rectangles = _Rectangles;

        static Rectangle CreateThick(Price price, int i, double max, double min, double width, double ratio) 
        {
            double high = price.mStart > price.mEnd ? price.mStart : price.mEnd;
            double low = price.mStart < price.mEnd ? price.mStart : price.mEnd;
            double top = ratio * (max - high);
            double left = i * (width + 2);
            double height = ratio * (high - low);

            Rectangle rect = new();
            rect.Width = width;
            rect.Height = high == low ? 1 : ratio * (high - low);
            rect.Fill = price.mStart == price.mEnd ? iColor.Gray : price.mStart > price.mEnd ? iColor.Green : iColor.Red;
            //rect.Margin = new(1, 1, 0, 0);

            Canvas.SetTop(rect, top);
            Canvas.SetLeft(rect, left);

            return rect;
        }
        static Rectangle CreateThin(Price price, int i, double max, double min, double width, double ratio)
        {
            double high = price.mMax;
            double low = price.mMin;
            double top = ratio * (max - high);
            double left = i * (width + 2) + ((width - 1) / 2);

            Rectangle rect = new();
            rect.Width = 1;
            rect.Height = ratio * (high - low);
            rect.Fill = price.mStart == price.mEnd ? iColor.Gray : price.mStart > price.mEnd ? iColor.Green : iColor.Red;

            Canvas.SetTop(rect, top);
            Canvas.SetLeft(rect, left);

            return rect;
        }
    }
}
