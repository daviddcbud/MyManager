using Microsoft.Practices.Prism.PubSubEvents;
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
    /// Interaction logic for RegisterView.xaml
    /// </summary>
    public partial class RegisterView : UserControl
    {
         RegisterVM viewModel;

         IUnityContainer container;
         public RegisterView(RegisterVM vm, IEventAggregator events, IUnityContainer container)
         {
              
            InitializeComponent();
            viewModel = vm;
            DataContext = vm;
            this.container = container;
            events.GetEvent<DetailsEvent>().Subscribe(ShowDetails);
        }
         void ShowDetails(int Id)
         {
             var window = container.Resolve<LineItemsWindow>();
             window.LoadData(Id);
             window.Show();
         }

        

        private void EnterAsTabAutoComplete_LostFocus(object sender, RoutedEventArgs e)
        {
            var bx = sender as EnterAsTabAutoComplete;
            var vm = bx.DataContext as RegisterItem;
            if (vm == null) return;
            vm.Category = vm.Categories.Where(x => x.Name == bx.Text).FirstOrDefault();
        }

        private void txtSearch_KeyUp(object sender, KeyEventArgs e)
        {

        }
    }
}
