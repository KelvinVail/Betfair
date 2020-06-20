namespace Betfair.Extensions
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Betfair.Betting;
    using Betfair.Stream.Responses;

    public abstract class OrderManagerBase
    {
        protected OrderManagerBase(IOrderService orderService)
        {
            this.OrderService = orderService;
        }

        protected MarketCache Market { get; private set; }

        protected IOrderService OrderService { get; }

        public abstract Task OnChange(ChangeMessage change);

        public abstract Task OnMarketClose();

        public virtual async Task Place(IEnumerable<LimitOrder> orders, string strategyRef = null)
        {
            if (orders is null) return;
            var limitOrders = orders.ToList();
            if (!limitOrders.Any()) return;
            await this.OrderService.Place(this.Market.MarketId, limitOrders, strategyRef);
        }

        public void LinkToMarket(MarketCache marketCache)
        {
            this.Market = marketCache;
        }
    }
}
