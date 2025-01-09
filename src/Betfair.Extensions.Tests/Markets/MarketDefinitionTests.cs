using Betfair.Core.Login;
using Betfair.Extensions.Markets;
using Betfair.Extensions.Markets.Enums;

namespace Betfair.Extensions.Tests.Markets;

public class MarketDefinitionTests
{
    private readonly Credentials _credentials = new ("username", "password", "appKey");
    private readonly SubscriptionMock _sub = new (Path.Combine("Data", "MarketDefinitions", "InitialImage.json"));
    private readonly MarketCache _market;

    public MarketDefinitionTests() =>
        _market = MarketCache.Create(_credentials, "1.235123059", _sub).Value;

    [Fact]
    public async Task MarketStartTimeIsSetFromTheInitialMarketDefinition()
    {
        await _market.Subscribe();

        _market.StartTime.Should().Be(new DateTime(2024, 10, 29, 12, 40, 0, DateTimeKind.Utc));
    }

    [Fact]
    public async Task DoNotSetTheStartTimeIfMarketIdsDoNotMatch()
    {
        var market = MarketCache.Create(_credentials, "1.1", _sub).Value;

        await market.Subscribe();

        market.StartTime.Should().Be(default);
    }

    [Fact]
    public async Task SetMarketStatusWhenOpen()
    {
        await _market.Subscribe();

        _market.Status.Should().Be(MarketStatus.Open);
    }

    [Fact]
    public async Task SetMarketStatusWhenInactive()
    {
        var sub = new SubscriptionMock(Path.Combine("Data", "MarketDefinitions", "InitialImage_inactive.json"));
        var market = MarketCache.Create(_credentials, "1.235123059", sub).Value;
        await market.Subscribe();

        market.Status.Should().Be(MarketStatus.Inactive);
    }

    [Fact]
    public async Task SetMarketStatusWhenSuspended()
    {
        var sub = new SubscriptionMock(Path.Combine("Data", "MarketDefinitions", "InitialImage_suspended.json"));
        var market = MarketCache.Create(_credentials, "1.235123059", sub).Value;
        await market.Subscribe();

        market.Status.Should().Be(MarketStatus.Suspended);
    }

    [Fact]
    public async Task SetMarketStatusWhenClosed()
    {
        var sub = new SubscriptionMock(Path.Combine("Data", "MarketDefinitions", "InitialImage_closed.json"));
        var market = MarketCache.Create(_credentials, "1.235123059", sub).Value;
        await market.Subscribe();

        market.Status.Should().Be(MarketStatus.Closed);
    }

    [Fact]
    public async Task SetMarketIsInPlay()
    {
        await _market.Subscribe();

        _market.IsInPlay.Should().BeTrue();
    }

    [Fact]
    public async Task SetMarketVersion()
    {
        await _market.Subscribe();

        _market.Version.Should().Be(6242839770);
    }
}
