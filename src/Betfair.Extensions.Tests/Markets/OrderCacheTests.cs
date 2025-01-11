using Betfair.Extensions.Markets;

namespace Betfair.Extensions.Tests.Markets;

public class OrderCacheTests
{
    [Theory]
    [InlineData(1.01, 10, 0.1)]
    [InlineData(3.5, 10, 25)]
    [InlineData(3.5, 0.01, 0.02)]
    public void IfWinShouldShowTheOutcomeOfAMatchedBack(double price, double size, double ifWin)
    {
        var cache = OrderCache.Create();

        cache.UpdateMatchedBacks(price, size);

        cache.IfWin.Should().BeApproximately(ifWin, 0.001);
    }

    [Theory]
    [InlineData(1.01, 10, 0.1)]
    [InlineData(3.5, 10, 25)]
    [InlineData(3.5, 0.01, 0.02)]
    public void IfWinShouldShowTheOutcomeOfMultipleMatchedBacks(double price, double size, double ifWin)
    {
        var cache = OrderCache.Create();
        cache.UpdateMatchedBacks(3, 9.99);
        var bet1IfWin = cache.IfWin;

        cache.UpdateMatchedBacks(price, size);

        cache.IfWin.Should().BeApproximately(bet1IfWin + ifWin, 0.001);
    }

    [Theory]
    [InlineData(1.01, 10, 0.1)]
    [InlineData(3.5, 10, 25)]
    [InlineData(3.5, 0.01, 0.02)]
    public void IfWinShouldOnlyShowTheOutcomeOfTheLatestMatchedBackAtAnyParticularPrice(double price, double size, double ifWin)
    {
        var cache = OrderCache.Create();
        cache.UpdateMatchedBacks(price, 9.99);

        cache.UpdateMatchedBacks(price, size);

        cache.IfWin.Should().BeApproximately(ifWin, 0.001);
    }

    [Theory]
    [InlineData(1.01, 10)]
    [InlineData(3.5, 10)]
    [InlineData(3.5, 0.01)]
    public void MatchedBacksUpdatesIfLoseValue(double price, double size)
    {
        var cache = OrderCache.Create();

        cache.UpdateMatchedBacks(price, size);

        cache.IfLose.Should().Be(-size);
    }
}
