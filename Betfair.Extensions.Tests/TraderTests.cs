namespace Betfair.Extensions.Tests
{
    using System.Threading;
    using System.Threading.Tasks;
    using Betfair.Extensions.Tests.TestDoubles;
    using Xunit;

    public class TraderTests
    {
        private readonly ExchangeStub exchange = new ExchangeStub();

        private readonly SubscriptionSpy subscription = new SubscriptionSpy();

        private readonly Trader trader;

        public TraderTests()
        {
            this.trader = new Trader(this.exchange, this.subscription);
        }

        [Fact]
        public void TraderIsSealed()
        {
            Assert.True(typeof(Trader).IsSealed);
        }

        [Fact]
        public async Task TradeMarketSubscribesToMarketStream()
        {
            await this.trader.TradeMarketAsync("MarketId", CancellationToken.None);
            Assert.Equal("CASOM", this.subscription.Actions);
        }

        [Fact]
        public async Task StreamIsDisconnectedWhenTokenIsCancelled()
        {
            var tokenSource = new CancellationTokenSource();
            tokenSource.Cancel();
            await this.trader.TradeMarketAsync("MarketId", tokenSource.Token);
            tokenSource.Dispose();
            Assert.Equal("CASOMD", this.subscription.Actions);
        }
    }
}
