namespace Betfair.Betting
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IOrderService
    {
        Task Place(string marketId, List<LimitOrder> orders, string strategyRef = null);

        Task Cancel(string marketId, List<LimitOrder> orders);
    }
}