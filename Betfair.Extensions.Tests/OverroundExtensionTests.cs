namespace Betfair.Extensions.Tests
{
    using Betfair.Extensions.Tests.TestDoubles;
    using Xunit;

    public class OverroundExtensionTests
    {
        private readonly MarketCache market = new MarketCache("1.2345");

        private readonly ChangeMessageStub change = new ChangeMessageStub();

        [Fact]
        public void ReturnZeroIfNull()
        {
            Assert.Equal(0, OverroundExtension.Overround(null));
        }

        [Fact]
        public void ReturnZeroIfMarketIsEmpty()
        {
            Assert.Equal(0, this.market.Overround());
        }

        [Theory]
        [InlineData(2.5)]
        [InlineData(1.5)]
        [InlineData(100)]
        public void ReturnOverroundForSingleRunner(double price)
        {
            var rc = new RunnerChangeStub().WithSelectionId(1).WithBestAvailableToBack(0, price, 100);
            var mc = new MarketChangeStub().WithMarketId("1.2345").WithRunnerChange(rc);
            this.market.OnChange(this.change.WithMarketChange(mc).Build());
            var expected = 1 / price;

            Assert.Equal(expected, this.market.Overround());
        }

        [Theory]
        [InlineData(2.5, 3.5)]
        [InlineData(1.5, 100)]
        public void ReturnOverroundForMultipleRunners(double price1, double price2)
        {
            var rc = new RunnerChangeStub().WithSelectionId(1).WithBestAvailableToBack(0, price1, 100);
            var rc2 = new RunnerChangeStub().WithSelectionId(2).WithBestAvailableToBack(0, price2, 100);
            var mc = new MarketChangeStub().WithMarketId("1.2345").WithRunnerChange(rc).WithRunnerChange(rc2);
            this.market.OnChange(this.change.WithMarketChange(mc).Build());
            var expected = (1 / price1) + (1 / price2);

            Assert.Equal(expected, this.market.Overround());
        }

        [Theory]
        [InlineData(2.5)]
        [InlineData(1.5)]
        [InlineData(100)]
        public void ReturnOverroundOnlyIfRunnerHasBestPriceAvailable(double price)
        {
            var rc = new RunnerChangeStub().WithSelectionId(1).WithBestAvailableToBack(0, price, 100);
            var rc2 = new RunnerChangeStub().WithSelectionId(2).WithBestAvailableToBack(1, 2, 100);
            var rc3 = new RunnerChangeStub().WithSelectionId(3);
            var mc = new MarketChangeStub()
                .WithMarketId("1.2345")
                .WithRunnerChange(rc)
                .WithRunnerChange(rc2)
                .WithRunnerChange(rc3);
            this.market.OnChange(this.change.WithMarketChange(mc).Build());
            var expected = 1 / price;

            Assert.Equal(expected, this.market.Overround());
        }
    }
}
