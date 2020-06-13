namespace Betfair.Extensions
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Betfair.Betting;
    using Betfair.Stream.Responses;

    public abstract class OrderManagerBase
    {
        private readonly IOrderService orderService;

        protected OrderManagerBase(IOrderService orderService)
        {
            this.orderService = orderService;
        }

        protected MarketCache Market { get; private set; }

        public abstract Task OnChange(ChangeMessage change);

        public abstract Task OnMarketClose();

        public virtual async Task Place(IEnumerable<LimitOrder> orders, string strategyRef = null)
        {
            await this.orderService.Place(this.Market.MarketId, orders.ToList(), strategyRef);
        }

        public void LinkToMarket(MarketCache marketCache)
        {
            this.Market = marketCache;
        }
    }
}
