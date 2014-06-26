using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Prism.PubSubEvents;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyManager
{
   public class BudgetVM:BindableBase
    {
        
       bool Loading = false;
        public decimal Total { get; set; }
       public ObservableCollection<BudgetItemVM> BudgetLineItems { get; set; }
        MoneyManagerEntities model;
        public List<Category> Categories {get;set;}
         
        public List<BudgetHeader> Dates { get; set; }
        BudgetHeader selectedDate;
        public BudgetHeader SelectedDate
        {
            get
            {
                return selectedDate;
            }
            set
            {
                SetProperty(ref this.selectedDate, value);
                LoadBudgetItems();
            }
        }
        
        void LoadBudgetItems()
        {
            Loading = true;
            BudgetLineItems = new ObservableCollection<BudgetItemVM>();
            var list = model.BudgetLineItems.Include("Category").Where(x => x.BudgetHeaderId == SelectedDate.ID).ToList();
            foreach (var item in list)
            {
                var newItem = container.Resolve<BudgetItemVM>();
                newItem.Id = item.ID;
                if(item.IsSavings.HasValue)  newItem.IsSavings = item.IsSavings.Value;
                newItem.Description = item.Description;
                newItem.Amount = item.Amount;
                newItem.AmountString = item.Amount.ToString("N2");
                newItem.Categories = Categories;
                newItem.Category = Categories.Where(x => x.ID == item.CategoryId).Single();
                newItem.CategoryName = newItem.Category.Name;
                newItem.IsDirty = false;
                BudgetLineItems.Add(newItem);
            }
            for (int i = BudgetLineItems.Count; i < 50; i++)
            {
                var newItem = container.Resolve<BudgetItemVM>();
                 newItem.Categories = Categories;
                 newItem.IsDirty = false;
                 BudgetLineItems.Add(newItem);
            }
            OnPropertyChanged(() => BudgetLineItems);
            ComputeTotal();
            Loading = false;

        }


        public decimal SavingsTotal { get; set; }
        void ComputeTotal()
        {
            Total = BudgetLineItems.Sum(x => x.Amount);
            SavingsTotal = BudgetLineItems.Where(x => x.IsSavings).Sum(x => x.Amount);
            OnPropertyChanged(() => Total);
            OnPropertyChanged(() => SavingsTotal);
            if (Loading) return;
            foreach (var item in BudgetLineItems.Where(x=>x.IsDirty))
            {
                var findIt = model.BudgetLineItems.Where(x => x.ID == item.Id).FirstOrDefault();
                if (findIt == null && item.Amount == 0) continue;
                if (findIt == null && item.Category !=null)
                 {
                    findIt = new BudgetLineItem();
                    findIt.BudgetHeaderId = selectedDate.ID;
                    model.BudgetLineItems.Add(findIt);
                }
                if (item.Category == null && findIt !=null)
                {
                    item.Id = 0;
                    model.BudgetLineItems.Remove(findIt);
                    model.SaveChanges();

                    continue;
                }
                if (item.Category == null) continue;
                findIt.IsSavings = item.IsSavings;
                findIt.CategoryId = item.Category.ID;
                findIt.Amount = item.Amount;
                findIt.Description = item.Description;
                if (findIt.Description == null) findIt.Description = "";
                model.SaveChanges();
                item.IsDirty = false;
                item.Id = findIt.ID;
                
            }
        }
        IEventAggregator events;
        IUnityContainer container;
        public DelegateCommand CloseCommand { get; set; }
        public  BudgetVM(IEventAggregator events,IUnityContainer container)
        {
            this.events = events;
            this.container = container;
            model = DAL.GetModel();
            LoadCategories();
            LoadDates();
            events.GetEvent<BudgetAmountChanged>().Subscribe((x) => ComputeTotal());
            CloseCommand = new DelegateCommand(() => events.GetEvent<CloseTabEvent>().Publish(null));
             
        }
        void LoadDates()
        {
            Dates = new List<BudgetHeader>();
            var query = model.BudgetHeaders.OrderByDescending(x => x.StartDate);
            foreach (var item in query)
            {
                Dates.Add(item);
            }
            var mostrecentdate = Dates.Where(x => x.StartDate <= DateTime.Today && x.EndDate >= DateTime.Today).Single();
            SelectedDate = mostrecentdate;

        }
        void LoadCategories()
        {

            Categories = DAL.Categories;
        }
    }
}
