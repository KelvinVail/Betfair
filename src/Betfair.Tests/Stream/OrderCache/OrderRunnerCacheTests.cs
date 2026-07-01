using Betfair.Stream.OrderCache;

namespace Betfair.Tests.Stream.OrderCache;

public class OrderRunnerCacheTests
{
    [Fact]
    public void SelectionIdIsSetFromConstructor()
    {
        var runner = new OrderRunnerCache(12345);

        runner.SelectionId.Should().Be(12345);
    }

    [Fact]
    public void HandicapDefaultsToZero()
    {
        var runner = new OrderRunnerCache(12345);

        runner.Handicap.Should().Be(0);
    }

    [Fact]
    public void HandicapIsSetFromConstructor()
    {
        var runner = new OrderRunnerCache(12345, 1.5);

        runner.Handicap.Should().Be(1.5);
    }

    [Fact]
    public void MatchedBacksIsInitialized()
    {
        var runner = new OrderRunnerCache(1);

        runner.MatchedBacks.Should().NotBeNull();
        runner.MatchedBacks.Count.Should().Be(0);
    }

    [Fact]
    public void MatchedLaysIsInitialized()
    {
        var runner = new OrderRunnerCache(1);

        runner.MatchedLays.Should().NotBeNull();
        runner.MatchedLays.Count.Should().Be(0);
    }

    [Fact]
    public void GetOrAddOrderCreatesNewOrder()
    {
        var runner = new OrderRunnerCache(1);

        var order = runner.GetOrAddOrder("bet1");

        order.Should().NotBeNull();
        order.BetId.Should().Be("bet1");
        runner.OrderCount.Should().Be(1);
    }

    [Fact]
    public void GetOrAddOrderReturnsExistingOrder()
    {
        var runner = new OrderRunnerCache(1);
        var first = runner.GetOrAddOrder("bet1");

        var second = runner.GetOrAddOrder("bet1");

        second.Should().BeSameAs(first);
        runner.OrderCount.Should().Be(1);
    }

    [Fact]
    public void GetOrderReturnsNullForUnknown()
    {
        var runner = new OrderRunnerCache(1);

        runner.GetOrder("unknown").Should().BeNull();
    }

    [Fact]
    public void GetOrderReturnsExistingOrder()
    {
        var runner = new OrderRunnerCache(1);
        runner.GetOrAddOrder("bet1");

        var order = runner.GetOrder("bet1");

        order.Should().NotBeNull();
        order!.BetId.Should().Be("bet1");
    }

    [Fact]
    public void RemoveOrderDeletesOrder()
    {
        var runner = new OrderRunnerCache(1);
        runner.GetOrAddOrder("bet1");

        runner.RemoveOrder("bet1");

        runner.GetOrder("bet1").Should().BeNull();
    }

    [Fact]
    public void GetOrAddStrategyCreatesNewStrategy()
    {
        var runner = new OrderRunnerCache(1);

        var strat = runner.GetOrAddStrategy("myRef");

        strat.Should().NotBeNull();
        strat.StrategyRef.Should().Be("myRef");
    }

    [Fact]
    public void GetOrAddStrategyReturnsExistingStrategy()
    {
        var runner = new OrderRunnerCache(1);
        var first = runner.GetOrAddStrategy("myRef");

        var second = runner.GetOrAddStrategy("myRef");

        second.Should().BeSameAs(first);
    }

    [Fact]
    public void StrategyMatchesExposesAllStrategies()
    {
        var runner = new OrderRunnerCache(1);
        runner.GetOrAddStrategy("strat1");
        runner.GetOrAddStrategy("strat2");

        runner.StrategyMatches.Should().HaveCount(2);
        runner.StrategyMatches.Should().ContainKey("strat1");
        runner.StrategyMatches.Should().ContainKey("strat2");
    }

    [Fact]
    public void ClearRemovesAllState()
    {
        var runner = new OrderRunnerCache(1);
        runner.MatchedBacks.Update(2.0, 50.0);
        runner.MatchedLays.Update(3.0, 25.0);
        runner.GetOrAddOrder("bet1");
        runner.GetOrAddStrategy("strat1");

        runner.Clear();

        runner.MatchedBacks.Count.Should().Be(0);
        runner.MatchedLays.Count.Should().Be(0);
        runner.OrderCount.Should().Be(0);
        runner.StrategyMatches.Should().BeEmpty();
    }

    [Fact]
    public void OrdersDictionaryExposesAllOrders()
    {
        var runner = new OrderRunnerCache(1);
        runner.GetOrAddOrder("bet1");
        runner.GetOrAddOrder("bet2");

        runner.Orders.Should().HaveCount(2);
        runner.Orders.Should().ContainKey("bet1");
        runner.Orders.Should().ContainKey("bet2");
    }

    [Fact]
    public void OrderSpanReturnsAllOrders()
    {
        var runner = new OrderRunnerCache(1);
        runner.GetOrAddOrder("bet1");
        runner.GetOrAddOrder("bet2");

        var span = runner.OrderSpan;

        span.Length.Should().Be(2);
        span[0].BetId.Should().Be("bet1");
        span[1].BetId.Should().Be("bet2");
    }

    [Fact]
    public void GetOrderByBytesFindsOrder()
    {
        var runner = new OrderRunnerCache(1);
        runner.GetOrAddOrder("bet1");

        var result = runner.GetOrderByBytes("bet1"u8);

        result.Should().NotBeNull();
        result!.BetId.Should().Be("bet1");
    }

    [Fact]
    public void GetOrderByBytesReturnsNullForUnknown()
    {
        var runner = new OrderRunnerCache(1);

        runner.GetOrderByBytes("unknown"u8).Should().BeNull();
    }

    [Fact]
    public void ClearPoolsOrdersForReuse()
    {
        var runner = new OrderRunnerCache(1);
        var original = runner.GetOrAddOrder("bet1");
        original.Price = 5.0;

        runner.Clear();

        // Adding a new order should reuse the pooled instance
        var reused = runner.GetOrAddOrder("bet2");
        reused.Price.Should().Be(double.NaN); // Reset was called
    }

    [Fact]
    public void RemoveOrderForNonExistentBetIdIsNoOp()
    {
        var runner = new OrderRunnerCache(1);
        runner.GetOrAddOrder("bet1");

        runner.RemoveOrder("nonexistent");

        runner.OrderCount.Should().Be(1);
    }

    [Fact]
    public void ArrayResizesWhenMoreThan8Orders()
    {
        var runner = new OrderRunnerCache(1);

        for (int i = 1; i <= 12; i++)
            runner.GetOrAddOrder($"bet{i}");

        runner.OrderCount.Should().Be(12);
        runner.GetOrder("bet12").Should().NotBeNull();
    }
}
