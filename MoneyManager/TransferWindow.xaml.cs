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
    /// Interaction logic for TransferWindow.xaml
    /// </summary>
    public partial class TransferWindow : Window
    {
        AccountTrackerEntities model;
        public TransferWindow()
        {
            InitializeComponent();
            model = new AccountTrackerEntities();
            var accounts = model.Accounts.Where(x => x.IsActive.Value).OrderBy(x => x.Name).ToList();
            from.ItemsSource = accounts;
            from.DisplayMemberPath = "Name";
            to.ItemsSource = accounts;
            to.DisplayMemberPath = "Name";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var header = new EntryHeader();
            header.TypeofEntry = 2;
            header.Comments = "";
            header.TransDate = DateTime.Today;
            var detail = new EntryDetail();
            detail.AccountID = ((Account)from.SelectedItem).AccountID;
            var damount=decimal.Parse(amount.Text);
            detail.Amount = damount * -1;
            header.EntryDetails.Add(detail);

            detail = new EntryDetail();
            detail.AccountID = ((Account)to.SelectedItem).AccountID;
            detail.Amount = damount;
            header.EntryDetails.Add(detail);
            model.EntryHeaders.Add(header);
            model.SaveChanges();
            DialogResult = true;
            Close();
        }
    }
}
