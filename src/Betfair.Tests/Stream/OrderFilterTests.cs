using Betfair.Stream.Messages;

namespace Betfair.Tests.Stream;

public class OrderFilterTests
{
    private readonly OrderFilter _filter = new ();

    [Fact]
    public void IncludeOverallPositionIsNullAsDefault() =>
        _filter.IncludeOverallPosition.Should().BeNull();

    [Fact]
    public void PartitionMatchedByStrategyRefIsNullAsDefault() =>
        _filter.PartitionMatchedByStrategyRef.Should().BeNull();

    [Fact]
    public void CustomerStrategyRefsIsNullAsDefault() =>
        _filter.CustomerStrategyRefs.Should().BeNull();

    [Fact]
    public void WithAggregatedPositionsSetsOverallPositionToTrue()
    {
        _filter.WithAggregatedPositions();

        _filter.IncludeOverallPosition.Should().BeTrue();
    }

    [Fact]
    public void WithDetailedPositionsSetsOverallPositionToFalse()
    {
        _filter.WithDetailedPositions();

        _filter.IncludeOverallPosition.Should().BeFalse();
    }

    [Fact]
    public void WithOrderPerStrategySetsPartitionToTrue()
    {
        _filter.WithOrdersPerStrategy();

        _filter.PartitionMatchedByStrategyRef.Should().BeTrue();
    }

    [Theory]
    [InlineData("Ref001")]
    [InlineData("experimental")]
    public void StrategyRefAreAddedToFilter(string strategyRef)
    {
        _filter.WithStrategyRefs(strategyRef);

        _filter.CustomerStrategyRefs.Should().Contain(strategyRef);
    }
}
