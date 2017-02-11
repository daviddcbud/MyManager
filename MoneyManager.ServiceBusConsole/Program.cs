using MoneyManager.ServiceBusServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoneyManager.ServiceBusConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Host.OpenTCPHost();
            Console.WriteLine("up");
            Console.ReadLine();
        }
    }
}
