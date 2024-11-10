using Betfair.Core.Login;
using Betfair.Extensions.Markets;
using Betfair.Extensions.Markets.Enums;
using Betfair.Extensions.Tests.TestDoubles;

namespace Betfair.Extensions.Tests.Markets;

public class BestAvailableTests
{
    private readonly Market _market = Market.Create(new Credentials("u", "p", "k"), "1.235123059", new SubscriptionStub()).Value;

    [Theory]
    [InlineData(1, 0, 1.01, 9.99)]
    [InlineData(2, 1, 3.5, 1.99)]
    [InlineData(9876543210, 10, 80, 3.24)]
    public void BestAvailableToBackCanBeSet(
        long id,
        int level,
        double price,
        double size)
    {
        _market.AddOrUpdateRunnerDefinition(id, RunnerStatus.Active, 0);

        _market.UpdateBestAvailableToBack(id, level, price, size);

        var expected = new PriceSize(price, size);
        var runner = _market.Runners.Single(x => x.Id == id);
        runner.BestAvailableToBack[level].Should().BeEquivalentTo(expected);
    }

    [Theory]
    [InlineData(1, 0, 1.01, 9.99)]
    [InlineData(2, 1, 3.5, 1.99)]
    [InlineData(9876543210, 10, 80, 3.24)]
    public void BestAvailableToBackCanBeUpdate(
        long id,
        int level,
        double price,
        double size)
    {
        _market.AddOrUpdateRunnerDefinition(id, RunnerStatus.Active, 0);
        _market.UpdateBestAvailableToBack(id, level, 1000, 0);

        _market.UpdateBestAvailableToBack(id, level, price, size);

        var expected = new PriceSize(price, size);
        var runner = _market.Runners.Single(x => x.Id == id);
        runner.BestAvailableToBack[level].Should().BeEquivalentTo(expected);
    }

    [Theory]
    [InlineData(1, 0, 1.01, 9.99)]
    [InlineData(2, 1, 3.5, 1.99)]
    [InlineData(9876543210, 10, 80, 3.24)]
    public void BestAvailableToLayCanBeSet(
        long id,
        int level,
        double price,
        double size)
    {
        _market.AddOrUpdateRunnerDefinition(id, RunnerStatus.Active, 0);

        _market.UpdateBestAvailableToLay(id, level, price, size);

        var expected = new PriceSize(price, size);
        var runner = _market.Runners.Single(x => x.Id == id);
        runner.BestAvailableToLay[level].Should().BeEquivalentTo(expected);
    }

    [Theory]
    [InlineData(1, 0, 1.01, 9.99)]
    [InlineData(2, 1, 3.5, 1.99)]
    [InlineData(9876543210, 10, 80, 3.24)]
    public void BestAvailableToLayCanBeUpdate(
        long id,
        int level,
        double price,
        double size)
    {
        _market.AddOrUpdateRunnerDefinition(id, RunnerStatus.Active, 0);
        _market.UpdateBestAvailableToLay(id, level, 1000, 0);

        _market.UpdateBestAvailableToLay(id, level, price, size);

        var expected = new PriceSize(price, size);
        var runner = _market.Runners.Single(x => x.Id == id);
        runner.BestAvailableToLay[level].Should().BeEquivalentTo(expected);
    }
}
