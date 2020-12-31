using System.Collections.Generic;
using System.Threading.Tasks;
using Betfair.Betting;

namespace Betfair.Extensions.Tests.TestDoubles
{
    public class OrderServiceSpy : IOrderService
    {
        public List<LimitOrder> LastOrdersPlaced { get; private set; }

        public string MarketId { get; private set; }

        public string StrategyRef { get; private set; }

        public string Actions { get; private set; }

        public async Task Place(string marketId, List<LimitOrder> orders, string strategyRef = null)
        {
            MarketId = marketId;
            StrategyRef = strategyRef;
            LastOrdersPlaced = orders;
            Actions += "P";
            await Task.CompletedTask;
        }

        public async Task Cancel(string marketId, List<string> betIds)
        {
            Actions += "c";
            await Task.CompletedTask;
        }

        public async Task CancelAll(string marketId)
        {
            Actions += "C";
            await Task.CompletedTask;
        }
    }
}
