using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Betfair.Betting;
using Betfair.Extensions.Tests.TestDoubles;
using Betfair.Stream.Responses;
using Xunit;

namespace Betfair.Extensions.Tests
{
    public class TraderTests
    {
        private readonly SubscriptionSpy _subscription = new SubscriptionSpy();
        private readonly Trader _trader;
        private readonly StrategySpy _strategy = new StrategySpy();
        private readonly OrderServiceSpy _orderService = new OrderServiceSpy();
        private readonly OrderManagerSpy _orderManager;

        public TraderTests()
        {
            _trader = new Trader(_subscription);
            _orderManager = new OrderManagerSpy(_orderService);
            _trader.AddStrategy(_strategy);
            _trader.SetOrderManager(_orderManager);
        }

        [Fact]
        public void TraderIsSealed()
        {
            Assert.True(typeof(Trader).IsSealed);
        }

        [Fact]
        public async Task ThrowIfMarketIdIsNull()
        {
            var ex = await Assert.ThrowsAsync<ArgumentNullException>(() => _trader.TradeMarket(null, 0, CancellationToken.None));
            Assert.Equal("Value cannot be null. (Parameter 'marketId')", ex.Message);
        }

        [Fact]
        public async Task ThrowIfMarketIdIsEmpty()
        {
            var ex = await Assert.ThrowsAsync<ArgumentNullException>(() => _trader.TradeMarket(string.Empty, 0, CancellationToken.None));
            Assert.Equal("Value cannot be null. (Parameter 'marketId')", ex.Message);
        }

        [Fact]
        public async Task ThrowIfTraderHasNoStrategies()
        {
            var emptyTrader = new Trader(_subscription);
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => emptyTrader.TradeMarket("MarketId", 0, CancellationToken.None));
            Assert.Equal("Trader must contain at least one strategy.", ex.Message);
        }

        [Fact]
        public async Task TradeSubscribesToMarketStream()
        {
            await _trader.TradeMarket("MarketId", 0, CancellationToken.None);
            Assert.StartsWith("CASO", _subscription.Actions, StringComparison.CurrentCulture);
        }

        [Fact]
        public async Task StreamIsDisconnectedWhenTokenIsCancelled()
        {
            var tokenSource = new CancellationTokenSource();
            tokenSource.Cancel();
            await _trader.TradeMarket("MarketId", 0, tokenSource.Token);
            tokenSource.Dispose();
            Assert.EndsWith("D", _subscription.Actions, StringComparison.CurrentCulture);
        }

        [Theory]
        [InlineData("1.2345")]
        [InlineData("MarketId")]
        [InlineData("1.987654")]
        public async Task MarketIsLinkedToStrategy(string marketId)
        {
            await _trader.TradeMarket(marketId, 0, default);
            Assert.Equal(marketId, _strategy.LinkedMarketId());
        }

        [Theory]
        [InlineData("1.2345")]
        [InlineData("MarketId")]
        [InlineData("1.987654")]
        public async Task MarketIsLinkedToEachStrategy(string marketId)
        {
            var strategy2 = new StrategySpy();
            _trader.AddStrategy(strategy2);
            await _trader.TradeMarket(marketId, 0, default);
            Assert.Equal(marketId, _strategy.LinkedMarketId());
            Assert.Equal(marketId, strategy2.LinkedMarketId());
        }

        [Theory]
        [InlineData("1.2345")]
        [InlineData("MarketId")]
        [InlineData("1.987654")]
        public async Task MarketIdIsSetInSubscription(string marketId)
        {
            await _trader.TradeMarket(marketId, 0, default);
            Assert.Equal(marketId, _subscription.MarketId);
        }

        [Fact]
        public async Task DataFieldsAreSetInSubscription()
        {
            _strategy.DataFilter.WithBestPrices();
            await _trader.TradeMarket("1.2345", 0, default);
            Assert.Contains("EX_BEST_OFFERS", _subscription.Fields);
        }

