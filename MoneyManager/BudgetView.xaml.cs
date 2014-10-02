using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Unity;
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
    /// Interaction logic for BudgetView.xaml
    /// </summary>
    public partial class BudgetView : UserControl
    {
        BudgetVM viewModel;
        public BudgetView(BudgetVM vm)
        {
            InitializeComponent();
            viewModel = vm;
            DataContext = vm;
        }

        

        private void EnterAsTabAutoComplete_LostFocus(object sender, RoutedEventArgs e)
        {
            var bx = sender as EnterAsTabAutoComplete;
            var vm = bx.DataContext as BudgetItemVM;
            vm.Category = vm.Categories.Where(x => x.Name == bx.Text).FirstOrDefault();
        }
        void PostRegister()
        {
            viewModel.PostRegister();
        }
        void PostSavings()
        {
            viewModel.PostSavings();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            PostSavings();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            PostRegister();
        }
    }
}
