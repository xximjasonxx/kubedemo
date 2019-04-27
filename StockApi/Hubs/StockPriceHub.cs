
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using StockApi.Clients;
using StockApi.Models;

namespace StockApi.Hubs
{
    public class StockPriceHub : Hub<IStockPriceClient>
    {
        public async Task<bool> SendPriceChangeAsync(StockPrice stockPrice)
        {
            await Clients.All.SendPriceChangeAsync(stockPrice);
            return true;
        }
    }
}