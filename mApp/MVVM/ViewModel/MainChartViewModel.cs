
using System.Collections.ObjectModel;

namespace mApp.MVVM.ViewModel;
public class MainChartViewModel:ObservableObject
{
    public ICommand? MouseWheelCommand { get; set; }
    public ICommand? LoadedCommand { get; set; }
    public ICommand? SizeChangedCommand { get; set; }


    private double _Top;
    private double _Bottom;
    private DateTime _StartDate;
    private Dictionary<DateTime, TradingData> _TradingDatas = new();
    private Stock _mStock = new();
    private ObservableCollection<CandleViewModel>? _CandleVMs;
    private Size _ChartSize = new(834, 305);
    private readonly Thickness _CandleMargin = new(2, 0, 2, 0);
    private double _CandleWidth = 10 ;
    public double Top { get => _Top; set { _Top = value; OnPropertyChanged(); } }
    public double Bottom { get => _Bottom; set { _Bottom = value; OnPropertyChanged(); } }
    public DateTime StartDate { get => _StartDate; set { _StartDate = value; OnPropertyChanged(); } }
    public Dictionary<DateTime,TradingData> TradingDatas { get => _TradingDatas; set { _TradingDatas = value; OnPropertyChanged(); } }
    public ObservableCollection<CandleViewModel>? CandleVMs { get => _CandleVMs; set { _CandleVMs = value; OnPropertyChanged(); } }

    public Stock mStock
    {
        get => _mStock;
        set 
        { 
            _mStock = value;
            Update();
            OnPropertyChanged();
        }
    }

    public MainChartViewModel(Stock stock)
    {
        mStock = stock;

        MouseWheelCommand = new RelayCommand<MouseWheelEventArgs>(e =>
        {
            double step = 1;
            double scale = ((e.Delta > 0) ? 1 : -1) * step;
            if (_CandleWidth + scale > 4)
                _CandleWidth += scale;
            Update();

            /*foreach (var candleVM in _CandleVMs!)
            {
                candleVM!.Zoom(e.Delta, _ChartSize);
            }*/
        });

        LoadedCommand = new RelayCommand<Size>(size => 
        {
        });
        SizeChangedCommand = new RelayCommand<SizeChangedEventArgs>(args =>
        {
            _ChartSize = args.NewSize;
        });
    }

    void Update(DateTime? startDate = null)
    {
        StartDate = startDate ?? mStock!.GetLastDate();
        TradingDatas = UpdateTradingDatas();
        Top = TradingDatas!.Max(x => x.Value.mMax);
        Bottom = TradingDatas!.Min(x => x.Value.mMin);
        CandleVMs = UpdateCandleVMs();
    }
    public void UpdateChart(Stock stock) => mStock = stock;
    private Dictionary<DateTime, TradingData> UpdateTradingDatas()
    {
        DateTime date = StartDate;
        Dictionary<DateTime, TradingData> result = new();

        int tradingCount = mStock!.TradingData.Count();
        int candleCount = GetCandleCount();
        int maxCount = candleCount < tradingCount ? candleCount : tradingCount;
        Trace.WriteLine($"GetCandleCount(): {GetCandleCount()}");
        Trace.WriteLine($"maxCount: {maxCount}");
        int count = 0;
        while (maxCount > 0 && count < tradingCount)
        {
            if (mStock!.TradingData.ContainsKey(date))
            {
                result.Add(date, mStock.TradingData[date]);
                maxCount--;
                Trace.WriteLine($"maxCount: {maxCount}");
            }

            date = date.AddDays(-1);
            count++;
        }
        return result;
    }
    private ObservableCollection<CandleViewModel> UpdateCandleVMs()
    {
        ObservableCollection<CandleViewModel> result = new();
        foreach (var entry in TradingDatas) 
        {
            CandleParameter cp = new()
            {
                Date = entry.Key,
                Width = _CandleWidth,
                Height = _ChartSize.Height,
                Top = Top,
                Bottom = Bottom,
                Tr = entry.Value
            };

            result.Add(new CandleViewModel(cp, _CandleMargin));
        }
        return result;
    }
    private int GetCandleCount() => (int)(_ChartSize.Width / (_CandleWidth + _CandleMargin.Left + _CandleMargin.Right)) + 1;

}
