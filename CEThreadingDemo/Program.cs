using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CEThreadingDemo
{
    public class Program
    {
        static void Main(string[] args)
        {
            CarrierEngine ce = new CarrierEngine();
            List<Quote> quotes = ce.GetRates();
            foreach (Quote q in quotes)
            {
                Console.WriteLine("{0} at ${1}", q.CarrierName, q.Price);
            }
        }
    }
}
