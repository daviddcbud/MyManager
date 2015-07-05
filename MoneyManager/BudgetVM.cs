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
using System.Windows.Media;

namespace MoneyManager
{
   public class BudgetVM:BindableBase
    {
        
       bool Loading = false;
        public decimal Total { get; set; }
        public decimal Total2 { get; set; }
       public ObservableCollection<BudgetItemVM> BudgetLineItems { get; set; }
       public ObservableCollection<BudgetItemVM> BudgetLineItems2 { get; set; }
       public ObservableCollection<EnvItemVm> EnvelopeItems { get; set; }
        MoneyManagerEntities model;
        public List<Category> Categories {get;set;}
        
        int month =0;
        int year = 0;
        public int Month
        {
            get
            {
                return month;
            }
            set
            {
                SetProperty(ref this.month, value);
                LoadBudgetItems();

            }
        }
        public int Year
        {
            get
            {
                return year;
            }
            set
            {
                SetProperty(ref this.year, value);
                LoadBudgetItems();
            }
        }

        void PostSaving(DateTime date)
        {
            var catid = model.Categories.Where(x => x.Name == "SAVINGS DEPOSIT").Single().ID;
             
            decimal total = 0;
            foreach (var item in BudgetLineItems)
            {
                if (item.IsSavings)
                {
                    total += item.Amount;
                }

            }
            var register = new RegisterLineItem();
            register.Amount = total;
            register.Date = date;
            register.CategoryId = catid;
            register.Description = "";
            register.Notes = "";
            register.IsCleared = false;
            model.RegisterLineItems.Add(register);
            model.SaveChanges();
        }
        public void PostSavings()
        {
            PostSaving(firstDate.StartDate);
            PostSaving(secondDate.StartDate);

        }
        void PostRegisterItems(DateTime date, ObservableCollection<BudgetItemVM> lineitems)
        {
            foreach (var item in lineitems)
            {
                if (item.Post)
                {
                    var register = new RegisterLineItem();
                    register.Amount = item.Amount;
                    register.Date = date;
                    register.CategoryId = item.Category.ID;

                    register.Description = "";
                    register.Notes = "";
                    register.IsCleared = false;
                    model.RegisterLineItems.Add(register);
                }

            }

            model.SaveChanges();
        }
        public void PostRegister()
        {
            PostRegisterItems(firstDate.StartDate, BudgetLineItems);
            PostRegisterItems(secondDate.StartDate, BudgetLineItems2);
            
        }
        void LoadBudgetItems()
        {
            if (month == 0 || year == 0) return;
            Loading = true;
            BudgetLineItems = new ObservableCollection<BudgetItemVM>();
            BudgetLineItems2 = new ObservableCollection<BudgetItemVM>();
            var monthdate=DateTime.Parse(month + "/1/" + year);
            firstDate = model.BudgetHeaders.Where(x => x.StartDate == monthdate).SingleOrDefault();
            if (firstDate == null)
            {
                firstDate = AddDate(month, 1, year);
            }
            if (firstDate != null)
            {
                var list = model.BudgetLineItems.Include("Category").Where(x => x.BudgetHeaderId == firstDate.ID).ToList();
                foreach (var item in list)
                {
                    var newItem = container.Resolve<BudgetItemVM>();
                    newItem.Id = item.ID;
                    if (item.IsSavings.HasValue) newItem.IsSavings = item.IsSavings.Value;
                    newItem.Description = item.Description;
                    newItem.Amount = item.Amount;
                    newItem.AmountString = item.Amount.ToString("N2");
                    newItem.Categories = Categories;
                    newItem.Category = Categories.Where(x => x.ID == item.CategoryId).Single();
                    newItem.CategoryName = newItem.Category.Name;
                    if (item.PostToRegister.HasValue) newItem.Post = item.PostToRegister.Value;
                    newItem.IsDirty = false;

                    if (!item.IsSavings.Value)
                    {
                        decimal spent = 0;
                        var register = model.RegisterLineItems.Where(x => x.Date <= firstDate.EndDate && x.Date >= firstDate.StartDate
                            && x.CategoryId == item.CategoryId).ToList();
                        if (register.Count > 0) spent = register.Sum(x => x.Amount);

                        var cc = model.CreditCardTransactions.Where(x => x.Date <= firstDate.EndDate && x.Date >= firstDate.StartDate
                            && x.CategoryId == item.CategoryId).ToList();
                        if (cc.Count > 0) spent += cc.Sum(x => x.Amount);



                        newItem.Balance = newItem.Amount - spent;
                    }
                    BudgetLineItems.Add(newItem);
                }
            }
            for (int i = BudgetLineItems.Count; i < 50; i++)
            {
                var newItem = container.Resolve<BudgetItemVM>();
                 newItem.Categories = Categories;
                 newItem.IsDirty = false;
                 BudgetLineItems.Add(newItem);
            }
            

            LoadBudgetItems2();
            OnPropertyChanged(() => BudgetLineItems);
            OnPropertyChanged(() => BudgetLineItems2);
            ComputeTotal();
            LoadEnvelopes();
            Loading = false;
            

        }
        void LoadEnvelopes()
        {
            EnvelopeItems = new ObservableCollection<EnvItemVm>();
            OnPropertyChanged(() => EnvelopeItems);
            var model = new MoneyManagerEntities();
            var year=Year;
            var month=Month;
            var items = model.Envelopes.Where(x => x.Year == year && x.Month == month).ToList();
            foreach (var item in items)
            {
                var VM = container.Resolve<EnvItemVm>();
                VM.Id = item.Id;
                VM.Amount = item.Amount;
                VM.AmountString = item.Amount.ToString("N2");
                VM.Description = item.Description;
                EnvelopeItems.Add(VM);
            }
            for (int i = EnvelopeItems.Count; i < 20; i++)
            {
                var newItem = container.Resolve<EnvItemVm>();
                
                newItem.IsDirty = false;
                EnvelopeItems.Add(newItem);
            }

            ComputeEnvelopeTotal();
        }

