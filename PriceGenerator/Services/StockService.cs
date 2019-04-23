
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PriceGenerator.Models;

namespace PriceGenerator.Services
{
    public class StockService
    {
        private readonly ILogger _logger;
        private readonly string _apiKey;
        private readonly string _baseUrl;
        private string[] _stockSymbols;

        public StockService(IConfiguration configuration, ILogger<StockService> logger)
        {
            _apiKey = configuration.GetValue<string>("ApiKey");
            _baseUrl = configuration.GetValue<string>("BaseUrl");
            _stockSymbols = configuration.GetSection("StockSymbols").Get<string[]>();

            _logger = logger;
        }

        public async Task<IDictionary<string, decimal>> GetInitialStockData()
        {
            var stockResults = await Task.WhenAll(
                _stockSymbols.Select(symbol => GetSymbolPrice(symbol))
            );

            _logger.LogInformation("Got initial prices");
            return stockResults.ToDictionary(x => x.Symbol, x => x.StockPrice);
        }

        async Task<Stock> GetSymbolPrice(string symbol)
        {
            string url = $"{_baseUrl}/stock/{symbol}/quote?token={_apiKey}";
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                    throw new Exception($"Failed to get stock price for {symbol}");

                var responseContent = await response.Content.ReadAsStringAsync();
                if (string.IsNullOrEmpty(responseContent))
                    throw new Exception($"Received no content for {symbol}");

                return JsonConvert.DeserializeObject<Stock>(responseContent);
            }
        }
    }
}