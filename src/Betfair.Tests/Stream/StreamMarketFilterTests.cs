using System.Runtime.InteropServices;
using Betfair.Stream;

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
        _filter.WithMarketId(marketId);

        _filter.MarketIds.Should().Contain(marketId);
    }

    [Theory]
    [InlineData("1")]
    [InlineData("1.234567")]
    public void CanBeInitializedWithMultipleMarketIds(string marketId)
    {
        _filter.WithMarketId(marketId).WithMarketId("2");

        _filter.MarketIds.Should().Contain(marketId);
        _filter.MarketIds.Should().Contain("2");
    }

    [Fact]
    public void CanNotBeCreatedWithDuplicateMarketIds()
    {
        _filter.WithMarketId("1").WithMarketId("1");

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
        _filter.WithCountryCode(countryCode);

        _filter.CountryCodes.Should().Contain(countryCode);
    }

    [Fact]
    public void CanBeInitializedWithMultipleCountryCodes()
    {
        _filter.WithCountryCode("GB");
        _filter.WithCountryCode("IE");

        _filter.CountryCodes.Should().Contain("GB");
        _filter.CountryCodes.Should().Contain("IE");
    }

    [Fact]
    public void CanNotBeCreatedWithDuplicateCountryCodes()
    {
        _filter.WithCountryCode("GB");
        _filter.WithCountryCode("GB");

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
    public void WhenInitializedMarketTypesIsNull() =>
        _filter.MarketTypes.Should().BeNull();

    [Theory]
    [InlineData("MarketType")]
    [InlineData("MATCH_ODDS")]
    [InlineData("HALF_TIME_SCORE")]
    public void CanBeInitializedWithMarketTypes(string marketType)
    {
        _filter.WithMarketType(marketType);

        _filter.MarketTypes.Should().Contain(marketType);
    }

    [Fact]
    public void CanBeInitializedWithMultipleMarketTypes()
    {
        _filter.WithMarketType("MATCH_ODDS");
        _filter.WithMarketType("HALF_TIME_SCORE");

        _filter.MarketTypes.Should().Contain("MATCH_ODDS");
        _filter.MarketTypes.Should().Contain("HALF_TIME_SCORE");
    }

    [Fact]
    public void CanNotBeCreatedWithDuplicateMarketTypes()
    {
        _filter.WithMarketType("MATCH_ODDS");
        _filter.WithMarketType("MATCH_ODDS");

        _filter.MarketTypes?.Count.Should().Be(1);
        _filter.MarketTypes.Should().Contain("MATCH_ODDS");
    }

    [Fact]
    public void WhenInitializedVenuesIsNull() =>
        _filter.Venues.Should().BeNull();

    [Theory]
    [InlineData("Venue")]
    [InlineData("Ascot")]
    [InlineData("Epsom")]
    public void CanBeInitializedWithVenue(string venue)
    {
        _filter.WithVenue(venue);

        _filter.Venues.Should().Contain(venue);
    }

    [Fact]
    public void CanBeInitializedWithMultipleVenues()
    {
        _filter.WithVenue("Ascot");
        _filter.WithVenue("Epsom");

        _filter.Venues.Should().Contain("Ascot");
        _filter.Venues.Should().Contain("Epsom");
    }

    [Fact]
    public void CanNotBeCreatedWithDuplicateVenues()
    {
        _filter.WithVenue("Ascot");
        _filter.WithVenue("Ascot");

        _filter.Venues?.Count.Should().Be(1);
        _filter.Venues.Should().Contain("Ascot");
    }

    [Fact]
    public void WhenInitializedEventTypeIdsIsNull() =>
        _filter.EventTypeIds.Should().BeNull();

    [Theory]
    [InlineData("EventTypeId")]
    [InlineData("1")]
    [InlineData("7")]
    public void CanBeInitializedWithEventTypeId(string eventTypeId)
    {
        _filter.WithEventTypeId(eventTypeId);

        _filter.EventTypeIds.Should().Contain(eventTypeId);
    }

    [Fact]
    public void CanBeInitializedWithMultipleEventTypeIds()
    {
        _filter.WithEventTypeId("1");
        _filter.WithEventTypeId("2");

        _filter.EventTypeIds.Should().Contain("1");
        _filter.EventTypeIds.Should().Contain("2");
    }

    [Fact]
    public void WhenInitializedDoesNotAddDuplicateEventTypeIds()
    {
        _filter.WithEventTypeId("1");
        _filter.WithEventTypeId("1");

        _filter.EventTypeIds?.Count.Should().Be(1);
        _filter.EventTypeIds.Should().Contain("1");
    }

    [Fact]
    public void WhenInitializedEventIdsIsNull() =>
        _filter.EventIds.Should().BeNull();

    [Theory]
    [InlineData("EventId")]
    [InlineData("12345")]
    [InlineData("98765")]
    public void CanBeInitializedWithEventId(string eventId)
    {
        _filter.WithEventId(eventId);

        _filter.EventIds.Should().Contain(eventId);
    }

    [Fact]
    public void CanBeInitializedWithMultipleEventIds()
    {
        _filter.WithEventId("12345");
        _filter.WithEventId("98765");

        _filter.EventIds.Should().Contain("12345");
        _filter.EventIds.Should().Contain("98765");
    }

    [Fact]
    public void WhenInitializedDoesNotAddDuplicateEventIds()
    {
        _filter.WithEventId("12345");
        _filter.WithEventId("12345");

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
    public void WhenInitializedBettingTypesIsNull() =>
        _filter.BettingTypes.Should().BeNull();

    [Theory]
    [InlineData("ODDS")]
    [InlineData("LINE")]
    public void CanBeInitializedWithBettingTypeOdds(string bettingType)
    {
        _filter.WithBettingType(bettingType);

        _filter.BettingTypes.Should().Contain(bettingType);
    }

    [Fact]
    public void WhenInitializedDoesNotAddDuplicateBettingTypes()
    {
        _filter.WithBettingType("ODDS");
        _filter.WithBettingType("ODDS");

        _filter.BettingTypes?.Count.Should().Be(1);
        _filter.BettingTypes.Should().Contain("ODDS");
    }
}