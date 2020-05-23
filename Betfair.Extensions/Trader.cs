namespace Betfair.Extensions
{
    using System.Threading;
    using System.Threading.Tasks;
    using Betfair.Exchange.Interfaces;
    using Betfair.Stream;

    public sealed class Trader
    {
        private readonly ISubscription subscription;

        public Trader(IExchangeService exchange, ISubscription subscription)
        {
            this.subscription = subscription;
        }

        public async Task TradeMarketAsync(string marketId, CancellationToken cancellationToken)
        {
            this.subscription.Connect();
            await this.subscription.Authenticate();
            await this.subscription.Subscribe(new MarketFilter(), new MarketDataFilter());
            await this.subscription.SubscribeToOrders();
            await foreach (var change in this.subscription.GetChanges().WithCancellation(cancellationToken))
            {
                if (cancellationToken.IsCancellationRequested) this.subscription.Disconnect();
            }
        }
    }
}
