using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.PubSubEvents;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyManager
{
    public class LoginVM
    {
        public ObservableCollection<LoginItemVM> Logins { get; set; }
        public List<LoginItemVM> AllLogins { get; set; }
        public DelegateCommand CloseCommand { get; set; }
        public LoginVM(IEventAggregator events)
        {
            Logins = new ObservableCollection<LoginItemVM>();
            AllLogins = new List<LoginItemVM>();
            CloseCommand = new DelegateCommand(() => events.GetEvent<CloseTabEvent>().Publish(null));
        }
        public void Filter(string filter)
        {
            Logins.Clear();
            foreach (var item in AllLogins)
            {
                if(item.Name.ToUpper().Contains(filter) || item.URL.Contains(filter))
                {
                    Logins.Add(item);
                }
            }
        }
        public void Load(string password)
        {
            using (var model = new LoginTrackerEntities())
            {
                var items = model.UserAccounts.OrderBy(x => x.Description);
                foreach (var item in items)
                {
                    var vm = new LoginItemVM();
                    vm.Name = item.Description;
                    vm.URL = item.URL;
                    vm.UserName = item.UserName;
                    vm.Password = EncDec.Decrypt(item.Password, password);
                    Logins.Add(vm);
                    AllLogins.Add(vm);
                }
            }
        }
         
    }
    public class LoginItemVM
    {
        public string Name { get; set; }
        public string URL { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
