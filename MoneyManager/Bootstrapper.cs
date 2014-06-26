using Microsoft.Practices.Prism.UnityExtensions;
using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Services;
using System.Windows;

using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.UnityExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
 

namespace MoneyManager
{
    public class Bootstrapper : UnityBootstrapper
    {

       
        protected override System.Windows.DependencyObject CreateShell()
        {

            

            
            return Container.Resolve<MainWindow>();
        }


        protected override IModuleCatalog CreateModuleCatalog()
        {
            return new DirectoryModuleCatalog() { ModulePath = @".\" };
        }

        protected override void InitializeShell()
        {
            base.InitializeShell();
            App.Current.MainWindow = (Window)Shell;
            var cats = DAL.Categories;
            App.Current.MainWindow.Show();
        }
    }
}
