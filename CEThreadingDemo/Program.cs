using System;
using System.Collections.Generic;

namespace CEThreadingDemo
{
    public class Program
    {
        static void Main(string[] args)
        {
            CarrierEngine ce = new CarrierEngine();
            ce.ValidateForRating();
            List<Quote> quotes = ce.GetRates();
            foreach (Quote q in quotes)
            {
                Console.WriteLine("{0} at ${1}", q.Carrier.CarrierName, q.Price);
            }
        }
    }
}
