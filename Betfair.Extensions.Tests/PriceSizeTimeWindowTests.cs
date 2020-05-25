namespace Betfair.Extensions.Tests
{
    using Betfair.Extensions;
    using Xunit;

    public class PriceSizeTimeWindowTests
    {
        private readonly PriceSizeTimeWindow tenSecondWindow = new PriceSizeTimeWindow(10);

        [Fact]
        public void AverageSizeInLastTenSecondsIsRecorded()
        {
            this.tenSecondWindow.Update(1.01, 1, 1000);
            this.tenSecondWindow.Update(1.01, 2, 5000);
            this.tenSecondWindow.Update(1.01, 1, 12000);

            Assert.Equal(1.5, this.tenSecondWindow.MeanSize);

            this.tenSecondWindow.Update(1.01, 2, 13000);

            Assert.Equal(1.67, this.tenSecondWindow.MeanSize, 2);
        }

        [Fact]
        public void AverageWeightedSizeInLastTenSecondsIsRecorded()
        {
            this.tenSecondWindow.Update(1.01, 1, 1000);
            this.tenSecondWindow.Update(5.00, 2, 5000);
            this.tenSecondWindow.Update(1.01, 1, 12000);
            Assert.Equal(3.67, this.tenSecondWindow.Vwap, 2);

            this.tenSecondWindow.Update(5.00, 2, 13000);
            Assert.Equal(4.2, this.tenSecondWindow.Vwap, 2);

            this.tenSecondWindow.Update(1.01, 1, 16000);
            Assert.Equal(3.0, this.tenSecondWindow.Vwap, 2);
        }

        [Fact]
        public void WhenMultiplePricesAddedWithTheSameTimeSizeIsAccumulated()
        {
            this.tenSecondWindow.Update(1.01, 1, 1000);
            this.tenSecondWindow.Update(5.00, 2, 1000);
            Assert.Equal(3, this.tenSecondWindow.MeanSize, 2);
            Assert.Equal(3.67, this.tenSecondWindow.Vwap, 2);
        }

        [Fact]
        public void AsSizesAreAddedSizePerSecondIsRecorded()
        {
            this.tenSecondWindow.Update(1.01, 10, 1000);
            this.tenSecondWindow.Update(5.00, 20, 5000);
            this.tenSecondWindow.Update(1.01, 30, 12000);
            Assert.Equal(5.0, this.tenSecondWindow.SizePerSecond);
        }
    }
}
