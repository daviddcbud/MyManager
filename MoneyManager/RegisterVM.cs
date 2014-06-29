using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
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
    public class CloseTabEvent : CompositePresentationEvent<object>
    {
    }
    public class RegisterVM : BindableBase
    {

        bool Loading = false;
        public decimal Total { get; set; }
        DateTime startDate;
        DateTime endDate;
        public DateTime StartDate {
            get{
                return startDate;
            }
            set{
                SetProperty(ref this.startDate,value);
                if (!Loading) LoadData();
            }
        }
         public DateTime EndDate {
            get{
                return endDate;
            }
            set{
                SetProperty(ref this.endDate,value);
                if(!Loading)   LoadData();
            }
        }
        public ObservableCollection<RegisterItem> LineItems { get; set; }
        MoneyManagerEntities model;
        public List<Category> Categories { get; set; }
         

        void LoadData()
        {
            Loading = true;
            LineItems =new  ObservableCollection<RegisterItem>();
            var list = model.RegisterLineItems.Include("Category").Where(x => x.Date >= StartDate && x.Date <= EndDate).OrderBy(x=>x.Date).ToList();
            foreach (var item in list)
            {
                var newItem = container.Resolve<RegisterItem>();
                newItem.Id = item.ID;
                newItem.Description=item.Description;
                newItem.Amount = item.Amount;
                newItem.Date = item.Date;
                newItem.AmountString = item.Amount.ToString("N2");
                newItem.Categories = Categories;
                if (item.CategoryId.HasValue)
                {
                    newItem.Category = Categories.Where(x => x.ID == item.CategoryId).Single();
                    newItem.CategoryName = newItem.Category.Name;
                }
                newItem.IsDirty = false;
                newItem.IsCleared = item.IsCleared;
                LineItems.Add(newItem);
            }
            for (int i = 1; i <= 2; i++)
            {
                var newItem = container.Resolve<RegisterItem>();
                newItem.IsCleared = true;
                newItem.Categories = Categories;
                newItem.Date = DateTime.Today;
                newItem.IsDirty = false;
                LineItems.Add(newItem);
            }
            OnPropertyChanged(() => LineItems);
            ComputeTotal();
            Loading = false;

        }
        public decimal RealTotal { get; set; }
        void ComputeTotal()
        {
            var beginTotal = model.sp_BalanceAsOf(StartDate.AddDays(-1)).FirstOrDefault() ;
            decimal begin = 0;
            if (beginTotal != null) begin = beginTotal.Value;
            Total = begin + LineItems.Where(x=>x.IsCleared==true).Sum(x => x.Amount);
            RealTotal = begin + LineItems.Sum(x => x.Amount);
            var cc = model.CreditCardTransactions.Where(x => x.Paid == false).Sum(x => x.Amount);
            RealTotal += cc;
            RealTotal *= -1;
            Total *= -1;
            OnPropertyChanged(() => Total);
            OnPropertyChanged(() => RealTotal);
            if (Loading) return;

            foreach (var item in LineItems.Where(x => x.IsDirty))
            {
                var findIt = model.RegisterLineItems.Where(x => x.ID == item.Id).FirstOrDefault();
                if (findIt == null && item.Amount == 0) continue;
                if (findIt == null && item.Amount !=0)
                {
                    findIt = new RegisterLineItem();
                     
                    model.RegisterLineItems.Add(findIt);
                }
                if (item.Category == null && findIt != null && findIt.Amount==0)
                {
                    item.Id = 0;
                    
                    model.RegisterLineItems.Remove(findIt);
                    try
                    {
                        model.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }
                    continue;
                }
                if (item.Category!=null)
                {
                    findIt.CategoryId = item.Category.ID;
                }
                else
                {
                    findIt.CategoryId = null;
                }
                findIt.Amount = item.Amount;
                findIt.IsCleared = item.IsCleared.Value;
                findIt.Date = item.Date.Value;
                findIt.Description = item.Description;
                findIt.Notes = item.Description;
                if (findIt.Description == null) findIt.Description = "";
                if (findIt.Notes == null) findIt.Notes = "";
                model.SaveChanges();
                item.IsDirty = false;
                item.Id = findIt.ID;

            }
            if (LineItems.Where(x => x.Id == 0).Count()  < 2)
            {
                Loading = true;
                var newItem = container.Resolve<RegisterItem>();
                newItem.IsCleared = true;
                newItem.Categories = Categories;
                newItem.Date = DateTime.Today;
                newItem.IsDirty = false;
                LineItems.Add(newItem);
                Loading = false;
            }

        }
        IEventAggregator events;
        IUnityContainer container;
        public DelegateCommand CloseCommand { get; set; }
        public RegisterVM(IEventAggregator events, IUnityContainer container)
        {
            this.events = events;
            this.container = container;
            model = DAL.GetModel();
            Loading = true;
            LoadCategories();
            CloseCommand = new DelegateCommand(() => events.GetEvent<CloseTabEvent>().Publish(null));
            events.GetEvent<RegisterAmountChanged>().Subscribe((x) => ComputeTotal());
            events.GetEvent<RegisterLineAmountChanged>().Subscribe((x) => UpdateAmount(x));
            events.GetEvent<DetailsSaveMe>().Subscribe((x) => SaveLineItem(x));
            StartDate = DateTime.Today.AddDays(-30);
            EndDate = DateTime.Today;
            LoadData();
            Loading = false;
        }
        void SaveLineItem(RegisterItem item)
        {
            var newItem =  new RegisterLineItem();

            model.RegisterLineItems.Add(newItem);
            if (item.Category != null)
            {
                newItem.CategoryId = item.Category.ID;
            }
            else
            {
                newItem.CategoryId = null;
            }
            newItem.Amount = item.Amount;
            newItem.IsCleared = item.IsCleared.Value;
            newItem.Date = item.Date.Value;
            newItem.Description = "";
            newItem.Notes = item.Description;
            if (newItem.Description == null) newItem.Description = "";
            if (newItem.Notes == null) newItem.Notes = "";
            model.SaveChanges();
            item.IsDirty = false;
            item.Id = newItem.ID;
        }
        void UpdateAmount(RegisterLineItem header)
        {
            var item = LineItems.Where(x => x.Id == header.ID).Single();
            item.Amount = header.Amount;
        }
        void LoadCategories()
        {

            Categories = DAL.Categories;
        }
    }
}
