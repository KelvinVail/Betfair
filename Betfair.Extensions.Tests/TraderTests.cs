namespace Betfair.Extensions.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Betfair.Betting;
    using Betfair.Extensions.Tests.TestDoubles;
    using Betfair.Stream.Responses;
    using Xunit;

    public class TraderTests
    {
        private readonly SubscriptionSpy subscription = new SubscriptionSpy();

        private readonly Trader trader;

        private readonly StrategySpy strategy = new StrategySpy();

        private readonly OrderManagerSpy orderManager = new OrderManagerSpy();

        public TraderTests()
        {
            this.trader = new Trader(this.subscription);
            this.trader.AddStrategy(this.strategy);
            this.trader.SetOrderManager(this.orderManager);
        }

        [Fact]
        public void TraderIsSealed()
        {
            Assert.True(typeof(Trader).IsSealed);
        }

        [Fact]
        public async Task ThrowIfMarketIdIsNull()
        {
            var ex = await Assert.ThrowsAsync<ArgumentNullException>(() => this.trader.TradeMarket(null, 0, CancellationToken.None));
            Assert.Equal("Value cannot be null. (Parameter 'marketId')", ex.Message);
        }

        [Fact]
        public async Task ThrowIfMarketIdIsEmpty()
        {
            var ex = await Assert.ThrowsAsync<ArgumentNullException>(() => this.trader.TradeMarket(string.Empty, 0, CancellationToken.None));
            Assert.Equal("Value cannot be null. (Parameter 'marketId')", ex.Message);
        }

        [Fact]
        public async Task ThrowIfTraderHasNoStrategies()
        {
            var emptyTrader = new Trader(this.subscription);
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => emptyTrader.TradeMarket("MarketId", 0, CancellationToken.None));
            Assert.Equal("Trader must contain at least one strategy.", ex.Message);
        }

        [Fact]
        public async Task TradeSubscribesToMarketStream()
        {
            await this.trader.TradeMarket("MarketId", 0, CancellationToken.None);
            Assert.StartsWith("CASO", this.subscription.Actions, StringComparison.CurrentCulture);
        }

        [Fact]
        public async Task StreamIsDisconnectedWhenTokenIsCancelled()
        {
            var tokenSource = new CancellationTokenSource();
            tokenSource.Cancel();
            await this.trader.TradeMarket("MarketId", 0, tokenSource.Token);
            tokenSource.Dispose();
            Assert.EndsWith("D", this.subscription.Actions, StringComparison.CurrentCulture);
        }

        [Theory]
        [InlineData("1.2345")]
        [InlineData("MarketId")]
        [InlineData("1.987654")]
        public async Task MarketIsLinkedToStrategy(string marketId)
        {
            await this.trader.TradeMarket(marketId, 0, default);
            Assert.Equal(marketId, this.strategy.LinkedMarketId());
        }

        [Theory]
        [InlineData("1.2345")]
        [InlineData("MarketId")]
        [InlineData("1.987654")]
        public async Task MarketIsLinkedToEachStrategy(string marketId)
        {
            var strategy2 = new StrategySpy();
            this.trader.AddStrategy(strategy2);
            await this.trader.TradeMarket(marketId, 0, default);
            Assert.Equal(marketId, this.strategy.LinkedMarketId());
            Assert.Equal(marketId, strategy2.LinkedMarketId());
        }

        [Theory]
        [InlineData("1.2345")]
        [InlineData("MarketId")]
        [InlineData("1.987654")]
        public async Task MarketIdIsSetInSubscription(string marketId)
        {
            await this.trader.TradeMarket(marketId, 0, default);
            Assert.Equal(marketId, this.subscription.MarketId);
        }

        [Fact]
        public async Task DataFieldsAreSetInSubscription()
        {
            this.strategy.DataFilter.WithBestPrices();
            await this.trader.TradeMarket("1.2345", 0, default);
            Assert.Contains("EX_BEST_OFFERS", this.subscription.Fields);
        }

        [Fact]
        public async Task DifferentDataFieldsAreSetInSubscription()
        {
            this.strategy.DataFilter.WithMarketDefinition();
            await this.trader.TradeMarket("1.2345", 0, default);
            Assert.DoesNotContain("EX_BEST_OFFERS", this.subscription.Fields);
            Assert.Contains("EX_MARKET_DEF", this.subscription.Fields);
        }

        [Fact]
        public async Task DataFieldsFromMultipleStrategiesAreMerged()
        {
            this.strategy.DataFilter.WithBestPrices();

            var strategy2 = new StrategySpy();
            strategy2.DataFilter.WithMarketDefinition();
            this.trader.AddStrategy(strategy2);

            await this.trader.TradeMarket("1.2345", 0, default);
            Assert.Contains("EX_BEST_OFFERS", this.subscription.Fields);
            Assert.Contains("EX_MARKET_DEF", this.subscription.Fields);
        }

        [Fact]
        public async Task ProcessesChangeMessages()
        {
            await this.trader.TradeMarket("MarketId", 0, CancellationToken.None);
            Assert.Equal("CASOM", this.subscription.Actions);
        }

        [Fact]
        public async Task UpdateMarketCacheUsedInStrategies()
        {
            var rc = new RunnerChangeStub().WithSelectionId(1).WithBestAvailableToBack(0, 2.5, 9.99);
            var mc = new MarketChangeStub().WithRunnerChange(rc);
            this.subscription.WithMarketChange(mc);
            await this.trader.TradeMarket("1.2345", 0, CancellationToken.None);

            Assert.Equal("CASOMM", this.subscription.Actions);
            Assert.Equal(1, this.strategy.RunnerCount());
        }

        [Fact]
        public async Task UpdateMarketCacheWithMultipleChanges()
        {
            var rc = new RunnerChangeStub().WithSelectionId(1).WithBestAvailableToBack(0, 2.5, 9.99);
            var mc = new MarketChangeStub().WithRunnerChange(rc);
            var rc2 = new RunnerChangeStub().WithSelectionId(2).WithBestAvailableToBack(0, 2.5, 9.99);
            var mc2 = new MarketChangeStub().WithRunnerChange(rc2);
            this.subscription.WithMarketChanges(new List<MarketChange> { mc, mc2 });
            await this.trader.TradeMarket("1.2345", 0, CancellationToken.None);

            Assert.Equal("CASOMM", this.subscription.Actions);
            Assert.Equal(2, this.strategy.RunnerCount());
        }

        [Theory]
        [InlineData(987654321)]
        [InlineData(12345)]
        [InlineData(4802394)]
        public async Task UpdateMarketCacheWithPublishedTime(long pt)
        {
            this.subscription.PublishTime = pt;
            var rc = new RunnerChangeStub().WithSelectionId(1).WithBestAvailableToBack(0, 2.5, 9.99);
            var mc = new MarketChangeStub().WithRunnerChange(rc);
            this.subscription.WithMarketChange(mc);
            await this.trader.TradeMarket("1.2345", 0, CancellationToken.None);

            Assert.Equal("CASOMM", this.subscription.Actions);
            Assert.Equal(pt, this.strategy.LastPublishedTime());
        }

        [Fact]
        public async Task StopProcessingMessageIfCancelled()
        {
            var source = new CancellationTokenSource();
            this.subscription.CancelAfterThisManyMessages(1, source);

            var rc = new RunnerChangeStub().WithSelectionId(1).WithBestAvailableToBack(0, 2.5, 9.99);
            var mc = new MarketChangeStub().WithRunnerChange(rc);
            var rc2 = new RunnerChangeStub().WithSelectionId(2).WithBestAvailableToBack(0, 2.5, 9.99);
            var mc2 = new MarketChangeStub().WithRunnerChange(rc2);
            this.subscription.WithMarketChange(mc);
            this.subscription.WithMarketChange(mc2);

            await this.trader.TradeMarket("1.2345", 0, source.Token);
            Assert.Equal("CASOMD", this.subscription.Actions);
        }

        [Fact]
        public async Task NotifyEachStrategyOfMarketUpdate()
        {
            var strategy2 = new StrategySpy();
            this.trader.AddStrategy(strategy2);

            var rc = new RunnerChangeStub().WithSelectionId(1).WithBestAvailableToBack(0, 2.5, 9.99);
            var mc = new MarketChangeStub().WithRunnerChange(rc);
            var rc2 = new RunnerChangeStub().WithSelectionId(2).WithBestAvailableToBack(0, 2.5, 9.99);
            var mc2 = new MarketChangeStub().WithRunnerChange(rc2);
            this.subscription.WithMarketChanges(new List<MarketChange> { mc, mc2 });

            await this.trader.TradeMarket("1.2345", 0, default);

            Assert.Equal(2, this.strategy.MarketUpdateCount);
            Assert.Equal(2, strategy2.MarketUpdateCount);
        }

        [Fact]
        public async Task TellEachStrategyWhatTheChangeWas()
        {
            var strategy2 = new StrategySpy();
            this.trader.AddStrategy(strategy2);

            var rc = new RunnerChangeStub().WithSelectionId(1).WithBestAvailableToBack(0, 2.5, 9.99);
            var mc = new MarketChangeStub().WithRunnerChange(rc);
            var rc2 = new RunnerChangeStub().WithSelectionId(2).WithBestAvailableToBack(0, 2.5, 9.99);
            var mc2 = new MarketChangeStub().WithRunnerChange(rc2);
            this.subscription.WithMarketChanges(new List<MarketChange> { mc, mc2 });

            await this.trader.TradeMarket("1.2345", 0, default);

            Assert.Equal(mc2, this.strategy.LastMarketChange);
            Assert.Equal(mc2, strategy2.LastMarketChange);
        }

        [Fact]
        public async Task ProcessLastMessageBeforeCancelling()
        {
            var source = new CancellationTokenSource();
            this.subscription.CancelAfterThisManyMessages(2, source);

            var rc = new RunnerChangeStub().WithSelectionId(1).WithBestAvailableToBack(0, 2.5, 9.99);
            var mc = new MarketChangeStub().WithRunnerChange(rc);
            var rc2 = new RunnerChangeStub().WithSelectionId(2).WithBestAvailableToBack(0, 2.5, 9.99);
            var mc2 = new MarketChangeStub().WithRunnerChange(rc2);
            this.subscription.WithMarketChange(mc);
            this.subscription.WithMarketChange(mc2);

            await this.trader.TradeMarket("1.2345", 0, source.Token);
            Assert.Equal(1, this.strategy.MarketUpdateCount);
        }

        [Fact]
        public async Task PassCancellationTokenToStrategies()
        {
            var strategy2 = new StrategySpy();
            this.trader.AddStrategy(strategy2);

            using var source = new CancellationTokenSource();

            await this.trader.TradeMarket("1.2345", 0, source.Token);

            Assert.Equal(source.Token, this.strategy.Token());
            Assert.Equal(source.Token, strategy2.Token());
        }

        [Fact]
        public void CanAddAnOrderManager()
        {
            this.trader.SetOrderManager(this.orderManager);
        }

        [Fact]
        public async Task ThrowIfOrderManagerNotSet()
        {
            var emptyTrader = new Trader(this.subscription);
            emptyTrader.AddStrategy(this.strategy);
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => emptyTrader.TradeMarket("MarketId", 0, CancellationToken.None));
            Assert.Equal("Trader must contain an OrderManager.", ex.Message);
        }

        [Theory]
        [InlineData(1, Side.Back, 2.5, 10.99)]
        [InlineData(2, Side.Lay, 50, 2.01)]
        public async Task OrdersFromAStrategyArePlaced(
            long id, Side side, double price, double size)
        {
            this.subscription.WithMarketChange(new MarketChange());
            var o1 = new LimitOrder(id, side, price, size);
            this.strategy.WithOrder(o1);
            await this.trader.TradeMarket("1.2345", 0, default);

            var placedOrder = this.orderManager.OrdersPlaced
                .First(o => o.SelectionId == id);
            Assert.Equal(side, placedOrder.Side);
            Assert.Equal(price, placedOrder.Price);
            Assert.Equal(size, placedOrder.Size);
        }

        [Fact]
        public async Task PlaceTradesFromMultipleStrategies()
        {
            this.subscription.WithMarketChange(new MarketChange());
            var s2 = new StrategySpy();
            this.trader.AddStrategy(s2);

            var o1 = new LimitOrder(1, Side.Back, 2.5, 10.99);
            this.strategy.WithOrder(o1);
            s2.WithOrder(o1);

            await this.trader.TradeMarket("1.2345", 0, default);

            Assert.Equal(2, this.orderManager.OrdersPlaced.Count());
        }

        [Fact]
        public async Task LinksMarketToOrderManager()
        {
            await this.trader.TradeMarket("1.2345", 0, default);

            Assert.Equal("1.2345", this.orderManager.MarketCache.MarketId);
        }

        [Fact]
        public async Task OrderManagerIsCalledOnEveryChange()
        {
            this.subscription.WithMarketChange(
                new MarketChangeStub().WithTotalMatched(10));
            await this.trader.TradeMarket("1.2345", 0, default);

            var mc = this.orderManager.LastChangeMessage
                .MarketChanges.Select(m => m.TotalAmountMatched).First();
            Assert.Equal(10, mc);
        }

        [Theory]
        [InlineData(100)]
        [InlineData(99)]
        [InlineData(291.01)]
        [InlineData(1.0101)]
        public async Task BankIsUsedAsStakeForSingleStrategy(double bank)
        {
            this.subscription.WithMarketChange(
                new MarketChangeStub().WithTotalMatched(10));
            await this.trader.TradeMarket("1.2345", bank, default);

            Assert.Equal(Math.Round(bank, 2), this.strategy.Stake);
        }

        [Theory]
        [InlineData(100)]
        [InlineData(1.0101)]
        public async Task SplitBankForMultipleStrategies(double bank)
        {
            this.subscription.WithMarketChange(
                new MarketChangeStub().WithTotalMatched(10));
            this.trader.AddStrategy(new StrategySpy());
            await this.trader.TradeMarket("1.2345", bank, default);

            Assert.Equal(Math.Round(bank / 2, 2), this.strategy.Stake);
        }

        [Theory]
        [InlineData(100, 10.99)]
        [InlineData(100, 1.0101)]
        public async Task AdjustBankForMarketLiability(double bank, double liability)
        {
            var rc = new RunnerChangeStub().WithBestAvailableToBack(0, 2.5, 2.99);
            var mc = new MarketChangeStub().WithTotalMatched(10).WithRunnerChange(rc);
            this.subscription.WithMarketChange(mc);

            var orc = new OrderRunnerChangeStub().WithUnmatchedBack(2.5, liability);
            var oc = new OrderChangeStub().WithOrderRunnerChange(orc);
            this.subscription.WithOrderChange(oc);
            this.subscription.WithMarketChange(mc);

            this.trader.AddStrategy(new StrategySpy());
            await this.trader.TradeMarket("1.2345", bank, default);

            Assert.Equal(Math.Round((bank - Math.Round(liability, 2)) / 2, 2), this.strategy.Stake);
        }

        [Fact]
        public async Task OnMarketCloseNotCalledIfNotCancelled()
        {
            var rc = new RunnerChangeStub().WithBestAvailableToBack(0, 2.5, 2.99);
            var mc = new MarketChangeStub().WithTotalMatched(10).WithRunnerChange(rc);
            this.subscription.WithMarketChange(mc);

            await this.trader.TradeMarket("1.2345", 0, default);

            Assert.False(this.orderManager.OnMarketCloseCalled);
        }

        [Fact]
        public async Task OnMarketCloseCalledIfCancelled()
        {
            var rc = new RunnerChangeStub().WithBestAvailableToBack(0, 2.5, 2.99);
            var mc = new MarketChangeStub().WithTotalMatched(10).WithRunnerChange(rc);
            this.subscription.WithMarketChange(mc);

            using var source = new CancellationTokenSource();
            source.Cancel();
            await this.trader.TradeMarket("1.2345", 0, source.Token);

            Assert.True(this.orderManager.OnMarketCloseCalled);
        }
    }
}
