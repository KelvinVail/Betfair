using System;
using System.Linq;
using Betfair.Stream.Responses;
using Xunit;

namespace Betfair.Extensions.Tests
{
    public class UnmatchedOrdersTests
    {
        private readonly UnmatchedOrders _unmatchedOrders = new UnmatchedOrders();

        [Fact]
        public void OnUpdateExceptsAnUnmatchedOrder()
        {
            _unmatchedOrders.Update(new UnmatchedOrder { BetId = "1" });
        }

        [Fact]
        public void OnUpdateThrowIfUnmatchedOrderIsNull()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => _unmatchedOrders.Update(null));
            Assert.Equal("unmatchedOrder", ex.ParamName);
        }

        [Fact]
        public void OnUpdateThrowIfBetIdIsNull()
        {
            var ex = Assert.Throws<ArgumentNullException>(
                () => _unmatchedOrders.Update(new UnmatchedOrder()));
            Assert.Equal("unmatchedOrder", ex.ParamName);
            Assert.StartsWith("BetId should not be null.", ex.Message, StringComparison.InvariantCulture);
        }

        [Theory]
        [InlineData("1", "B", "E", 9.99)]
        [InlineData("2", "L", "X", 20.50)]
        public void OnUpdateCachesTheUnmatchedOrder(
            string betId,
            string side,
            string orderStatus,
            double sizeRemaining)
        {
            AddUnmatchedOrder(betId, side, orderStatus, sizeRemaining);

            var order = _unmatchedOrders.ToList().Single();

            AssertOrderIsEqual(betId, side, orderStatus, sizeRemaining, order);
        }

        [Theory]
        [InlineData("1", "B", "E", 9.99)]
        [InlineData("2", "L", "X", 20.50)]
        public void OnUpdateAddsUnmatchedOrderToExistingUnmatchedOrders(
            string betId,
            string side,
            string orderStatus,
            double sizeRemaining)
        {
            AddUnmatchedOrder("999", "B", "E", 45.24);
            AddUnmatchedOrder(betId, side, orderStatus, sizeRemaining);

            var orders = _unmatchedOrders.ToList();
            Assert.Equal(2, orders.Count);

            var order = orders.Single(w => w.BetId == betId);
            AssertOrderIsEqual(betId, side, orderStatus, sizeRemaining, order);
        }

        [Theory]
        [InlineData("1", "B", "E", 9.99)]
        [InlineData("2", "L", "X", 20.50)]
        public void OnUpdateUnmatchedOrderIsUpdatedIfItExists(
            string betId,
            string side,
            string orderStatus,
            double sizeRemaining)
        {
            AddUnmatchedOrder(betId, side, orderStatus, 1000);
            AddUnmatchedOrder(betId, side, orderStatus, sizeRemaining);

            var order = _unmatchedOrders.ToList().Single();

            AssertOrderIsEqual(betId, side, orderStatus, sizeRemaining, order);
        }

        [Fact]
        public void OnUpdateRemoveTheOrderIfItIsCompleted()
        {
            AddUnmatchedOrder("1", "B", "E", 9.99);
            var order = _unmatchedOrders.ToList().Single();
            AssertOrderIsEqual("1", "B", "E", 9.99, order);

            AddUnmatchedOrder("1", "B", "EC", 9.99);

            Assert.Empty(_unmatchedOrders.ToList());
        }

        [Fact]
        public void OnUpdateOnlyCompletedOrdersAreRemoved()
        {
            AddUnmatchedOrder("1", "B", "E", 9.99);
            AddUnmatchedOrder("2", "L", "E", 20.50);
            AddUnmatchedOrder("1", "B", "EC", 9.99);

            var order = _unmatchedOrders.ToList().Single();

            AssertOrderIsEqual("2", "L", "E", 20.50, order);
        }

        [Fact]
        public void OnUpdateDoNotAddOrderIfItIsComplete()
        {
            AddUnmatchedOrder("1", "B", "EC", 9.99);
            Assert.Empty(_unmatchedOrders.ToList());
        }

        private static void AssertOrderIsEqual(
            string betId,
            string side,
            string orderStatus,
            double sizeRemaining,
            UnmatchedOrder order)
        {
            Assert.Equal(betId, order.BetId);
            Assert.Equal(side, order.Side);
            Assert.Equal(orderStatus, order.OrderStatus);
            Assert.Equal(sizeRemaining, order.SizeRemaining);
        }

        private void AddUnmatchedOrder(string betId, string side, string orderStatus, double sizeRemaining)
        {
            var uo = new UnmatchedOrder
            {
                BetId = betId,
                Side = side,
                OrderStatus = orderStatus,
                SizeRemaining = sizeRemaining,
            };

            _unmatchedOrders.Update(uo);
        }
    }
}
