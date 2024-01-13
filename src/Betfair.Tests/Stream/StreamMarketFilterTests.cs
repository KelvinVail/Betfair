using Betfair.Core;
using Betfair.Stream.Messages;

namespace Betfair.Tests.Stream;

public class StreamMarketFilterTests
{
    private readonly StreamMarketFilter _filter = new ();

    [Fact]
    public void WhenInitializedMarketIdsIsNull() =>
        _filter.MarketIds.Should().BeNull();

    [Theory]
    [InlineData("1")]
    [InlineData("1.234567")]
    public void CanBeInitializedWithMarketId(string marketId)
    {
        _filter.IncludeMarketIds(marketId);

        _filter.MarketIds.Should().Contain(marketId);
    }

    [Theory]
    [InlineData("1")]
    [InlineData("1.234567")]
    public void CanBeInitializedWithMultipleMarketIds(string marketId)
    {
        _filter.IncludeMarketIds(marketId).IncludeMarketIds("2");

        _filter.MarketIds.Should().Contain(marketId);
        _filter.MarketIds.Should().Contain("2");
    }

    [Fact]
    public void CanNotBeCreatedWithDuplicateMarketIds()
    {
        _filter.IncludeMarketIds("1").IncludeMarketIds("1");

        _filter.MarketIds?.Count.Should().Be(1);
        _filter.MarketIds.Should().Contain("1");
    }

    [Fact]
    public void WhenInitializedCountryCodesIsNull() =>
        _filter.CountryCodes.Should().BeNull();

    [Theory]
    [InlineData("CountryCode")]
    [InlineData("IE")]
    [InlineData("GB")]
    public void CanBeInitializedWithCountryCodes(string countryCode)
    {
        _filter.IncludeCountries(countryCode);

        _filter.CountryCodes.Should().Contain(countryCode);
    }

    [Fact]
    public void CanBeInitializedWithMultipleCountryCodes()
    {
        _filter.IncludeCountries("GB");
        _filter.IncludeCountries("IE");

        _filter.CountryCodes.Should().Contain("GB");
        _filter.CountryCodes.Should().Contain("IE");
    }

    [Fact]
    public void CanNotBeCreatedWithDuplicateCountryCodes()
    {
        _filter.IncludeCountries("GB");
        _filter.IncludeCountries("GB");

        _filter.CountryCodes?.Count.Should().Be(1);
        _filter.CountryCodes.Should().Contain("GB");
    }

    [Fact]
    public void WhenInitializedTurnInPlayEnabledIsNullByDefault() =>
        _filter.TurnInPlayEnabled.Should().BeNull();

    [Fact]
    public void CanBeInitializedWithInPlayMarketsOnly()
    {
        _filter.IncludeInPlayMarketsOnly();

        _filter.TurnInPlayEnabled.Should().BeTrue();
    }

    [Fact]
    public void CanBeInitializedToExcludeInPlayMarkets()
    {
        _filter.ExcludeInPlayMarkets();

        _filter.TurnInPlayEnabled.Should().BeFalse();
    }

    [Fact]
    public void WhenInitializedMarketTypesIsNull() =>
        _filter.MarketTypes.Should().BeNull();

    [Theory]
    [InlineData("MarketType")]
    [InlineData("MATCH_ODDS")]
    [InlineData("HALF_TIME_SCORE")]
    public void CanBeInitializedWithMarketTypes(string marketType)
    {
        _filter.IncludeMarketTypes(marketType);

        _filter.MarketTypes.Should().Contain(marketType);
    }

    [Fact]
    public void CanBeInitializedWithMultipleMarketTypes()
    {
        _filter.IncludeMarketTypes("MATCH_ODDS");
        _filter.IncludeMarketTypes("HALF_TIME_SCORE");

        _filter.MarketTypes.Should().Contain("MATCH_ODDS");
        _filter.MarketTypes.Should().Contain("HALF_TIME_SCORE");
    }

