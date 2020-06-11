namespace Betfair.Extensions.Tests
{
    using System.Collections.Generic;
    using System.Threading;
    using Betfair.Betting;
    using Betfair.Extensions.Tests.TestDoubles;
    using Betfair.Stream;
    using Betfair.Stream.Responses;
    using Xunit;

    public class StrategyTests : StrategyBase
    {
        private readonly MarketCache market = new MarketCache("1.2345");

        private readonly ChangeMessageStub change = new ChangeMessageStub();

        private MarketChange mChange;

        public StrategyTests()
        {
            this.LinkToMarket(this.market);
        }

        public override MarketDataFilter DataFilter { get; } = new MarketDataFilter().WithBestPrices();

        public override int RatioOfBankToUse { get; } = 1;

        public override List<LimitOrder> GetOrders(MarketChange marketChange, double stake)
        {
            this.mChange = marketChange;
            return new List<LimitOrder>();
        }

        [Fact]
        public void StrategyBaseIsAbstract()
        {
            Assert.True(typeof(StrategyBase).IsAbstract);
        }

        [Fact]
        public void CanBeLinkedToAMarket()
        {
            Assert.Equal("1.2345", this.Market.MarketId);
        }

        [Fact]
        public void StrategyIsAskedForOrdersWhenTheMarketHasBeenUpdated()
        {
            var orders = this.GetOrders(new MarketChange(), 0);
            Assert.IsType<List<LimitOrder>>(orders);
        }

        [Fact]
        public void StrategyIsToldWhatHasBeenUpdated()
        {
            var rc = new RunnerChangeStub()
                .WithSelectionId(1)
                .WithBestAvailableToBack(0, 2.5, 100)
                .WithBestAvailableToLay(0, 3.0, 200);
            var mc = new MarketChangeStub().WithRunnerChange(rc);
            this.market.OnChange(this.change.WithMarketChange(mc).Build());

            this.GetOrders(mc, 0);
            Assert.Single(this.Market.Runners);
            Assert.Equal(mc, this.mChange);
        }

        [Fact]
        public void PassCancellationToken()
        {
            using var source = new CancellationTokenSource();
            this.WithCancellationToken(source.Token);
            Assert.Equal(source.Token, this.CancellationToken);
        }
    }
}