        private void ComputeEnvelopeTotal()
        {
            EnvTotal = EnvelopeItems.Sum(x => x.Amount);
            OnPropertyChanged(() => EnvTotal);
        }

        private void LoadBudgetItems2()
        {
            BudgetLineItems2 = new ObservableCollection<BudgetItemVM>();
            var monthdate = DateTime.Parse(month + "/16/" + year);

            secondDate = model.BudgetHeaders.Where(x => x.StartDate == monthdate).OrderBy(x => x.StartDate).SingleOrDefault();
            if (secondDate == null)
            {
                secondDate = AddDate(month, 1, year);
            }
            if (secondDate != null)
            {
                var list = model.BudgetLineItems.Include("BudgetHeader").Include("Category").Where(x => x.BudgetHeaderId == secondDate.ID).ToList();

                foreach (var item in list)
                {
                    var newItem = container.Resolve<BudgetItemVM>();
                    newItem.Id = item.ID;
                    if (item.IsSavings.HasValue) newItem.IsSavings = item.IsSavings.Value;
                    newItem.Description = item.Description;
                    newItem.Amount = item.Amount;
                    newItem.AmountString = item.Amount.ToString("N2");
                    newItem.Categories = Categories;
                    newItem.Category = Categories.Where(x => x.ID == item.CategoryId).Single();
                    newItem.CategoryName = newItem.Category.Name;
                    if (item.PostToRegister.HasValue) newItem.Post = item.PostToRegister.Value;
                    newItem.IsDirty = false;

                    if (!item.IsSavings.Value)
                    {
                        decimal spent = 0;
                        var register = model.RegisterLineItems.Where(x => x.Date <= secondDate.EndDate && x.Date >= secondDate.StartDate
                            && x.CategoryId == item.CategoryId).ToList();
                        if (register.Count > 0) spent = register.Sum(x => x.Amount);

                        var cc = model.CreditCardTransactions.Where(x => x.Date <= secondDate.EndDate && x.Date >= secondDate.StartDate
                            && x.CategoryId == item.CategoryId).ToList();
                        if (cc.Count > 0) spent += cc.Sum(x => x.Amount);



                        newItem.Balance = newItem.Amount - spent;
                    }
                    BudgetLineItems2.Add(newItem);
                }
            }
            for (int i = BudgetLineItems2.Count; i < 50; i++)
            {
                var newItem = container.Resolve<BudgetItemVM>();
                newItem.Categories = Categories;
                newItem.IsDirty = false;
                BudgetLineItems2.Add(newItem);
            }
            OnPropertyChanged(() => BudgetLineItems2);
        }

        private BudgetHeader AddDate(int month, int day, int year)
        {
            var budgetheader = new BudgetHeader();
            budgetheader.StartDate = DateTime.Parse(month + "/" + day + "/" + year);
            if (day == 1)
            {
                budgetheader.EndDate = DateTime.Parse(month + "/15/" + year);
            }
            else
            {
                budgetheader.EndDate = budgetheader.StartDate.AddMonths(1).Subtract(new TimeSpan(1, 0, 0, 0));
            }
            model.BudgetHeaders.Add(budgetheader);
            model.SaveChanges();
            return budgetheader;
        }


