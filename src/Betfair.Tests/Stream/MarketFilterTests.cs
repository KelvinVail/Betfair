//using Betfair.Betting;

//namespace Betfair.Tests.Stream;

//public class MarketFilterTests
//{
//    private readonly MarketFilter _marketFilter = new ();

//    [Fact]
//    public void WhenInitializedMarketIdsIsNull()
//    {
//        Assert.Null(_marketFilter.MarketIds);
//    }

//    [Fact]
//    public void CanBeInitializedWithMarketId()
//    {
//        _marketFilter.WithMarketId("1");
//        Assert.Contains("1", _marketFilter.MarketIds);
//    }

//    [Fact]
//    public void CanBeInitializedWithMultipleMarketIds()
//    {
//        _marketFilter.WithMarketId("1").WithMarketId("2");
//        Assert.Contains("1", _marketFilter.MarketIds);
//        Assert.Contains("2", _marketFilter.MarketIds);
//    }

//    [Fact]
//    public void WhenInitializedDoesNotAddDuplicateMarketIds()
//    {
//        _marketFilter.WithMarketId("1").WithMarketId("1");
//        Assert.Single(_marketFilter.MarketIds);
//    }

//    [Fact]
//    public void WhenInitializedCountryCodesIsNull()
//    {
//        Assert.Null(_marketFilter.CountryCodes);
//    }

//    [Theory]
//    [InlineData("CountryCode")]
//    [InlineData("IE")]
//    [InlineData("GB")]
//    public void CanBeInitializedWithCountryCodes(string countryCode)
//    {
//        _marketFilter.WithCountryCode(countryCode);
//        Assert.Contains(countryCode, _marketFilter.CountryCodes);
//    }

//    [Fact]
//    public void CanBeInitializedWithMultipleCountryCodes()
//    {
//        _marketFilter.WithCountryCode("GB");
//        _marketFilter.WithCountryCode("IE");
//        Assert.Contains("GB", _marketFilter.CountryCodes);
//        Assert.Contains("IE", _marketFilter.CountryCodes);
//    }

//    [Fact]
//    public void WhenInitializedDoesNotAddDuplicateCountryCodes()
//    {
//        _marketFilter.WithCountryCode("GB");
//        _marketFilter.WithCountryCode("GB");
//        Assert.Single(_marketFilter.CountryCodes);
//    }

//    [Fact]
//    public void WhenInitializedTurnInPlayEnabledIsNullByDefault()
//    {
//        Assert.Null(_marketFilter.TurnInPlayEnabled);
//    }

//    [Fact]
//    public void CanBeInitializedWithInPlayMarketsOnly()
//    {
//        _marketFilter.WithInPlayMarketsOnly();
//        Assert.True(_marketFilter.TurnInPlayEnabled);
//    }

//    [Fact]
//    public void WhenInitializedMarketTypesIsNull()
//    {
//        Assert.Null(_marketFilter.MarketTypes);
//    }

//    [Theory]
//    [InlineData("MarketType")]
//    [InlineData("MATCH_ODDS")]
//    [InlineData("HALF_TIME_SCORE")]
//    public void CanBeInitializedWithMarketTypes(string marketType)
//    {
//        _marketFilter.WithMarketType(marketType);
//        Assert.Contains(marketType, _marketFilter.MarketTypes);
//    }

//    [Fact]
//    public void CanBeInitializedWithMultipleMarketTypes()
//    {
//        _marketFilter.WithMarketType("MATCH_ODDS");
//        _marketFilter.WithMarketType("HALF_TIME_SCORE");
//        Assert.Contains("MATCH_ODDS", _marketFilter.MarketTypes);
//        Assert.Contains("HALF_TIME_SCORE", _marketFilter.MarketTypes);
//    }

//    [Fact]
//    public void WhenInitializedDoesNotAddDuplicateMarketTypes()
//    {
//        _marketFilter.WithMarketType("MATCH_ODDS");
//        _marketFilter.WithMarketType("MATCH_ODDS");
//        Assert.Single(_marketFilter.MarketTypes);
//    }

//    [Fact]
//    public void WhenInitializedVenuesIsNull()
//    {
//        Assert.Null(_marketFilter.Venues);
//    }

//    [Theory]
//    [InlineData("Venue")]
//    [InlineData("Ascot")]
//    [InlineData("Epsom")]
//    public void CanBeInitializedWithVenue(string venue)
//    {
//        _marketFilter.WithVenue(venue);
//        Assert.Contains(venue, _marketFilter.Venues);
//    }

//    [Fact]
//    public void CanBeInitializedWithMultipleVenues()
//    {
//        _marketFilter.WithVenue("Ascot");
//        _marketFilter.WithVenue("Epsom");
//        Assert.Contains("Ascot", _marketFilter.Venues);
//        Assert.Contains("Epsom", _marketFilter.Venues);
//    }

