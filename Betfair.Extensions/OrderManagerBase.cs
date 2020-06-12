namespace Betfair.Extensions
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Betfair.Betting;
    using Betfair.Stream.Responses;

    public abstract class OrderManagerBase
    {
        protected MarketCache Market { get; private set; }

        public abstract Task PlaceOrders(IEnumerable<LimitOrder> orders);

        public abstract Task OnChange(ChangeMessage change);

        public abstract Task OnMarketClose();

        public void LinkToMarket(MarketCache marketCache)
        {
            this.Market = marketCache;
        }
    }
}
