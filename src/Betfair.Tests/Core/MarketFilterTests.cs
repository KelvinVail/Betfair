using Betfair.Core;

namespace Betfair.Tests.Core;

public class MarketFilterTests : MarketFilter<MarketFilterTests>
{
    [Fact]
    public void IsCreatedWithNullMarketTypes() =>
        MarketTypes.Should().BeNull();

    [Fact]
    public void MarketTypeIdsAreAddedToMarketTypes()
    {
        WithMarketTypes(MarketType.Win, MarketType.CorrectScore);

        MarketTypes.Should().Contain(MarketType.Win.Id);
        MarketTypes.Should().Contain(MarketType.CorrectScore.Id);
    }

    [Fact]
    public void TextMarketTypeIdsAreAddedToMarketTypes()
    {
        WithMarketTypes("WIN", "CORRECT_SCORE");

        MarketTypes.Should().Contain(MarketType.Win.Id);
        MarketTypes.Should().Contain(MarketType.CorrectScore.Id);
    }

    [Fact]
    public void IsCreatedWithNullMarketTypeCodes() =>
        MarketTypeCodes.Should().BeNull();

    [Fact]
    public void MarketTypeIdsAreAddedToMarketTypeCodes()
    {
        WithMarketTypes(MarketType.Win, MarketType.CorrectScore);

        MarketTypeCodes.Should().Contain(MarketType.Win.Id);
        MarketTypeCodes.Should().Contain(MarketType.CorrectScore.Id);
    }

    [Fact]
    public void TextMarketTypeIdsAreAddedToMarketTypeCodes()
    {
        WithMarketTypes("WIN", "CORRECT_SCORE");

        MarketTypeCodes.Should().Contain(MarketType.Win.Id);
        MarketTypeCodes.Should().Contain(MarketType.CorrectScore.Id);
    }

    [Fact]
    public void AddingNullMarketTypeHasNoEffect()
    {
        WithMarketTypes(MarketType.Win, null!);

        MarketTypes.Should().Contain(MarketType.Win.Id);
        MarketTypes.Should().NotContainNulls();
        MarketTypeCodes.Should().Contain(MarketType.Win.Id);
        MarketTypeCodes.Should().NotContainNulls();
    }

    [Fact]
    public void AddingNullMarketTypeStringHasNoEffect()
    {
        WithMarketTypes("WIN", null!);

        MarketTypes.Should().Contain(MarketType.Win.Id);
        MarketTypes.Should().NotContainNulls();
        MarketTypeCodes.Should().Contain(MarketType.Win.Id);
        MarketTypeCodes.Should().NotContainNulls();
    }

    [Fact]
    public void IsCreatedWithNullEventTypeIds() =>
        EventTypeIds.Should().BeNull();

    [Fact]
    public void EventTypesAreAddedToEventTypesIds()
    {
        WithEventTypes(EventType.HorseRacing, EventType.AmericanFootball);

        EventTypeIds.Should().Contain(EventType.HorseRacing.Id);
        EventTypeIds.Should().Contain(EventType.AmericanFootball.Id);
    }

    [Fact]
    public void TextEventTypeIdsAreAddedToEventTypes()
    {
        WithEventTypes(1, 7);

        EventTypeIds.Should().Contain(1);
        EventTypeIds.Should().Contain(7);
    }

    [Fact]
    public void AddingNullEventTypeHasNoEffect()
    {
        WithEventTypes(EventType.HorseRacing, null!);

        EventTypeIds.Should().Contain(EventType.HorseRacing.Id);
        EventTypeIds.Should().NotContainNulls();
    }

    [Fact]
    public void AddingNullEventTypeIdHasNoEffect()
    {
        WithEventTypes((int[])null!);

        EventTypeIds.Should().BeNull();
    }

    [Fact]
    public void IsCreatedWithNullMarketIds() =>
        MarketIds.Should().BeNull();

    [Theory]
    [InlineData("1.23456789")]
    public void MarketIdIsAddedToMarketIds(string marketId)
    {
        WithMarketIds(marketId, "1.999");

        MarketIds.Should().Contain(marketId);
        MarketIds.Should().Contain("1.999");
    }

    [Fact]
    public void AddingNullMarketIdHasNoEffect()
    {
        WithMarketIds("1.999", null!);

        MarketIds.Should().Contain("1.999");
        MarketIds.Should().NotContainNulls();
    }

    [Fact]
    public void IsCreatedWithNullMarketCountries() =>
        MarketCountries.Should().BeNull();

    [Fact]
    public void CountryCodesAreAddedToMarketCountries()
    {
        WithCountries(Country.Algeria, Country.Argentina);

        MarketCountries.Should().Contain(Country.Algeria.Id);
        MarketCountries.Should().Contain(Country.Argentina.Id);
    }

    [Fact]
    public void TextCountryCodesAreAddedToMarketTypes()
    {
        WithCountries("XX", "ZZ");

        MarketCountries.Should().Contain("XX");
        MarketCountries.Should().Contain("ZZ");
    }

    [Fact]
    public void IsCreatedWithNullCountryCodes() =>
        CountryCodes.Should().BeNull();

    [Fact]
    public void CountryCodesAreAddedToCountryCodes()
    {
        WithCountries(Country.Algeria, Country.Argentina);

        CountryCodes.Should().Contain(Country.Algeria.Id);
        CountryCodes.Should().Contain(Country.Argentina.Id);
    }

    [Fact]
    public void TextCountryCodesAreAddedToCountryCodes()
    {
        WithCountries("XX", "ZZ");

        CountryCodes.Should().Contain("XX");
        CountryCodes.Should().Contain("ZZ");
    }

    [Fact]
    public void AddingNullCountryHasNoEffect()
    {
        WithCountries(Country.Ireland, null!);

        MarketCountries.Should().Contain(Country.Ireland.Id);
        MarketCountries.Should().NotContainNulls();
        CountryCodes.Should().Contain(Country.Ireland.Id);
        CountryCodes.Should().NotContainNulls();
    }

    [Fact]
    public void AddingNullCountryCodeHasNoEffect()
    {
        WithCountries("XX", null!);

        MarketCountries.Should().Contain("XX");
        MarketCountries.Should().NotContainNulls();
        CountryCodes.Should().Contain("XX");
        CountryCodes.Should().NotContainNulls();
    }
}
