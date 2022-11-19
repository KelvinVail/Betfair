using Betfair.Betting;
using FluentAssertions;

namespace Betfair.Tests.Betting;

public class MarketFilterTests
{
    [Fact]
    public void CanBeCreatedWithNoFilters()
    {
        var filter = MarketFilter.Create();

        filter.ToString().Should().Be("\"filter\":{}");
    }

    [Theory]
    [InlineData("1")]
    [InlineData("2")]
    [InlineData("7")]
    public void CanBeCreatedWithAnEventType(string id)
    {
        var eventType = new EventType { Id = id };

        var filter = MarketFilter.Create().With(eventType);

        filter.ToString().Should().Be($"\"filter\":{{\"eventTypeIds\":[\"{id}\"]}}");
    }

    [Fact]
    public void NullEventTypeReturnsAnEmptyFilter()
    {
        var filter = MarketFilter.Create().With((EventType)null);

        filter.ToString().Should().Be("\"filter\":{}");
    }
}