        [Fact]
        public async Task DifferentDataFieldsAreSetInSubscription()
        {
            _strategy.DataFilter.WithMarketDefinition();
            await _trader.TradeMarket("1.2345", 0, default);
            Assert.DoesNotContain("EX_BEST_OFFERS", _subscription.Fields);
            Assert.Contains("EX_MARKET_DEF", _subscription.Fields);
        }

        [Fact]
        public async Task DataFieldsFromMultipleStrategiesAreMerged()
        {
            _strategy.DataFilter.WithBestPrices();

            var strategy2 = new StrategySpy();
            strategy2.DataFilter.WithMarketDefinition();
            _trader.AddStrategy(strategy2);

            await _trader.TradeMarket("1.2345", 0, default);
            Assert.Contains("EX_BEST_OFFERS", _subscription.Fields);
            Assert.Contains("EX_MARKET_DEF", _subscription.Fields);
        }

        [Fact]
        public async Task ProcessesChangeMessages()
        {
            await _trader.TradeMarket("MarketId", 0, CancellationToken.None);
            Assert.Equal("CASOM", _subscription.Actions);
        }

        [Fact]
        public async Task UpdateMarketCacheUsedInStrategies()
        {
            var rc = new RunnerChangeStub().WithSelectionId(1).WithBestAvailableToBack(0, 2.5, 9.99);
            var mc = new MarketChangeStub().WithRunnerChange(rc);
            _subscription.WithMarketChange(mc);
            await _trader.TradeMarket("1.2345", 0, CancellationToken.None);

            Assert.Equal("CASOMM", _subscription.Actions);
            Assert.Equal(1, _strategy.RunnerCount());
        }

        [Fact]
        public async Task UpdateMarketCacheWithMultipleChanges()
        {
            var rc = new RunnerChangeStub().WithSelectionId(1).WithBestAvailableToBack(0, 2.5, 9.99);
            var mc = new MarketChangeStub().WithRunnerChange(rc);
            var rc2 = new RunnerChangeStub().WithSelectionId(2).WithBestAvailableToBack(0, 2.5, 9.99);
            var mc2 = new MarketChangeStub().WithRunnerChange(rc2);
            _subscription.WithMarketChanges(new List<MarketChange> { mc, mc2 });
            await _trader.TradeMarket("1.2345", 0, CancellationToken.None);

            Assert.Equal("CASOMM", _subscription.Actions);
            Assert.Equal(2, _strategy.RunnerCount());
        }

        [Theory]
        [InlineData(987654321)]
        [InlineData(12345)]
        [InlineData(4802394)]
        public async Task UpdateMarketCacheWithPublishedTime(long pt)
        {
            _subscription.PublishTime = pt;
            var rc = new RunnerChangeStub().WithSelectionId(1).WithBestAvailableToBack(0, 2.5, 9.99);
            var mc = new MarketChangeStub().WithRunnerChange(rc);
            _subscription.WithMarketChange(mc);
            await _trader.TradeMarket("1.2345", 0, CancellationToken.None);

            Assert.Equal("CASOMM", _subscription.Actions);
            Assert.Equal(pt, _strategy.LastPublishedTime());
        }

        [Fact]
        public async Task StopProcessingMessageIfCancelled()
        {
            var source = new CancellationTokenSource();
            _subscription.CancelAfterThisManyMessages(1, source);

            var rc = new RunnerChangeStub().WithSelectionId(1).WithBestAvailableToBack(0, 2.5, 9.99);
            var mc = new MarketChangeStub().WithRunnerChange(rc);
            var rc2 = new RunnerChangeStub().WithSelectionId(2).WithBestAvailableToBack(0, 2.5, 9.99);
            var mc2 = new MarketChangeStub().WithRunnerChange(rc2);
            _subscription.WithMarketChange(mc);
            _subscription.WithMarketChange(mc2);

            await _trader.TradeMarket("1.2345", 0, source.Token);
            Assert.Equal("CASOMD", _subscription.Actions);
        }