    [Fact]
    public void CanNotBeCreatedWithDuplicateMarketTypes()
    {
        _filter.IncludeMarketTypes("MATCH_ODDS");
        _filter.IncludeMarketTypes("MATCH_ODDS");

        _filter.MarketTypes?.Count.Should().Be(1);
        _filter.MarketTypes.Should().Contain("MATCH_ODDS");
    }

    [Fact]
    public void WhenInitializedVenuesIsNull() =>
        _filter.Venues.Should().BeNull();

    [Fact]
    public void CanBeInitializedWithVenues()
    {
        _filter.IncludeVenues(Venue.Albany, Venue.AliceSprings);

        _filter.Venues.Should().Contain(Venue.Albany.Id);
        _filter.Venues.Should().Contain(Venue.AliceSprings.Id);
    }

    [Fact]
    public void AddingNullVenueHasNoEffect()
    {
        _filter.IncludeVenues(Venue.Albany, null!);

        _filter.Venues.Should().Contain(Venue.Albany.Id);
        _filter.Venues.Should().NotContainNulls();
    }

    [Fact]
    public void AddingNullVenueIdHasNoEffect()
    {
        _filter.IncludeVenues("Albany", null!);

        _filter.Venues.Should().Contain("Albany");
        _filter.Venues.Should().NotContainNulls();
    }

    [Fact]
    public void AddingNullVenueArray()
    {
        _filter.IncludeVenues((Venue[])null!);

        _filter.Venues.Should().BeNull();
    }

    [Fact]
    public void AddingNullVenueIdArrayHasNoEffect()
    {
        _filter.IncludeVenues((string[])null!);

        _filter.Venues.Should().BeNull();
    }

    [Theory]
    [InlineData("Venue")]
    [InlineData("Ascot")]
    [InlineData("Epsom")]
    public void CanBeInitializedWithVenueId(string venue)
    {
        _filter.IncludeVenues(venue);

        _filter.Venues.Should().Contain(venue);
    }

    [Fact]
    public void CanBeInitializedWithMultipleVenues()
    {
        _filter.IncludeVenues("Ascot");
        _filter.IncludeVenues("Epsom");

        _filter.Venues.Should().Contain("Ascot");
        _filter.Venues.Should().Contain("Epsom");
    }

    [Fact]
    public void CanNotBeCreatedWithDuplicateVenues()
    {
        _filter.IncludeVenues("Ascot");
        _filter.IncludeVenues("Ascot");

        _filter.Venues?.Count.Should().Be(1);
        _filter.Venues.Should().Contain("Ascot");
    }

    [Fact]
    public void WhenInitializedEventTypeIdsIsNull() =>
        _filter.EventTypeIds.Should().BeNull();

    [Theory]
    [InlineData(1)]
    [InlineData(7)]
    public void CanBeInitializedWithEventTypeId(int eventTypeId)
    {
        _filter.IncludeEventTypes(eventTypeId);

        _filter.EventTypeIds.Should().Contain(eventTypeId);
    }

    [Fact]
    public void CanBeInitializedWithMultipleEventTypeIds()
    {
        _filter.IncludeEventTypes(1);
        _filter.IncludeEventTypes(2);

        _filter.EventTypeIds.Should().Contain(1);
        _filter.EventTypeIds.Should().Contain(2);
    }

    [Fact]
    public void WhenInitializedDoesNotAddDuplicateEventTypeIds()
    {
        _filter.IncludeEventTypes(1);
        _filter.IncludeEventTypes(1);

        _filter.EventTypeIds?.Count.Should().Be(1);
        _filter.EventTypeIds.Should().Contain(1);
    }

    [Fact]
    public void WhenInitializedEventIdsIsNull() =>
        _filter.EventIds.Should().BeNull();

    [Fact]
    public void AddingNullEventIdsHasNoEffect()
    {
        _filter.IncludeEventIds("12345", null!);

        _filter.EventIds.Should().Contain("12345");
        _filter.EventIds.Should().NotContainNulls();
    }

