
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace StockApi.Listeners
{
    public class PriceChangeListenerService : IHostedService
    {
        public Task StartAsync(CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}