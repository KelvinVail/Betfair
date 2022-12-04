using Betfair.Betting;
using Utf8Json;
using Utf8Json.Resolvers;

namespace Betfair.Tests.Betting;

public class MarketProjectionTests
{
    [Fact]
    public void CanBeCreatedWithMarketStartTime()
    {
        var projection = new MarketProjection()
            .WithMarketStartTime();

        var json = JsonSerializer.ToJsonString(projection, StandardResolver.AllowPrivateExcludeNullCamelCase);

        json.Should().Be("[\"MARKET_START_TIME\"]");
    }

    [Fact]
    public void ShouldBeCreatedWithoutDuplicates()
    {
        var projection = new MarketProjection()
            .WithMarketStartTime()
            .WithMarketStartTime();

        var json = JsonSerializer.ToJsonString(projection, StandardResolver.AllowPrivateExcludeNullCamelCase);

        json.Should().Be("[\"MARKET_START_TIME\"]");
    }

    [Fact]
    public void CanBeCreatedWithCompetition()
    {
        var projection = new MarketProjection()
            .WithCompetition()
            .WithCompetition();

        var json = JsonSerializer.ToJsonString(projection, StandardResolver.AllowPrivateExcludeNullCamelCase);

        json.Should().Be("[\"COMPETITION\"]");
    }

    [Fact]
    public void CanBeCreatedWithEvent()
    {
        var projection = new MarketProjection()
            .WithEvent();

        var json = JsonSerializer.ToJsonString(projection, StandardResolver.AllowPrivateExcludeNullCamelCase);

        json.Should().Be("[\"EVENT\"]");
    }

    [Fact]
    public void CanBeCreatedWithEventType()
    {
        var projection = new MarketProjection()
            .WithEventType();

        var json = JsonSerializer.ToJsonString(projection, StandardResolver.AllowPrivateExcludeNullCamelCase);

        json.Should().Be("[\"EVENT_TYPE\"]");
    }

    [Fact]
    public void CanBeCreatedWithMarketDescription()
    {
        var projection = new MarketProjection()
            .WithMarketDescription();

        var json = JsonSerializer.ToJsonString(projection, StandardResolver.AllowPrivateExcludeNullCamelCase);

        json.Should().Be("[\"MARKET_DESCRIPTION\"]");
    }

    [Fact]
    public void CanBeCreatedWithRunnerDescription()
    {
        var projection = new MarketProjection()
            .WithRunnerDescription();

        var json = JsonSerializer.ToJsonString(projection, StandardResolver.AllowPrivateExcludeNullCamelCase);

        json.Should().Be("[\"RUNNER_DESCRIPTION\"]");
    }

    [Fact]
    public void CanBeCreatedWithRunnerMetadata()
    {
        var projection = new MarketProjection()
            .WithRunnerMetadata();

        var json = JsonSerializer.ToJsonString(projection, StandardResolver.AllowPrivateExcludeNullCamelCase);

        json.Should().Be("[\"RUNNER_METADATA\"]");
    }
}
