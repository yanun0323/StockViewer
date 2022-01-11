
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
            if (_Func.Invoke().mStockModel.Id == mStockModel.Id)
                return;
            _Func.Invoke().UpdateStock(mStockModel.Id);
        });
    }
}
