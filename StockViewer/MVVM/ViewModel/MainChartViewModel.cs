﻿
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
    string _PER = "";

    public ICommand? MouseUpCommand { get; set; }
    public ICommand? MouseMoveCommand { get; set; }
    public ICommand? SizeChangedCommand { get; set; }
    public ICommand? LoadedCommand { get; set; }
    public string PER { get => _PER; set { _PER = value; OnPropertyChanged(); } }
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
        if (stockModel != null)
        {
            _StockModel = stockModel; 
            _PER = string.Join(" : ", $"{ _StockModel.LastPrice.Key:yyyy/MM/dd}", _StockModel.LastPrice.Value.Per);
            PER = _PER;
        }
        Refresh();
    }



    void Refresh(Size? newSize = null)
    {
        if (newSize != null)
            _Canvas = newSize.Value;

        _BarParam.Count = (_StockModel!.PriceData.Count() < _BarParam.Count) ? _StockModel!.PriceData.Count() : _BarParam.Count;
        double width = (_Canvas.Width / _BarParam.Count) - 2;

        if (width < _BarParam.MinWidth)
        {
            _BarParam.Count = (int)(_Canvas.Width / (_BarParam.MinWidth + 2));
            width = (_Canvas.Width / _BarParam.Count) - 2;
        }
        _BarParam.Width = width;

        int index = _BarParam.Count;
        var data = _StockModel!.PriceData.Reverse().Skip(_BarParam.Start).Take(index);
        double max = data.Max(x => x.Value.mMax);
        double min = data.Min(x => x.Value.mMin);

        double ratio = _Canvas.Height / (max - min);
        List<Task> tasks = new();
        RectengleModel[] temp = new RectengleModel[index * 2];
        int size = index;
        foreach ((DateTime d, Price price) in data)
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

        _Rectangles = null;
        _Rectangles = new();

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