        [Fact]
        public async Task NotifyEachStrategyOfMarketUpdate()
        {
            var strategy2 = new StrategySpy();
            _trader.AddStrategy(strategy2);

            var rc = new RunnerChangeStub().WithSelectionId(1).WithBestAvailableToBack(0, 2.5, 9.99);
            var mc = new MarketChangeStub().WithRunnerChange(rc);
            var rc2 = new RunnerChangeStub().WithSelectionId(2).WithBestAvailableToBack(0, 2.5, 9.99);
            var mc2 = new MarketChangeStub().WithRunnerChange(rc2);
            _subscription.WithMarketChanges(new List<MarketChange> { mc, mc2 });

            await _trader.TradeMarket("1.2345", 0, default);

            Assert.Equal(2, _strategy.MarketUpdateCount);
            Assert.Equal(2, strategy2.MarketUpdateCount);
        }

        [Fact]
        public async Task TellEachStrategyWhatTheChangeWas()
        {
            var strategy2 = new StrategySpy();
            _trader.AddStrategy(strategy2);

            var rc = new RunnerChangeStub().WithSelectionId(1).WithBestAvailableToBack(0, 2.5, 9.99);
            var mc = new MarketChangeStub().WithRunnerChange(rc);
            var rc2 = new RunnerChangeStub().WithSelectionId(2).WithBestAvailableToBack(0, 2.5, 9.99);
            var mc2 = new MarketChangeStub().WithRunnerChange(rc2);
            _subscription.WithMarketChanges(new List<MarketChange> { mc, mc2 });

            await _trader.TradeMarket("1.2345", 0, default);

            Assert.Equal(mc2, _strategy.LastMarketChange);
            Assert.Equal(mc2, strategy2.LastMarketChange);
        }

        [Fact]
        public async Task ProcessLastMessageBeforeCancelling()
        {
            var source = new CancellationTokenSource();
            _subscription.CancelAfterThisManyMessages(2, source);

            var rc = new RunnerChangeStub().WithSelectionId(1).WithBestAvailableToBack(0, 2.5, 9.99);
            var mc = new MarketChangeStub().WithRunnerChange(rc);
            var rc2 = new RunnerChangeStub().WithSelectionId(2).WithBestAvailableToBack(0, 2.5, 9.99);
            var mc2 = new MarketChangeStub().WithRunnerChange(rc2);
            _subscription.WithMarketChange(mc);
            _subscription.WithMarketChange(mc2);

            await _trader.TradeMarket("1.2345", 0, source.Token);
            Assert.Equal(1, _strategy.MarketUpdateCount);
        }

        [Fact]
        public async Task PassCancellationTokenToStrategies()
        {
            var strategy2 = new StrategySpy();
            _trader.AddStrategy(strategy2);

            using var source = new CancellationTokenSource();

            await _trader.TradeMarket("1.2345", 0, source.Token);

            Assert.Equal(source.Token, _strategy.Token());
            Assert.Equal(source.Token, strategy2.Token());
        }

        [Fact]
        public void CanAddAnOrderManager()
        {
            _trader.SetOrderManager(_orderManager);
        }

