using Betfair.Stream.MarketCache;

namespace Betfair.Tests.Stream.MarketCache;

public class MarketCacheTests
{
    [Theory]
    [InlineData("1.241629436")]
    [InlineData("1.999999999")]
    public void MarketIdIsSetFromConstructor(string marketId)
    {
        var cache = new Betfair.Stream.MarketCache.MarketCache(marketId);

        cache.MarketId.Should().Be(marketId);
    }

    [Fact]
    public void MarketIdBytesMatchUtf8EncodedMarketId()
    {
        var cache = new Betfair.Stream.MarketCache.MarketCache("1.241629436");

        cache.MarketIdBytes.Should().BeEquivalentTo(System.Text.Encoding.UTF8.GetBytes("1.241629436"));
    }

    [Fact]
    public void NewCacheHasZeroRunners()
    {
        var cache = new Betfair.Stream.MarketCache.MarketCache("1.1");

        cache.RunnerCount.Should().Be(0);
    }

    [Fact]
    public void GetOrAddRunnerCreatesNewRunner()
    {
        var cache = new Betfair.Stream.MarketCache.MarketCache("1.1");

        var runner = cache.GetOrAddRunner(12345);

        runner.Should().NotBeNull();
        runner.SelectionId.Should().Be(12345);
        cache.RunnerCount.Should().Be(1);
    }

    [Fact]
    public void GetOrAddRunnerReturnsExistingRunner()
    {
        var cache = new Betfair.Stream.MarketCache.MarketCache("1.1");
        var first = cache.GetOrAddRunner(12345);

        var second = cache.GetOrAddRunner(12345);

        second.Should().BeSameAs(first);
        cache.RunnerCount.Should().Be(1);
    }

    [Fact]
    public void GetOrAddRunnerSupportsMultipleRunners()
    {
        var cache = new Betfair.Stream.MarketCache.MarketCache("1.1");

        cache.GetOrAddRunner(111);
        cache.GetOrAddRunner(222);
        cache.GetOrAddRunner(333);

        cache.RunnerCount.Should().Be(3);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1.5)]
    public void GetOrAddRunnerSetsHandicap(double handicap)
    {
        var cache = new Betfair.Stream.MarketCache.MarketCache("1.1");

        var runner = cache.GetOrAddRunner(12345, handicap);

        runner.Handicap.Should().Be(handicap);
    }

    [Fact]
    public void GetRunnerReturnsNullForUnknownSelection()
    {
        var cache = new Betfair.Stream.MarketCache.MarketCache("1.1");

        cache.GetRunner(99999).Should().BeNull();
    }

    [Fact]
    public void GetRunnerReturnsExistingRunner()
    {
        var cache = new Betfair.Stream.MarketCache.MarketCache("1.1");
        var added = cache.GetOrAddRunner(12345);

        var found = cache.GetRunner(12345);

        found.Should().BeSameAs(added);
    }

    [Fact]
    public void ClearRunnersRemovesAllRunners()
    {
        var cache = new Betfair.Stream.MarketCache.MarketCache("1.1");
        cache.GetOrAddRunner(111);
        cache.GetOrAddRunner(222);

        cache.ClearRunners();

        cache.RunnerCount.Should().Be(0);
        cache.GetRunner(111).Should().BeNull();
        cache.GetRunner(222).Should().BeNull();
    }

    [Fact]
    public void RunnerSpanReturnsAllRunners()
    {
        var cache = new Betfair.Stream.MarketCache.MarketCache("1.1");
        cache.GetOrAddRunner(111);
        cache.GetOrAddRunner(222);

        var span = cache.RunnerSpan;

        span.Length.Should().Be(2);
        span[0].SelectionId.Should().Be(111);
        span[1].SelectionId.Should().Be(222);
    }

    [Fact]
    public void RunnersDictionaryContainsAllRunners()
    {
        var cache = new Betfair.Stream.MarketCache.MarketCache("1.1");
        cache.GetOrAddRunner(111);
        cache.GetOrAddRunner(222);

        var dict = cache.Runners;

        dict.Should().HaveCount(2);
        dict.Should().ContainKey(111);
        dict.Should().ContainKey(222);
    }

    [Fact]
    public void TotalMatchedDefaultsToNull()
    {
        var cache = new Betfair.Stream.MarketCache.MarketCache("1.1");

        cache.TotalMatched.Should().BeNull();
    }

    [Fact]
    public void IsImageDefaultsToFalse()
    {
        var cache = new Betfair.Stream.MarketCache.MarketCache("1.1");

        cache.IsImage.Should().BeFalse();
    }

    [Fact]
    public void DefinitionIsInitialized()
    {
        var cache = new Betfair.Stream.MarketCache.MarketCache("1.1");

        cache.Definition.Should().NotBeNull();
    }

    [Fact]
    public void GetOrAddRunnerGrowsArrayBeyondInitialCapacity()
    {
        var cache = new Betfair.Stream.MarketCache.MarketCache("1.1");

        for (int i = 1; i <= 20; i++)
            cache.GetOrAddRunner(i);

        cache.RunnerCount.Should().Be(20);
        cache.GetRunner(20).Should().NotBeNull();
    }
}
