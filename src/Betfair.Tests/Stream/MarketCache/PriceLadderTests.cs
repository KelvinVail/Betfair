using Betfair.Stream.MarketCache;

namespace Betfair.Tests.Stream.MarketCache;

public class PriceLadderTests
{
    private readonly PriceLadder _ladder = new ();

    [Fact]
    public void NewLadderHasZeroCount()
    {
        _ladder.Count.Should().Be(0);
    }

    [Theory]
    [InlineData(1.5, 100.0)]
    [InlineData(3.75, 42.5)]
    [InlineData(1000.0, 0.01)]
    public void UpdateAddsPriceLevel(double price, double size)
    {
        _ladder.Update(price, size);

        _ladder[price].Should().Be(size);
        _ladder.Count.Should().Be(1);
    }

    [Fact]
    public void UpdateReplacesExistingPriceLevel()
    {
        _ladder.Update(2.5, 100.0);
        _ladder.Update(2.5, 200.0);

        _ladder[2.5].Should().Be(200.0);
        _ladder.Count.Should().Be(1);
    }

    [Fact]
    public void UpdateWithZeroSizeRemovesPriceLevel()
    {
        _ladder.Update(3.0, 50.0);
        _ladder.Update(3.0, 0);

        _ladder[3.0].Should().Be(0);
        _ladder.Count.Should().Be(0);
    }

    [Fact]
    public void IndexerReturnsZeroForNonExistentPrice()
    {
        _ladder[99.0].Should().Be(0);
    }

    [Fact]
    public void MultipleUpdatesTrackSeparatePriceLevels()
    {
        _ladder.Update(1.5, 10.0);
        _ladder.Update(2.0, 20.0);
        _ladder.Update(3.5, 30.0);

        _ladder.Count.Should().Be(3);
        _ladder[1.5].Should().Be(10.0);
        _ladder[2.0].Should().Be(20.0);
        _ladder[3.5].Should().Be(30.0);
    }

    [Fact]
    public void ClearRemovesAllLevels()
    {
        _ladder.Update(1.5, 10.0);
        _ladder.Update(2.0, 20.0);

        _ladder.Clear();

        _ladder.Count.Should().Be(0);
        _ladder[1.5].Should().Be(0);
        _ladder[2.0].Should().Be(0);
    }

    [Fact]
    public void RemovingNonExistentPriceDoesNotThrow()
    {
        var act = () => _ladder.Update(5.0, 0);

        act.Should().NotThrow();
        _ladder.Count.Should().Be(0);
    }

    [Fact]
    public void GetEnumeratorIteratesAllActiveLevels()
    {
        _ladder.Update(1.0, 10.0);
        _ladder.Update(2.0, 20.0);
        _ladder.Update(3.0, 30.0);

        var pairs = new Dictionary<double, double>();
        foreach (var kvp in _ladder)
            pairs[kvp.Key] = kvp.Value;

        pairs.Should().HaveCount(3);
        pairs[1.0].Should().Be(10.0);
        pairs[2.0].Should().Be(20.0);
        pairs[3.0].Should().Be(30.0);
    }
}
