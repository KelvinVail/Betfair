namespace Betfair.Extensions.Tests
{
    using System;
    using System.Linq;
    using Betfair.Stream.Responses;
    using Xunit;

    public class UnmatchedOrdersTests
    {
        private readonly UnmatchedOrders unmatchedOrders = new UnmatchedOrders();

        [Fact]
        public void OnUpdateExceptsAnUnmatchedOrder()
        {
            this.unmatchedOrders.Update(new UnmatchedOrder { BetId = "1" });
        }

        [Fact]
        public void OnUpdateThrowIfUnmatchedOrderIsNull()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => this.unmatchedOrders.Update(null));
            Assert.Equal("unmatchedOrder", ex.ParamName);
        }

        [Fact]
        public void OnUpdateThrowIfBetIdIsNull()
        {
            var ex = Assert.Throws<ArgumentNullException>(
                () => this.unmatchedOrders.Update(new UnmatchedOrder()));
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
            this.AddUnmatchedOrder(betId, side, orderStatus, sizeRemaining);

            var order = this.unmatchedOrders.ToList().Single();

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
            this.AddUnmatchedOrder("999", "B", "E", 45.24);
            this.AddUnmatchedOrder(betId, side, orderStatus, sizeRemaining);

            var orders = this.unmatchedOrders.ToList();
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
            this.AddUnmatchedOrder(betId, side, orderStatus, 1000);
            this.AddUnmatchedOrder(betId, side, orderStatus, sizeRemaining);

            var order = this.unmatchedOrders.ToList().Single();

            AssertOrderIsEqual(betId, side, orderStatus, sizeRemaining, order);
        }

        [Fact]
        public void OnUpdateRemoveTheOrderIfItIsCompleted()
        {
            this.AddUnmatchedOrder("1", "B", "E", 9.99);
            var order = this.unmatchedOrders.ToList().Single();
            AssertOrderIsEqual("1", "B", "E", 9.99, order);

            this.AddUnmatchedOrder("1", "B", "EC", 9.99);

            Assert.Empty(this.unmatchedOrders.ToList());
        }

        [Fact]
        public void OnUpdateOnlyCompletedOrdersAreRemoved()
        {
            this.AddUnmatchedOrder("1", "B", "E", 9.99);
            this.AddUnmatchedOrder("2", "L", "E", 20.50);
            this.AddUnmatchedOrder("1", "B", "EC", 9.99);

            var order = this.unmatchedOrders.ToList().Single();

            AssertOrderIsEqual("2", "L", "E", 20.50, order);
        }

        [Fact]
        public void OnUpdateDoNotAddOrderIfItIsComplete()
        {
            this.AddUnmatchedOrder("1", "B", "EC", 9.99);
            Assert.Empty(this.unmatchedOrders.ToList());
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

            this.unmatchedOrders.Update(uo);
        }
    }
}
