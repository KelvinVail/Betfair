using Betfair.Core.Login;
using Betfair.Extensions.Markets;
using Betfair.Extensions.Markets.Enums;
using Betfair.Extensions.Tests.TestDoubles;

namespace Betfair.Extensions.Tests.Markets;

public class BestAvailableTests
{
    private readonly Market _market = Market.Create(new Credentials("u", "p", "k"), "1.235123059", new SubscriptionStub()).Value;

    [Theory]
    [InlineData(1, 1.01, 9.99)]
    public void BestAvailableToBackLevelZeroCanBeUpdated(long id, double price, double size)
    {
        _market.AddOrUpdateRunnerDefinition(id, RunnerStatus.Active, 0);

        _market.UpdateBestAvailableToBack(id, 0, price, size);

        var runner = _market.Runners.Single(x => x.Id == id);
        runner.BestAvailableToBackPrice(0).Should().Be(Price.Of(price));
    }
}
