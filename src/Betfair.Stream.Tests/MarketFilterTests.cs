namespace Betfair.Stream.Tests
{
    using Xunit;

    public class MarketFilterTests
    {
        private readonly MarketFilter marketFilter = new MarketFilter();

        [Fact]
        public void WhenInitializedMarketIdsIsNull()
        {
            Assert.Null(this.marketFilter.MarketIds);
        }

        [Fact]
        public void CanBeInitializedWithMarketId()
        {
            this.marketFilter.WithMarketId("1");
            Assert.Contains("1", this.marketFilter.MarketIds);
        }

        [Fact]
        public void CanBeInitializedWithMultipleMarketIds()
        {
            this.marketFilter.WithMarketId("1").WithMarketId("2");
            Assert.Contains("1", this.marketFilter.MarketIds);
            Assert.Contains("2", this.marketFilter.MarketIds);
        }

        [Fact]
        public void WhenInitializedDoesNotAddDuplicateMarketIds()
        {
            this.marketFilter.WithMarketId("1").WithMarketId("1");
            Assert.Single(this.marketFilter.MarketIds);
        }

        [Fact]
        public void WhenInitializedCountryCodesIsNull()
        {
            Assert.Null(this.marketFilter.CountryCodes);
        }

        [Theory]
        [InlineData("CountryCode")]
        [InlineData("IE")]
        [InlineData("GB")]
        public void CanBeInitializedWithCountryCodes(string countryCode)
        {
            this.marketFilter.WithCountryCode(countryCode);
            Assert.Contains(countryCode, this.marketFilter.CountryCodes);
        }

        [Fact]
        public void CanBeInitializedWithMultipleCountryCodes()
        {
            this.marketFilter.WithCountryCode("GB");
            this.marketFilter.WithCountryCode("IE");
            Assert.Contains("GB", this.marketFilter.CountryCodes);
            Assert.Contains("IE", this.marketFilter.CountryCodes);
        }

        [Fact]
        public void WhenInitializedDoesNotAddDuplicateCountryCodes()
        {
            this.marketFilter.WithCountryCode("GB");
            this.marketFilter.WithCountryCode("GB");
            Assert.Single(this.marketFilter.CountryCodes);
        }

        [Fact]
        public void WhenInitializedTurnInPlayEnabledIsNullByDefault()
        {
            Assert.Null(this.marketFilter.TurnInPlayEnabled);
        }

        [Fact]
        public void CanBeInitializedWithInPlayMarketsOnly()
        {
            this.marketFilter.WithInPlayMarketsOnly();
            Assert.True(this.marketFilter.TurnInPlayEnabled);
        }

        [Fact]
        public void WhenInitializedMarketTypesIsNull()
        {
            Assert.Null(this.marketFilter.MarketTypes);
        }

        [Theory]
        [InlineData("MarketType")]
        [InlineData("MATCH_ODDS")]
        [InlineData("HALF_TIME_SCORE")]
        public void CanBeInitializedWithMarketTypes(string marketType)
        {
            this.marketFilter.WithMarketType(marketType);
            Assert.Contains(marketType, this.marketFilter.MarketTypes);
        }

        [Fact]
        public void CanBeInitializedWithMultipleMarketTypes()
        {
            this.marketFilter.WithMarketType("MATCH_ODDS");
            this.marketFilter.WithMarketType("HALF_TIME_SCORE");
            Assert.Contains("MATCH_ODDS", this.marketFilter.MarketTypes);
            Assert.Contains("HALF_TIME_SCORE", this.marketFilter.MarketTypes);
        }

        [Fact]
        public void WhenInitializedDoesNotAddDuplicateMarketTypes()
        {
            this.marketFilter.WithMarketType("MATCH_ODDS");
            this.marketFilter.WithMarketType("MATCH_ODDS");
            Assert.Single(this.marketFilter.MarketTypes);
        }

        [Fact]
        public void WhenInitializedVenuesIsNull()
        {
            Assert.Null(this.marketFilter.Venues);
        }

        [Theory]
        [InlineData("Venue")]
        [InlineData("Ascot")]
        [InlineData("Epsom")]
        public void CanBeInitializedWithVenue(string venue)
        {
            this.marketFilter.WithVenue(venue);
            Assert.Contains(venue, this.marketFilter.Venues);
        }

        [Fact]
        public void CanBeInitializedWithMultipleVenues()
        {
            this.marketFilter.WithVenue("Ascot");
            this.marketFilter.WithVenue("Epsom");
            Assert.Contains("Ascot", this.marketFilter.Venues);
            Assert.Contains("Epsom", this.marketFilter.Venues);
        }

        [Fact]
        public void WhenInitializedDoesNotAddDuplicateVenues()
        {
            this.marketFilter.WithVenue("Ascot");
            this.marketFilter.WithVenue("Ascot");
            Assert.Single(this.marketFilter.Venues);
        }

        [Fact]
        public void WhenInitializedEventTypeIdsIsNull()
        {
            Assert.Null(this.marketFilter.EventTypeIds);
        }

        [Theory]
        [InlineData("EventTypeId")]
        [InlineData("1")]
        [InlineData("7")]
        public void CanBeInitializedWithEventTypeId(string eventTypeId)
        {
            this.marketFilter.WithEventTypeId(eventTypeId);
            Assert.Contains(eventTypeId, this.marketFilter.EventTypeIds);
        }

        [Fact]
        public void CanBeInitializedWithMultipleEventTypeIds()
        {
            this.marketFilter.WithEventTypeId("1");
            this.marketFilter.WithEventTypeId("2");
            Assert.Contains("1", this.marketFilter.EventTypeIds);
            Assert.Contains("2", this.marketFilter.EventTypeIds);
        }

        [Fact]
        public void WhenInitializedDoesNotAddDuplicateEventTypeIds()
        {
            this.marketFilter.WithEventTypeId("1");
            this.marketFilter.WithEventTypeId("1");
            Assert.Single(this.marketFilter.EventTypeIds);
        }

        [Fact]
        public void WhenInitializedEventIdsIsNull()
        {
            Assert.Null(this.marketFilter.EventIds);
        }

        [Theory]
        [InlineData("EventId")]
        [InlineData("12345")]
        [InlineData("98765")]
        public void CanBeInitializedWithEventId(string eventId)
        {
            this.marketFilter.WithEventId(eventId);
            Assert.Contains(eventId, this.marketFilter.EventIds);
        }

        [Fact]
        public void CanBeInitializedWithMultipleEventIds()
        {
            this.marketFilter.WithEventId("12345");
            this.marketFilter.WithEventId("98765");
            Assert.Contains("12345", this.marketFilter.EventIds);
            Assert.Contains("98765", this.marketFilter.EventIds);
        }

        [Fact]
        public void WhenInitializedDoesNotAddDuplicateEventIds()
        {
            this.marketFilter.WithEventId("12345");
            this.marketFilter.WithEventId("12345");
            Assert.Single(this.marketFilter.EventIds);
        }

        [Fact]
        public void WhenInitializedBspMarketIsNullByDefault()
        {
            Assert.Null(this.marketFilter.BspMarket);
        }

        [Fact]
        public void CanBeInitializedWithBspMarketsOnly()
        {
            this.marketFilter.WithBspMarketsOnly();
            Assert.True(this.marketFilter.BspMarket);
        }

        [Fact]
        public void WhenInitializedBettingTypesIsNull()
        {
            Assert.Null(this.marketFilter.BettingTypes);
        }

        [Fact]
        public void CanBeInitializedWithBettingTypeOdds()
        {
            this.marketFilter.WithBettingTypeOdds();
            Assert.Contains("ODDS", this.marketFilter.BettingTypes);
        }

        [Fact]
        public void CanBeInitializedWithBettingTypeLine()
        {
            this.marketFilter.WithBettingTypeLine();
            Assert.Contains("LINE", this.marketFilter.BettingTypes);
        }

        [Fact]
        public void CanBeInitializedWithBettingTypeRange()
        {
            this.marketFilter.WithBettingTypeRange();
            Assert.Contains("RANGE", this.marketFilter.BettingTypes);
        }

        [Fact]
        public void CanBeInitializedWithBettingTypeAsianHandicapDoubleLine()
        {
            this.marketFilter.WithBettingTypeAsianHandicapDoubleLine();
            Assert.Contains("ASIAN_HANDICAP_DOUBLE_LINE", this.marketFilter.BettingTypes);
        }

        [Fact]
        public void CanBeInitializedWithBettingTypeAsianHandicapSingleLine()
        {
            this.marketFilter.WithBettingTypeAsianHandicapSingleLine();
            Assert.Contains("ASIAN_HANDICAP_SINGLE_LINE", this.marketFilter.BettingTypes);
        }

        [Fact]
        public void CanBeInitializedWithMultipleBettingTypes()
        {
            this.marketFilter.WithBettingTypeOdds();
            this.marketFilter.WithBettingTypeLine();
            Assert.Contains("ODDS", this.marketFilter.BettingTypes);
            Assert.Contains("LINE", this.marketFilter.BettingTypes);
        }

        [Fact]
        public void WhenInitializedDoesNotAddDuplicateBettingTypes()
        {
            this.marketFilter.WithBettingTypeOdds();
            this.marketFilter.WithBettingTypeOdds();
            Assert.Single(this.marketFilter.BettingTypes);
        }
    }
}
