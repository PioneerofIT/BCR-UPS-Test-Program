using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BCR_Reader_Pro.ViewModel;

namespace BCR_Reader_Pro.View
{
    /// <summary>
    /// AutoRunView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class AutoRunView : Page
    {
        public AutoRunView()
        {
            InitializeComponent();
            DataContext = new AutoRunViewModel();
        }
    }
}
