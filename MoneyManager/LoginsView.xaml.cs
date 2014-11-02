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
    /// Interaction logic for LoginsView.xaml
    /// </summary>
    public partial class LoginsView : UserControl
    {
        private string password;
        public LoginsView(LoginVM vm)
        {
            InitializeComponent();
            DataContext = vm;
        }
        public void Load(string password)
        {
            this.password = password;
            var vm = DataContext as LoginVM;
            vm.Load(password);
        }

        private void TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            var vm = DataContext as LoginVM;
            vm.Filter(txtFilter.Text);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var edit = new EditLogin();
             var button= sender as Button;

             edit.Load(button.Tag as LoginItemVM,this.password);
             edit.ShowDialog();
             var vm = DataContext as LoginVM;
            vm.Load(password);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var edit = new EditLogin();
            
            edit.Load(new LoginItemVM(),this.password );
            edit.ShowDialog();
            var vm = DataContext as LoginVM;
            vm.Load(password);
        }
    }
}
