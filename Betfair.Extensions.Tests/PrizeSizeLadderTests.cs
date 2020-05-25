namespace Betfair.Extensions.Tests
{
    using System.Linq;
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

        [Fact]
        public void TotalSizeIsRecorded()
        {
            var priceSizes = new PriceSizesStub()
                .WithPriceSize(1.01, 2.00)
                .WithPriceSize(1.02, 5.00);
            this.priceSizeLadder.Update(priceSizes, 0);

            Assert.Equal(7.00, this.priceSizeLadder.TotalSize, 2);
        }

        [Fact]
        public void VwapIsRecorded()
        {
            var priceSizes = new PriceSizesStub()
                .WithPriceSize(1.01, 2.00)
                .WithPriceSize(1.02, 5.00)
                .WithPriceSize(5.00, 20.00)
                .WithPriceSize(50, 3.99);
            this.priceSizeLadder.Update(priceSizes, 0);

            var weightedPrice = priceSizes.Sum(n => n[0] * n[1]);
            var totalSize = priceSizes.Sum(s => s[1]);
            var expectedVwap = weightedPrice / totalSize;

            Assert.Equal(expectedVwap, this.priceSizeLadder.Vwap, 2);
        }

        [Fact]
        public void MostTradedPriceIsRecorded()
        {
            var priceSizes = new PriceSizesStub()
                .WithPriceSize(1.01, 2.00)
                .WithPriceSize(1.02, 5.00)
                .WithPriceSize(5.00, 20.00)
                .WithPriceSize(50, 3.99);
            this.priceSizeLadder.Update(priceSizes, 0);

            Assert.Equal(5.00, this.priceSizeLadder.PriceWithMostSize, 2);
        }

        [Fact]
        public void WhenPricesSizesAreNullDoNotUpdateTimeWindows()
        {
            var priceSizes = new PriceSizesStub().WithPriceSize(1.01, 1);

            this.priceSizeLadder.Update(priceSizes, 1000);
            Assert.Equal(1, this.priceSizeLadder.TenSecondWindow.MeanSize, 2);

            this.priceSizeLadder.Update(null, 20000);
            Assert.Equal(1, this.priceSizeLadder.TenSecondWindow.MeanSize, 2);
        }

        [Fact]
        public void TenSecondWindowIsUpdated()
        {
            this.AddSizeToLadder(5, 0);
            this.AssertTenSecondMeanIs(5);

            this.AddSizeToLadder(10, 11);
            this.AssertTenSecondMeanIs(10);
        }

        [Fact]
        public void TwentySecondWindowIsUpdated()
        {
            this.AddSizeToLadder(5, 0);
            this.AssertTwentySecondMeanIs(5);

            this.AddSizeToLadder(10, 10);
            this.AssertTwentySecondMeanIs(7.5);

            this.AddSizeToLadder(20, 21);
            this.AssertTwentySecondMeanIs(15);
        }

        [Fact]
        public void ThirtySecondWindowIsUpdated()
        {
            this.AddSizeToLadder(10, 0);
            this.AddSizeToLadder(10, 10);
            this.AddSizeToLadder(20, 20);
            this.AddSizeToLadder(50, 31);
            this.AssertThirtySecondMeanIs(26.67);
        }

        private void AddSizeToLadder(double size, int seconds)
        {
            var priceSizes1 = new PriceSizesStub().WithPriceSize(1.01, size);
            this.priceSizeLadder.Update(priceSizes1, seconds * 1000);
        }

        private void AssertTenSecondMeanIs(double value)
        {
            Assert.Equal(value, this.priceSizeLadder.TenSecondWindow.MeanSize, 2);
        }

        private void AssertTwentySecondMeanIs(double value)
        {
            Assert.Equal(value, this.priceSizeLadder.TwentySecondWindow.MeanSize, 2);
        }

        private void AssertThirtySecondMeanIs(double value)
        {
            Assert.Equal(value, this.priceSizeLadder.ThirtySecondWindow.MeanSize, 2);
        }
    }
}
