
using System;

namespace StockPriceApi.Models
{
    public class StockPrice
    {
        public string Symbol { get; set; }

        public decimal NewPrice { get; set; }

        public DateTime PublishTime { get; set; }
    }
}