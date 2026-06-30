using Betfair.Core;

namespace Betfair.Tests.Core;

public class MarketFilterAdditionalTests : MarketFilter<MarketFilterAdditionalTests>
{
    [Fact]
    public void WithTextQuerySetsTextQuery()
    {
        WithTextQuery("horse");

        TextQuery.Should().Be("horse");
    }

    [Fact]
    public void WithExchangeIdsSetsExchangeIds()
    {
        WithExchangeIds("1", "2");

        ExchangeIds.Should().Contain("1");
        ExchangeIds.Should().Contain("2");
    }

    [Fact]
    public void WithExchangeIdsNullHasNoEffect()
    {
        WithExchangeIds(null!);

        ExchangeIds.Should().BeNull();
    }

    [Fact]
    public void WithExchangeIdsFiltersNullEntries()
    {
        WithExchangeIds("1", null!);

        ExchangeIds.Should().Contain("1");
        ExchangeIds.Should().NotContainNulls();
    }

    [Fact]
    public void WithEventIdsSetsEventIds()
    {
        WithEventIds("100", "200");

        EventIds.Should().Contain("100");
        EventIds.Should().Contain("200");
    }

    [Fact]
    public void WithEventIdsNullHasNoEffect()
    {
        WithEventIds(null!);

        EventIds.Should().BeNull();
    }

    [Fact]
    public void WithEventIdsFiltersNullEntries()
    {
        WithEventIds("100", null!);

        EventIds.Should().Contain("100");
        EventIds.Should().NotContainNulls();
    }

    [Fact]
    public void WithCompetitionIdsSetsCompetitionIds()
    {
        WithCompetitionIds("C1", "C2");

        CompetitionIds.Should().Contain("C1");
        CompetitionIds.Should().Contain("C2");
    }

    [Fact]
    public void WithCompetitionIdsNullHasNoEffect()
    {
        WithCompetitionIds(null!);

        CompetitionIds.Should().BeNull();
    }

    [Fact]
    public void WithCompetitionIdsFiltersNullEntries()
    {
        WithCompetitionIds("C1", null!);

        CompetitionIds.Should().Contain("C1");
        CompetitionIds.Should().NotContainNulls();
    }

    [Fact]
    public void WithVenuesSetsVenues()
    {
        WithVenues("Ascot", "Cheltenham");

        Venues.Should().Contain("Ascot");
        Venues.Should().Contain("Cheltenham");
    }

    [Fact]
    public void WithVenuesNullHasNoEffect()
    {
        WithVenues(null!);

        Venues.Should().BeNull();
    }

    [Fact]
    public void WithVenuesFiltersNullEntries()
    {
        WithVenues("Ascot", null!);

        Venues.Should().Contain("Ascot");
        Venues.Should().NotContainNulls();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    [InlineData(null)]
    public void WithBspOnlySetsBspOnly(bool? value)
    {
        WithBspOnly(value);

        BspOnly.Should().Be(value);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    [InlineData(null)]
    public void WithTurnInPlayEnabledSetsTurnInPlayEnabled(bool? value)
    {
        WithTurnInPlayEnabled(value);

        TurnInPlayEnabled.Should().Be(value);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    [InlineData(null)]
    public void WithInPlayOnlySetsInPlayOnly(bool? value)
    {
        WithInPlayOnly(value);

        InPlayOnly.Should().Be(value);
    }

    [Fact]
    public void WithMarketBettingTypesSetsMarketBettingTypes()
    {
        WithMarketBettingTypes("ODDS", "LINE");

        MarketBettingTypes.Should().Contain("ODDS");
        MarketBettingTypes.Should().Contain("LINE");
    }

    [Fact]
    public void WithMarketBettingTypesNullHasNoEffect()
    {
        WithMarketBettingTypes(null!);

        MarketBettingTypes.Should().BeNull();
    }

    [Fact]
    public void WithMarketBettingTypesFiltersNullEntries()
    {
        WithMarketBettingTypes("ODDS", null!);

        MarketBettingTypes.Should().Contain("ODDS");
        MarketBettingTypes.Should().NotContainNulls();
    }

    [Fact]
    public void WithOrdersSetsWithOrdersFilter()
    {
        WithOrders("EXECUTABLE", "EXECUTION_COMPLETE");

        WithOrdersFilter.Should().Contain("EXECUTABLE");
        WithOrdersFilter.Should().Contain("EXECUTION_COMPLETE");
    }

    [Fact]
    public void WithOrdersNullHasNoEffect()
    {
        WithOrders(null!);

        WithOrdersFilter.Should().BeNull();
    }

    [Fact]
    public void WithRaceTypesSetsRaceTypes()
    {
        WithRaceTypes("Flat", "Hurdle");

        RaceTypes.Should().Contain("Flat");
        RaceTypes.Should().Contain("Hurdle");
    }

    [Fact]
    public void WithRaceTypesNullHasNoEffect()
    {
        WithRaceTypes(null!);

        RaceTypes.Should().BeNull();
    }

    [Fact]
    public void WithEventTypesNullStringArrayHasNoEffect()
    {
        WithEventTypes((string[])null!);

        EventTypeIds.Should().BeNull();
    }

    [Fact]
    public void WithMarketIdsNullHasNoEffect()
    {
        WithMarketIds(null!);

        MarketIds.Should().BeNull();
    }

    [Fact]
    public void WithCountriesNullCountryArrayHasNoEffect()
    {
        WithCountries((Country[])null!);

        MarketCountries.Should().BeNull();
    }

    [Fact]
    public void WithCountriesNullStringArrayHasNoEffect()
    {
        WithCountries((string[])null!);

        MarketCountries.Should().BeNull();
    }

    [Fact]
    public void WithMarketTypesNullMarketTypeArrayHasNoEffect()
    {
        WithMarketTypes((MarketType[])null!);

        MarketTypes.Should().BeNull();
    }

    [Fact]
    public void WithMarketTypesNullStringArrayHasNoEffect()
    {
        WithMarketTypes((string[])null!);

        MarketTypes.Should().BeNull();
    }
}