        public decimal SavingsTotal { get; set; }
        void ComputeTotal()
        {
            Total = BudgetLineItems.Sum(x => x.Amount) * -1;
            Total2 = BudgetLineItems2.Sum(x => x.Amount) * -1;
            MonthTotal = Total + Total2;
            SavingsTotal = BudgetLineItems.Where(x => x.IsSavings).Sum(x => x.Amount);
            SavingsTotal2 = BudgetLineItems2.Where(x => x.IsSavings).Sum(x => x.Amount);
            MonthSavingsTotal = SavingsTotal + SavingsTotal2;
            OnPropertyChanged(() => Total);
            OnPropertyChanged(() => Total2);
            OnPropertyChanged(() => SavingsTotal);
            OnPropertyChanged(() => SavingsTotal2);
            OnPropertyChanged(() => MonthSavingsTotal);
            OnPropertyChanged(() => MonthTotal);
            Color1 = new SolidColorBrush(Colors.Green);
            Color2 = new SolidColorBrush(Colors.Green);
            MonthColor = new SolidColorBrush(Colors.Green);
            if (Total < 0) Color1 = new SolidColorBrush(Colors.Red);
            if (Total2 < 0) Color2 =new SolidColorBrush(Colors.Red);
            if (MonthTotal < 0) MonthColor = new SolidColorBrush(Colors.Red);
            OnPropertyChanged(() => Color1);
            OnPropertyChanged(() => Color2);
            OnPropertyChanged(() => MonthColor);

            if (Loading) return;
            SaveLinesItems(firstDate, BudgetLineItems);
            SaveLinesItems(secondDate, BudgetLineItems2);
            SaveEnvelope();
            ComputeEnvelopeTotal();
        }
        void SaveEnvelope()
        {
            foreach(var item in EnvelopeItems.Where(x=>x.IsDirty))
            {
                var findIt = model.Envelopes.Where(x => x.Id == item.Id).FirstOrDefault();
                if (findIt == null && item.Amount == 0) continue;
                if (findIt == null)
                {
                    findIt = new Envelope();
                    findIt.Month = month;
                    findIt.Year = year;
                    findIt.Description = item.Description;
                    findIt.Amount = item.Amount;
                    model.Envelopes.Add(findIt);
                }
                if  (findIt != null && string.IsNullOrEmpty(findIt.Description) && findIt.Amount==0)
                {
                    item.Id = 0;
                    model.Envelopes.Remove(findIt);
                    model.SaveChanges();
                    continue;
                }
                 
                
                findIt.Amount = item.Amount;
                findIt.Description = item.Description;
                if (findIt.Description == null) findIt.Description = "";
                model.SaveChanges();
                item.IsDirty = false;
                item.Id = findIt.Id;

            }
        }

        private void SaveLinesItems(BudgetHeader date, ObservableCollection<BudgetItemVM> lineitems)
        {
            foreach (var item in lineitems.Where(x => x.IsDirty))
            {
                var findIt = model.BudgetLineItems.Where(x => x.ID == item.Id).FirstOrDefault();
                if (findIt == null && item.Amount == 0) continue;
                if (findIt == null && item.Category != null)
                {
                    findIt = new BudgetLineItem();
                    findIt.BudgetHeaderId = date.ID;
                    model.BudgetLineItems.Add(findIt);
                }
                if (item.Category == null && findIt != null)
                {
                    item.Id = 0;
                    model.BudgetLineItems.Remove(findIt);
                    model.SaveChanges();
                    continue;
                }
                if (item.Category == null) continue;
                findIt.IsSavings = item.IsSavings;
                findIt.PostToRegister = item.Post;
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
        private BudgetHeader firstDate;
        private BudgetHeader secondDate;
        public DelegateCommand CloseCommand { get; set; }
        public  BudgetVM(IEventAggregator events,IUnityContainer container)
        {
            this.events = events;
            this.container = container;
            model = DAL.GetModel();
            LoadCategories();
             
            events.GetEvent<BudgetAmountChanged>().Subscribe((x) => ComputeTotal());
            CloseCommand = new DelegateCommand(() => events.GetEvent<CloseTabEvent>().Publish(null));
            Month = DateTime.Today.Month;
            Year = DateTime.Today.Year;
        }
        public void Reload()
        {
            
            Categories = DAL.Categories;
            OnPropertyChanged(() => Categories);
            LoadBudgetItems();
        }
        void LoadCategories()
        {

            Categories = DAL.Categories;
        }

        public decimal SavingsTotal2 { get; set; }

        public decimal MonthTotal { get; set; }

        public decimal MonthSavingsTotal { get; set; }

        public SolidColorBrush Color1 { get; set; }

        public SolidColorBrush Color2 { get; set; }

        public SolidColorBrush MonthColor { get; set; }

        public decimal EnvTotal { get; set; }

        internal EnvItemVm NewEnvItem()
        {
            return container.Resolve<EnvItemVm>();
        }
    }
}
