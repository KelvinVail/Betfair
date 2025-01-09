using Betfair.Extensions.Markets;

namespace Betfair.Extensions.Tests.Markets;

public class OrderCacheTests
{
    [Theory]
    [InlineData(1.01, 10, 0.1)]
    [InlineData(3.5, 10, 25)]
    [InlineData(3.5, 0.01, 0.02)]
    public void MatchedBacksUpdateIfWinValue(double price, double size, double ifWin)
    {
        var cache = OrderCache.Create();

        cache.UpdateMatchedBacks(price, size);

        cache.IfWin.Should().BeApproximately(ifWin, 0.001);
    }

    [Theory]
    [InlineData(1.01, 10)]
    [InlineData(3.5, 10)]
    [InlineData(3.5, 0.01)]
    public void MatchedBacksUpdateIfLoseValue(double price, double size)
    {
        var cache = OrderCache.Create();

        cache.UpdateMatchedBacks(price, size);

        cache.IfLose.Should().Be(size);
    }
}
