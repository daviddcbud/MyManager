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
    /// Interaction logic for NewSavingsCatWindow.xaml
    /// </summary>
    public partial class NewSavingsCatWindow : Window
    {
        public int Id { get; set; }
        public NewSavingsCatWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var model = new AccountTrackerEntities();
            var savings = new Account();
            if (Id > 0)
            {
                savings = model.Accounts.Where(x => x.AccountID == Id).Single();
            }
            else
            {
                model.Accounts.Add(savings);
            }
            savings.Name = txtName.Text;
            savings.IsActive = true;
            savings.Goal = decimal.Parse(Goal.Text);
            savings.SortOrder = 100;
            model.SaveChanges();
            this.Close();
        }

        internal void Load()
        {
            var model = new AccountTrackerEntities();
            var savings = model.Accounts.Where(x => x.AccountID == Id).Single();
            txtName.Text = savings.Name;
            Goal.Text = savings.Goal.Value.ToString();
             


        }
    }
}
