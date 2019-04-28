
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using StockPriceApi.Models;

namespace StockPriceApi.Hubs
{
    public class StockPriceHub : Hub<IPriceChangeClient>
    {
        public async Task SendPriceAsync(StockPrice stockPrice) => await Clients.All.SendStockPriceAsync(stockPrice);
    }
}