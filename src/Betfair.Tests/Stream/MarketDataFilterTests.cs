using Betfair.Stream;

namespace Betfair.Tests.Stream
{
    public class MarketDataFilterTests
    {
        private readonly MarketDataFilter _marketDataFilter = new MarketDataFilter();

        [Fact]
        public void WhenInitializedLadderLevelsIsNull()
        {
            Assert.Null(_marketDataFilter.LadderLevels);
        }

        [Fact]
        public void WhenInitializedFieldsIsNull()
        {
            Assert.Null(_marketDataFilter.Fields);
        }

        [Fact]
        public void CanBeInitializedWithMarketDefinition()
        {
            _marketDataFilter.WithMarketDefinition();
            Assert.Contains("EX_MARKET_DEF", _marketDataFilter.Fields);
        }

        [Fact]
        public void CanBeInitializedWithBestPricesIncludingVirtual()
        {
            _marketDataFilter.WithBestPricesIncludingVirtual();
            Assert.Contains("EX_BEST_OFFERS_DISP", _marketDataFilter.Fields);
        }

        [Fact]
        public void CanBeInitializedWithMultipleDataFields()
        {
            _marketDataFilter.WithMarketDefinition();
            _marketDataFilter.WithBestPricesIncludingVirtual();
            Assert.Contains("EX_MARKET_DEF", _marketDataFilter.Fields);
            Assert.Contains("EX_BEST_OFFERS_DISP", _marketDataFilter.Fields);
        }

        [Fact]
        public void WhenInitializedWithBestPricesIncludingVirtualLadderLevelIsDefaultedToThree()
        {
            _marketDataFilter.WithBestPricesIncludingVirtual();
            Assert.Equal(3, _marketDataFilter.LadderLevels);
        }

        [Fact]
        public void CanBeInitializedWithBestPrices()
        {
            _marketDataFilter.WithBestPrices();
            Assert.Contains("EX_BEST_OFFERS", _marketDataFilter.Fields);
        }

        [Fact]
        public void WhenInitializedWithBestPricesLadderLevelIsDefaultedToThree()
        {
            _marketDataFilter.WithBestPrices();
            Assert.Equal(3, _marketDataFilter.LadderLevels);
        }

        [Fact]
        public void CanBeInitializedWithFullOffersLadder()
        {
            _marketDataFilter.WithFullOffersLadder();
            Assert.Contains("EX_ALL_OFFERS", _marketDataFilter.Fields);
        }

        [Fact]
        public void CanBeInitializedWithFullTradedLadder()
        {
            _marketDataFilter.WithFullTradedLadder();
            Assert.Contains("EX_TRADED", _marketDataFilter.Fields);
        }

        [Fact]
        public void CanBeInitializedWithMarketAndRunnerTradedVolume()
        {
            _marketDataFilter.WithMarketAndRunnerTradedVolume();
            Assert.Contains("EX_TRADED_VOL", _marketDataFilter.Fields);
        }

        [Fact]
        public void CanBeInitializedWithLastTradedPrice()
        {
            _marketDataFilter.WithLastTradedPrice();
            Assert.Contains("EX_LTP", _marketDataFilter.Fields);
        }

        [Fact]
        public void CanBeInitializedWithStartingPriceLadder()
        {
            _marketDataFilter.WithStartingPriceLadder();
            Assert.Contains("SP_TRADED", _marketDataFilter.Fields);
        }

