using Betfair.Core.Login;
using Betfair.Extensions.Markets;
using Betfair.Extensions.Markets.Enums;
using Betfair.Extensions.Tests.TestDoubles;

namespace Betfair.Extensions.Tests.Markets;

public class TradedLadderTests
{
    private readonly MarketCache _market = MarketCache.Create(new Credentials("u", "p", "k"), "1.235123059", new SubscriptionStub()).Value;

    [Theory]
    [InlineData(1, 1.01, 9.99)]
    [InlineData(2, 3.5, 1.99)]
    [InlineData(9876543210, 80, 3.24)]
    public void TradedLadderCanBeSet(
        long id,
        double price,
        double size)
    {
        _market.AddOrUpdateRunnerDefinition(id, RunnerStatus.Active, 0);

        _market.UpdateTradedLadder(id, price, size);

        var runner = _market.Runners.Single(x => x.Id == id);
        runner.TradedLadder[price].Should().Be(size);
    }

    [Theory]
    [InlineData(1, 1.01, 9.99)]
    [InlineData(2, 3.5, 1.99)]
    [InlineData(9876543210, 80, 3.24)]
    public void TradedLadderCanBeUpdated(
        long id,
        double price,
        double size)
    {
        _market.AddOrUpdateRunnerDefinition(id, RunnerStatus.Active, 0);
        _market.UpdateTradedLadder(id, price, 0);

        _market.UpdateTradedLadder(id, price, size);

        var runner = _market.Runners.Single(x => x.Id == id);
        runner.TradedLadder[price].Should().Be(size);
    }
}
