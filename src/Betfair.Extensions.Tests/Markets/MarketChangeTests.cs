using Betfair.Core.Login;
using Betfair.Extensions.Markets;

namespace Betfair.Extensions.Tests.Markets;

public class MarketChangeTests
{
    private readonly Credentials _credentials = new ("username", "password", "appKey");
    private readonly SubscriptionStub _sub = new (Path.Combine("Data", "MarketDefinitions", "InitialImage.json"));
    private readonly Market _market;

    public MarketChangeTests() =>
        _market = Market.Create(_credentials, "1.235123059", _sub).Value;

    [Fact]
    public async Task SetMarketTotalMatched()
    {
        await _market.Subscribe();

        _market.TradedVolume.Should().Be(17540.83);
    }

    [Fact]
    public async Task SetPublishTime()
    {
        await _market.Subscribe();

        _market.PublishTime.Should().Be(1730202588118);
    }

    [Fact]
    public async Task DoNotSetThePublishTimeIfMarketIdsDoNotMatch()
    {
        var market = Market.Create(_credentials, "1.1", _sub).Value;

        await market.Subscribe();

        market.PublishTime.Should().Be(default);
    }
}
