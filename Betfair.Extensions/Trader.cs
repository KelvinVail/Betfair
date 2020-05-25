﻿namespace Betfair.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Betfair.Stream;

    public sealed class Trader
    {
        private readonly ISubscription subscription;

        private readonly List<IStrategy> strategies = new List<IStrategy>();

        public Trader(ISubscription subscription)
        {
            this.subscription = subscription;
        }

        public void AddStrategy(IStrategy strategy)
        {
            this.strategies.Add(strategy);
        }

        public async Task TradeMarket(string marketId, CancellationToken cancellationToken)
        {
            this.PassCancellationTokenToStrategies(cancellationToken);
            await this.Subscribe(marketId);
            await foreach (var change in this.subscription.GetChanges().WithCancellation(cancellationToken))
            {
                this.DisconnectStreamIfCancelled(cancellationToken);
                this.strategies.ForEach(async s => await s.OnChange(change));
                if (cancellationToken.IsCancellationRequested) break;
            }
        }

        private async Task Subscribe(string marketId)
        {
            if (string.IsNullOrEmpty(marketId)) throw new ArgumentNullException(nameof(marketId));
            if (this.strategies.Count == 0) throw new InvalidOperationException("Trader must contain at least one strategy.");

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

        private void PassCancellationTokenToStrategies(CancellationToken cancellationToken)
        {
            this.strategies.ForEach(s => s.WithCancellationToken(cancellationToken));
        }
    }
}