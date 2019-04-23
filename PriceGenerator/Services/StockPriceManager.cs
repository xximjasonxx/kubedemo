
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace PriceGenerator.Services
{
    public class StockPriceManager
    {
        private readonly ILogger _logger;

        private IDictionary<string, decimal> _stockPriceData;

        public StockPriceManager(ILogger<StockPriceManager> logger)
        {
            _logger = logger;
        }

        public void Initialize(IDictionary<string, decimal> stockPriceData)
        {
            _stockPriceData = stockPriceData;
        }

        public void SendPriceChanges()
        {
            var randomPriceDifferenceGenerator = new Random(DateTime.Now.Second);
            _stockPriceData = _stockPriceData.Select(kv =>
            {
                var priceChange = (decimal)randomPriceDifferenceGenerator.Next(-1000, 1000) / 100;
                _logger.LogInformation($"{kv.Key} [{kv.Value}] - Change: {priceChange}");
                
                return new {
                    Symbol = kv.Key,
                    Price = kv.Value,
                    PriceChange = priceChange
                };
            })
            .ToDictionary(x => x.Symbol, x => x.Price + x.PriceChange);
        }
    }
}