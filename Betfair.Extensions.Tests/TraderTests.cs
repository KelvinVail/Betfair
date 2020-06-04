namespace Betfair.Extensions.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Betfair.Extensions.Tests.TestDoubles;
    using Betfair.Stream.Responses;
    using Xunit;

    public class TraderTests
    {
        private readonly SubscriptionSpy subscription = new SubscriptionSpy();

        private readonly Trader trader;

        private readonly StrategySpy strategy = new StrategySpy();

        public TraderTests()
        {
            this.trader = new Trader(this.subscription);
            this.trader.AddStrategy(this.strategy);
        }

        [Fact]
        public void TraderIsSealed()
        {
            Assert.True(typeof(Trader).IsSealed);
        }

        [Fact]
        public async Task ThrowIfMarketIdIsNull()
        {
            var ex = await Assert.ThrowsAsync<ArgumentNullException>(() => this.trader.TradeMarket(null, CancellationToken.None));
            Assert.Equal("Value cannot be null. (Parameter 'marketId')", ex.Message);
        }

        [Fact]
        public async Task ThrowIfMarketIdIsEmpty()
        {
            var ex = await Assert.ThrowsAsync<ArgumentNullException>(() => this.trader.TradeMarket(string.Empty, CancellationToken.None));
            Assert.Equal("Value cannot be null. (Parameter 'marketId')", ex.Message);
        }

        [Fact]
        public async Task ThrowIfTraderHasNoStrategies()
        {
            var emptyTrader = new Trader(this.subscription);
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => emptyTrader.TradeMarket("MarketId", CancellationToken.None));
            Assert.Equal("Trader must contain at least one strategy.", ex.Message);
        }

        [Fact]
        public async Task TradeSubscribesToMarketStream()
        {
            await this.trader.TradeMarket("MarketId", CancellationToken.None);
            Assert.StartsWith("CASO", this.subscription.Actions, StringComparison.CurrentCulture);
        }

        [Fact]
        public async Task StreamIsDisconnectedWhenTokenIsCancelled()
        {
            var tokenSource = new CancellationTokenSource();
            tokenSource.Cancel();
            await this.trader.TradeMarket("MarketId", tokenSource.Token);
            tokenSource.Dispose();
            Assert.EndsWith("D", this.subscription.Actions, StringComparison.CurrentCulture);
        }

        [Theory]
        [InlineData("1.2345")]
        [InlineData("MarketId")]
        [InlineData("1.987654")]
        public async Task MarketIsLinkedToStrategy(string marketId)
        {
            await this.trader.TradeMarket(marketId, default);
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
            await this.trader.TradeMarket(marketId, default);
            Assert.Equal(marketId, this.strategy.LinkedMarketId());
            Assert.Equal(marketId, strategy2.LinkedMarketId());
        }

        [Theory]
        [InlineData("1.2345")]
        [InlineData("MarketId")]
        [InlineData("1.987654")]
        public async Task MarketIdIsSetInSubscription(string marketId)
        {
            await this.trader.TradeMarket(marketId, default);
            Assert.Equal(marketId, this.subscription.MarketId);
        }

        [Fact]
        public async Task DataFieldsAreSetInSubscription()
        {
            this.strategy.DataFilter.WithBestPrices();
            await this.trader.TradeMarket("1.2345", default);
            Assert.Contains("EX_BEST_OFFERS", this.subscription.Fields);
        }

        [Fact]
        public async Task DifferentDataFieldsAreSetInSubscription()
        {
            this.strategy.DataFilter.WithMarketDefinition();
            await this.trader.TradeMarket("1.2345", default);
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

            await this.trader.TradeMarket("1.2345", default);
            Assert.Contains("EX_BEST_OFFERS", this.subscription.Fields);
            Assert.Contains("EX_MARKET_DEF", this.subscription.Fields);
        }

        [Fact]
        public async Task ProcessesChangeMessages()
        {
            await this.trader.TradeMarket("MarketId", CancellationToken.None);
            Assert.Equal("CASOM", this.subscription.Actions);
        }

        [Fact]
        public async Task UpdateMarketCacheUsedInStrategies()
        {
            var rc = new RunnerChangeStub().WithSelectionId(1).WithBestAvailableToBack(0, 2.5, 9.99);
            var mc = new MarketChangeStub().WithRunnerChange(rc);
            this.subscription.WithMarketChange(mc);
            await this.trader.TradeMarket("1.2345", CancellationToken.None);

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
            await this.trader.TradeMarket("1.2345", CancellationToken.None);

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
            await this.trader.TradeMarket("1.2345", CancellationToken.None);

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

            await this.trader.TradeMarket("1.2345", source.Token);
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

            await this.trader.TradeMarket("1.2345", default);

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

            await this.trader.TradeMarket("1.2345", default);

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

            await this.trader.TradeMarket("1.2345", source.Token);
            Assert.Equal(1, this.strategy.MarketUpdateCount);
        }

        [Fact]
        public async Task PassCancellationTokenToStrategies()
        {
            var strategy2 = new StrategySpy();
            this.trader.AddStrategy(strategy2);

            using var source = new CancellationTokenSource();

            await this.trader.TradeMarket("1.2345", source.Token);

            Assert.Equal(source.Token, this.strategy.GetCancellationToken());
            Assert.Equal(source.Token, strategy2.GetCancellationToken());
        }
    }
}
