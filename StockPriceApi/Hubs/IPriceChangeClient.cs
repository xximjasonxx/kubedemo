
using System.Threading.Tasks;
using StockPriceApi.Models;

namespace StockPriceApi.Hubs
{
    public interface IPriceChangeClient
    {
        Task SendStockPriceAsync(StockPrice stockPrice);
    }
}