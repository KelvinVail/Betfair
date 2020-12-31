using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Betfair.Betting;
using Betfair.Stream;
using Betfair.Stream.Responses;

namespace Betfair.Extensions
{
    public sealed class Trader
    {
        private readonly ISubscription _subscription;
        private readonly List<StrategyBase> _strategies = new List<StrategyBase>();
        private OrderManagerBase _orderManager;

        public Trader(ISubscription subscription)
        {
            _subscription = subscription;
        }

        public void AddStrategy(StrategyBase strategy)
        {
            _strategies.Add(strategy);
        }

        public void SetOrderManager(OrderManagerBase manager)
        {
            _orderManager = manager;
        }

        public async Task TradeMarket(
            string marketId, double bank, CancellationToken cancellationToken)
        {
            var market = CreateMarketCache(marketId);
            LinkMarkets(market, cancellationToken);
            await Subscribe(marketId);
            await foreach (var change in _subscription.GetChanges())
            {
                UpdateMarketCache(change, market);
                await PlaceOrdersFromEachStrategy(change, bank + market.Liability);
                await UpdateOrders(change);

                if (!cancellationToken.IsCancellationRequested) continue;
                await _orderManager.OnMarketClose();
                DisconnectStreamIfCancelled(cancellationToken);
                break;
            }
        }

        private static void UpdateMarketCache(ChangeMessage change, MarketCache market)
        {
            market.OnChange(change);
        }

        private MarketCache CreateMarketCache(string marketId)
        {
            if (string.IsNullOrEmpty(marketId))
                throw new ArgumentNullException(nameof(marketId));
            if (_strategies.Count == 0)
                throw new InvalidOperationException("Trader must contain at least one strategy.");
            if (_orderManager is null)
                throw new InvalidOperationException("Trader must contain an OrderManager.");
            return new MarketCache(marketId);
        }

        private void LinkMarkets(
            MarketCache market, CancellationToken cancellationToken)
        {
            _strategies.ForEach(s => s.LinkToMarket(market));
            _strategies.ForEach(s => s.WithCancellationToken(cancellationToken));
            _orderManager.LinkToMarket(market);
        }

        private async Task PlaceOrdersFromEachStrategy(
            ChangeMessage change, double bank)
        {
            var bankPerStrategy = bank / _strategies.Count;
            if (change.Operation == "mcm")
            {
                var allOrders = new List<LimitOrder>();
                change.MarketChanges.ForEach(
                    mc => _strategies.ForEach(
                        s =>
                        {
                            var o = s.GetOrders(mc, Math.Round(bankPerStrategy, 2));
                            if (o != null)
                                allOrders.AddRange(o);
                        }));

                if (allOrders.Count > 0)
                    await _orderManager.Place(allOrders);
            }
        }

        private async Task UpdateOrders(ChangeMessage change)
        {
            await _orderManager.OnChange(change);
        }

        private async Task Subscribe(string marketId)
        {
            var marketFilter = new MarketFilter().WithMarketId(marketId);

            _subscription.Connect();
            await _subscription.Authenticate();
            await _subscription.Subscribe(marketFilter, GetMergedDataFilters());
            await _subscription.SubscribeToOrders();
        }

        private void DisconnectStreamIfCancelled(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested) _subscription.Disconnect();
        }

        private MarketDataFilter GetMergedDataFilters()
        {
            var dataFiler = new MarketDataFilter();
            foreach (var marketDataFilter in
                _strategies.Select(s => s.DataFilter).ToList())
                dataFiler.Merge(marketDataFilter);

            return dataFiler;
        }
    }
}
