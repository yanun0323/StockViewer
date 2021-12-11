using mApp.ViewModel;

namespace mApp.View;
/// <summary>
/// Interaction logic for MainChart.xaml
/// </summary>
public partial class MainChart : UserControl
{
    public MainChart()
    {
        InitializeComponent();
        /*Canvas canvas = UserCanvas;
        Label label = new Label();
        label.Content = MainChartStock == null ? "Null" : MainChartStock.Name;
        canvas.Children.Add(label);*/
    }
    public static readonly DependencyProperty StockProperty =
    DependencyProperty.Register(nameof(MainChartStock), typeof(Stock),
    typeof(MainChart));

    public Stock? MainChartStock
    {
        get => (Stock?)this.GetValue(StockProperty);
        set => this.SetValue(StockProperty, value);
    }
}
