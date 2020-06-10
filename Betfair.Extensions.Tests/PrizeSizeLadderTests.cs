namespace Betfair.Extensions.Tests
{
    using System;
    using Betfair.Extensions;
    using Betfair.Extensions.Tests.TestDoubles;
    using Xunit;

    public class PrizeSizeLadderTests
    {
        private readonly PriceSizeLadder priceSizeLadder = new PriceSizeLadder();

        [Fact]
        public void UpdateTheLadder()
        {
            var priceSizes = new PriceSizesStub().WithPriceSize(1.01, 2.00);
            this.priceSizeLadder.Update(priceSizes, 0);

            Assert.Equal(2.00, this.priceSizeLadder.GetSizeForPrice(1.01), 2);
        }

        [Fact]
        public void OverwriteTheLadder()
        {
            var priceSizes = new PriceSizesStub().WithPriceSize(1.01, 2.00);
            this.priceSizeLadder.Update(priceSizes, 0);
            var priceSizes2 = new PriceSizesStub().WithPriceSize(1.01, 3.00);
            this.priceSizeLadder.Update(priceSizes2, 0);

            Assert.Equal(3.00, this.priceSizeLadder.GetSizeForPrice(1.01), 2);
        }

        [Fact]
        public void IfLadderDoesNotContainPrizeReturnZeroSize()
        {
            Assert.Equal(0.00, this.priceSizeLadder.GetSizeForPrice(1.01), 2);
        }

        [Fact]
        public void WithMultiplePriceSizesUpdateTheLadderWithEach()
        {
            var priceSizes = new PriceSizesStub()
                .WithPriceSize(1.01, 2.00)
                .WithPriceSize(1.02, 5.00);
            this.priceSizeLadder.Update(priceSizes, 0);

            Assert.Equal(2.00, this.priceSizeLadder.GetSizeForPrice(1.01), 2);
            Assert.Equal(5.00, this.priceSizeLadder.GetSizeForPrice(1.02), 2);
        }

        [Theory]
        [InlineData(1.01, 2.00)]
        [InlineData(3.5, 10.99)]
        [InlineData(100, 99.99)]
        public void CalculateTotalReturn(double price, double size)
        {
            var priceSizes = new PriceSizesStub()
                .WithPriceSize(price, size)
                .WithPriceSize(12, 20.78);
            this.priceSizeLadder.Update(priceSizes, 0);
            var expected = Math.Round((price * size) - size, 2);
            expected += Math.Round((12 * 20.78) - 20.78, 2);

            Assert.Equal(expected, this.priceSizeLadder.TotalReturn());
        }

        [Theory]
        [InlineData(1.01, 2.00)]
        [InlineData(3.5, 10.99)]
        [InlineData(100, 99.99)]
        public void CalculateTotalSize(double price, double size)
        {
            var priceSizes = new PriceSizesStub()
                .WithPriceSize(price, size)
                .WithPriceSize(12, 20.78);
            this.priceSizeLadder.Update(priceSizes, 0);
            var expected = size + 20.78;

            Assert.Equal(expected, this.priceSizeLadder.TotalSize());
        }

        [Fact]
        public void HandleNullPrice()
        {
            var priceSizes = new PriceSizesStub()
                .WithPriceSize(null, 10.99);
            this.priceSizeLadder.Update(priceSizes, 0);

            Assert.Equal(0, this.priceSizeLadder.GetSizeForPrice(0));
        }

        [Fact]
        public void HandleNullSize()
        {
            var priceSizes = new PriceSizesStub()
                .WithPriceSize(1.01, null);
            this.priceSizeLadder.Update(priceSizes, 0);

            Assert.Equal(0, this.priceSizeLadder.GetSizeForPrice(1.01));
        }
    }
}
