
using System.Threading.Tasks;
using StockApi.Models;

namespace StockApi.Clients
{
    public interface IStockPriceClient
    {
        Task<bool> SendPriceChangeAsync(StockPrice stockPrice);
    }
}