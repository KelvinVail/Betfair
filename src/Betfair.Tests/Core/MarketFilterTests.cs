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
        IncludeMarketTypes(MarketType.Win)
            .IncludeMarketTypes(MarketType.CorrectScore);

        MarketTypes.Should().Contain(MarketType.Win.Id);
        MarketTypes.Should().Contain(MarketType.CorrectScore.Id);
    }

    [Fact]
    public void TextMarketTypeIdsAreAddedToMarketTypes()
    {
        IncludeMarketTypes("WIN")
            .IncludeMarketTypes("CORRECT_SCORE");

        MarketTypes.Should().Contain(MarketType.Win.Id);
        MarketTypes.Should().Contain(MarketType.CorrectScore.Id);
    }

    [Fact]
    public void IsCreatedWithNullMarketTypeCodes() =>
        MarketTypeCodes.Should().BeNull();

    [Fact]
    public void MarketTypeIdsAreAddedToMarketTypeCodes()
    {
        IncludeMarketTypes(MarketType.Win)
            .IncludeMarketTypes(MarketType.CorrectScore);

        MarketTypeCodes.Should().Contain(MarketType.Win.Id);
        MarketTypeCodes.Should().Contain(MarketType.CorrectScore.Id);
    }

    [Fact]
    public void TextMarketTypeIdsAreAddedToMarketTypeCodes()
    {
        IncludeMarketTypes("WIN")
            .IncludeMarketTypes("CORRECT_SCORE");

        MarketTypeCodes.Should().Contain(MarketType.Win.Id);
        MarketTypeCodes.Should().Contain(MarketType.CorrectScore.Id);
    }

    [Fact]
    public void AddingNullMarketTypeHasNoEffect()
    {
        IncludeMarketTypes(MarketType.Win)
            .IncludeMarketTypes((MarketType)null!);

        MarketTypes.Should().Contain(MarketType.Win.Id);
        MarketTypes.Should().NotContainNulls();
        MarketTypeCodes.Should().Contain(MarketType.Win.Id);
        MarketTypeCodes.Should().NotContainNulls();
    }

    [Fact]
    public void AddingNullMarketTypeStringHasNoEffect()
    {
        IncludeMarketTypes("WIN")
            .IncludeMarketTypes((string)null!);

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
        IncludeEventTypes(EventType.HorseRacing)
            .IncludeEventTypes(EventType.AmericanFootball);

        EventTypeIds.Should().Contain(EventType.HorseRacing.Id);
        EventTypeIds.Should().Contain(EventType.AmericanFootball.Id);
    }

    [Fact]
    public void TextEventTypeIdsAreAddedToEventTypes()
    {
        IncludeEventTypes(1)
            .IncludeEventTypes(7);

        EventTypeIds.Should().Contain(1);
        EventTypeIds.Should().Contain(7);
    }

    [Fact]
    public void AddingNullEventTypeHasNoEffect()
    {
        IncludeEventTypes(EventType.HorseRacing)
            .IncludeEventTypes((EventType)null!);

        EventTypeIds.Should().Contain(EventType.HorseRacing.Id);
        EventTypeIds.Should().NotContainNulls();
    }

    [Fact]
    public void AddingNullEventTypeIdHasNoEffect()
    {
        IncludeEventTypes(7)
            .IncludeEventTypes((int[])null!);

        EventTypeIds.Should().Contain(EventType.HorseRacing.Id);
        EventTypeIds.Should().NotContainNulls();
    }

    [Fact]
    public void IsCreatedWithNullMarketIds() =>
        MarketIds.Should().BeNull();

    [Theory]
    [InlineData("1.23456789")]
    public void MarketIdIsAddedToMarketIds(string marketId)
    {
        IncludeMarketIds(marketId)
            .IncludeMarketIds("1.999");

        MarketIds.Should().Contain(marketId);
        MarketIds.Should().Contain("1.999");
    }

    [Fact]
    public void AddingNullMarketIdHasNoEffect()
    {
        IncludeMarketIds(null!)
            .IncludeMarketIds("1.999");

        MarketIds.Should().Contain("1.999");
        MarketIds.Should().NotContainNulls();
    }

    [Fact]
    public void IsCreatedWithNullMarketCountries() =>
        MarketCountries.Should().BeNull();

    [Fact]
    public void CountryCodesAreAddedToMarketCountries()
    {
        IncludeCountries(Country.Algeria)
            .IncludeCountries(Country.Argentina);

        MarketCountries.Should().Contain(Country.Algeria.Id);
        MarketCountries.Should().Contain(Country.Argentina.Id);
    }

    [Fact]
    public void TextCountryCodesAreAddedToMarketTypes()
    {
        IncludeCountries("XX")
            .IncludeCountries("ZZ");

        MarketCountries.Should().Contain("XX");
        MarketCountries.Should().Contain("ZZ");
    }

    [Fact]
    public void IsCreatedWithNullCountryCodes() =>
        CountryCodes.Should().BeNull();

    [Fact]
    public void CountryCodesAreAddedToCountryCodes()
    {
        IncludeCountries(Country.Algeria)
            .IncludeCountries(Country.Argentina);

        CountryCodes.Should().Contain(Country.Algeria.Id);
        CountryCodes.Should().Contain(Country.Argentina.Id);
    }

    [Fact]
    public void TextCountryCodesAreAddedToCountryCodes()
    {
        IncludeCountries("XX")
            .IncludeCountries("ZZ");

        CountryCodes.Should().Contain("XX");
        CountryCodes.Should().Contain("ZZ");
    }

    [Fact]
    public void AddingNullCountryHasNoEffect()
    {
        IncludeCountries((Country)null!)
            .IncludeCountries(Country.Ireland);

        MarketCountries.Should().Contain(Country.Ireland.Id);
        MarketCountries.Should().NotContainNulls();
        CountryCodes.Should().Contain(Country.Ireland.Id);
        CountryCodes.Should().NotContainNulls();
    }

    [Fact]
    public void AddingNullCountryCodeHasNoEffect()
    {
        IncludeCountries((string)null!)
            .IncludeCountries("XX");

        MarketCountries.Should().Contain("XX");
        MarketCountries.Should().NotContainNulls();
        CountryCodes.Should().Contain("XX");
        CountryCodes.Should().NotContainNulls();
    }
}
