using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MoneyManager
{
    /// <summary>
    /// Interaction logic for AnalysisDetailsWindow.xaml
    /// </summary>
    public partial class AnalysisDetailsWindow : Window
    {
        public AnalysisDetailsWindow()
        {
            InitializeComponent();
        }
        public void  Load(string category , DateTime from,DateTime to)
        {
            var list = new List<LineItemDetail>();
            using(var model= new MoneyManagerEntities())
            {
                var linedetails = model.RegisterLineItemDetails.Include("Category").Include("RegisterLineItem").Where(x => x.RegisterLineItem.Date >= from
                    && x.RegisterLineItem.Date <= to && x.Category.Name == category).OrderBy(x=>x.RegisterLineItem.Date);
                foreach(var item in linedetails)
                {
                    var find = list.Where(x => x.Name == item.Note).FirstOrDefault();
                  //  if (find == null)
                   // {
                        find = new LineItemDetail();
                        find.Date = item.RegisterLineItem.Date;
                        find.Name = item.Note;
                        list.Add(find);
                    //}
                    find.Amount += item.Amount;
                }

                var rlines = model.RegisterLineItems.Include("Category").Where(x => x.Date >= from
                    && x.Date <= to && x.Category.Name == category);
                foreach (var item in rlines)
                {
                    var find = list.Where(x => x.Name == item.Description ).FirstOrDefault();
                    //if (find == null)
                    //{
                        find = new LineItemDetail();
                        find.Date = item.Date;
                        find.Name = item.Description;
                        list.Add(find);
                    //}
                    find.Amount += item.Amount;
                }

                var cc = model.CreditCardTransactions.Include("Category").Where(x => x.Date >= from
                    && x.Date <= to && x.Category.Name == category && x.Paid ==false );
                foreach (var item in cc)
                {
                    var find = list.Where(x => x.Name == item.Notes).FirstOrDefault();
                    //if (find == null)
                    //{
                        find = new LineItemDetail();
                        find.Date = item.Date;
                        find.Name = item.Notes ;
                        list.Add(find);
                    //}
                    find.Amount += item.Amount;
                }

                grid.ItemsSource = list.OrderBy(x => x.Date).ToList();
            }
        }
    }

    public class LineItemDetail
    {
        public string Name { get; set; }
        public decimal Amount { get; set; }

        public DateTime   Date { get; set; }
    }
}
