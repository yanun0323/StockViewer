namespace mApp.MVVM.ViewModel;
public class CandleViewModel : ObservableObject
{

    public ICommand? SizeChangedCommand { get; set; }
    public ICommand? LoadedCommand { get; set; }
    

    private Candle? _Candle;
    private double _CandleHeightRatio = 0.7;
    private Volume? _Volume;
    private double _VolumeHeightRatio = 0.2;
    private Thickness _CandleMargin;

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

    public CandleViewModel(CandleParameter parameter, Thickness candleMargin, double highestVolume)
    {
        Candle = new(parameter, _CandleHeightRatio);
        Volume = new(parameter, _VolumeHeightRatio);
        _CandleMargin = candleMargin;

        SizeChangedCommand = new RelayCommand<SizeChangedEventArgs>(Args => 
        {
            Resize(height: Args.NewSize.Height);
        });
    }
    public void Resize(double? height = null, double? width = null, double? top = null, double? bottom = null, double? highestVolume = null) {
        Candle = Candle!.Resize(height, width, top, bottom);
        Volume = Volume!.Resize(height, highestVolume);
    } 
}
