namespace Betfair.Extensions.Tests
{
    using Betfair.Extensions.Tests.TestDoubles;
    using Xunit;

    public class OrderManagerBaseTests
    {
        private readonly OrderManagerSpy orderManager = new OrderManagerSpy();

        [Fact]
        public void CanBeLinkedToAMarket()
        {
            this.orderManager.LinkToMarket(new MarketCache("1.2345"));
            Assert.Equal("1.2345", this.orderManager.MarketCache.MarketId);
        }
    }
}
