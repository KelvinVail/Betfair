namespace Betfair.Extensions.Tests
{
    using System.Threading.Tasks;
    using Betfair.Extensions.Tests.TestDoubles;
    using Betfair.Stream;
    using Betfair.Stream.Responses;
    using Xunit;

    public class StrategyTests : StrategyBase
    {
        private readonly MarketCache market = new MarketCache("1.2345");

        private MarketChange mChange;

        public StrategyTests()
        {
            this.LinkToMarket(this.market);
        }

        public override MarketDataFilter DataFilter { get; } = new MarketDataFilter().WithBestPrices();

        public override async Task OnMarketUpdate(MarketChange marketChange)
        {
            this.mChange = marketChange;
            await Task.CompletedTask;
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
        public async Task StrategyIsToldWhenTheMarketHasBeenUpdated()
        {
            await this.OnMarketUpdate(new MarketChange());
        }

        [Fact]
        public async Task StrategyIsToldWhenAndWhatHasBeenUpdated()
        {
            var rc = new RunnerChangeStub()
                .WithSelectionId(1)
                .WithBestAvailableToBack(0, 2.5, 100)
                .WithBestAvailableToLay(0, 3.0, 200);
            var mc = new MarketChangeStub().WithRunnerChange(rc);
            this.market.OnMarketChange(mc, 0);

            await this.OnMarketUpdate(mc);
            Assert.Single(this.Market.Runners);
            Assert.Equal(mc, this.mChange);
        }
    }
}
