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
        _filter.WithMarketIds(marketId);

        _filter.MarketIds.Should().Contain(marketId);
    }

    [Theory]
    [InlineData("1")]
    [InlineData("1.234567")]
    public void CanBeInitializedWithMultipleMarketIds(string marketId)
    {
        _filter.WithMarketIds(marketId).WithMarketIds("2");

        _filter.MarketIds.Should().Contain(marketId);
        _filter.MarketIds.Should().Contain("2");
    }

    [Fact]
    public void CanNotBeCreatedWithDuplicateMarketIds()
    {
        _filter.WithMarketIds("1").WithMarketIds("1");

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
        _filter.WithCountries(countryCode);

        _filter.CountryCodes.Should().Contain(countryCode);
    }

    [Fact]
    public void CanBeInitializedWithMultipleCountryCodes()
    {
        _filter.WithCountries("GB");
        _filter.WithCountries("IE");

        _filter.CountryCodes.Should().Contain("GB");
        _filter.CountryCodes.Should().Contain("IE");
    }

    [Fact]
    public void CanNotBeCreatedWithDuplicateCountryCodes()
    {
        _filter.WithCountries("GB");
        _filter.WithCountries("GB");

        _filter.CountryCodes?.Count.Should().Be(1);
        _filter.CountryCodes.Should().Contain("GB");
    }

    [Fact]
    public void WhenInitializedTurnInPlayEnabledIsNullByDefault() =>
        _filter.TurnInPlayEnabled.Should().BeNull();

    [Fact]
    public void CanBeInitializedWithInPlayMarketsOnly()
    {
        _filter.WithInPlayMarketsOnly();

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
        _filter.WithMarketTypes(marketType);

        _filter.MarketTypes.Should().Contain(marketType);
    }

    [Fact]
    public void CanBeInitializedWithMultipleMarketTypes()
    {
        _filter.WithMarketTypes("MATCH_ODDS");
        _filter.WithMarketTypes("HALF_TIME_SCORE");

        _filter.MarketTypes.Should().Contain("MATCH_ODDS");
        _filter.MarketTypes.Should().Contain("HALF_TIME_SCORE");
    }

    [Fact]
    public void CanNotBeCreatedWithDuplicateMarketTypes()
    {
        _filter.WithMarketTypes("MATCH_ODDS");
        _filter.WithMarketTypes("MATCH_ODDS");

        _filter.MarketTypes?.Count.Should().Be(1);
        _filter.MarketTypes.Should().Contain("MATCH_ODDS");
    }

    [Fact]
    public void WhenInitializedVenuesIsNull() =>
        _filter.Venues.Should().BeNull();

    [Fact]
    public void CanBeInitializedWithVenues()
    {
        _filter.WithVenues(Venue.Albany, Venue.AliceSprings);

        _filter.Venues.Should().Contain(Venue.Albany.Id);
        _filter.Venues.Should().Contain(Venue.AliceSprings.Id);
    }

    [Fact]
    public void AddingNullVenueHasNoEffect()
    {
        _filter.WithVenues(Venue.Albany, null!);

        _filter.Venues.Should().Contain(Venue.Albany.Id);
        _filter.Venues.Should().NotContainNulls();
    }

    [Fact]
    public void AddingNullVenueIdHasNoEffect()
    {
        _filter.WithVenues("Albany", null!);

        _filter.Venues.Should().Contain("Albany");
        _filter.Venues.Should().NotContainNulls();
    }

    [Fact]
    public void AddingNullVenueArray()
    {
        _filter.WithVenues((Venue[])null!);

        _filter.Venues.Should().BeNull();
    }

    [Fact]
    public void AddingNullVenueIdArrayHasNoEffect()
    {
        _filter.WithVenues((string[])null!);

        _filter.Venues.Should().BeNull();
    }

    [Theory]
    [InlineData("Venue")]
    [InlineData("Ascot")]
    [InlineData("Epsom")]
    public void CanBeInitializedWithVenueId(string venue)
    {
        _filter.WithVenues(venue);

        _filter.Venues.Should().Contain(venue);
    }

    [Fact]
    public void CanBeInitializedWithMultipleVenues()
    {
        _filter.WithVenues("Ascot");
        _filter.WithVenues("Epsom");

        _filter.Venues.Should().Contain("Ascot");
        _filter.Venues.Should().Contain("Epsom");
    }

    [Fact]
    public void CanNotBeCreatedWithDuplicateVenues()
    {
        _filter.WithVenues("Ascot");
        _filter.WithVenues("Ascot");

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
        _filter.WithEventTypes(eventTypeId);

        _filter.EventTypeIds.Should().Contain(eventTypeId);
    }

    [Fact]
    public void CanBeInitializedWithMultipleEventTypeIds()
    {
        _filter.WithEventTypes(1);
        _filter.WithEventTypes(2);

        _filter.EventTypeIds.Should().Contain(1);
        _filter.EventTypeIds.Should().Contain(2);
    }

    [Fact]
    public void WhenInitializedDoesNotAddDuplicateEventTypeIds()
    {
        _filter.WithEventTypes(1);
        _filter.WithEventTypes(1);

        _filter.EventTypeIds?.Count.Should().Be(1);
        _filter.EventTypeIds.Should().Contain(1);
    }

    [Fact]
    public void WhenInitializedEventIdsIsNull() =>
        _filter.EventIds.Should().BeNull();

    [Fact]
    public void AddingNullEventIdsHasNoEffect()
    {
        _filter.WithEventsIds("12345", null!);

        _filter.EventIds.Should().Contain("12345");
        _filter.EventIds.Should().NotContainNulls();
    }

    [Fact]
    public void AddingNullEventIdArrayHasNoEffect()
    {
        _filter.WithEventsIds(null!);

        _filter.EventIds.Should().BeNull();
    }

    [Theory]
    [InlineData("EventId")]
    [InlineData("12345")]
    [InlineData("98765")]
    public void CanBeInitializedWithEventId(string eventId)
    {
        _filter.WithEventsIds(eventId);

        _filter.EventIds.Should().Contain(eventId);
    }

    [Fact]
    public void CanBeInitializedWithMultipleEventIds()
    {
        _filter.WithEventsIds("12345");
        _filter.WithEventsIds("98765");

        _filter.EventIds.Should().Contain("12345");
        _filter.EventIds.Should().Contain("98765");
    }

    [Fact]
    public void WhenInitializedDoesNotAddDuplicateEventIds()
    {
        _filter.WithEventsIds("12345");
        _filter.WithEventsIds("12345");

        _filter.EventIds?.Count.Should().Be(1);
        _filter.EventIds.Should().Contain("12345");
    }

    [Fact]
    public void WhenInitializedBspMarketIsNullByDefault() =>
        _filter.BspMarket.Should().BeNull();

    [Fact]
    public void CanBeInitializedWithBspMarketsOnly()
    {
        _filter.WithBspMarketsOnly();

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
        _filter.WithBettingTypes(BettingType.Odds, BettingType.Line);

        _filter.BettingTypes.Should().Contain(BettingType.Odds.Id);
        _filter.BettingTypes.Should().Contain(BettingType.Line.Id);
    }

    [Fact]
    public void AddNullBettingTypeHasNoEffect()
    {
        _filter.WithBettingTypes(BettingType.Odds, null!);

        _filter.BettingTypes.Should().Contain(BettingType.Odds.Id);
        _filter.BettingTypes.Should().NotContainNulls();
    }

    [Fact]
    public void AddNullBettingTypeArrayHasNoEffect()
    {
        _filter.WithBettingTypes(null!);

        _filter.BettingTypes.Should().BeNull();
    }
}