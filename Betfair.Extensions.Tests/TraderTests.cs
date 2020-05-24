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
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => emptyTrader.TradeMarket("MarketId", CancellationToken.None));
            Assert.Equal("Trader must contain at least one strategy.", ex.Message);
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

        [Theory]
        [InlineData("MarketId")]
        [InlineData("1.23456789")]
        public async Task CorrectMarketIdIsUsedInSubscription(string marketId)
        {
            await this.trader.TradeMarket(marketId, CancellationToken.None);
            Assert.Equal(marketId, this.subscription.MarketId);
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
        public async Task SingleDataFilterFieldsAreSet()
        {
            this.strategy.DataFilter.WithBestPrices();
            await this.trader.TradeMarket("MarketId", CancellationToken.None);
            Assert.Contains("EX_BEST_OFFERS", this.subscription.Fields);
            Assert.Single(this.subscription.Fields);
        }

        [Fact]
        public async Task MultipleDataFilterFieldsAreSet()
        {
            this.strategy.DataFilter
                .WithLastTradedPrice()
                .WithMarketDefinition();
            await this.trader.TradeMarket("MarketId", CancellationToken.None);
            Assert.Contains("EX_LTP", this.subscription.Fields);
            Assert.Contains("EX_MARKET_DEF", this.subscription.Fields);
            Assert.Equal(2, this.subscription.Fields.Count);
        }

        [Fact]
        public async Task MultipleStrategyDataFilterFieldsAreSet()
        {
            this.strategy.DataFilter
                .WithLastTradedPrice()
                .WithMarketDefinition();
            var secondStrategy = new StrategySpy();
            secondStrategy.DataFilter.WithBestPrices();
            this.trader.AddStrategy(secondStrategy);

            await this.trader.TradeMarket("MarketId", CancellationToken.None);
            Assert.Contains("EX_LTP", this.subscription.Fields);
            Assert.Contains("EX_MARKET_DEF", this.subscription.Fields);
            Assert.Contains("EX_BEST_OFFERS", this.subscription.Fields);
            Assert.Equal(3, this.subscription.Fields.Count);
        }

        [Fact]
        public async Task CancellationTokenIsPassedToStrategies()
        {
            var tokenSource = new CancellationTokenSource();
            var secondStrategy = new StrategySpy();
            this.trader.AddStrategy(secondStrategy);
            await this.trader.TradeMarket("MarketId", tokenSource.Token);
            Assert.Equal(tokenSource.Token, this.strategy.Token);
            Assert.Equal(tokenSource.Token, secondStrategy.Token);
            tokenSource.Dispose();
        }
    }
}
