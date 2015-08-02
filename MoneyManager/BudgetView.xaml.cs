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
        string formatForCSV(string s)
        {
            s = Convert.ToChar(34) + s + Convert.ToChar(34);
            return s;
        }
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {

            var envItems = new List<EnvItemVm>();
            foreach (var item in viewModel.EnvelopeItems)
            {
                envItems.Add(item);
            }
            var total = viewModel.NewEnvItem();
            total.Amount= viewModel.EnvelopeItems.Sum(x => x.Amount);
            total.Description="TOTAL";
            envItems.Add( total);
            var removeIt = envItems.Where(x => x.Amount == 0).ToList();
            foreach (var r in removeIt) envItems.Remove(r);
            var path = "budget" + viewModel.Month + "-" + viewModel.Year + ".csv";
            var writer = new System.IO.StreamWriter(path);
            writer.WriteLine(formatForCSV("1st Pay Period") + "," + formatForCSV("") + "," + formatForCSV("")
                + "," + formatForCSV("") + "," + formatForCSV("") + "," +
                formatForCSV("") + "," + formatForCSV("2nd Pay Period") + "," 
                 + formatForCSV("") + "," +
                formatForCSV("") + ","  + formatForCSV("Envelopes") + "," + formatForCSV(""));

            for( var i=0;i<= viewModel.BudgetLineItems.Count -1;i++)
            {
                var item=viewModel.BudgetLineItems[i];
                var seconditem="";
                var secondName="";
                if(i<= viewModel.BudgetLineItems2.Count-1){
                    seconditem=(viewModel.BudgetLineItems2[i].Amount*-1).ToString("N2");
                    secondName =viewModel.BudgetLineItems2[i].CategoryName;
                }

                 var envItemName="";
                var envItemAmount="";
                if (i <= envItems.Count - 1)
                {
                    envItemAmount = envItems[i].Amount.ToString("N2");
                    envItemName = envItems[i].Description;
                    if (string.IsNullOrEmpty(envItemName)) envItemAmount = "";
                }
                if (!string.IsNullOrEmpty(item.CategoryName) || !string.IsNullOrEmpty(secondName))
                {
                    writer.WriteLine(formatForCSV((item.Amount*-1).ToString("N2")) + "," +
                        formatForCSV(item.CategoryName) + "," + formatForCSV("")
                    + "," + formatForCSV("") + "," + formatForCSV("") + "," +
                    formatForCSV(seconditem) + "," + formatForCSV(secondName) + "," 
                    + formatForCSV("") + "," +
                formatForCSV("") + ","  + formatForCSV(envItemName) + "," + formatForCSV(envItemAmount)
                    );
                }
            }

            var diff = viewModel.BudgetLineItems2.Count - viewModel.BudgetLineItems.Count;
            if (diff > 0)
            {
                for (int i = viewModel.BudgetLineItems.Count; i <= viewModel.BudgetLineItems2.Count-1; i++)
                {
                    var item=viewModel.BudgetLineItems2[i];
                     var envItemName="";
                var envItemAmount="";
                if (i <= envItems.Count - 1)
                {
                    envItemAmount = envItems[i].Amount.ToString("N2");
                    envItemName = envItems[i].Description;
                    if (string.IsNullOrEmpty(envItemName)) envItemAmount = "";
                }
                    if (!string.IsNullOrEmpty(item.CategoryName))
                    {
                        writer.WriteLine(formatForCSV("") + "," + formatForCSV("") + "," + formatForCSV("")
                        + "," + formatForCSV("") + "," + formatForCSV("") + "," + formatForCSV((item.Amount*-1).ToString("N2")) + "," +
                        formatForCSV(item.CategoryName) + "," 
                          + formatForCSV("") + "," +
                        formatForCSV("") + ","  + formatForCSV(envItemName) + "," + formatForCSV(envItemAmount)
                    );
                    }
                }
            }


            

           


            writer.WriteLine(formatForCSV(viewModel.Total.ToString("N2")) + "," + formatForCSV("Net") + "," + formatForCSV("")
                + "," + formatForCSV("") + "," + formatForCSV("") + "," + formatForCSV(viewModel.Total2.ToString("N2")) + "," +
                formatForCSV("Net"));

            writer.WriteLine();
            writer.WriteLine(",,," + formatForCSV("Monthly Net") + "," + formatForCSV(viewModel.MonthTotal.ToString("N2")));

            writer.Close();
            System.Diagnostics.Process.Start(path);
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            var newcate = new NewCategoryWin();
            newcate.ShowDialog();
            viewModel.Reload();
        }
    }
}
