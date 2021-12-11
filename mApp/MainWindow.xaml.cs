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
        //private int ChartLine = 10;

        public MainWindow()
        {
            InitializeComponent();
            GenerateMainChart();
            //GenerateSearchPopup();
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
            string serach = mainViewModel.SearchWords;
            if(serach.Length == 1 && !IsNumber(serach[0]))
                OpenSearchPopup();
            else if (serach.Length > 1 && serach != "Search...")
                OpenSearchPopup();
            else
            CloseSearchPopup();

            static bool IsNumber(char cha) => cha <= '9' && cha >= '0';
        }

        private void SearchBox_GotFocus(object sender, RoutedEventArgs e)
        {
            SearchBox.Foreground = Brushes.DarkGray;
            SearchBox.Text = "";
        }

        private void SearchBox_LostFocus(object sender, RoutedEventArgs e)
        {
            CloseSearchPopup();
            if (mainViewModel.SearchWords == "")
                mainViewModel.SearchWords = "Search...";

            SearchBox.Foreground = Brushes.LightGray;
        }

        private void SearchBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return && searchBoxPopupStack.Children.Count > 0)
            {
                Button button = (Button) searchBoxPopupStack.Children[0];
                string id = (string)button.Content;
                mainViewModel.Update_DisplayStock(id.Split(" ")[0]);
                mainViewModel.SearchWords = "";
            }
        }
        private void OpenSearchPopup()
        {
            if (searchBoxPopup == null || searchBoxPopupStack == null) 
                return;
            searchBoxPopupStack.Children.Clear();
            var found = mainViewModel.StockList.Where(x => x.Contains(SearchBox.Text));
            foreach (var name in found)
            {
                Button button = new();
                button.Content = name!;
                button.Background = Brushes.White;
                button.ClickMode = ClickMode.Press;
                button.FontSize = 20;
                button.HorizontalAlignment = HorizontalAlignment.Center;
                button.VerticalAlignment = VerticalAlignment.Center;
                button.Foreground = Brushes.Gray;
                button.HorizontalContentAlignment = HorizontalAlignment.Left;
                button.BorderThickness = new(0);
                button.Width = searchBoxPopup.Width - 2 * searchBoxPopupBorder.CornerRadius.TopLeft;
                button.Click += SearchResult_Selected;
                searchBoxPopupStack.Children.Add(button);
            }
            if (found.Any())
                searchBoxPopup!.IsOpen = true;
            else
                searchBoxPopup!.IsOpen = false;

            searchBoxPopupScroll.ScrollToTop();
        }
        private void CloseSearchPopup()
        {
            if (searchBoxPopup == null || !searchBoxPopup.IsOpen)
                return;

            searchBoxPopup!.IsOpen = false;
            searchBoxPopupStack.Children.Clear();
            mainViewModel.SearchWords = "";
        }
        private void SearchResult_Selected(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            string id = (string)button.Content;
            mainViewModel.Update_DisplayStock(id.Split(" ")[0]);
            CloseSearchPopup();
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
