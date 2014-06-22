using Microsoft.Practices.Prism.Commands;
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
    public class RegisterAmountChanged : CompositePresentationEvent<object>
    {
    }
    public class DetailsEvent : CompositePresentationEvent<int>
    {
    }
    public class DetailsSaveMe : CompositePresentationEvent<RegisterItem>
    {
    }
    public class RegisterItem:BindableBase
    {
        public DelegateCommand DetailsCommand { get; set; }
        public bool IsDirty { get; set; }
       public int Id { get; set; }
        decimal amount = 0;
        string amountString = "";
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
                events.GetEvent<RegisterAmountChanged>().Publish(null);
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
                events.GetEvent<RegisterAmountChanged>().Publish(null);
            }
        }
        bool? isCleared;
        public bool? IsCleared
        {
            get
            {
                return isCleared;
            }
            set
            {

                if (IsCleared != value) IsDirty = true;
                SetProperty(ref this.isCleared, value);
                events.GetEvent<RegisterAmountChanged>().Publish(null);
            }
        }
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
                events.GetEvent<RegisterAmountChanged>().Publish(null);
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
                events.GetEvent<RegisterAmountChanged>().Publish(null);
            }
        }
         
        public List<Category> Categories { get; set; }
        public List<string> CategoryNames { get; set; }
        public string CategoryName { get; set; }
        IEventAggregator events;
        public RegisterItem(IEventAggregator events)
        {
            this.events = events;
            Categories = new List<Category>();
            CategoryNames = new List<string>();
            DetailsCommand = new DelegateCommand(() => ShowDetails());
        }
        void ShowDetails()
        {
            if(this.Id==0){
                events.GetEvent<DetailsSaveMe>().Publish(this);
            }
            events.GetEvent<DetailsEvent>().Publish(this.Id);
        }

    }
}
