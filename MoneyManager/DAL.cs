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
        
    }
}
