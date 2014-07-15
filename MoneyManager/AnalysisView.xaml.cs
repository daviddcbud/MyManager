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

namespace MoneyManager
{
    /// <summary>
    /// Interaction logic for AnalysisView.xaml
    /// </summary>
    public partial class AnalysisView : UserControl
    {
        public AnalysisView(AnalysisViewVM vm)
        {
            InitializeComponent();
            DataContext = vm;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var vm=DataContext as AnalysisViewVM;
            var button = sender as Button;
            var lineitem = button.Tag as AnalysisItem;
            var win = new AnalysisDetailsWindow();
            win.Load(lineitem.Category, vm.StartDate, vm.EndDate);
            win.Show();
        }
    }
}
