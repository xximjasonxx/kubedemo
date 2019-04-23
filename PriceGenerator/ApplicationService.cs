
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PriceGenerator.Services;

namespace PriceGenerator
{
    public class ApplicationService : IHostedService, IDisposable
    {
        private readonly ILogger _logger;
        private readonly StockService _stockService;
        private readonly StockPriceManager _stockPriceManager;
        
        private Timer _timer;

        public ApplicationService(ILogger<ApplicationService> logger, StockService stockService, StockPriceManager stockPriceManager)
        {
            _logger = logger;
            _stockService = stockService;
            _stockPriceManager = stockPriceManager;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting Service");

            var stockData = await _stockService.GetInitialStockData();
            _stockPriceManager.Initialize(stockData);
            _timer = new Timer(Execute, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Service stopping");
            _timer.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        void Execute(object userState)
        {
            _stockPriceManager.SendPriceChanges();
        }
    }
}