namespace Betfair.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Betfair.Stream.Responses;

    public class UnmatchedOrders
    {
        private readonly Dictionary<string, UnmatchedOrder> unmatchedOrders
            = new Dictionary<string, UnmatchedOrder>();

        public void Update(UnmatchedOrder unmatchedOrder)
        {
            var uo = ValidateOrder(unmatchedOrder);
            this.AddOrderToCache(uo);
            this.RemoveOrderIfComplete(uo);
        }

        public IList<UnmatchedOrder> ToList()
        {
            return this.unmatchedOrders.Values.ToList();
        }

        private static UnmatchedOrder ValidateOrder(UnmatchedOrder unmatchedOrder)
        {
            if (unmatchedOrder is null) throw new ArgumentNullException(nameof(unmatchedOrder));
            if (unmatchedOrder.BetId is null)
                throw new ArgumentNullException(nameof(unmatchedOrder), "BetId should not be null.");

            return unmatchedOrder;
        }

        private void AddOrderToCache(UnmatchedOrder uo)
        {
            if (!this.unmatchedOrders.ContainsKey(uo.BetId))
                this.unmatchedOrders.Add(uo.BetId, null);
            this.unmatchedOrders[uo.BetId] = uo;
        }

        private void RemoveOrderIfComplete(UnmatchedOrder uo)
        {
            if (uo.OrderStatus == "EC")
                this.unmatchedOrders.Remove(uo.BetId);
        }
    }
}
