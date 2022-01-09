namespace StockViewer.MVVM.ViewModel;
public class CandleViewModel : ObservableObject
{
    private DateTime _Date;
    private Candle? _Candle;
    private Volume? _Volume;
    private bool ShowMonth;

    public static readonly double CandleHeightRatio = 0.9;
    public static readonly double VolumeHeightRatio = 0.45;
    public double DateLabelHeight { get => 15; }
    public string Month { get =>  ShowMonth ? $"{_Date:yyyy/MM}" : ""; }
    public Candle? Candle
    {
        get { return _Candle; }
        set { _Candle = value; OnPropertyChanged(); }
    }
    public Volume? Volume
    {
        get { return _Volume; }
        set { _Volume = value; OnPropertyChanged(); }
    }
    public Thickness CandleMargin { get => MainChartViewModel.CandleMargin; }
    public DateTime Date
    {
        get { return _Date; }
        set { _Date = value; }
    }

    public CandleViewModel(DateTime dateTime, Price price, ChartParameter parameter, int highestVolume, bool showMonth = false)
    {
        ShowMonth = showMonth;
        _Date = dateTime;
        Candle = new(dateTime, price, parameter);
        Volume = new(dateTime, price, parameter, highestVolume);
    }

    public void Resize(double? height = null, double? width = null, double? top = null, double? bottom = null, int? highestVolume = null) {
        Candle = Candle!.Resize(height , width, top, bottom);
        Volume = Volume!.Resize(height , highestVolume);
    } 
}
