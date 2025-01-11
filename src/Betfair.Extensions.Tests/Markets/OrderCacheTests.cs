using Betfair.Extensions.Markets;

namespace Betfair.Extensions.Tests.Markets;

public class OrderCacheTests
{
    // Backs IfWin
    [Theory]
    [InlineData(1.01, 10, 0.1)]
    [InlineData(3.5, 10, 25)]
    [InlineData(3.5, 0.01, 0.02)]
    public void IfWinShouldShowTheOutcomeOfAMatchedBack(
        double price,
        double size,
        double ifWin)
    {
        var cache = OrderCache.Create();

        cache.UpdateMatchedBacks(price, size);

        cache.IfWin.Should().BeApproximately(ifWin, 0.001);
    }

    [Theory]
    [InlineData(1.01, 10, 0.1)]
    [InlineData(3.5, 10, 25)]
    [InlineData(3.5, 0.01, 0.02)]
    public void IfWinShouldShowTheOutcomeOfMultipleMatchedBacks(
        double price,
        double size,
        double ifWin)
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
    public void IfWinShouldOnlyShowTheOutcomeOfTheLatestMatchedBackAtAnyParticularPrice(
        double price,
        double size,
        double ifWin)
    {
        var cache = OrderCache.Create();
        cache.UpdateMatchedBacks(price, 9.99);

        cache.UpdateMatchedBacks(price, size);

        cache.IfWin.Should().BeApproximately(ifWin, 0.001);
    }

    // Backs IfLose
    [Theory]
    [InlineData(1.01, 10)]
    [InlineData(3.5, 10)]
    [InlineData(3.5, 0.01)]
    public void IfLoseForABackShouldBeNegativeTheSize(
        double price,
        double size)
    {
        var cache = OrderCache.Create();

        cache.UpdateMatchedBacks(price, size);

        cache.IfLose.Should().Be(-size);
    }

    [Theory]
    [InlineData(1.01, 10)]
    [InlineData(3.5, 10)]
    [InlineData(3.5, 0.01)]
    public void IfLoseShouldShowTheOutcomeOfMultipleMatchedBacks(
        double price,
        double size)
    {
        var cache = OrderCache.Create();
        cache.UpdateMatchedBacks(3, 9.99);
        var bet1IfLose = cache.IfLose;

        cache.UpdateMatchedBacks(price, size);

        cache.IfLose.Should().Be(bet1IfLose - size);
    }

    [Theory]
    [InlineData(1.01, 10)]
    [InlineData(3.5, 10)]
    [InlineData(3.5, 0.01)]
    public void IfLoseShouldOnlyShowTheOutcomeOfTheLatestMatchedBackAtAnyParticularPrice(
        double price,
        double size)
    {
        var cache = OrderCache.Create();
        cache.UpdateMatchedBacks(price, 9.99);

        cache.UpdateMatchedBacks(price, size);

        cache.IfLose.Should().Be(-size);
    }

    // Lays IfWin
    [Theory]
    [InlineData(1.01, 10, -0.1)]
    [InlineData(3.5, 10, -25)]
    [InlineData(3.5, 0.01, -0.02)]
    public void IfWinForALayShouldBeNegativeThePayout(
        double price,
        double size,
        double ifWin)
    {
        var cache = OrderCache.Create();

        cache.UpdateMatchedLays(price, size);

        cache.IfWin.Should().BeApproximately(ifWin, 0.001);
    }

    [Theory]
    [InlineData(1.01, 10, -0.1)]
    [InlineData(3.5, 10, -25)]
    [InlineData(3.5, 0.01, -0.02)]
    public void IfWinShouldShowTheOutcomeOfMultipleMatchedLays(
        double price,
        double size,
        double ifWin)
    {
        var cache = OrderCache.Create();
        cache.UpdateMatchedLays(3, 9.99);
        var bet1IfWin = cache.IfWin;

        cache.UpdateMatchedLays(price, size);

        cache.IfWin.Should().BeApproximately(bet1IfWin + ifWin, 0.001);
    }

    [Theory]
    [InlineData(1.01, 10, -0.1)]
    [InlineData(3.5, 10, -25)]
    [InlineData(3.5, 0.01, -0.02)]
    public void IfWinShouldOnlyShowTheOutcomeOfTheLatestMatchedLayAtAnyParticularPrice(
        double price,
        double size,
        double ifWin)
    {
        var cache = OrderCache.Create();
        cache.UpdateMatchedLays(price, 9.99);

        cache.UpdateMatchedLays(price, size);

        cache.IfWin.Should().BeApproximately(ifWin, 0.001);
    }

    // Lays IfLose
    [Theory]
    [InlineData(1.01, 10)]
    [InlineData(3.5, 10)]
    [InlineData(3.5, 0.01)]
    public void IfLoseForALayShouldBeTheSize(
        double price,
        double size)
    {
        var cache = OrderCache.Create();

        cache.UpdateMatchedLays(price, size);

        cache.IfLose.Should().Be(size);
    }

    [Theory]
    [InlineData(1.01, 10)]
    [InlineData(3.5, 10)]
    [InlineData(3.5, 0.01)]
    public void IfLoseShouldShowTheOutcomeOfMultipleMatchedLays(
        double price,
        double size)
    {
        var cache = OrderCache.Create();
        cache.UpdateMatchedLays(3, 9.99);
        var bet1IfLose = cache.IfLose;

        cache.UpdateMatchedLays(price, size);

        cache.IfLose.Should().Be(bet1IfLose + size);
    }

    [Theory]
    [InlineData(1.01, 10)]
    [InlineData(3.5, 10)]
    [InlineData(3.5, 0.01)]
    public void IfLoseShouldOnlyShowTheOutcomeOfTheLatestMatchedLayAtAnyParticularPrice(
        double price,
        double size)
    {
        var cache = OrderCache.Create();
        cache.UpdateMatchedLays(price, 9.99);

        cache.UpdateMatchedLays(price, size);

        cache.IfLose.Should().Be(size);
    }
}
