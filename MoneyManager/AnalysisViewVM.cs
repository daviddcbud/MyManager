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
    public class AnalysisItem
    {
        public string Category { get; set; }
        public decimal BudgetAmount { get; set; }
        public decimal ActualAmount { get; set; }
        public decimal Excess
        {
            get
            {
                return BudgetAmount - ActualAmount;
            }
        }
    }
    public class AnalysisViewVM:BindableBase
    {
        public ObservableCollection<AnalysisItem> LineItems { get; set; }
        MoneyManagerEntities model;
         IEventAggregator events;
        IUnityContainer container;
        DateTime startDate;
        DateTime endDate;
        bool Loading = false;
        public DateTime StartDate
        {
            get
            {
                return startDate;
            }
            set
            {
                SetProperty(ref this.startDate, value);
                if (!Loading) LoadData();
            }
        }
        public DateTime EndDate
        {
            get
            {
                return endDate;
            }
            set
            {
                SetProperty(ref this.endDate, value);
                if (!Loading) LoadData();
            }
        }
        public DelegateCommand CloseCommand { get; set; }
        public AnalysisViewVM(IEventAggregator events, IUnityContainer container)
        {
            this.events = events;
            this.container = container;
            model = DAL.GetModel();
            Loading = true;
            LineItems = new ObservableCollection<AnalysisItem>();
            StartDate = DateTime.Parse(DateTime.Today.Month + "/1/" + DateTime.Today.Year);
            var nextMonth = DateTime.Now.AddMonths(1);
            EndDate = DateTime.Parse(nextMonth.Month + "/1/" + nextMonth.Year);
            nextMonth = nextMonth.AddDays(-1);
            LoadData();
            Loading = false;
            CloseCommand = new DelegateCommand(() => events.GetEvent<CloseTabEvent>().Publish(null));
             
        }
        void LoadData()
        {
            LineItems.Clear();
            var items = model.sp_Analysis(StartDate, EndDate);
            foreach (var item in items)
            {
                var lineItem = new AnalysisItem();
                lineItem.ActualAmount = item.ActualAmount;
                lineItem.BudgetAmount = item.BudgetAmount;
                lineItem.Category = item.Name;
                LineItems.Add(lineItem);
            }
            var totalLine = new AnalysisItem();
            totalLine.Category = "TOTAL";
            totalLine.ActualAmount = LineItems.Sum(x => x.ActualAmount);
            totalLine.BudgetAmount = LineItems.Sum(x => x.BudgetAmount);
            LineItems.Add(totalLine);

        }
    }

}
