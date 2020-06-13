namespace Betfair.Extensions.Tests.TestDoubles
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Betfair.Betting;

    public class OrderServiceSpy : IOrderService
    {
        public List<LimitOrder> LastOrdersPlaced { get; private set; }

        public string MarketId { get; private set; }

        public string StrategyRef { get; private set; }

        public async Task Place(string marketId, List<LimitOrder> orders, string strategyRef = null)
        {
            this.MarketId = marketId;
            this.StrategyRef = strategyRef;
            this.LastOrdersPlaced = orders;
            await Task.CompletedTask;
        }

        public async Task Cancel(string marketId, List<string> betIds)
        {
            await Task.CompletedTask;
        }

        public async Task CancelAll(string marketId)
        {
            await Task.CompletedTask;
        }
    }
}
