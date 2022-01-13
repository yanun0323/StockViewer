
namespace StockViewer.MVVM.ViewModel;
public class MainChartViewModel : ObservableObject
{
    public static Thickness CandleMargin = new(1, 0, 1, 0);
    public static double GridWidth = 7;

    IStockModel _StockModel;
    Size _Canvas = new(904, 356);
    Func<IBarParameter> _Func;
    IBarParameter _BarParam { get => _Func.Invoke(); }
    ObservableCollection<RectengleModel>? _Rectangles = new();

    public ICommand? MouseUpCommand { get; set; }
    public ICommand? MouseMoveCommand { get; set; }
    public ICommand? SizeChangedCommand { get; set; }
    public ICommand? LoadedCommand { get; set; }
    public ObservableCollection<RectengleModel>? Rectangles { get => _Rectangles; set { _Rectangles = value; OnPropertyChanged(); } }

    public MainChartViewModel(IStockModel stockModel, Func<IBarParameter> func)
    {
        _StockModel = stockModel;
        _Func = func;
        Refresh();

        MouseUpCommand = new RelayCommand<MouseButtonEventArgs>(e => 
        {
        });

        MouseMoveCommand = new RelayCommand<MouseEventArgs>(e =>
        {
        });

        SizeChangedCommand = new RelayCommand<SizeChangedEventArgs>(args =>
        {
            _Canvas = args.NewSize;
            Refresh(args.NewSize);
        });
    }



    public void Update(IStockModel? stockModel = null)
    {
        Trace.WriteLine($" --- _BarParam.Count: {_BarParam.Count}");
        if (stockModel != null)
            _StockModel = stockModel;
        Refresh();
    }



    void Refresh(Size? newSize = null)
    {
        if (newSize != null)
            _Canvas = newSize.Value;

        _Rectangles = null;
        _Rectangles = new();

        _BarParam.Count = (_StockModel!.PriceData.Count() < _BarParam.Count) ? _StockModel!.PriceData.Count() : _BarParam.Count;
        double width = (_Canvas.Width / _BarParam.Count) - 2;

        if (width < _BarParam.MinWidth)
        {
            _BarParam.Count = (int)(_Canvas.Width / (_BarParam.MinWidth + 2));
            width = (_Canvas.Width / _BarParam.Count) - 2;
        }
        _BarParam.Width = width;

        int index = _BarParam.Count;
        double max = _StockModel!.PriceData.Skip(_BarParam.Start).Take(index).Max(x => x.Value.mMax);
        double min = _StockModel!.PriceData.Skip(_BarParam.Start).Take(index).Min(x => x.Value.mMin);

        double ratio = _Canvas.Height / (max - min);
        List<Task> tasks = new();
        RectengleModel[] temp = new RectengleModel[index * 2];
        int size = index;
        foreach ((DateTime _, Price price) in _StockModel!.PriceData.Take(index + _BarParam.Start).Skip(_BarParam.Start))
        {
            if (--index < 0)
                break;

            int i = index;
            Price copy = price;
            Task t = new(() => {
                temp[i] = (CreateThick(copy, i, max, min, width, ratio));
                temp[i + size] = (CreateThin(copy, i, max, min, width, ratio));
            });
            t.Start();
            tasks.Add(t);
        }
        Task.WhenAll(tasks).Wait();

        for (int i = 0; i < temp.Count(); i++)
        {
            _Rectangles.Add(temp[i]);
        }

        Rectangles = _Rectangles;

        static RectengleModel CreateThick(Price price, int i, double max, double min, double width, double ratio) 
        {
            double high = price.mStart > price.mEnd ? price.mStart : price.mEnd;
            double low = price.mStart < price.mEnd ? price.mStart : price.mEnd;

            return new RectengleModel() { 
                Width = width,
                Left = i * (width + 2),
                Top = ratio * (max - high),
                Height = high == low ? 1 : ratio * (high - low),
                Color = price.mStart == price.mEnd ? iColor.Gray : price.mEnd > price.mStart ? iColor.Red : iColor.Green,
            };
        }
        static RectengleModel CreateThin(Price price, int i, double max, double min, double width, double ratio)
        {
            double high = price.mMax;
            double low = price.mMin;

            return new RectengleModel()
            {
                Width = 1,
                Left = i * (width + 2) + ((width - 1) / 2),
                Top = ratio * (max - high),
                Height = ratio * (high - low),
                Color = price.mStart == price.mEnd ? iColor.Gray : price.mEnd > price.mStart ? iColor.Red : iColor.Green,
            };
        }
    }
}
