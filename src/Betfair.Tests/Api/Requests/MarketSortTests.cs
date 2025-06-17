using Betfair.Api.Betting.Endpoints.ListMarketCatalogue.Enums;

namespace Betfair.Tests.Api.Requests;

public class MarketSortTests
{
    [Fact]
    public void CanCreateMarketSortMinimumTraded() =>
        JsonSerializer.Serialize(MarketSort.MinimumTraded).Should().Be("\"MINIMUM_TRADED\"");

    [Fact]
    public void CanCreateMarketSortMaximumTraded() =>
        JsonSerializer.Serialize(MarketSort.MaximumTraded).Should().Be("\"MAXIMUM_TRADED\"");

    [Fact]
    public void CanCreateMarketSortMinimumAvailable() =>
        JsonSerializer.Serialize(MarketSort.MinimumAvailable).Should().Be("\"MINIMUM_AVAILABLE\"");

    [Fact]
    public void CanCreateMarketSortMaximumAvailable() =>
        JsonSerializer.Serialize(MarketSort.MaximumAvailable).Should().Be("\"MAXIMUM_AVAILABLE\"");

    [Fact]
    public void CanCreateMarketSortFirstToStart() =>
        JsonSerializer.Serialize(MarketSort.FirstToStart).Should().Be("\"FIRST_TO_START\"");

    [Fact]
    public void CanCreateMarketSortLastToStart() =>
        JsonSerializer.Serialize(MarketSort.LastToStart).Should().Be("\"LAST_TO_START\"");
}
