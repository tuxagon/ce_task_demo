using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CEThreadingDemo
{
    public static class Logger
    {
        private static object _lock = new object();
        public static void Log(string message)
        {
            lock (_lock)
            {
                using (System.IO.StreamWriter sw = new System.IO.StreamWriter(@"C:\Testing_Directory\logs\ced.log", true))
                {
                    sw.WriteLine(message);
                }
            }

            Console.WriteLine(message);
        }
    }

    public class Quote
    {
        public int Id { get; set; }
        public Carrier Carrier { get; set; }
        public decimal Price { get; set; }

        public Quote()
        {

        }
    }

    public class QuoteNotification
    {
        private Quote quote_;
        public Quote Quote { get { return quote_; } }

        public QuoteNotification(Quote quote)
        {
            quote_ = quote;
        }

        public void Resolve()
        {
            Logger.Log("Resolved quote " + quote_.Id);
        }
    }

    public class Carrier
    {
        private static int quoteNum_ = 1;
        public string CarrierName { get; set; }

        public Carrier(string name = "")
        {
            Logger.Log("Adding carrier \"" + name + "\"");
            CarrierName = name;
        }

        public List<Quote> GetQuotes()
        {
            Random r = new Random();
            int qn = r.Next(1, 5);
            List<Quote> quotes = new List<Quote>();
            while (qn > 0)
            {
                quotes.Add(new Quote
                {
                    Id = quoteNum_++,
                    Carrier = this,
                    Price = Convert.ToDecimal(r.NextDouble() * 100)
                });
                qn--;
            }
            Logger.Log("Carrier brought back " + quotes.Count + " quotes");
            return quotes;
        }
    }

    public class Account
    {
        public string AccountID { get; set; }
    }

    public class CarrierEngine
    {
        public List<Account> Accounts { get; set; }
        public Load Load { get; set; }

        private Queue<QuoteNotification> quoteQueue_;
        private ManualResetEventSlim doneEvent_;

        public CarrierEngine()
        {
            this.Accounts = new List<Account>();
            this.Load = new Load();
            this.quoteQueue_ = new Queue<QuoteNotification>();
            this.doneEvent_ = new ManualResetEventSlim(false);
            LoadData();
        }

        public void ValidateForRating()
        {
            Logger.Log("Validated");
        }

        public void FilterAccounts()
        {
            Logger.Log("Accounts filtered");
        }

        public void RateCarriers()
        {
            try
            {
                object _lock = new object();

                StartQuoteQueue(this.Accounts.Count);

                Logger.Log("Rating...");
                Parallel.ForEach(
                    this.Accounts, 
                    new ParallelOptions { MaxDegreeOfParallelism = 10 }, 
                    (o) => {
                        Logger.Log("Starting account " + o.AccountID);
                        Carrier c = new Carrier("Carrier for account " + o.AccountID);
                        List<Quote> qs = c.GetQuotes();
                        foreach (Quote q in qs)
                        {
                            Logger.Log("Adding quote to queue " + q.Id + " : { " + q.Carrier.CarrierName + ", " + q.Price + " }");
                            quoteQueue_.Enqueue(new QuoteNotification(q));
                        }
                    });

                //doneEvent_.Wait();
                Logger.Log("Rated");

                Logger.Log("Done");
                Console.ReadLine();
            }

            catch (Exception ex)
            {
                Logger.Log("ERROR ERROR ERROR : Something went wrong with rating");
            }
        }

        private void StartQuoteQueue(int remaining)
        {
            Logger.Log("Starting quote thread");
            Task.Factory.StartNew((state) =>
            {
                try
                {
                    int r = (int)state;
                    while(r > 0)
                    {
                        if (quoteQueue_.Count > 0)
                        {
                            QuoteNotification qn = quoteQueue_.Dequeue();
                            Logger.Log("Resolving quote " + qn.Quote.Id);
                            qn.Resolve();
                            r--;
                        }
                    }
                    Logger.Log("Quotes handled");
                    doneEvent_.Set();
                }

                catch (Exception ex)
                {
                    Logger.Log("ERROR ERROR ERROR : Something went wrong with quote queue");
                }
            }, remaining);
            Logger.Log("Quote thread started");
        }

        private void LoadData()
        {
            Logger.Log("Loading Accounts");
            for (int i = 0; i < 10; i++)
            {
                this.Accounts.Add(new Account { AccountID = i.ToString() });
                //Logger.Log("Adding account ID : " + this.Accounts[i].AccountID);
            }
            Logger.Log("Accounts Loaded");
        }
    }

    public class Load
    {
        public int LoadID { get; set; }
    }
}
