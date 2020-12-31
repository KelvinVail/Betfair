using System.Collections.Generic;
using System.Threading;
using Betfair.Betting;
using Betfair.Stream;
using Betfair.Stream.Responses;

namespace Betfair.Extensions
{
    public abstract class StrategyBase
    {
        public abstract MarketDataFilter DataFilter { get; }

        protected MarketCache Market { get; private set; }

        protected CancellationToken CancellationToken { get; private set; }

        public void WithCancellationToken(CancellationToken cancellationToken)
        {
            CancellationToken = cancellationToken;
        }

        public void LinkToMarket(MarketCache marketCache)
        {
            Market = marketCache;
        }

        public abstract List<LimitOrder> GetOrders(MarketChange marketChange, double stake);
    }
}
