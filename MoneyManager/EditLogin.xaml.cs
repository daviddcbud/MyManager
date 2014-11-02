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
    /// Interaction logic for EditLogin.xaml
    /// </summary>
    public partial class EditLogin : Window
    {
        private string password;
        public EditLogin()
        {
            InitializeComponent();
        }

        public void Load(LoginItemVM edit, string password)
        {
            this.password = password;
            DataContext = edit;

        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            using (var model = new LoginTrackerEntities())
            {
                var vm = DataContext as LoginItemVM;
                var editor = new UserAccount();
                
                 
                if (vm.Id == 0)
                {
                    editor.UserID = 1;
                    model.UserAccounts.Add(editor);
                }
                else
                {
                    editor = model.UserAccounts.Where(x => x.ID == vm.Id).Single();
                }
                editor.URL = vm.URL;
                editor.Description = vm.Name;
                editor.UserName = vm.UserName;
                editor.Password = EncDec.Encrypt(vm.Password, this.password);
                model.SaveChanges();
            }
            Close();
        }
    }
}
