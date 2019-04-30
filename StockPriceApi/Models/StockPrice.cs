
using System;

namespace StockPriceApi.Models
{
    public class StockPriceChange
    {
        public string Symbol { get; set; }

        public decimal NewPrice { get; set; }
        public decimal PriceChange { get; set; }

        public DateTime PublishTime { get; set; }
    }
}