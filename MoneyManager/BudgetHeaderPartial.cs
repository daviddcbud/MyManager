using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyManager
{
    public partial class Category
    {
        public override string ToString()
        {
            return Name;
        }
    }
    public partial class BudgetHeader
    {
        public string Display
        {
            get
            {
                return StartDate.ToShortDateString() + " - " + EndDate.ToShortDateString();
            }
        }
    }
}
