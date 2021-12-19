namespace mApp.MVVM.ViewModel;
public class CandleViewModel : ObservableObject
{
    public ICommand? SizeChangedCommand { get; set; }
    public ICommand? LoadedCommand { get; set; }
    
    private Candle? _candle;
    public Candle? Candle
    {
        get { return _candle; }
        set { _candle = value; OnPropertyChanged(); }
    }
    public CandleViewModel(CandleParameter parameter)
    {
        Candle = new(parameter);
        SizeChangedCommand = new RelayCommand<SizeChangedEventArgs>(Args => 
        {
            Candle = Candle.Resize(Args.NewSize.Height);
        });
    }
}
