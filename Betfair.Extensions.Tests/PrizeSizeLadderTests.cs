namespace Betfair.Extensions.Tests
{
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
    }
}
