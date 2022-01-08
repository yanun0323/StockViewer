
namespace StockViewer.MVVM.ViewModel;
public class PickStockBlockViewModel : ObservableObject
{
    public StockModel mStockModel { get; set; }

    public PickStockBlockViewModel(StockModel stockModel)
    {
        mStockModel = stockModel;
    }
}
