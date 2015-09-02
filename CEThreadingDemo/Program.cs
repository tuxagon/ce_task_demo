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
            ce.RateCarriers();
        }
    }
}
