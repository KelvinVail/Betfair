using Betfair.Stream.OrderCache;

namespace Betfair.Tests.Stream.OrderCache;

public class StrategyMatchCacheTests
{
    [Fact]
    public void StrategyRefIsSetFromConstructor()
    {
        var cache = new StrategyMatchCache("myStrat"u8.ToArray());

        cache.StrategyRef.Should().Be("myStrat");
    }

    [Fact]
    public void MatchedBacksIsInitialized()
    {
        var cache = new StrategyMatchCache("s1"u8.ToArray());

        cache.MatchedBacks.Should().NotBeNull();
        cache.MatchedBacks.Count.Should().Be(0);
    }

    [Fact]
    public void MatchedLaysIsInitialized()
    {
        var cache = new StrategyMatchCache("s1"u8.ToArray());

        cache.MatchedLays.Should().NotBeNull();
        cache.MatchedLays.Count.Should().Be(0);
    }

    [Fact]
    public void ClearRemovesAllLadderData()
    {
        var cache = new StrategyMatchCache("s1"u8.ToArray());
        cache.MatchedBacks.Update(2.0, 50.0);
        cache.MatchedLays.Update(3.0, 25.0);

        cache.Clear();

        cache.MatchedBacks.Count.Should().Be(0);
        cache.MatchedLays.Count.Should().Be(0);
    }

    [Fact]
    public void StrategyRefBytesReturnsCorrectSpan()
    {
        var cache = new StrategyMatchCache("myStrat"u8.ToArray());

        cache.StrategyRefBytes.ToArray().Should().BeEquivalentTo("myStrat"u8.ToArray());
    }

    [Fact]
    public void StrategyRefEqualsReturnsTrueForMatch()
    {
        var cache = new StrategyMatchCache("myStrat"u8.ToArray());

        cache.StrategyRefEquals("myStrat"u8).Should().BeTrue();
    }

    [Fact]
    public void StrategyRefEqualsReturnsFalseForMismatch()
    {
        var cache = new StrategyMatchCache("myStrat"u8.ToArray());

        cache.StrategyRefEquals("other"u8).Should().BeFalse();
    }
}
