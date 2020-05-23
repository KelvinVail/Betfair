namespace Betfair.Extensions
{
    using System;
    using System.Collections.Generic;
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
            await this.Subscribe();
            await foreach (var change in this.subscription.GetChanges().WithCancellation(cancellationToken))
            {
                this.DisconnectIfCancelled(cancellationToken);
                if (cancellationToken.IsCancellationRequested) break;

                this.strategies.ForEach(async s => await s.OnChange(change));
            }
        }

        private async Task Subscribe()
        {
            if (this.strategies.Count == 0) throw new InvalidOperationException();
            this.subscription.Connect();
            await this.subscription.Authenticate();
            await this.subscription.Subscribe(new MarketFilter(), new MarketDataFilter());
            await this.subscription.SubscribeToOrders();
        }

        private void DisconnectIfCancelled(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested) this.subscription.Disconnect();
        }
    }
}
