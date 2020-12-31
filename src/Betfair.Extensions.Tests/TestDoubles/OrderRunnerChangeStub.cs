using System.Collections.Generic;
using Betfair.Stream.Responses;

namespace Betfair.Extensions.Tests.TestDoubles
{
    public class OrderRunnerChangeStub : OrderRunnerChange
    {
        public OrderRunnerChangeStub(long selectionId = 12345)
        {
            SelectionId = selectionId;
        }

        public OrderRunnerChangeStub WithMatchedBack(double? price, double? size)
        {
            MatchedBacks ??= new List<List<double?>>();
            MatchedBacks.Add(new List<double?> { price, size });
            return this;
        }

        public OrderRunnerChangeStub WithMatchedLay(double? price, double? size)
        {
            MatchedLays ??= new List<List<double?>>();
            MatchedLays.Add(new List<double?> { price, size });
            return this;
        }

        public OrderRunnerChangeStub WithUnmatchedLay(
            double? price, double? size, string betId = "1", long? placedDate = 0)
        {
            UnmatchedOrders ??= new List<UnmatchedOrder>();
            var uo = new UnmatchedOrder
            {
                BetId = betId,
                Side = "L",
                Price = price,
                SizeRemaining = size,
                PlacedDate = placedDate,
            };
            UnmatchedOrders.Add(uo);
            return this;
        }

        public OrderRunnerChangeStub WithUnmatchedBack(
            double? price, double? size, string betId = "1", long? placedDate = 0, string orderStatus = "E")
        {
            UnmatchedOrders ??= new List<UnmatchedOrder>();
            var uo = new UnmatchedOrder
            {
                BetId = betId,
                Side = "B",
                Price = price,
                SizeRemaining = size,
                PlacedDate = placedDate,
                OrderStatus = orderStatus,
            };
            UnmatchedOrders.Add(uo);
            return this;
        }
    }
}
