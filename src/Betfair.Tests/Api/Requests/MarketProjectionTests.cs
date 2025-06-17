using Betfair.Api.Betting.Endpoints.ListMarketCatalogue.Enums;

namespace Betfair.Tests.Api.Requests;

public class MarketProjectionTests
{
    [Fact]
    public void CanCreateMarketProjectionMarketStartTime() =>
        JsonSerializer.Serialize(MarketProjection.MarketStartTime).Should().Be("\"MARKET_START_TIME\"");

    [Fact]
    public void CanCreateMarketProjectionCompetition() =>
        JsonSerializer.Serialize(MarketProjection.Competition).Should().Be("\"COMPETITION\"");

    [Fact]
    public void CanCreateMarketProjectionEvent() =>
        JsonSerializer.Serialize(MarketProjection.Event).Should().Be("\"EVENT\"");

    [Fact]
    public void CanCreateMarketProjectionEventType() =>
        JsonSerializer.Serialize(MarketProjection.EventType).Should().Be("\"EVENT_TYPE\"");

    [Fact]
    public void CanCreateMarketProjectionMarketDescription() =>
        JsonSerializer.Serialize(MarketProjection.MarketDescription).Should().Be("\"MARKET_DESCRIPTION\"");

    [Fact]
    public void CanCreateMarketProjectionRunnerDescription() =>
        JsonSerializer.Serialize(MarketProjection.RunnerDescription).Should().Be("\"RUNNER_DESCRIPTION\"");

    [Fact]
    public void CanCreateMarketProjectionRunnerMetadata() =>
        JsonSerializer.Serialize(MarketProjection.RunnerMetadata).Should().Be("\"RUNNER_METADATA\"");
}
