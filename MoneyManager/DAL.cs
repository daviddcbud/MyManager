using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyManager
{
    public static class DAL
    {
      static   MoneyManagerEntities model=new MoneyManagerEntities();
       public static  MoneyManagerEntities GetModel()
        {
            return model;
        }
      static List<Category> categories;
      public static List<Category> Categories
      {
          get
          {
              if (categories == null)
              {
                  categories = model.Categories.OrderBy(x => x.Name).ToList();
              }
              return categories;
          }
      }
        
    }
}
