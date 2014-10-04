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
                LoadRegisterItem(item);
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
            ShowTotals();

        }

        private void LoadRegisterItem(RegisterLineItem item)
        {
            var newItem = container.Resolve<RegisterItem>();
            newItem.Id = item.ID;
            newItem.Description = item.Description;
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
        public decimal RealTotal { get; set; }
        public decimal CreditCardTotal { get; set; }
        public decimal OutstandingBudget { get; set; }
        void ComputeTotal()
        {

            
            
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
                if (item.Category == null && findIt != null && item.Amount==0)
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
            ShowTotals();
        }

        private void ShowTotals()
        {
            var beginTotal = model.sp_BalanceAsOf(StartDate.AddDays(-1)).FirstOrDefault();
            decimal begin = 0;
            if (beginTotal != null) begin = beginTotal.Value;
            Total = begin + LineItems.Where(x => x.IsCleared == true).Sum(x => x.Amount);
            var noncleared = model.RegisterLineItems.Where(x => x.IsCleared == false).ToList();
            if (noncleared.Count != 0)
            {
                var nonclearedAmount = noncleared.Sum(x => x.Amount);
                RealTotal = Total + nonclearedAmount;
            }
            else
            {
                RealTotal = Total;
            }
            var ccList = model.CreditCardTransactions.Where(x => x.Paid == false).ToList();
            decimal cc = 0;
            if(ccList.Count > 0) cc= ccList.Sum(x => x.Amount);

            CreditCardTotal = cc;
            RealTotal += cc;
            //get remaining unspent budget items
            var fromd = DateTime.Today;
            var endDate = DateTime.Today;
            if (EndDate.Day > 15)
            {
                endDate = DateTime.Parse(EndDate.Month + "/1/" + EndDate.Year).AddMonths(1).AddDays(-1);
                fromd = DateTime.Parse(EndDate.Month + "/16/" + EndDate.Year);
            }
            else
            {
                endDate = DateTime.Parse(EndDate.Month + "/15/" + EndDate.Year);
                fromd = DateTime.Parse(EndDate.Month + "/1/" + EndDate.Year);
            }
            OutstandingBudget = model.sp_OutstandingBudget(fromd, endDate).First().Value;
            RealTotal += OutstandingBudget;
            RealTotal *= -1;
            Total *= -1;
            OnPropertyChanged(() => Total);
            OnPropertyChanged(() => RealTotal);
            OnPropertyChanged(() => CreditCardTotal);
            OnPropertyChanged(() => OutstandingBudget);
        }
        IEventAggregator events;
        IUnityContainer container;
        public DelegateCommand CloseCommand { get; set; }
        
        public DelegateCommand SearchCommand { get; set; }
        public string SearchText { get; set; }
        public RegisterVM(IEventAggregator events, IUnityContainer container)
        {
            this.events = events;
            this.container = container;
            model = DAL.GetModel();
            Loading = true;
            LoadCategories();
            CloseCommand = new DelegateCommand(() => events.GetEvent<CloseTabEvent>().Publish(null));
            SearchCommand = new DelegateCommand(() => Search());
             
            events.GetEvent<RegisterAmountChanged>().Subscribe((x) => ComputeTotal());
            events.GetEvent<RegisterLineAmountChanged>().Subscribe((x) => UpdateAmount(x));
            events.GetEvent<DetailsSaveMe>().Subscribe((x) => SaveLineItem(x));
            events.GetEvent<DeleteMe>().Subscribe((x) => DeleteItem(x));
            StartDate = DateTime.Today.AddDays(-30);
            EndDate = DateTime.Today;
            LoadData();
            Loading = false;
        }
        void Search()
        {
            if (SearchText != "")
            {
                decimal amount = 0;
                if (decimal.TryParse(SearchText, out amount))
                {
                     amount = decimal.Parse(SearchText);
                    var query = model.RegisterLineItems.Include("Category").Where(x => x.Amount == amount).ToList();
                    LineItems.Clear();
                    foreach (var item in query)
                    {
                        LoadRegisterItem(item);
                    }

                }
                else if (SearchText.ToUpper() =="U")
                {
                    var query = model.RegisterLineItems.Include("Category").Where(x => x.IsCleared==false).ToList();
                    LineItems.Clear();
                    foreach (var item in query)
                    {
                        LoadRegisterItem(item);
                    }
                }

            }
             
            else
            {
                //clear filter
                LoadData();
            }
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
        void DeleteItem(RegisterItem item)
        {
            if (item.Id > 0)
            {
                var toRemove = model.RegisterLineItems.Where(x => x.ID == item.Id).Single();
                model.RegisterLineItems.Remove(toRemove);
                model.SaveChanges();
            }
            LineItems.Remove(item);
            ComputeTotal();
            
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
