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

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            var win = new NewSavingsCatWindow();
            
           
                var vm = DataContext as SavingsViewVM;
                vm.LoadData();
            
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            var win = new NewSavingsCatWindow();
            var btn = sender as Button;
            win.Id = int.Parse(btn.Tag.ToString());
            win.Load();
            win.ShowDialog();
            
                var vm = DataContext as SavingsViewVM;
                vm.LoadData();
            
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var id = int.Parse(btn.Tag.ToString());
            var model = new AccountTrackerEntities();
            var savings = model.Accounts.Where(x => x.AccountID == id).Single();
            model.Accounts.Remove(savings);
            model.SaveChanges();
            var vm = DataContext as SavingsViewVM;
            vm.LoadData();
        }

        string formatForCSV(string s)
        {
            s = Convert.ToChar(34) + s + Convert.ToChar(34);
            return s;
        }
        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            var path="savings.csv";
            var writer = new System.IO.StreamWriter(path);
            writer.WriteLine(formatForCSV("Name") + "," + formatForCSV("Balance")  + "," + formatForCSV("Goal") + "," + formatForCSV("Diff"));
            var vm = DataContext as SavingsViewVM;
            foreach (var item in vm.Data)
            {
                writer.WriteLine(formatForCSV(item.Name) + "," + formatForCSV(item.Balance.ToString("N2")) + "," + formatForCSV(item.Goal.ToString("N2")) + "," + formatForCSV(item.Diff.ToString("N2")));
            }
            writer.WriteLine(formatForCSV("Total") + "," + formatForCSV(vm.Data.Sum(x => x.Balance).ToString("N2")) + "," + formatForCSV(vm.Data.Sum(x => x.Goal).ToString("N2")) + "," + formatForCSV(vm.Data.Sum(x => x.Diff).ToString("N2")));
            writer.Close();
            System.Diagnostics.Process.Start(path);

                
        }
    }
}
