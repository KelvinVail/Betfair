namespace Betfair.Extensions
{
    using System.Threading;
    using System.Threading.Tasks;
    using Betfair.Stream;
    using Betfair.Stream.Responses;

    public abstract class StrategyBase
    {
        public abstract MarketDataFilter DataFilter { get; }

        protected MarketCache Market { get; private set; }

        protected CancellationToken CancellationToken { get; private set; }

        public void WithCancellationToken(CancellationToken cancellationToken)
        {
            this.CancellationToken = cancellationToken;
        }

        public void LinkToMarket(MarketCache marketCache)
        {
            this.Market = marketCache;
        }

        public abstract Task OnMarketUpdate(MarketChange marketChange);
    }
}
