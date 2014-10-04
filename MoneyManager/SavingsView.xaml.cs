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
    /// Interaction logic for SavingsView.xaml
    /// </summary>
    public partial class SavingsView : UserControl
    {
        public SavingsView(SavingsViewVM vm)
        {
            InitializeComponent();
            DataContext = vm;
           
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var win = new TransferWindow();
            if (win.ShowDialog().Value)
            {
                var vm = DataContext as SavingsViewVM;
                vm.LoadData();
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var win = new ChangeWindow();
            win.IsDeposit = true;
            if (win.ShowDialog().Value)
            {
                var vm = DataContext as SavingsViewVM;
                vm.LoadData();
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            var win = new ChangeWindow();
            win.IsDeposit = false;
            if (win.ShowDialog().Value)
            {
                var vm = DataContext as SavingsViewVM;
                vm.LoadData();
            }
        }
    }
}
