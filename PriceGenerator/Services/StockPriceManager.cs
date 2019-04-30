
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using PriceGenerator.Models;

namespace PriceGenerator.Services
{
    public class StockPriceManager
    {
        private readonly ILogger _logger;
        private readonly MessagePublisherService _messagePublisherService;

        private IDictionary<string, Stock> _stockPriceData;

        public StockPriceManager(ILogger<StockPriceManager> logger, MessagePublisherService messagePublisherService)
        {
            _logger = logger;
            _messagePublisherService = messagePublisherService;
        }

        public void Initialize(ICollection<Stock> startingPriceData)
        {
            _stockPriceData = startingPriceData.ToDictionary(x => x.Symbol, x => x);
        }

        public void SendPriceChanges()
        {
            var randomPriceDifferenceGenerator = new Random(DateTime.Now.Second);
            var updatedPrices = _stockPriceData.Select(kv =>
            {
                var priceChange = (decimal)randomPriceDifferenceGenerator.Next(-1000, 1000) / 1000;
                return new StockPriceChange
                {
                    Symbol = kv.Key,
                    NewPrice = kv.Value.StockPrice + priceChange,
                    PriceChange = priceChange,
                    PublishTime = DateTime.UtcNow
                };
            }).ToList();
            
            _messagePublisherService.PublishPriceChanges(updatedPrices).GetAwaiter().GetResult();
            UpdateLocalPrices(updatedPrices);
        }

        void UpdateLocalPrices(ICollection<StockPriceChange> priceChanges)
        {
            foreach (var priceChange in priceChanges)
            {
                _stockPriceData[priceChange.Symbol].StockPrice = priceChange.NewPrice;
            }
        }
    }
}