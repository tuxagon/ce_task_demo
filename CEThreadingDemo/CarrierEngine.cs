using System;
using System.Collections.Generic;

namespace CEThreadingDemo
{
    public class Quote
    {
        public Carrier Carrier { get; set; }
        public decimal Price { get; set; }

        public Quote()
        {
            this.Carrier = new Carrier();
        }
    }

    public class Carrier
    {
        public string CarrierName { get; set; }
    }

    public class Account
    {
        public string AccountID { get; set; }
    }

    public class CarrierEngine
    {
        public List<Account> Accounts { get; set; }
        public Load Load { get; set; }

        public CarrierEngine()
        {
            this.Accounts = new List<Account>();
            this.Load = new Load();
            LoadData();
        }

        public void ValidateForRating()
        {

        }

        public void FilterAccounts()
        {

        }

        public List<Quote> GetRates()
        {
            throw new NotImplementedException();
        }

        private void LoadData()
        {
            // get data
        }
    }

    public class Load
    {
        public int LoadID { get; set; }
    }
}
