using System;
using System.Collections.Generic;
using System.Linq;
using Betfair.Stream.Responses;

namespace Betfair.Extensions
{
    public class UnmatchedOrders
    {
        private readonly Dictionary<string, UnmatchedOrder> _unmatchedOrders
            = new Dictionary<string, UnmatchedOrder>();

        public void Update(UnmatchedOrder unmatchedOrder)
        {
            var uo = ValidateOrder(unmatchedOrder);
            AddOrderToCache(uo);
            RemoveOrderIfComplete(uo);
        }

        public IList<UnmatchedOrder> ToList()
        {
            return _unmatchedOrders.Values.ToList();
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
            if (!_unmatchedOrders.ContainsKey(uo.BetId))
                _unmatchedOrders.Add(uo.BetId, null);
            _unmatchedOrders[uo.BetId] = uo;
        }

        private void RemoveOrderIfComplete(UnmatchedOrder uo)
        {
            if (uo.OrderStatus == "EC")
                _unmatchedOrders.Remove(uo.BetId);
        }
    }
}
