namespace Betfair.Tests.Stream
{
    using Betfair.Streaming;
    using Xunit;

    public class MarketDataFilterTests
    {
        private readonly MarketDataFilter marketDataFilter = new MarketDataFilter();

        [Fact]
        public void DefaultInitializationHasNoDataFilters()
        {
            Assert.Empty(this.marketDataFilter.Fields);
        }

        [Fact]
        public void DefaultInitializationHasZeroLadderLevels()
        {
            Assert.Equal(0, this.marketDataFilter.LadderLevels);
        }

        [Fact]
        public void CanBeInitializedWithMarketDefinition()
        {
            this.marketDataFilter.WithMarketDefinition();
            Assert.Contains("EX_MARKET_DEF", this.marketDataFilter.Fields);
        }

        [Fact]
        public void CanBeInitializedWithBestPricesIncludingVirtual()
        {
            this.marketDataFilter.WithBestPricesIncludingVirtual();
            Assert.Contains("EX_BEST_OFFERS_DISP", this.marketDataFilter.Fields);
        }

        [Fact]
        public void CanBeInitializedWithMultipleDataFields()
        {
            this.marketDataFilter.WithMarketDefinition();
            this.marketDataFilter.WithBestPricesIncludingVirtual();
            Assert.Contains("EX_MARKET_DEF", this.marketDataFilter.Fields);
            Assert.Contains("EX_BEST_OFFERS_DISP", this.marketDataFilter.Fields);
        }

        [Fact]
        public void WhenInitializedWithBestPricesIncludingVirtualLadderLevelIsDefaultedToThree()
        {
            this.marketDataFilter.WithBestPricesIncludingVirtual();
            Assert.Equal(3, this.marketDataFilter.LadderLevels);
        }

        [Fact]
        public void CanBeInitializedWithBestPrices()
        {
            this.marketDataFilter.WithBestPrices();
            Assert.Contains("EX_BEST_OFFERS", this.marketDataFilter.Fields);
        }

        [Fact]
        public void WhenInitializedWithBestPricesLadderLevelIsDefaultedToThree()
        {
            this.marketDataFilter.WithBestPrices();
            Assert.Equal(3, this.marketDataFilter.LadderLevels);
        }

        [Fact]
        public void CanBeInitializedWithFullOffersLadder()
        {
            this.marketDataFilter.WithFullOffersLadder();
            Assert.Contains("EX_ALL_OFFERS", this.marketDataFilter.Fields);
        }

        [Fact]
        public void CanBeInitializedWithFullTradedLadder()
        {
            this.marketDataFilter.WithFullTradedLadder();
            Assert.Contains("EX_TRADED", this.marketDataFilter.Fields);
        }

        [Fact]
        public void CanBeInitializedWithMarketAndRunnerTradedVolume()
        {
            this.marketDataFilter.WithMarketAndRunnerTradedVolume();
            Assert.Contains("EX_TRADED_VOL", this.marketDataFilter.Fields);
        }

        [Fact]
        public void CanBeInitializedWithLastTradedPrice()
        {
            this.marketDataFilter.WithLastTradedPrice();
            Assert.Contains("EX_LTP", this.marketDataFilter.Fields);
        }

        [Fact]
        public void CanBeInitializedWithStartingPriceLadder()
        {
            this.marketDataFilter.WithStartingPriceLadder();
            Assert.Contains("SP_TRADED", this.marketDataFilter.Fields);
        }

        [Fact]
        public void CanBeInitializedWithStartingPriceProjection()
        {
            this.marketDataFilter.WithStartingPriceProjection();
            Assert.Contains("SP_PROJECTED", this.marketDataFilter.Fields);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public void CanBeInitializedWithLadderLevel(int levels)
        {
            this.marketDataFilter.WithLadderLevels(levels);
            Assert.Equal(levels, this.marketDataFilter.LadderLevels);
        }

        [Fact]
        public void WhenInitializedWithLadderLevelGreaterThenTenLevelLadderIsDefaultedToTen()
        {
            this.marketDataFilter.WithLadderLevels(11);
            Assert.Equal(10, this.marketDataFilter.LadderLevels);
        }

        [Fact]
        public void WhenLaddersLevelsHaveBeenSetLadderLevelIsNotDefaultedByBestPrices()
        {
            this.marketDataFilter.WithLadderLevels(10);
            this.marketDataFilter.WithBestPrices();
            Assert.Equal(10, this.marketDataFilter.LadderLevels);
        }

        [Fact]
        public void WhenLaddersLevelsHaveBeenSetLadderLevelIsNotDefaultedByBestPricesIncludingVirtual()
        {
            this.marketDataFilter.WithLadderLevels(10);
            this.marketDataFilter.WithBestPricesIncludingVirtual();
            Assert.Equal(10, this.marketDataFilter.LadderLevels);
        }
    }
}
