namespace Betfair.Extensions
{
    using System.Threading.Tasks;
    using Betfair.Stream;
    using Betfair.Stream.Responses;

    public abstract class StrategyBase
    {
        public abstract MarketDataFilter DataFilter { get; }

        protected MarketCache Market { get; private set; }

        public void LinkToMarket(MarketCache marketCache)
        {
            this.Market = marketCache;
        }

        public abstract Task OnMarketUpdate(MarketChange marketChange);
    }
}
