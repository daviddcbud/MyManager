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
    /// Interaction logic for ChangeWindow.xaml
    /// </summary>
    public partial class ChangeWindow : Window
    {
        AccountTrackerEntities model;
        public ChangeWindow()
        {
            InitializeComponent();
            model = new AccountTrackerEntities();
            var accounts = model.Accounts.Where(x => x.IsActive.Value).OrderBy(x => x.Name).ToList();
            account1.ItemsSource = accounts;
            account1.DisplayMemberPath = "Name";
            account2.ItemsSource = accounts;
            account2.DisplayMemberPath = "Name";
            account3.ItemsSource = accounts;
            account3.DisplayMemberPath = "Name";
            account4.ItemsSource = accounts;
            account4.DisplayMemberPath = "Name";
            account5.ItemsSource = accounts;
            account5.DisplayMemberPath = "Name";
            account6.ItemsSource = accounts;
            account6.DisplayMemberPath = "Name";
            account7.ItemsSource = accounts;
            account7.DisplayMemberPath = "Name";
            amount1.Text = "0";
            amount2.Text = "0";
            amount3.Text = "0";
            amount4.Text = "0";
            amount5.Text = "0";
            amount6.Text = "0";
            amount7.Text = "0";


        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            decimal amount = 0;
            amount = ParseIt(amount1);
            var header = new EntryHeader();
            if (IsDeposit)
            {
                header.TypeofEntry = 0;
            }
            else
            {
                header.TypeofEntry = 1;
            }
            header.Comments = "";
            header.TransDate =DateTime.Today;
            model.EntryHeaders.Add(header);
            if (amount != 0)
            {
                AddDetail(header, amount, ((Account)account1.SelectedItem).AccountID );
            }
            amount = ParseIt(amount2);
            if (amount != 0)
            {
                AddDetail(header, amount, ((Account)account2.SelectedItem).AccountID);
            }

            amount = ParseIt(amount3);
            if (amount != 0)
            {
                AddDetail(header, amount, ((Account)account3.SelectedItem).AccountID);
            }

            amount = ParseIt(amount4);
            if (amount != 0)
            {
                AddDetail(header, amount, ((Account)account4.SelectedItem).AccountID);
            }

            amount = ParseIt(amount5);
            if (amount != 0)
            {
                AddDetail(header, amount, ((Account)account5.SelectedItem).AccountID);
            }
          

            amount = ParseIt(amount6);
            if (amount != 0)
            {
                AddDetail(header, amount, ((Account)account6.SelectedItem).AccountID);
            }

            amount = ParseIt(amount7);
            if (amount != 0)
            {
                AddDetail(header, amount, ((Account)account7.SelectedItem).AccountID);
            }

            model.SaveChanges();
            DialogResult = true;
            Close();
        }
        void AddDetail(EntryHeader header, decimal amount, int id)
        {
            var detail = new EntryDetail();
            header.EntryDetails.Add(detail);
            detail.AccountID = id;
            detail.Amount = amount;
            if (!IsDeposit) detail.Amount *= -1;


        }
        private void amount_TextChanged(object sender, TextChangedEventArgs e)
        {
            decimal total = 0;
            total += ParseIt(amount1);
            total += ParseIt(amount2);
            total += ParseIt(amount3);
            total += ParseIt(amount4);
            total += ParseIt(amount5);
            total += ParseIt(amount6);
            total += ParseIt(amount7);

            txtTotal.Text = total.ToString("N2");
        }
        decimal ParseIt(TextBox txt)
        {
            decimal amount = 0;
            if (decimal.TryParse(txt.Text, out amount))
            {
                return amount;
            }
            return 0;
        }

        public bool IsDeposit { get; set; }
    }
}
