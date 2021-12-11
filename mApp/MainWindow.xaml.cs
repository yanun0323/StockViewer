using mApp.View;
using System.Windows.Shapes;
using Color = mApp.Model.Color;

namespace mAPP
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int ChartLine = 10;
        public MainWindow()
        {
            InitializeComponent();
            GenerateMainChart();
        }
        private void GenerateMainChart()
        {
            DateTime updateTime = mainViewModel.Update;



            //Trace.WriteLine($"{h} {ah} {w} {aw}");


            /*Canvas mainChartCanvas = MainChartCanvas;
            var tradingData = (DataContext as MainViewModel)!.DisplayStock!.TradingData;
            int maxVolume = tradingData.Max(x => x.Value.Volume);
            int position_X = 10;
            int count = 0;
            int canvasWidth = 0;
            foreach (var entry in tradingData)
            {
                Rectangle volume = new();
                volume.Style = mainChartCanvas.FindResource("Volume") as Style;
                volume.Height = 100.0 * entry.Value.Volume / maxVolume;
                volume.Width = Volume_Width; 

                mainChartCanvas.Children.Add(volume);
                Canvas.SetRight(volume, (Volume_Offset_X + Volume_Width) * count + position_X);
                Canvas.SetBottom(volume, 0);
                count++;
            }
            canvasWidth = (Volume_Offset_X + Volume_Width) * count + position_X;
            mainChartCanvas.Width = canvasWidth;
            MainChartScroll.ScrollToRightEnd();*/
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Application.Current.MainWindow.WindowState == WindowState.Maximized)
            {
                Application.Current.MainWindow.WindowState = WindowState.Normal;
            }

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (SearchBox.Text.Length > 1)
                OpenSearchPopup((TextBox)sender!);
            else
            CloseSearchPopup((TextBox)sender!);
        }
        private void TextBoxFind_TextInput(object sender, TextCompositionEventArgs e)
        {
        }

        private void SearchBox_GotFocus(object sender, RoutedEventArgs e)
        {
            SearchBox.Text = "";
        }

        private void SearchBox_LostFocus(object sender, RoutedEventArgs e)
        {
            CloseSearchPopup((TextBox)sender!);
            if (SearchBox.Text == "")
                SearchBox.Text = "Search...";
        }

        private void SearchBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                //mainViewModel.Update_DisplayStock(SearchBox.Text);
            }
        }

        private void OpenSearchPopup(TextBox sender)
        {
            if (searchBoxPopupStack == null)
                searchBoxPopupStack = new();

            searchBoxPopupStack.HorizontalAlignment = HorizontalAlignment.Center;
            searchBoxPopupStack.Orientation = Orientation.Vertical;
            
            foreach (var found in mainViewModel.StockList.Where(x => x.Contains(SearchBox.Text)))
            {
                Button stock = new();
                stock.Content = found!;
                stock.Background = Brushes.White;
                stock.ClickMode = ClickMode.Press;
                stock.FontSize = 20;
                stock.HorizontalAlignment = HorizontalAlignment.Center;
                stock.VerticalAlignment = VerticalAlignment.Center;
                stock.Foreground = Brushes.Gray;
                stock.Click += SearchContextMenu_Selected;
                //stock.MouseDown += SearchContextMenu_Selected;
                searchBoxPopupStack.Children.Add(stock);
            }
            searchBoxPopup.IsOpen = true;
        }
        private void CloseSearchPopup(TextBox sender)
        {
            if (searchBoxPopup == null || !searchBoxPopup.IsOpen)
                return;

            searchBoxPopup.IsOpen = false;
            searchBoxPopup.Child = null;
            SearchBox.Text = "";
        }

        private void SearchContextMenu_Selected(object sender, MouseButtonEventArgs e)//MouseButtonEventArgs e)
        {
            Trace.WriteLine($"A");
            Button stock = (Button)sender;
            string id = (string)stock.Content;
            id = id.Split(" ")[0];
            Trace.WriteLine($"{id} {stock.Name} {stock.Name.Split(" ")[0]} {stock.Name.Split(" ")[1]}");
            mainViewModel.Update_DisplayStock(id);

        }
        private void SearchContextMenu_Selected(object sender, RoutedEventArgs e)//MouseButtonEventArgs e)
        {
            Trace.WriteLine($"B");
            Button stock = (Button)sender;
            string id = (string)stock.Content;
            mainViewModel.Update_DisplayStock(id.Split(" ")[0]);

        }


        private double NowChartPos;
        private double StartMouseX;
        private void K_Chart_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                NowChartPos = MainChartScroll.HorizontalOffset;
                StartMouseX = e.GetPosition(MainChartScroll).X;
            }
        }

        private void K_Chart_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var nowX = e.GetPosition(MainChartScroll).X;
                MainChartScroll.ScrollToHorizontalOffset(NowChartPos + StartMouseX - nowX);
            }
        }

        private void Scale_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta > 0)
            {
                Trace.WriteLine("Zoom In");
            }

            else if (e.Delta < 0)
            {
                Trace.WriteLine("Zoom Out");
            }

        }
    }
}
