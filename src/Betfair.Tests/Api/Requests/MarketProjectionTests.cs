using Betfair.Api.Requests;

namespace Betfair.Tests.Api.Requests;

public class MarketProjectionTests
{
    [Fact]
    public void CanCreateMarketProjectionMarketStartTime() =>
        MarketProjection.MarketStartTime.Value.Should().Be("MARKET_START_TIME");

    [Fact]
    public void CanCreateMarketProjectionCompetition() =>
        MarketProjection.Competition.Value.Should().Be("COMPETITION");

    [Fact]
    public void CanCreateMarketProjectionEvent() =>
        MarketProjection.Event.Value.Should().Be("EVENT");

    [Fact]
    public void CanCreateMarketProjectionEventType() =>
        MarketProjection.EventType.Value.Should().Be("EVENT_TYPE");

    [Fact]
    public void CanCreateMarketProjectionMarketDescription() =>
        MarketProjection.MarketDescription.Value.Should().Be("MARKET_DESCRIPTION");

    [Fact]
    public void CanCreateMarketProjectionRunnerDescription() =>
        MarketProjection.RunnerDescription.Value.Should().Be("RUNNER_DESCRIPTION");

    [Fact]
    public void CanCreateMarketProjectionRunnerMetadata() =>
        MarketProjection.RunnerMetadata.Value.Should().Be("RUNNER_METADATA");
}
