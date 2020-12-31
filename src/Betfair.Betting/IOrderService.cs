using System.Collections.Generic;
using System.Threading.Tasks;

namespace Betfair.Betting
{
    public interface IOrderService
    {
        Task Place(string marketId, List<LimitOrder> orders, string strategyRef = null);

        Task Cancel(string marketId, List<string> betIds);

        Task CancelAll(string marketId);
    }
}