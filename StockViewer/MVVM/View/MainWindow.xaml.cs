using System.Threading.Tasks;
using System.Windows.Shapes;
using Color = StockViewer.Library.iColor;

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
            else if (serach.Length > 1)
                OpenSearchPopup();
            else
            CloseSearchPopup();

            static bool IsNumber(char cha) => cha <= '9' && cha >= '0';
        }

        private void SearchBox_LostFocus(object sender, RoutedEventArgs e)
        {
            CloseSearchPopup();
        }

        private void SearchBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return && searchBoxPopupStack.Children.Count > 0)
            {
                Button button = (Button) searchBoxPopupStack.Children[0];
                string id = (string)button.Content;
                mainViewModel.UpdateStock(id.Split(" ")[0]);
                CloseSearchPopup();
            }
        }
        private void OpenSearchPopup()
        {
            if (searchBoxPopup == null || searchBoxPopupStack == null) 
                return;
            searchBoxPopupStack.Children.Clear();

            string target = SearchBox.Text;
            IEnumerable<string> found = mainViewModel.StockList.ContainsKey(target[0]) ? mainViewModel.StockList[target[0]] : new();
            List<char> span = new();

            Trace.WriteLine($"Search");
            for (int i = 0; i < target.Length; i++)
            {
                span.Add(target[i]);
                if (i == 0 || !found.Any())
                    continue;
                string temp = string.Join("", span);
                found = found.Where(x => x.Contains(temp));
            }
            Trace.WriteLine($"Search found: {found.Count()}");
            Trace.WriteLine("Create Button");
            foreach (string idName in found)
            {
                Button button = new();
                button.Content = idName;
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
            mainViewModel.UpdateStock(id.Split(" ")[0]);
            CloseSearchPopup();
        }

        private void SearchBoxGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {

        }
    }
}
