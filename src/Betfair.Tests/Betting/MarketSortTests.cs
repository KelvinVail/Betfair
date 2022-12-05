using Betfair.Betting;

namespace Betfair.Tests.Betting;

public class MarketSortTests
{
    [Fact]
    public void CanBeCreatedWithMinimumTraded()
    {
        var sort = MarketSort.MinimumTraded;

        sort.Value.Should().Be("MINIMUM_TRADED");
    }

    [Fact]
    public void CanBeCreatedWithMaximumTraded()
    {
        var sort = MarketSort.MaximumTraded;

        sort.Value.Should().Be("MAXIMUM_TRADED");
    }

    [Fact]
    public void CanBeCreatedWithMinimumAvailable()
    {
        var sort = MarketSort.MinimumAvailable;

        sort.Value.Should().Be("MINIMUM_AVAILABLE");
    }

    [Fact]
    public void CanBeCreatedWithMaximumAvailable()
    {
        var sort = MarketSort.MaximumAvailable;

        sort.Value.Should().Be("MAXIMUM_AVAILABLE");
    }

    [Fact]
    public void CanBeCreatedWithFirstToStart()
    {
        var sort = MarketSort.FirstToStart;

        sort.Value.Should().Be("FIRST_TO_START");
    }

    [Fact]
    public void CanBeCreatedWithLastToStart()
    {
        var sort = MarketSort.LastToStart;

        sort.Value.Should().Be("LAST_TO_START");
    }
}
