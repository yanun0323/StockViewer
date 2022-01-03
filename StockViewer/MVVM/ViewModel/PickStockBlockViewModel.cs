
namespace StockViewer.MVVM.ViewModel;
public class PickStockBlockViewModel : ObservableObject
{
    private TitleStock? _mStock;

    public TitleStock? mStock
    {
        get { return _mStock; }
        set { _mStock = value;}
    }

    public PickStockBlockViewModel(TitleStock stock)
    {
        mStock = stock;
    }
}
