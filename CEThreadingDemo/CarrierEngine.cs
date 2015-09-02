using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CEThreadingDemo
{
    public class Quote
    {
        public int Id { get; set; }
        public Carrier Carrier { get; set; }
        public decimal Price { get; set; }

        public Quote()
        {
            this.Carrier = new Carrier();
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
            Console.WriteLine("Resolved quote " + Quote.Id);
        }
    }

    public class Carrier
    {
        private static int quoteNum_ = 1;
        public string CarrierName { get; set; }

        public Carrier(string name = "")
        {
            CarrierName = name;
        }

        public List<Quote> GetQuotes()
        {
            return new List<Quote>
            {
                new Quote
                {
                    Id = quoteNum_++,
                    Carrier = this,
                    Price = 3.45M
                }
            };
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
            Console.WriteLine("Validated");
        }

        public void FilterAccounts()
        {
            Console.WriteLine("Filtered Accounts");
        }

        public void RateCarriers()
        {
            try
            {
                object _lock = new object();
                int iter = 0;
                int tc = 10;
                int total = 10;
                int remaining = total;

                StartQuoteQueue(remaining);

                Console.WriteLine("Starting rating");
                Timer t = new Timer((o) =>
                {
                    Console.WriteLine("Starting burst " + iter);
                    List<Task> tasks = new List<Task>();
                    for (int i = tc * iter; i < total && i < (tc * (iter + 1)); i++)
                    {
                        Console.WriteLine("Starting rating for Carrier " + i);
                        tasks.Add(Task.Factory.StartNew((state) =>
                        {
                            try
                            {
                                Carrier c = new Carrier(state.ToString());
                                Console.WriteLine("Getting quote for " + c.CarrierName);
                                List<Quote> qs = c.GetQuotes();
                                foreach (Quote q in qs)
                                {
                                    Console.WriteLine("Adding quote to queue");
                                    quoteQueue_.Enqueue(new QuoteNotification(q));
                                }
                            }

                            catch (Exception ex)
                            {
                                Console.WriteLine("An in task exception occurred");
                            }

                            finally
                            {
                                lock (_lock)
                                {
                                    --remaining;
                                }
                            }
                        }, "Carrier " + i));
                    }
                    Console.WriteLine("Started all for burst " + iter);
                    ++iter;

                    try
                    {
                        Task.WaitAll(tasks.ToArray());
                    }
                    catch (AggregateException ex)
                    {
                        ex.Handle((x) =>
                        {
                            Console.Write("An aggregate exception occurred");
                            return true;
                        });
                    }

                    Console.WriteLine("burst complete");

                }, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));

                doneEvent_.Wait();
                t.Dispose();

                Console.WriteLine("threading complete\nPress enter/return to finish");
                Console.ReadLine();
            }

            catch (Exception ex)
            {
                Console.WriteLine("An error occurred");
            }
        }

        private void StartQuoteQueue(int remaining)
        {
            Console.WriteLine("Starting Quote Thread");
            Task.Factory.StartNew((state) =>
            {
                try
                {
                    int r = (int)state;
                    while(r > 0)
                    {
                        if (quoteQueue_.Count > 0)
                        {
                            Console.WriteLine("Resolving quote");
                            QuoteNotification qn = quoteQueue_.Dequeue();
                            qn.Resolve();
                            r--;
                            Console.WriteLine("Quote resolved");
                        }
                    }
                    Console.WriteLine("Rating done");
                    doneEvent_.Set();
                }

                catch (Exception ex)
                {
                    Console.WriteLine("An in task exception occurred");
                }
            }, remaining);
            Console.WriteLine("Quote Thread Started");
        }

        private void LoadData()
        {
            Console.WriteLine("Data retrieved");
        }
    }

    public class Load
    {
        public int LoadID { get; set; }
    }
}