//    [Fact]
//    public void WhenInitializedDoesNotAddDuplicateVenues()
//    {
//        _marketFilter.WithVenue("Ascot");
//        _marketFilter.WithVenue("Ascot");
//        Assert.Single(_marketFilter.Venues);
//    }

//    [Fact]
//    public void WhenInitializedEventTypeIdsIsNull()
//    {
//        Assert.Null(_marketFilter.EventTypeIds);
//    }

//    [Theory]
//    [InlineData("EventTypeId")]
//    [InlineData("1")]
//    [InlineData("7")]
//    public void CanBeInitializedWithEventTypeId(string eventTypeId)
//    {
//        _marketFilter.WithEventTypeId(eventTypeId);
//        Assert.Contains(eventTypeId, _marketFilter.EventTypeIds);
//    }

//    [Fact]
//    public void CanBeInitializedWithMultipleEventTypeIds()
//    {
//        _marketFilter.WithEventTypeId("1");
//        _marketFilter.WithEventTypeId("2");
//        Assert.Contains("1", _marketFilter.EventTypeIds);
//        Assert.Contains("2", _marketFilter.EventTypeIds);
//    }

//    [Fact]
//    public void WhenInitializedDoesNotAddDuplicateEventTypeIds()
//    {
//        _marketFilter.WithEventTypeId("1");
//        _marketFilter.WithEventTypeId("1");
//        Assert.Single(_marketFilter.EventTypeIds);
//    }

//    [Fact]
//    public void WhenInitializedEventIdsIsNull()
//    {
//        Assert.Null(_marketFilter.EventIds);
//    }

//    [Theory]
//    [InlineData("EventId")]
//    [InlineData("12345")]
//    [InlineData("98765")]
//    public void CanBeInitializedWithEventId(string eventId)
//    {
//        _marketFilter.WithEventId(eventId);
//        Assert.Contains(eventId, _marketFilter.EventIds);
//    }

//    [Fact]
//    public void CanBeInitializedWithMultipleEventIds()
//    {
//        _marketFilter.WithEventId("12345");
//        _marketFilter.WithEventId("98765");
//        Assert.Contains("12345", _marketFilter.EventIds);
//        Assert.Contains("98765", _marketFilter.EventIds);
//    }

//    [Fact]
//    public void WhenInitializedDoesNotAddDuplicateEventIds()
//    {
//        _marketFilter.WithEventId("12345");
//        _marketFilter.WithEventId("12345");
//        Assert.Single(_marketFilter.EventIds);
//    }

//    [Fact]
//    public void WhenInitializedBspMarketIsNullByDefault()
//    {
//        Assert.Null(_marketFilter.BspMarket);
//    }

//    [Fact]
//    public void CanBeInitializedWithBspMarketsOnly()
//    {
//        _marketFilter.WithBspMarketsOnly();
//        Assert.True(_marketFilter.BspMarket);
//    }

//    [Fact]
//    public void WhenInitializedBettingTypesIsNull()
//    {
//        Assert.Null(_marketFilter.BettingTypes);
//    }

//    [Fact]
//    public void CanBeInitializedWithBettingTypeOdds()
//    {
//        _marketFilter.WithBettingTypeOdds();
//        Assert.Contains("ODDS", _marketFilter.BettingTypes);
//    }

//    [Fact]
//    public void CanBeInitializedWithBettingTypeLine()
//    {
//        _marketFilter.WithBettingTypeLine();
//        Assert.Contains("LINE", _marketFilter.BettingTypes);
//    }

//    [Fact]
//    public void CanBeInitializedWithBettingTypeRange()
//    {
//        _marketFilter.WithBettingTypeRange();
//        Assert.Contains("RANGE", _marketFilter.BettingTypes);
//    }

//    [Fact]
//    public void CanBeInitializedWithBettingTypeAsianHandicapDoubleLine()
//    {
//        _marketFilter.WithBettingTypeAsianHandicapDoubleLine();
//        Assert.Contains("ASIAN_HANDICAP_DOUBLE_LINE", _marketFilter.BettingTypes);
//    }

//    [Fact]
//    public void CanBeInitializedWithBettingTypeAsianHandicapSingleLine()
//    {
//        _marketFilter.WithBettingTypeAsianHandicapSingleLine();
//        Assert.Contains("ASIAN_HANDICAP_SINGLE_LINE", _marketFilter.BettingTypes);
//    }

//    [Fact]
//    public void CanBeInitializedWithMultipleBettingTypes()
//    {
//        _marketFilter.WithBettingTypeOdds();
//        _marketFilter.WithBettingTypeLine();
//        Assert.Contains("ODDS", _marketFilter.BettingTypes);
//        Assert.Contains("LINE", _marketFilter.BettingTypes);
//    }

//    [Fact]
//    public void WhenInitializedDoesNotAddDuplicateBettingTypes()
//    {
//        _marketFilter.WithBettingTypeOdds();
//        _marketFilter.WithBettingTypeOdds();
//        Assert.Single(_marketFilter.BettingTypes);
//    }
//}