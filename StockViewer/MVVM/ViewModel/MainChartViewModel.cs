
using System.Collections.ObjectModel;

using System.Windows.Shapes;

namespace StockViewer.MVVM.ViewModel;
public class MainChartViewModel : ObservableObject
{

    Size? _Canvas;
    IStockModel? _StockModel;
    ObservableCollection<Rectangle> _Rectangles = new();
    public ICommand? MouseUpCommand { get; set; }
    public ICommand? MouseMoveCommand { get; set; }
    public ICommand? SizeChangedCommand { get; set; }
    public ICommand? LoadedCommand { get; set; }
    public ICommand? CoordMouseMoveCommand { get; set; }

    public Size? Canvas { get => _Canvas; set { _Canvas = value; OnPropertyChanged(); } }
    public ObservableCollection<Rectangle> Rectangles { get => _Rectangles; set { _Rectangles = value; OnPropertyChanged(); } }

    public MainChartViewModel(IStockModel stockModel)
    {
        _StockModel = stockModel;

        MouseUpCommand = new RelayCommand<MouseButtonEventArgs>(e => 
        {
        });

        MouseMoveCommand = new RelayCommand<MouseEventArgs>(e =>
        {
        });

        SizeChangedCommand = new RelayCommand<SizeChangedEventArgs>(args =>
        {
        });

        LoadedCommand = new RelayCommand<Grid>(obj =>
        {
        });
    }
}
