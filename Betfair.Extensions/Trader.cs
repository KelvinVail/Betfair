namespace Betfair.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Betfair.Betting;
    using Betfair.Stream;
    using Betfair.Stream.Responses;

    public sealed class Trader
    {
        private readonly ISubscription subscription;

        private readonly List<StrategyBase> strategies = new List<StrategyBase>();

        private OrderManagerBase orderManager;

        public Trader(ISubscription subscription)
        {
            this.subscription = subscription;
        }

        public void AddStrategy(StrategyBase strategy)
        {
            this.strategies.Add(strategy);
        }

        public void SetOrderManager(OrderManagerBase manager)
        {
            this.orderManager = manager;
        }

        public async Task TradeMarket(
            string marketId, double bank, CancellationToken cancellationToken)
        {
            var market = this.CreateMarketCache(marketId);
            this.LinkMarkets(market, cancellationToken);
            await this.Subscribe(marketId);
            await foreach (var change in this.subscription.GetChanges())
            {
                UpdateMarketCache(change, market);
                await this.PlaceOrdersFromEachStrategy(change, bank + market.Liability);
                await this.UpdateOrders(change);

                if (this.CheckCancellationToken(cancellationToken)) break;
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
            if (this.strategies.Count == 0)
                throw new InvalidOperationException("Trader must contain at least one strategy.");
            if (this.orderManager is null)
                throw new InvalidOperationException("Trader must contain an OrderManager.");
            return new MarketCache(marketId);
        }

        private void LinkMarkets(
            MarketCache market, CancellationToken cancellationToken)
        {
            this.strategies.ForEach(s => s.LinkToMarket(market));
            this.strategies.ForEach(s => s.WithCancellationToken(cancellationToken));
            this.orderManager.LinkToMarket(market);
        }

        private async Task PlaceOrdersFromEachStrategy(
            ChangeMessage change, double bank)
        {
            var bankPerStrategy = bank / this.strategies.Count;
            if (change.Operation == "mcm")
            {
                var allOrders = new List<LimitOrder>();
                change.MarketChanges.ForEach(
                    mc => this.strategies.ForEach(
                        s =>
                        {
                            var o = s.GetOrders(mc, Math.Round(bankPerStrategy, 2));
                            if (o != null)
                                allOrders.AddRange(o);
                        }));

                if (allOrders.Count > 0)
                    await this.orderManager.Place(allOrders);
            }
        }

        private async Task UpdateOrders(ChangeMessage change)
        {
            await this.orderManager.OnChange(change);
        }

        private bool CheckCancellationToken(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                this.orderManager.OnMarketClose();
            this.DisconnectStreamIfCancelled(cancellationToken);
            return cancellationToken.IsCancellationRequested;
        }

        private async Task Subscribe(string marketId)
        {
            var marketFilter = new MarketFilter().WithMarketId(marketId);

            this.subscription.Connect();
            await this.subscription.Authenticate();
            await this.subscription.Subscribe(marketFilter, this.GetMergedDataFilters());
            await this.subscription.SubscribeToOrders();
        }

        private void DisconnectStreamIfCancelled(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested) this.subscription.Disconnect();
        }

        private MarketDataFilter GetMergedDataFilters()
        {
            var dataFiler = new MarketDataFilter();
            foreach (var marketDataFilter in
                this.strategies.Select(s => s.DataFilter).ToList())
                dataFiler.Merge(marketDataFilter);

            return dataFiler;
        }
    }
}
