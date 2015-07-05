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
    /// Interaction logic for NewCategoryWin.xaml
    /// </summary>
    public partial class NewCategoryWin : Window
    {
        public NewCategoryWin()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var model = new MoneyManagerEntities();
            var categ = new Category();
            categ.Name = txtname.Text.ToUpper();
            model.Categories.Add(categ);
            model.SaveChanges();
             
            DAL.Categories.Add(categ);

            this.Close();
        }
    }
}
