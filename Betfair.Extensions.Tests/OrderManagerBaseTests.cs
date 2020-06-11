namespace Betfair.Extensions.Tests
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Betfair.Betting;
    using Betfair.Stream.Responses;
    using Xunit;

    public class OrderManagerBaseTests : OrderManagerBase
    {

        [Fact]
        public void CanBeLinkedToAMarket()
        {
            this.LinkToMarket(new MarketCache("1.2345"));
            Assert.Equal("1.2345", this.Market.MarketId);
        }

        public override async Task PlaceOrders(IEnumerable<LimitOrder> orders)
        {
            await Task.CompletedTask;
        }

        public override async Task OnChange(ChangeMessage change)
        {
            await Task.CompletedTask;
        }

        public override async Task OnMarketClose()
        {
            await Task.CompletedTask;
        }
    }
}