        [Fact]
        public async Task ThrowIfOrderManagerNotSet()
        {
            var emptyTrader = new Trader(_subscription);
            emptyTrader.AddStrategy(_strategy);
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => emptyTrader.TradeMarket("MarketId", 0, CancellationToken.None));
            Assert.Equal("Trader must contain an OrderManager.", ex.Message);
        }

        [Theory]
        [InlineData(1, Side.Back, 2.5, 10.99)]
        [InlineData(2, Side.Lay, 50, 2.01)]
        public async Task OrdersFromAStrategyArePlaced(
            long id, Side side, double price, double size)
        {
            _subscription.WithMarketChange(new MarketChange());
            var o1 = new LimitOrder(id, side, price, size);
            _strategy.WithOrder(o1);
            await _trader.TradeMarket("1.2345", 0, default);

            var placedOrder = _orderService.LastOrdersPlaced
                .First(o => o.SelectionId == id);
            Assert.Equal(side, placedOrder.Side);
            Assert.Equal(price, placedOrder.Price);
            Assert.Equal(size, placedOrder.Size);
        }

        [Fact]
        public async Task PlaceTradesFromMultipleStrategies()
        {
            _subscription.WithMarketChange(new MarketChange());
            var s2 = new StrategySpy();
            _trader.AddStrategy(s2);

            var o1 = new LimitOrder(1, Side.Back, 2.5, 10.99);
            _strategy.WithOrder(o1);
            s2.WithOrder(o1);

            await _trader.TradeMarket("1.2345", 0, default);

            Assert.Equal(2, _orderService.LastOrdersPlaced.Count);
        }

        [Fact]
        public async Task LinksMarketToOrderManager()
        {
            await _trader.TradeMarket("1.2345", 0, default);

            Assert.Equal("1.2345", _orderManager.MarketCache.MarketId);
        }

        [Fact]
        public async Task OrderManagerIsCalledOnEveryChange()
        {
            _subscription.WithMarketChange(
                new MarketChangeStub().WithTotalMatched(10));
            await _trader.TradeMarket("1.2345", 0, default);

            var mc = _orderManager.LastChangeMessage
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
            _subscription.WithMarketChange(
                new MarketChangeStub().WithTotalMatched(10));
            await _trader.TradeMarket("1.2345", bank, default);

            Assert.Equal(Math.Round(bank, 2), _strategy.Stake);
        }

        [Theory]
        [InlineData(100)]
        [InlineData(1.0101)]
        public async Task SplitBankForMultipleStrategies(double bank)
        {
            _subscription.WithMarketChange(
                new MarketChangeStub().WithTotalMatched(10));
            _trader.AddStrategy(new StrategySpy());
            await _trader.TradeMarket("1.2345", bank, default);

            Assert.Equal(Math.Round(bank / 2, 2), _strategy.Stake);
        }

        [Theory]
        [InlineData(100, 10.99)]
        [InlineData(100, 1.0101)]
        public async Task AdjustBankForMarketLiability(double bank, double liability)
        {
            var rc = new RunnerChangeStub().WithBestAvailableToBack(0, 2.5, 2.99);
            var mc = new MarketChangeStub().WithTotalMatched(10).WithRunnerChange(rc);
            _subscription.WithMarketChange(mc);

            var orc = new OrderRunnerChangeStub().WithUnmatchedBack(2.5, liability);
            var oc = new OrderChangeStub().WithOrderRunnerChange(orc);
            _subscription.WithOrderChange(oc);
            _subscription.WithMarketChange(mc);

            _trader.AddStrategy(new StrategySpy());
            await _trader.TradeMarket("1.2345", bank, default);

            Assert.Equal(Math.Round((bank - Math.Round(liability, 2)) / 2, 2), _strategy.Stake);
        }

        [Fact]
        public async Task OnMarketCloseNotCalledIfNotCancelled()
        {
            var rc = new RunnerChangeStub().WithBestAvailableToBack(0, 2.5, 2.99);
            var mc = new MarketChangeStub().WithTotalMatched(10).WithRunnerChange(rc);
            _subscription.WithMarketChange(mc);

            await _trader.TradeMarket("1.2345", 0, default);

            Assert.False(_orderManager.OnMarketCloseCalled);
        }

        [Fact]
        public async Task OnMarketCloseCalledIfCancelled()
        {
            var rc = new RunnerChangeStub().WithBestAvailableToBack(0, 2.5, 2.99);
            var mc = new MarketChangeStub().WithTotalMatched(10).WithRunnerChange(rc);
            _subscription.WithMarketChange(mc);

            using var source = new CancellationTokenSource();
            source.Cancel();
            await _trader.TradeMarket("1.2345", 0, source.Token);

            Assert.True(_orderManager.OnMarketCloseCalled);
        }
    }
}
