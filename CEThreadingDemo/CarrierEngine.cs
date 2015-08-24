using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CEThreadingDemo
{
    public class Quote
    {
        public string CarrierName { get; set; }
        public decimal Price { get; set; }
    }

    public class CarrierEngine
    {
        public List<Quote> GetRates(params int[] subscriptionIDs)
        {
            throw new NotImplementedException();
        }
    }
}
