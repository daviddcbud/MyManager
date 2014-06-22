using MoneyManager.Controls;
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
    /// Interaction logic for CCTransView.xaml
    /// </summary>
    public partial class CCTransView : UserControl
    {
        public CCTransView(CCTransViewVM vm)
        {
            InitializeComponent();
            DataContext = vm;
        }


        private void EnterAsTabAutoComplete_LostFocus(object sender, RoutedEventArgs e)
        {
            var bx = sender as EnterAsTabAutoComplete;
            var vm = bx.DataContext as CCItemVM;
            vm.Category = vm.Categories.Where(x => x.Name == bx.Text).FirstOrDefault();
        }
    }
}
