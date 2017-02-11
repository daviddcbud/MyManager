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
        public ObservableCollection<LoginItemVM> AllLogins { get; set; }
        public DelegateCommand CloseCommand { get; set; }
        public LoginVM(IEventAggregator events)
        {
            Logins = new ObservableCollection<LoginItemVM>();
            AllLogins = new ObservableCollection<LoginItemVM>();
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
            if (System.Configuration.ConfigurationManager.AppSettings["logins"] == "YES")
            {
                LoadServiceBus(password);
                return;
            }
            Logins.Clear();
            AllLogins.Clear();
            using (var model = new LoginTrackerEntities())
            {
                var items = model.UserAccounts.OrderBy(x => x.Description);
                foreach (var item in items)
                {
                    var vm = new LoginItemVM();
                    vm.Id = item.ID;
                    vm.Name = item.Description;
                    vm.URL = item.URL;
                    vm.UserName = item.UserName;
                    vm.Password = EncDec.Decrypt(item.Password, password);
                    Logins.Add(vm);
                    AllLogins.Add(vm);
                }
            }
        }

        private void LoadServiceBus(string password)
        {
            if (string.IsNullOrEmpty(password)) return;
            Logins.Clear();
            AllLogins.Clear();
            using (var ch = ServiceBusUtils.CreateChannel())
            {
                var list = ch.GetLogins(password);
                foreach(var item in list)
                {
                    var vm = new LoginItemVM();
                    
                    vm.Name = item.Description;
                    vm.URL = item.Url;
                    vm.UserName = item.UserName;
                    vm.Password = item.Password;
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

        public int Id { get; set; }
    }
}
