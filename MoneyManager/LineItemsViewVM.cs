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
    public class RegisterLineAmountChanged: CompositePresentationEvent<RegisterLineItem>
    {
    }
    public class RegisterLineUpdated : CompositePresentationEvent<object>
    {
    }
    public class LineItemsViewVM:BindableBase
    {
         
        bool Loading = false;
        public decimal Total { get; set; }

        public ObservableCollection<RegisterLineItemDetailVM> LineItems { get; set; }
        MoneyManagerEntities model;
        public List<Category> Categories { get; set; }
        int Id;

       public void Load(int Id)
        {
            this.Id = Id;
            Loading = true;
            LineItems = new ObservableCollection<RegisterLineItemDetailVM>();
            var list = model.RegisterLineItemDetails.Include("Category").Where(x=>x.LineItemID == Id).ToList();
            foreach (var item in list)
            {
                var newItem = container.Resolve<RegisterLineItemDetailVM>();
                newItem.Id = item.ID;
                newItem.Note = item.Note;
                newItem.Amount = item.Amount;
                newItem.AmountString = item.Amount.ToString("N2");
                newItem.Categories = Categories;
                newItem.Category = Categories.Where(x => x.ID == item.CategoryId).Single();
                newItem.CategoryName = newItem.Category.Name;
                newItem.IsDirty = false;
                LineItems.Add(newItem);
            }
            for (int i = 1; i <= 2; i++)
            {
                var newItem = container.Resolve<RegisterLineItemDetailVM>();
                
                newItem.Categories = Categories;
                 
                newItem.IsDirty = false;
                LineItems.Add(newItem);
            }
            OnPropertyChanged(() => LineItems);
            ComputeTotal();
            Loading = false;

        }

        void ComputeTotal()
        {
             
            Total =  LineItems.Sum(x => x.Amount);

            OnPropertyChanged(() => Total);
            if (Loading) return;

            foreach (var item in LineItems.Where(x => x.IsDirty))
            {
                var findIt = model.RegisterLineItemDetails.Where(x => x.ID == item.Id).FirstOrDefault();
                if (findIt == null && item.Amount == 0) continue;
                if (findIt == null && item.Amount !=0)
                {
                    findIt = new RegisterLineItemDetail();

                    model.RegisterLineItemDetails.Add(findIt);
                }
                if (item.Category == null && findIt != null)
                {
                    item.Id = 0;
                    item.Amount = 0;
                    model.RegisterLineItemDetails.Remove(findIt);
                    model.SaveChanges();

                    continue;
                }
                if (item.Category == null) continue;
                findIt.CategoryId = item.Category.ID;
                findIt.Amount = item.Amount;
                findIt.Note = item.Note;
                findIt.LineItemID = this.Id;
                try
                {
                    model.SaveChanges();
                }
                catch (Exception ex)
                {
                    throw;
                }
                item.IsDirty = false;
                item.Id = findIt.ID;

            }
            var header = model.RegisterLineItems.Where(x => x.ID == Id).Single();
            header.Amount = LineItems.Sum(x => x.Amount);
            model.SaveChanges();
            events.GetEvent<RegisterLineAmountChanged>().Publish(header);
            if (LineItems.Where(x => x.Id == 0).Count() == 0)
            {
                Loading = true;
                var newItem = container.Resolve<RegisterLineItemDetailVM>();
               
                newItem.Categories = Categories;
                 
                newItem.IsDirty = false;
                LineItems.Add(newItem);
                Loading = false;
            }

        }
        IEventAggregator events;
        IUnityContainer container;
        public LineItemsViewVM(IEventAggregator events, IUnityContainer container)
        {
            this.events = events;
            this.container = container;
            model = DAL.GetModel();
            Loading = true;
            LoadCategories();
            events.GetEvent<RegisterLineUpdated>().Subscribe((x) => ComputeTotal());
            
            
             
            Loading = false;
        }
        
        void LoadCategories()
        {

            Categories = new List<Category>();
            var list = model.Categories.OrderBy(x => x.Name).ToList();
            foreach (var item in list)
            {
                Categories.Add(item);

            }
        }
        
    }
}
