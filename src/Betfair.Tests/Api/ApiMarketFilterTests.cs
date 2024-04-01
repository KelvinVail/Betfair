using System.Globalization;
using Betfair.Api.Requests;
using Betfair.Core;

namespace Betfair.Tests.Api;

public class ApiMarketFilterTests
{
    [Fact]
    public void ExtendsMarketFilter() =>
        new ApiMarketFilter().Should()
            .BeAssignableTo(typeof(MarketFilter<ApiMarketFilter>));

    [Fact]
    public void CreatedWithNullFromDate() =>
        new ApiMarketFilter().MarketStartTime.Should().BeNull();

    [Theory]
    [InlineData("2023-12-13", "2023-12-13T00:00:00Z")]
    [InlineData("2023-12-13 +01:00", "2023-12-12T23:00:00Z")]
    [InlineData("2023-12-13 15:16:17 +00:00", "2023-12-13T15:16:17Z")]
    [InlineData("2023-12-13 15:16:17 +01:00", "2023-12-13T14:16:17Z")]
    public void FromDateCanBeAdded(DateTimeOffset dateTime, string expected)
    {
        var filter = new ApiMarketFilter().FromMarketStart(dateTime);

        filter.MarketStartTime!.From.Should().Be(expected);
    }

    [Theory]
    [InlineData("2023-12-13", "2023-12-13T00:00:00Z")]
    [InlineData("2023-12-13 +01:00", "2023-12-12T23:00:00Z")]
    [InlineData("2023-12-13 15:16:17 +00:00", "2023-12-13T15:16:17Z")]
    [InlineData("2023-12-13 15:16:17 +01:00", "2023-12-13T14:16:17Z")]
    public void ToDateCanBeAdded(DateTimeOffset dateTime, string expected)
    {
        var filter = new ApiMarketFilter().ToMarketStart(dateTime);

        filter.MarketStartTime!.To.Should().Be(expected);
    }

    [Fact]
    public void TodaysCardIsDefined()
    {
        var card = new ApiMarketFilter().TodaysCard();

        card.EventTypeIds.Should().Contain(EventType.HorseRacing.Id);

        card.MarketTypeCodes.Should().Contain(MarketType.Win.Id);
        card.MarketTypes.Should().Contain(MarketType.Win.Id);

        card.MarketCountries.Should().Contain(Country.UnitedKingdom.Id);
        card.MarketCountries.Should().Contain(Country.Ireland.Id);

        card.CountryCodes.Should().Contain(Country.UnitedKingdom.Id);
        card.CountryCodes.Should().Contain(Country.Ireland.Id);

        card.MarketStartTime!.From.Should().Be(DateTime.Today.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ", NumberFormatInfo.InvariantInfo));
        card.MarketStartTime.To.Should().Be(DateTime.Today.ToUniversalTime().AddDays(1).ToString("yyyy-MM-ddTHH:mm:ssZ", NumberFormatInfo.InvariantInfo));
    }
}
