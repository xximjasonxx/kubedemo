
using Newtonsoft.Json;

namespace PriceGenerator.Models
{
    public class Stock
    {
        public string Symbol { get; set; }

        public string CompanyName { get; set; }

        [JsonProperty("latestPrice")]
        public decimal StockPrice { get; set; }
    }
}