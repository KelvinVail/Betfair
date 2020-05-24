namespace Betfair.Stream.Tests
{
    using System;
    using Xunit;

    public class MarketDataFilterTests
    {
        private readonly MarketDataFilter marketDataFilter = new MarketDataFilter();

        [Fact]
        public void WhenInitializedLadderLevelsIsNull()
        {
            Assert.Null(this.marketDataFilter.LadderLevels);
        }

        [Fact]
        public void WhenInitializedFieldsIsNull()
        {
            Assert.Null(this.marketDataFilter.Fields);
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

        [Fact]
        public void MergeTwoFieldHashSets()
        {
            this.marketDataFilter.WithBestPrices();
            var newDataFilter = new MarketDataFilter().WithLastTradedPrice();
            this.marketDataFilter.Merge(newDataFilter);
            Assert.Contains("EX_BEST_OFFERS", this.marketDataFilter.Fields);
            Assert.Contains("EX_LTP", this.marketDataFilter.Fields);
            Assert.Equal(2, this.marketDataFilter.Fields.Count);
        }

        [Fact]
        public void HandleMergeIfOtherIsNull()
        {
            this.marketDataFilter.WithBestPrices();
            this.marketDataFilter.Merge(null);
            Assert.Contains("EX_BEST_OFFERS", this.marketDataFilter.Fields);
            Assert.Single(this.marketDataFilter.Fields);
        }

        [Fact]
        public void HandleMergeIfOtherFieldsAreNull()
        {
            this.marketDataFilter.WithBestPrices();
            var newDataFilter = new MarketDataFilter();
            this.marketDataFilter.Merge(newDataFilter);
            Assert.Contains("EX_BEST_OFFERS", this.marketDataFilter.Fields);
            Assert.Single(this.marketDataFilter.Fields);
        }

        [Fact]
        public void OnMergeHandleIfBothLadderLevelsAreNull()
        {
            this.marketDataFilter.Merge(new MarketDataFilter());
            Assert.Null(this.marketDataFilter.LadderLevels);
        }

        [Fact]
        public void HandleMergeIfThisFieldsAreNull()
        {
            var newDataFilter = new MarketDataFilter().WithBestPrices();
            this.marketDataFilter.Merge(newDataFilter);
            Assert.Contains("EX_BEST_OFFERS", this.marketDataFilter.Fields);
            Assert.Single(this.marketDataFilter.Fields);
        }

        [Theory]
        [InlineData(3)]
        [InlineData(6)]
        [InlineData(9)]
        public void OnMergeHandleIfThisLadderLevelIsNull(int levels)
        {
            var newDataFilter = new MarketDataFilter().WithLadderLevels(levels);
            this.marketDataFilter.Merge(newDataFilter);
            Assert.Equal(levels, this.marketDataFilter.LadderLevels);
        }

        [Theory]
        [InlineData(3)]
        [InlineData(6)]
        [InlineData(9)]
        public void OnMergeHandleIfOtherLadderLevelIsNull(int levels)
        {
            this.marketDataFilter.WithLadderLevels(levels);
            var newDataFilter = new MarketDataFilter();
            this.marketDataFilter.Merge(newDataFilter);
            Assert.Equal(levels, this.marketDataFilter.LadderLevels);
        }

        [Fact]
        public void MergeTwoLadderLevels()
        {
            this.marketDataFilter.WithLadderLevels(3);
            var newDataFilter = new MarketDataFilter().WithLadderLevels(5);
            this.marketDataFilter.Merge(newDataFilter);
            Assert.Equal(5, this.marketDataFilter.LadderLevels);
        }
    }
}
