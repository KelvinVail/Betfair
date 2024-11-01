using Betfair.Core.Login;
using Betfair.Extensions.Markets;

namespace Betfair.Extensions.Tests.Markets;

public class RunnerDefinitionTests
{
    private readonly Credentials _credentials = new ("username", "password", "appKey");
    private readonly SubscriptionStub _sub = new (Path.Combine("Data", "MarketDefinitions", "InitialImage.json"));
    private readonly Market _market;

    public RunnerDefinitionTests() =>
        _market = Market.Create(_credentials, "1.235123059", _sub).Value;

    [Fact]
    public async Task ThereShouldBeFourActiveRunners()
    {
        await _market.Subscribe();

        _market.Runners.Count.Should().Be(4);
    }
}
