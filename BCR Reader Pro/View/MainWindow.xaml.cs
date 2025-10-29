using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BCR_Reader_Pro
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        readonly List<Page> _pages;
        int _i;
        public MainWindow()
        {
            InitializeComponent();
            _pages = new List<Page> {
            new BcrView(),       // Page2
             new UpsView()      // Page1
            // 필요 시 계속 추가
            };

            // 저널(뒤로가기 기록) 비우기
            ContentFrm.Navigated += (_, __) =>
            {
                while (ContentFrm.NavigationService.RemoveBackEntry() != null) { }
            };

            _i = 0;
            ContentFrm.Navigate(_pages[_i]);
            DataContext = new MainViewModel();
        }
        void PrevButton_Click(object s, RoutedEventArgs e)
        {
            if (_i <= 0) return;
            ContentFrm.Navigate(_pages[--_i]);
            UpdateButtons();
        }

        void NextButton_Click(object s, RoutedEventArgs e)
        {
            if (_i >= _pages.Count - 1) return;
            ContentFrm.Navigate(_pages[++_i]);
            UpdateButtons();
        }

        void UpdateButtons()
        {
            PrevButton.IsEnabled = _i > 0;
            NextButton.IsEnabled = _i < _pages.Count - 1;
        }
    }
}