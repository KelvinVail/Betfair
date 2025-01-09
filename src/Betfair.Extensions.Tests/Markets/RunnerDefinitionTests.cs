using Betfair.Core.Login;
using Betfair.Extensions.Markets;
using Betfair.Extensions.Markets.Enums;
using Betfair.Extensions.Tests.TestDoubles;

namespace Betfair.Extensions.Tests.Markets;

public class RunnerDefinitionTests
{
    private readonly Credentials _credentials = new ("username", "password", "appKey");
    private readonly SubscriptionStub _sub = new ();
    private readonly MarketCache _market;

    public RunnerDefinitionTests() =>
        _market = MarketCache.Create(_credentials, "1.235123059", _sub).Value;

    [Theory]
    [InlineData(1, RunnerStatus.Active, 0)]
    [InlineData(2, RunnerStatus.Active, 15.99)]
    [InlineData(1, RunnerStatus.Hidden, 0)]
    [InlineData(1, RunnerStatus.Loser, 0)]
    [InlineData(1, RunnerStatus.Placed, 0)]
    [InlineData(1, RunnerStatus.Winner, 0)]
    public void ARunnerCanBeAdded(long id, RunnerStatus status, double adj)
    {
        _market.AddOrUpdateRunnerDefinition(id, status, adj);

        _market.Runners.Should().ContainSingle();
        _market.Runners.First().Id.Should().Be(id);
        _market.Runners.First().Status.Should().Be(status);
        _market.Runners.First().AdjustmentFactor.Should().Be(adj);
    }

    [Theory]
    [InlineData(RunnerStatus.Winner)]
    [InlineData(RunnerStatus.Loser)]
    public void TheStatusOfARunnerCanBeUpdated(RunnerStatus status)
    {
        _market.AddOrUpdateRunnerDefinition(1, RunnerStatus.Active, 0);

        _market.AddOrUpdateRunnerDefinition(1, status, 0);

        _market.Runners.First().Status.Should().Be(status);
    }

    [Theory]
    [InlineData(10)]
    [InlineData(99)]
    [InlineData(23.45)]
    public void AdjustmentFactorCanBeUpdated(double adj)
    {
        _market.AddOrUpdateRunnerDefinition(1, RunnerStatus.Active, 0);

        _market.AddOrUpdateRunnerDefinition(1, RunnerStatus.Active, adj);

        _market.Runners.First().AdjustmentFactor.Should().Be(adj);
    }

    [Fact]
    public void RemovedRunnersAreRemovedFromTheCache()
    {
        _market.AddOrUpdateRunnerDefinition(1, RunnerStatus.Active, 1.0);
        _market.Runners.Should().ContainSingle();

        _market.AddOrUpdateRunnerDefinition(1, RunnerStatus.Removed, 99.9);

        _market.Runners.Should().BeEmpty();
    }
}
