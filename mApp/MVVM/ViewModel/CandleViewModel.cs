namespace mApp.MVVM.ViewModel;
public class CandleViewModel : ObservableObject
{

    public ICommand? SizeChangedCommand { get; set; }
    public ICommand? LoadedCommand { get; set; }

    private DateTime _Date;
    private Candle? _Candle;
    private readonly double _CandleHeightRatio = 0.9;
    private Volume? _Volume;
    private readonly double _VolumeHeightRatio = 0.3;
    private Thickness _CandleMargin;
    private bool ShowMonth;

    public double DateLabelHeight { get => 15; }
    public string Month { get =>  ShowMonth ? $"{_Date.Month}" : ""; }
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
    public Thickness CandleMargin
    {
        get { return _CandleMargin; }
        set { _CandleMargin = value;}
    }
    public DateTime Date
    {
        get { return _Date; }
        set { _Date = value; }
    }

    public CandleViewModel(CandleParameter parameter, Thickness candleMargin, bool showMonth = false)
    {
        ShowMonth = showMonth;
        _CandleMargin = candleMargin;
        _Date = parameter.Date;
        Candle = new(parameter, _CandleHeightRatio);
        Volume = new(parameter, _VolumeHeightRatio);

        SizeChangedCommand = new RelayCommand<SizeChangedEventArgs>(Args => 
        {
            //Resize(height: Args.NewSize.Height);
        });
    }

    public void Resize(double? height = null, double? width = null, double? top = null, double? bottom = null, int? highestVolume = null) {
        Candle = Candle!.Resize(height - DateLabelHeight, width, top, bottom);
        Volume = Volume!.Resize(height - DateLabelHeight, highestVolume);
    } 
}
