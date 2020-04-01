namespace Betfair.Betting.Tests
{
    using System;
    using Xunit;

    public class OrdersTests
    {
        private readonly Orders orders = new Orders();

        [Theory]
        [InlineData("Ref")]
        [InlineData("StrategyRef")]
        [InlineData("MyOrders")]
        public void StrategyReferenceCanBeSetInConstructor(string reference)
        {
            var orders = new Orders(reference);
            Assert.Equal(reference, orders.StrategyRef);
        }

        [Theory]
        [InlineData("ThisIsLongerThanFifteenCharacters")]
        [InlineData("StrategyReference")]
        [InlineData("LotsAndLotsOfOrders")]
        public void ThrowIfStrategyReferenceIsOver15Characters(string reference)
        {
            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => new Orders(reference));
            Assert.Equal("strategyRef", ex.ParamName);
            Assert.Equal($"{reference} must be less than 15 characters. (Parameter 'strategyRef')", ex.Message);
        }

        [Fact]
        public void StrategyReferenceCanBeNull()
        {
            var nullOrders = new Orders(null);
            Assert.Null(nullOrders.StrategyRef);
        }

        [Fact]
        public void StrategyReferenceCanBeEmpty()
        {
            var emptyOrders = new Orders(string.Empty);
            Assert.Null(emptyOrders.StrategyRef);
        }
    }
}
