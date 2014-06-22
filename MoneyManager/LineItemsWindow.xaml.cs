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
using System.Windows.Shapes;

namespace MoneyManager
{
    /// <summary>
    /// Interaction logic for LineItemsWindow.xaml
    /// </summary>
    public partial class LineItemsWindow : Window
    {
        public LineItemsWindow(LineItemsViewVM vm)
        {
            InitializeComponent();
            DataContext = vm;
        }

        internal void LoadData(int Id)
        {
            var vm = DataContext as LineItemsViewVM;
            vm.Load(Id);
             
        }
        private void EnterAsTabAutoComplete_LostFocus(object sender, RoutedEventArgs e)
        {
            var bx = sender as EnterAsTabAutoComplete;
            var vm = bx.DataContext as RegisterLineItemDetailVM;
            vm.Category = vm.Categories.Where(x => x.Name == bx.Text).FirstOrDefault();
        }
    }
}
