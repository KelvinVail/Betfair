using Betfair.Stream.OrderCache;

namespace Betfair.Tests.Stream.OrderCache;

public class OrderCacheTests
{
    [Theory]
    [InlineData("1.241629436")]
    [InlineData("1.999999999")]
    public void MarketIdIsSetFromConstructor(string marketId)
    {
        var cache = new Betfair.Stream.OrderCache.OrderCache(marketId);

        cache.MarketId.Should().Be(marketId);
    }

    [Fact]
    public void MarketIdBytesMatchUtf8EncodedMarketId()
    {
        var cache = new Betfair.Stream.OrderCache.OrderCache("1.241629436");

        cache.MarketIdBytes.Should().BeEquivalentTo(System.Text.Encoding.UTF8.GetBytes("1.241629436"));
    }

    [Fact]
    public void NewCacheHasZeroRunners()
    {
        var cache = new Betfair.Stream.OrderCache.OrderCache("1.1");

        cache.RunnerCount.Should().Be(0);
    }

    [Fact]
    public void GetOrAddRunnerCreatesNewRunner()
    {
        var cache = new Betfair.Stream.OrderCache.OrderCache("1.1");

        var runner = cache.GetOrAddRunner(12345);

        runner.Should().NotBeNull();
        runner.SelectionId.Should().Be(12345);
        cache.RunnerCount.Should().Be(1);
    }

    [Fact]
    public void GetOrAddRunnerReturnsExistingRunner()
    {
        var cache = new Betfair.Stream.OrderCache.OrderCache("1.1");
        var first = cache.GetOrAddRunner(12345);

        var second = cache.GetOrAddRunner(12345);

        second.Should().BeSameAs(first);
        cache.RunnerCount.Should().Be(1);
    }

    [Fact]
    public void GetRunnerReturnsNullForUnknown()
    {
        var cache = new Betfair.Stream.OrderCache.OrderCache("1.1");

        cache.GetRunner(99999).Should().BeNull();
    }

    [Fact]
    public void GetRunnerReturnsExistingRunner()
    {
        var cache = new Betfair.Stream.OrderCache.OrderCache("1.1");
        cache.GetOrAddRunner(12345);

        var runner = cache.GetRunner(12345);

        runner.Should().NotBeNull();
        runner!.SelectionId.Should().Be(12345);
    }

    [Fact]
    public void ClearRunnersRemovesAllRunners()
    {
        var cache = new Betfair.Stream.OrderCache.OrderCache("1.1");
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
        var cache = new Betfair.Stream.OrderCache.OrderCache("1.1");
        cache.GetOrAddRunner(111);
        cache.GetOrAddRunner(222);
        cache.GetOrAddRunner(333);

        var span = cache.RunnerSpan;

        span.Length.Should().Be(3);
        span[0].SelectionId.Should().Be(111);
        span[1].SelectionId.Should().Be(222);
        span[2].SelectionId.Should().Be(333);
    }

    [Fact]
    public void GetOrAddRunnerSetsHandicap()
    {
        var cache = new Betfair.Stream.OrderCache.OrderCache("1.1");

        var runner = cache.GetOrAddRunner(12345, 2.5);

        runner.Handicap.Should().Be(2.5);
    }

    [Fact]
    public void ArrayResizesWhenMoreThan16Runners()
    {
        var cache = new Betfair.Stream.OrderCache.OrderCache("1.1");

        for (int i = 1; i <= 20; i++)
            cache.GetOrAddRunner(i);

        cache.RunnerCount.Should().Be(20);
        cache.GetRunner(20).Should().NotBeNull();
    }

    [Fact]
    public void AccountIdDefaultsToNull()
    {
        var cache = new Betfair.Stream.OrderCache.OrderCache("1.1");
        cache.AccountId.Should().BeNull();
    }

    [Fact]
    public void ClosedDefaultsToNull()
    {
        var cache = new Betfair.Stream.OrderCache.OrderCache("1.1");
        cache.Closed.Should().BeNull();
    }

    [Fact]
    public void IsImageDefaultsToFalse()
    {
        var cache = new Betfair.Stream.OrderCache.OrderCache("1.1");
        cache.IsImage.Should().BeFalse();
    }
}
