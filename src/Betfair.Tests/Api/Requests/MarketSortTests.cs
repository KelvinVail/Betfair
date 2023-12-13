using Betfair.Api.Requests;

namespace Betfair.Tests.Api.Requests;

public class MarketSortTests
{
    [Fact]
    public void CanCreateMarketSortMinimumTraded() =>
        MarketSort.MinimumTraded.Value.Should().Be("MINIMUM_TRADED");

    [Fact]
    public void CanCreateMarketSortMaximumTraded() =>
        MarketSort.MaximumTraded.Value.Should().Be("MAXIMUM_TRADED");

    [Fact]
    public void CanCreateMarketSortMinimumAvailable() =>
        MarketSort.MinimumAvailable.Value.Should().Be("MINIMUM_AVAILABLE");

    [Fact]
    public void CanCreateMarketSortMaximumAvailable() =>
        MarketSort.MaximumAvailable.Value.Should().Be("MAXIMUM_AVAILABLE");

    [Fact]
    public void CanCreateMarketSortFirstToStart() =>
        MarketSort.FirstToStart.Value.Should().Be("FIRST_TO_START");

    [Fact]
    public void CanCreateMarketSortLastToStart() =>
        MarketSort.LastToStart.Value.Should().Be("LAST_TO_START");
}
