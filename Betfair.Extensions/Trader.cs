namespace Betfair.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Betfair.Stream;
    using Betfair.Stream.Responses;

    public sealed class Trader
    {
        private readonly ISubscription subscription;

        private readonly List<StrategyBase> strategies = new List<StrategyBase>();

        public Trader(ISubscription subscription)
        {
            this.subscription = subscription;
        }

        public void AddStrategy(StrategyBase strategy)
        {
            this.strategies.Add(strategy);
        }

        public async Task TradeMarket(string marketId, double bank, CancellationToken cancellationToken)
        {
            var market = this.CreateMarketCache(marketId);
            this.LinkMarketToEachStrategy(market, cancellationToken);
            await this.Subscribe(marketId);
            await foreach (var change in this.subscription.GetChanges())
            {
                UpdateMarketCache(change, market);
                this.GetOrdersFromEachStrategy(change);

                if (this.OnCancellation(cancellationToken)) break;
            }
        }

        private static void UpdateMarketCache(ChangeMessage change, MarketCache market)
        {
            if (change.Operation == "mcm")
            {
                change.MarketChanges.ForEach(mc => market.OnChange(change));
            }
        }

        private MarketCache CreateMarketCache(string marketId)
        {
            if (string.IsNullOrEmpty(marketId))
                throw new ArgumentNullException(nameof(marketId));
            if (this.strategies.Count == 0)
                throw new InvalidOperationException("Trader must contain at least one strategy.");
            return new MarketCache(marketId);
        }

        private void LinkMarketToEachStrategy(MarketCache market, CancellationToken cancellationToken)
        {
            this.strategies.ForEach(s => s.LinkToMarket(market));
            this.strategies.ForEach(s => s.WithCancellationToken(cancellationToken));
        }

        private void GetOrdersFromEachStrategy(ChangeMessage change)
        {
            if (change.Operation == "mcm")
            {
                change.MarketChanges.ForEach(mc => this.strategies.ForEach(s => s.GetOrders(mc, 0)));
            }
        }

        private bool OnCancellation(CancellationToken cancellationToken)
        {
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
            foreach (var marketDataFilter in this.strategies.Select(s => s.DataFilter).ToList())
            {
                dataFiler.Merge(marketDataFilter);
            }

            return dataFiler;
        }
    }
}
