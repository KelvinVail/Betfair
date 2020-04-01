namespace Betfair.Betting.Tests
{
    using System;
    using Xunit;

    public class LimitOrderTests
    {
        private readonly LimitOrder limitOrder = new LimitOrder("MarketId", 2147483648);

        [Fact]
        public void MarketIdIsSetInConstructor()
        {
            Assert.Equal("MarketId", this.limitOrder.MarketId);
        }

        [Fact]
        public void ThrowIfMarketIdIsNull()
        {
            var ex = Assert.Throws<ArgumentNullException>(() => new LimitOrder(null, -1));
            Assert.Equal("marketId", ex.ParamName);
            Assert.Equal("MarketId should not be null or empty. (Parameter 'marketId')", ex.Message);
        }

        [Fact]
        public void SelectionIdIsSetInConstructor()
        {
            Assert.Equal(2147483648, this.limitOrder.SelectionId);
        }

        [Fact]
        public void SideCanBeSetAsBack()
        {
            this.limitOrder.Side = Side.Back;
            Assert.Equal(Side.Back, this.limitOrder.Side);
        }

        [Fact]
        public void SideCanBeSetAsLay()
        {
            this.limitOrder.Side = Side.Lay;
            Assert.Equal(Side.Lay, this.limitOrder.Side);
        }
    }
}
