using System.Collections.Generic;
using System.Threading;
using Betfair.Betting;
using Betfair.Extensions.Tests.TestDoubles;
using Betfair.Stream;
using Betfair.Stream.Responses;
using Xunit;

namespace Betfair.Extensions.Tests
{
    public class StrategyTests : StrategyBase
    {
        private readonly MarketCache _market = new MarketCache("1.2345");
        private readonly ChangeMessageStub _change = new ChangeMessageStub();
        private MarketChange _mChange;

        public StrategyTests()
        {
            LinkToMarket(_market);
        }

        public override MarketDataFilter DataFilter { get; } = new MarketDataFilter().WithBestPrices();

        public override List<LimitOrder> GetOrders(MarketChange marketChange, double stake)
        {
            _mChange = marketChange;
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
            Assert.Equal("1.2345", Market.MarketId);
        }

        [Fact]
        public void StrategyIsAskedForOrdersWhenTheMarketHasBeenUpdated()
        {
            var orders = GetOrders(new MarketChange(), 0);
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
            _market.OnChange(_change.WithMarketChange(mc).Build());

            GetOrders(mc, 0);
            Assert.Single(Market.Runners);
            Assert.Equal(mc, _mChange);
        }

        [Fact]
        public void PassCancellationToken()
        {
            using var source = new CancellationTokenSource();
            WithCancellationToken(source.Token);
            Assert.Equal(source.Token, CancellationToken);
        }
    }
}
