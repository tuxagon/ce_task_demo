using System;
using System.Collections.Generic;

namespace CEThreadingDemo
{
    public class Program
    {
        static void Main(string[] args)
        {
            try
            {
                CarrierEngine ce = new CarrierEngine();
                ce.LoadData();
                ce.ValidateForRating();
                ce.FilterAccounts(a => a.AccountID == "1");
                ce.RateCarriers();
            }
            catch (Exception e)
            {

            }
        }
    }
}