        [Fact]
        public void CanBeInitializedWithStartingPriceProjection()
        {
            _marketDataFilter.WithStartingPriceProjection();
            Assert.Contains("SP_PROJECTED", _marketDataFilter.Fields);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public void CanBeInitializedWithLadderLevel(int levels)
        {
            _marketDataFilter.WithLadderLevels(levels);
            Assert.Equal(levels, _marketDataFilter.LadderLevels);
        }

        [Fact]
        public void WhenInitializedWithLadderLevelGreaterThenTenLevelLadderIsDefaultedToTen()
        {
            _marketDataFilter.WithLadderLevels(11);
            Assert.Equal(10, _marketDataFilter.LadderLevels);
        }

        [Fact]
        public void WhenLaddersLevelsHaveBeenSetLadderLevelIsNotDefaultedByBestPrices()
        {
            _marketDataFilter.WithLadderLevels(10);
            _marketDataFilter.WithBestPrices();
            Assert.Equal(10, _marketDataFilter.LadderLevels);
        }

        [Fact]
        public void WhenLaddersLevelsHaveBeenSetLadderLevelIsNotDefaultedByBestPricesIncludingVirtual()
        {
            _marketDataFilter.WithLadderLevels(10);
            _marketDataFilter.WithBestPricesIncludingVirtual();
            Assert.Equal(10, _marketDataFilter.LadderLevels);
        }

        [Fact]
        public void MergeTwoFieldHashSets()
        {
            _marketDataFilter.WithBestPrices();
            var newDataFilter = new MarketDataFilter().WithLastTradedPrice();
            _marketDataFilter.Merge(newDataFilter);
            Assert.Contains("EX_BEST_OFFERS", _marketDataFilter.Fields);
            Assert.Contains("EX_LTP", _marketDataFilter.Fields);
            Assert.Equal(2, _marketDataFilter.Fields.Count);
        }

        [Fact]
        public void HandleMergeIfOtherIsNull()
        {
            _marketDataFilter.WithBestPrices();
            _marketDataFilter.Merge(null);
            Assert.Contains("EX_BEST_OFFERS", _marketDataFilter.Fields);
            Assert.Single(_marketDataFilter.Fields);
        }

        [Fact]
        public void HandleMergeIfOtherFieldsAreNull()
        {
            _marketDataFilter.WithBestPrices();
            var newDataFilter = new MarketDataFilter();
            _marketDataFilter.Merge(newDataFilter);
            Assert.Contains("EX_BEST_OFFERS", _marketDataFilter.Fields);
            Assert.Single(_marketDataFilter.Fields);
        }

        [Fact]
        public void OnMergeHandleIfBothLadderLevelsAreNull()
        {
            _marketDataFilter.Merge(new MarketDataFilter());
            Assert.Null(_marketDataFilter.LadderLevels);
        }

        [Fact]
        public void HandleMergeIfThisFieldsAreNull()
        {
            var newDataFilter = new MarketDataFilter().WithBestPrices();
            _marketDataFilter.Merge(newDataFilter);
            Assert.Contains("EX_BEST_OFFERS", _marketDataFilter.Fields);
            Assert.Single(_marketDataFilter.Fields);
        }

        [Theory]
        [InlineData(3)]
        [InlineData(6)]
        [InlineData(9)]
        public void OnMergeHandleIfThisLadderLevelIsNull(int levels)
        {
            var newDataFilter = new MarketDataFilter().WithLadderLevels(levels);
            _marketDataFilter.Merge(newDataFilter);
            Assert.Equal(levels, _marketDataFilter.LadderLevels);
        }

        [Theory]
        [InlineData(3)]
        [InlineData(6)]
        [InlineData(9)]
        public void OnMergeHandleIfOtherLadderLevelIsNull(int levels)
        {
            _marketDataFilter.WithLadderLevels(levels);
            var newDataFilter = new MarketDataFilter();
            _marketDataFilter.Merge(newDataFilter);
            Assert.Equal(levels, _marketDataFilter.LadderLevels);
        }

        [Fact]
        public void MergeTwoLadderLevels()
        {
            _marketDataFilter.WithLadderLevels(3);
            var newDataFilter = new MarketDataFilter().WithLadderLevels(5);
            _marketDataFilter.Merge(newDataFilter);
            Assert.Equal(5, _marketDataFilter.LadderLevels);
        }
    }
}
