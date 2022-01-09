
namespace StockViewer.MVVM.ViewModel;
public class PickStockBlockViewModel : ObservableObject
{
    public ICommand? ClickCommand { get; set; }
    public StockModel mStockModel { get; set; }
    private Func<MainViewModel> _Func;

    public PickStockBlockViewModel(StockModel stockModel, Func<MainViewModel> func)
    {
        mStockModel = stockModel;
        _Func = func;

        ClickCommand = new RelayCommand<RoutedEventArgs>(arg =>
        {
            Trace.WriteLine($"SmartPick Click A!");
            _Func.Invoke().UpdateStock(mStockModel.Id);
            Trace.WriteLine($"SmartPick Click B!");
        });
    }
}
