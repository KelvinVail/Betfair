namespace Betfair.Extensions.Tests.TestDoubles
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Betfair.Betting;
    using Betfair.Stream.Responses;

    public class OrderManagerSpy : OrderManagerBase
    {
        public IEnumerable<LimitOrder> OrdersPlaced { get; private set; }

        public ChangeMessage LastChangeMessage { get; private set; }

        public bool OnMarketCloseCalled { get; private set; }

        public override async Task PlaceOrders(IEnumerable<LimitOrder> orders)
        {
            this.OrdersPlaced = orders;
            await Task.CompletedTask;
        }

        public override async Task OnChange(ChangeMessage change)
        {
            this.LastChangeMessage = change;
            await Task.CompletedTask;
        }

        public override async Task OnMarketClose()
        {
            this.OnMarketCloseCalled = true;
            await Task.CompletedTask;
        }

        public MarketCache MarketCache => this.Market;
    }
}
