
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using StockPriceApi.Models;

namespace StockPriceApi.Hubs
{
    public class StockPriceHub : Hub<IPriceChangeClient>
    {
        public async Task SendPriceAsync(StockPriceChange stockPrice) => await Clients.All.ReceiveStockPrice(stockPrice);
    }
}