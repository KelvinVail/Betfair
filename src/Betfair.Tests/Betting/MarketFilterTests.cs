using Betfair.Betting;

namespace Betfair.Tests.Betting;

public class MarketFilterTests
{
    [Fact]
    public void IsEmptyAsDefault()
    {
        var filter = new MarketFilter();

        filter.ToJsonString().Should().Be("\"filter\":{}");
    }

    [Fact]
    public void IsEmptyIfEventTypeIsNull()
    {
        var filter = new MarketFilter().WithEventType(null);

        filter.ToJsonString().Should().Be("\"filter\":{}");
    }

    [Theory]
    [InlineData("1")]
    [InlineData("2")]
    [InlineData("7")]
    public void CanBeCreatedWithEventType(string eventId)
    {
        var eventType = new EventType { Id = eventId };

        var filter = new MarketFilter().WithEventType(eventType);

        filter.ToJsonString().Should()
            .Be($"\"filter\":{{\"eventTypeIds\":[{eventId}]}}");
    }

    [Theory]
    [InlineData("1")]
    [InlineData("2")]
    [InlineData("7")]
    public void CanBeCreatedMultipleWithEventTypes(string eventId)
    {
        var eventType = new EventType { Id = eventId };

        var filter = new MarketFilter()
            .WithEventType(new EventType { Id = "999" })
            .WithEventType(eventType);

        filter.ToJsonString().Should()
            .Be($"\"filter\":{{\"eventTypeIds\":[999,{eventId}]}}");
    }

    [Fact]
    public void IsEmptyIfMarketTypeIsNull()
    {
        var filter = new MarketFilter().WithMarketType(null);

        filter.ToJsonString().Should().Be("\"filter\":{}");
    }

    [Theory]
    [InlineData("WIN")]
    [InlineData("PLACE")]
    [InlineData("EACH_WAY")]
    public void CanBeCreatedWithMarketType(string marketTypeId)
    {
        var marketType = new MarketType { Id = marketTypeId };

        var filter = new MarketFilter().WithMarketType(marketType);

        filter.ToJsonString().Should()
            .Be($"\"filter\":{{\"marketTypeCodes\":[\"{marketTypeId}\"]}}");
    }

    [Theory]
    [InlineData("WIN")]
    [InlineData("PLACE")]
    [InlineData("EACH_WAY")]
    public void CanBeCreatedWithMultipleMarketType(string marketTypeId)
    {
        var marketType = new MarketType { Id = marketTypeId };

        var filter = new MarketFilter()
            .WithMarketType(new MarketType { Id = "OTHER_PLACE" })
            .WithMarketType(marketType);

        filter.ToJsonString().Should()
            .Be($"\"filter\":{{\"marketTypeCodes\":[\"OTHER_PLACE\",\"{marketTypeId}\"]}}");
    }

    [Fact]
    public void IsEmptyIfCountryCodeIsNull()
    {
        var filter = new MarketFilter().WithCountryCode(null);

        filter.ToJsonString().Should().Be("\"filter\":{}");
    }

    [Theory]
    [InlineData("GB")]
    [InlineData("US")]
    [InlineData("DE")]
    public void CanBeCreatedWithCountryCode(string countryCode)
    {
        var filter = new MarketFilter().WithCountryCode(countryCode);

        filter.ToJsonString().Should()
            .Be($"\"filter\":{{\"marketCountries\":[\"{countryCode}\"]}}");
    }

    [Theory]
    [InlineData("GB")]
    [InlineData("US")]
    [InlineData("DE")]
    public void CanBeCreatedWithMultipleCountryCode(string countryCode)
    {
        var filter = new MarketFilter()
            .WithCountryCode("FR")
            .WithCountryCode(countryCode);

        filter.ToJsonString().Should()
            .Be($"\"filter\":{{\"marketCountries\":[\"FR\",\"{countryCode}\"]}}");
    }
}
