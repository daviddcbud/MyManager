using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Prism.PubSubEvents;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyManager
{
    public class SavingsAccountVM{
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Balance { get; set; }
        public decimal Goal { get; set; }
        public decimal Diff
        {
            get{
                return Goal - Balance;
            }
        }
    }
    public class SavingsViewVM : BindableBase
    {
        public DelegateCommand CloseCommand { get; set; }
        AccountTrackerEntities model;
        public ObservableCollection<SavingsAccountVM> Data { get; set; }
        public decimal Balance
        {
            get
            {
                return Data.Sum(x => x.Balance);
            }
        }
        public SavingsViewVM(IEventAggregator events)
        {
            Data = new ObservableCollection<SavingsAccountVM>();
            model = new AccountTrackerEntities();
            CloseCommand = new DelegateCommand(() => events.GetEvent<CloseTabEvent>().Publish(null));
            LoadData();
        }
        public void LoadData()
        {
            model = new AccountTrackerEntities();
            Data.Clear();
            var list = model.Accounts.Where(x => x.IsActive.Value).OrderBy(x => x.SortOrder).ToList();
            foreach (var item in list)
            {
                var vm = new SavingsAccountVM();
                vm.Id = item.AccountID;
                if(item.Goal.HasValue)vm.Goal = item.Goal.Value ;
                vm.Name = item.Name;
                vm.Balance = item.EntryDetails.Sum(x => x.Amount);
                Data.Add(vm);

            }
            OnPropertyChanged(() => Data);
            OnPropertyChanged(() => Balance);
        }
    }
}
