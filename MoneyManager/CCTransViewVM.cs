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
   public class CCTransViewVM: BindableBase
    {

       public DelegateCommand PayCommand { get; set; }
        public DelegateCommand SelectAllCommand { get; set; }
        public DelegateCommand CloseCommand { get; set; }
       bool Loading = false;
        public decimal Total { get; set; }
       public ObservableCollection<CCItemVM> LineItems { get; set; }
        MoneyManagerEntities model;
        public List<Category> Categories {get;set;}
         
         
        void LoadItems()
        {
            Loading = true;
            LineItems = new ObservableCollection<CCItemVM>();
            var list = model.CreditCardTransactions.Include("Category").Where(x => x.Paid ==false).OrderBy(x=>x.Date).ToList();
            foreach (var item in list)
            {
                var newItem = container.Resolve<CCItemVM>();
                newItem.Id = item.ID;
                newItem.IsPaid = false;
                newItem.Description = item.Notes;
                newItem.Amount = item.Amount;
                newItem.Date = item.Date;
                newItem.AmountString = item.Amount.ToString("N2");
                newItem.Categories = Categories;
                newItem.Category = Categories.Where(x => x.ID == item.CategoryId).Single();
                newItem.CategoryName = newItem.Category.Name;
                newItem.IsDirty = false;
                LineItems.Add(newItem);
            }
            for (int i = 1; i < 2; i++)
            {
                var newItem = container.Resolve<CCItemVM>();
                 newItem.Categories = Categories;
                 newItem.Date = DateTime.Today;
                 newItem.IsDirty = false;
                 LineItems.Add(newItem);
            }
            OnPropertyChanged(() => LineItems);
            ComputeTotal();
            Loading = false;

        }

        public decimal TotalToPay { get; set; }
        void ComputeTotal()
        {
            TotalToPay = LineItems.Where(x=>x.IsPaid).Sum(x => x.Amount);
            Total = LineItems.Sum(x => x.Amount);
            OnPropertyChanged(() => Total);
            OnPropertyChanged(() => TotalToPay);
            if (Loading) return;
            foreach (var item in  LineItems.Where(x=>x.IsDirty))
            {
                var findIt = model.CreditCardTransactions.Where(x => x.ID == item.Id).FirstOrDefault();
                if (findIt == null && item.Amount == 0) continue;
                if (findIt == null && item.Category !=null)
                 {
                    findIt = new CreditCardTransaction();
                     
                    model.CreditCardTransactions.Add(findIt);
                }
                if (item.Category == null && findIt !=null)
                {
                    item.Id = 0;
                    model.CreditCardTransactions.Remove(findIt);
                    model.SaveChanges();

                    continue;
                }
                 
                if (item.Category == null) continue;
                findIt.CategoryId = item.Category.ID;
                findIt.Date = item.Date.Value;
                findIt.Amount = item.Amount;
                findIt.Notes = item.Description;
                if (findIt.Notes == null) findIt.Notes = "";
                model.SaveChanges();
                item.IsDirty = false;
                item.Id = findIt.ID;
                
            }

            if (LineItems.Where(x => x.Id == 0).Count() < 2)
            {
                Loading = true;
                var newItem = container.Resolve<CCItemVM>();
                 
                newItem.Categories = Categories;
                newItem.Date = DateTime.Today;
                newItem.IsDirty = false;
                LineItems.Add(newItem);
                Loading = false;
            }
            
        }
        IEventAggregator events;
        IUnityContainer container;
        public  CCTransViewVM(IEventAggregator events,IUnityContainer container)
        {
            this.events = events;
            this.container = container;
            model = DAL.GetModel();
            LoadCategories();
            LoadItems();
            events.GetEvent<CCAmountChanged>().Subscribe((x) => ComputeTotal());
            PayCommand = new DelegateCommand(Pay);
            SelectAllCommand = new DelegateCommand(() =>
              {
                  Loading = true;
                  foreach (var l in LineItems) l.IsPaid = true;
                  Loading = false;
                  ComputeTotal();
              });
            CloseCommand = new DelegateCommand(() => events.GetEvent<CloseTabEvent>().Publish(null));

        }
        void Pay()
        {
            var items = LineItems.Where(x => x.IsPaid);
            var newItem = new RegisterLineItem();
            newItem.Date = DateTime.Today;
            newItem.Amount = items.Sum(x => x.Amount);
            newItem.Description = "CC PAYMENT";
            newItem.Notes = "CC PAYMENT";
            model.RegisterLineItems.Add(newItem);
            foreach (var item in items)
            {
                var detail = new RegisterLineItemDetail();
                detail.Amount = item.Amount;
                detail.CategoryId = item.Category.ID;
                detail.Note = item.Description;
                newItem.RegisterLineItemDetails.Add(detail);
                var trans = model.CreditCardTransactions.Where(x => x.ID == item.Id).Single();
                trans.Paid = true;
            }
            model.SaveChanges();
            events.GetEvent<CloseTabEvent>().Publish(null);
        }
        void LoadCategories()
        {

            Categories = DAL.Categories;
        }
    }
}
