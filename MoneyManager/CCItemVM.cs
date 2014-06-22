using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Mvvm;
using Microsoft.Practices.Prism.PubSubEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoneyManager
{
    public class CCAmountChanged : CompositePresentationEvent<object>
    {
    }

    public class CCItemVM : BindableBase
    {
        public bool IsDirty { get; set; }
        public int Id { get; set; }
        decimal amount = 0;
        string amountString = "";
        DateTime? date;
        public DateTime? Date
        {
            get
            {
                return date;
            }
            set
            {

                if (date != value) IsDirty = true;
                SetProperty(ref this.date, value);
                events.GetEvent<CCAmountChanged>().Publish(null);
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
                events.GetEvent<CCAmountChanged>().Publish(null);
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
                events.GetEvent<CCAmountChanged>().Publish(null);
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
                events.GetEvent<CCAmountChanged>().Publish(null);
            }
        }
        public List<Category> Categories { get; set; }
        public List<string> CategoryNames { get; set; }
        public string CategoryName { get; set; }
        IEventAggregator events;
        public CCItemVM(IEventAggregator events)
        {
            this.events = events;
            Categories = new List<Category>();
            CategoryNames = new List<string>();
        }

        bool isPaid = false;
        public bool IsPaid
        {
            get
            {
                return isPaid;
            }
            set
            {

                if (isPaid != value) IsDirty = true;
                SetProperty(ref this.isPaid, value);
                events.GetEvent<CCAmountChanged>().Publish(null);
            }
        }
    }
}