    [Fact]
    public void AddingNullEventIdArrayHasNoEffect()
    {
        _filter.IncludeEventIds(null!);

        _filter.EventIds.Should().BeNull();
    }

    [Theory]
    [InlineData("EventId")]
    [InlineData("12345")]
    [InlineData("98765")]
    public void CanBeInitializedWithEventId(string eventId)
    {
        _filter.IncludeEventIds(eventId);

        _filter.EventIds.Should().Contain(eventId);
    }

    [Fact]
    public void CanBeInitializedWithMultipleEventIds()
    {
        _filter.IncludeEventIds("12345");
        _filter.IncludeEventIds("98765");

        _filter.EventIds.Should().Contain("12345");
        _filter.EventIds.Should().Contain("98765");
    }

    [Fact]
    public void WhenInitializedDoesNotAddDuplicateEventIds()
    {
        _filter.IncludeEventIds("12345");
        _filter.IncludeEventIds("12345");

        _filter.EventIds?.Count.Should().Be(1);
        _filter.EventIds.Should().Contain("12345");
    }

    [Fact]
    public void WhenInitializedBspMarketIsNullByDefault() =>
        _filter.BspMarket.Should().BeNull();

    [Fact]
    public void CanBeInitializedWithBspMarketsOnly()
    {
        _filter.IncludeBspMarketsOnly();

        _filter.BspMarket.Should().BeTrue();
    }

    [Fact]
    public void CanBeInitializedWithExcludingBspMarkets()
    {
        _filter.ExcludeBspMarkets();

        _filter.BspMarket.Should().BeFalse();
    }

    [Fact]
    public void WhenInitializedBettingTypesIsNull() =>
        _filter.BettingTypes.Should().BeNull();

    [Fact]
    public void CanBeInitializedWithBettingTypes()
    {
        _filter.IncludeBettingTypes(BettingType.Odds, BettingType.Line);

        _filter.BettingTypes.Should().Contain(BettingType.Odds.Id);
        _filter.BettingTypes.Should().Contain(BettingType.Line.Id);
    }

    [Fact]
    public void AddNullBettingTypeHasNoEffect()
    {
        _filter.IncludeBettingTypes(BettingType.Odds, null!);

        _filter.BettingTypes.Should().Contain(BettingType.Odds.Id);
        _filter.BettingTypes.Should().NotContainNulls();
    }

    [Fact]
    public void AddNullBettingTypeIdHasNoEffect()
    {
        _filter.IncludeBettingTypes("ODDS", null!);

        _filter.BettingTypes.Should().Contain("ODDS");
        _filter.BettingTypes.Should().NotContainNulls();
    }

    [Fact]
    public void AddNullBettingTypeArrayHasNoEffect()
    {
        _filter.IncludeBettingTypes((BettingType[])null!);

        _filter.BettingTypes.Should().BeNull();
    }

    [Fact]
    public void AddNullBettingTypeIdArrayHasNoEffect()
    {
        _filter.IncludeBettingTypes((string[])null!);

        _filter.BettingTypes.Should().BeNull();
    }

    [Theory]
    [InlineData("ODDS")]
    [InlineData("LINE")]
    public void CanBeInitializedWithBettingTypeOddsIds(string bettingType)
    {
        _filter.IncludeBettingTypes(bettingType);

        _filter.BettingTypes.Should().Contain(bettingType);
    }

    [Fact]
    public void CanBeInitializedWithMultipleBettingTypeOdds()
    {
        _filter.IncludeBettingTypes("ODDS");
        _filter.IncludeBettingTypes("LINE");

        _filter.BettingTypes.Should().Contain("ODDS");
        _filter.BettingTypes.Should().Contain("LINE");
    }

    [Fact]
    public void WhenInitializedDoesNotAddDuplicateBettingTypes()
    {
        _filter.IncludeBettingTypes("ODDS");
        _filter.IncludeBettingTypes("ODDS");

        _filter.BettingTypes?.Count.Should().Be(1);
        _filter.BettingTypes.Should().Contain("ODDS");
    }
}