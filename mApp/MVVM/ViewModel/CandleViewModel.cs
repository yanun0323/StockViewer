namespace mApp.MVVM.ViewModel;
public class CandleViewModel : ObservableObject
{

    public ICommand? SizeChangedCommand { get; set; }
    public ICommand? LoadedCommand { get; set; }
    

    private Candle? _Candle;
    private Volume? _Volume;
    private Thickness _CandleMargin;

    public Candle? Candle
    {
        get { return _Candle; }
        set { _Candle = value; OnPropertyChanged(); }
    }
    public Thickness CandleMargin
    {
        get { return _CandleMargin; }
        set { _CandleMargin = value; OnPropertyChanged(); }
    }

    public CandleViewModel(CandleParameter parameter, Thickness candleMargin)
    {
        Candle = new(parameter);
        _CandleMargin = candleMargin;

        SizeChangedCommand = new RelayCommand<SizeChangedEventArgs>(Args => 
        {
            Resize(height: Args.NewSize.Height);
        });
    }
    public void Resize(double? height = null, double? width = null, double? top = null, double? bottom = null) => Candle = Candle!.Resize(height, width, top, bottom);
}
