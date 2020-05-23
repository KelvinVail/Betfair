namespace Betfair.Extensions.Tests
{
    using System;
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
        public async Task ThrowIfTraderHasNoStrategies()
        {
            var emptyTrader = new Trader(this.subscription);
            await Assert.ThrowsAsync<InvalidOperationException>(() => emptyTrader.TradeMarket("MarketId", CancellationToken.None));
        }

        [Fact]
        public async Task TradeMarketSubscribesToMarketStream()
        {
            await this.trader.TradeMarket("MarketId", CancellationToken.None);
            Assert.Equal("CASOM", this.subscription.Actions);
        }

        [Fact]
        public async Task StreamIsDisconnectedWhenTokenIsCancelled()
        {
            var tokenSource = new CancellationTokenSource();
            tokenSource.Cancel();
            await this.trader.TradeMarket("MarketId", tokenSource.Token);
            tokenSource.Dispose();
            Assert.Equal("CASOMD", this.subscription.Actions);
        }

        [Fact]
        public async Task StopsProcessingChangeMessagesWhenTokenIsCancelled()
        {
            var tokenSource = new CancellationTokenSource();
            tokenSource.Cancel();
            this.subscription.Messages.Add(new ChangeMessage());
            await this.trader.TradeMarket("MarketId", tokenSource.Token);
            tokenSource.Dispose();
            Assert.Equal("CASOMD", this.subscription.Actions);
        }

        [Fact]
        public async Task CanHandleMultipleChangeMessages()
        {
            this.subscription.Messages.Add(new ChangeMessage());
            await this.trader.TradeMarket("MarketId", CancellationToken.None);
            Assert.Equal("CASOMM", this.subscription.Actions);
        }

        [Fact]
        public async Task TradeMarketsCallsStrategy()
        {
            await this.trader.TradeMarket("MarketId", CancellationToken.None);
            Assert.Equal("a", this.strategy.ClocksProcessed);
        }

        [Fact]
        public async Task ChangeMessageIsProcessedByStrategy()
        {
            this.subscription.Messages.Add(new ChangeMessage { Clock = "b" });
            await this.trader.TradeMarket("MarketId", CancellationToken.None);
            Assert.Equal("ab", this.strategy.ClocksProcessed);
        }

        [Fact]
        public async Task MultipleStrategiesHandleChanges()
        {
            var secondStrategy = new StrategySpy();
            this.trader.AddStrategy(secondStrategy);
            await this.trader.TradeMarket("MarketId", CancellationToken.None);
            Assert.Equal("a", this.strategy.ClocksProcessed);
            Assert.Equal("a", secondStrategy.ClocksProcessed);
        }

        // CorrectMarketIdIsUsedInSubscription

        // TODO Merge MarketDataFilters from each strategy
    }
}
