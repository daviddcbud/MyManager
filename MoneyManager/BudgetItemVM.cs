using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Prism.PubSubEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyManager
{
    public class BudgetAmountChanged : CompositePresentationEvent<object>
    {
    }
    
    public class BudgetItemVM:BindableBase
    {
        public bool IsDirty { get; set; }
        public int Id { get; set; }
        decimal amount = 0;
        string amountString = "";
        bool post = false;
        public bool Post
        {
            get
            {
                return post;
            }
            set
            {

                if (post != value) IsDirty = true;
                SetProperty(ref this.post, value);
                events.GetEvent<BudgetAmountChanged>().Publish(null);
            }
        }
        bool isSavings = false;
        public bool IsSavings
        {
            get
            {
                return isSavings;
            }
            set
            {

                if (isSavings != value) IsDirty = true;
                SetProperty(ref this.isSavings, value);
                events.GetEvent<BudgetAmountChanged>().Publish(null);
            }
        }
        public string AmountString
        {
            get
            {
                return amountString;
            }
            set
            {
                SetProperty(ref this.amountString, value);
                decimal parseit = 0;
                if (decimal.TryParse(amountString, out parseit))
                {
                    Amount = parseit;
                }
            }
        }
        public decimal Amount
        {
            get
            {
                return amount;
            }
            set
            {
                if (amount != value) IsDirty = true;
                SetProperty(ref this.amount, value);
                events.GetEvent<BudgetAmountChanged>().Publish(null);
            }
        }
        Category category;
        public Category Category
        {
            get
            {
                return category;
            }
            set
            {
                var oldName = "";
                if (category != null) oldName = category.Name;
                var newName = "";
                if (value != null) newName = value.Name;
                if (oldName != newName) IsDirty = true;
                SetProperty(ref this.category, value);
                events.GetEvent<BudgetAmountChanged>().Publish(null);
            }
        }
        string description = "";
        public string Description
        {
            get
            {
                return description;
            }
            set
            {

                if (description != value) IsDirty = true;
                SetProperty(ref this.description, value);
                events.GetEvent<BudgetAmountChanged>().Publish(null);
            }
        }
        public List<Category> Categories { get; set; }
        public List<string> CategoryNames { get; set; }
        public string CategoryName { get; set; }
        IEventAggregator events;
        public BudgetItemVM(IEventAggregator events)
        {
            this.events = events;
            Categories = new List<Category>();
            CategoryNames = new List<string>();
        }
    }
}
