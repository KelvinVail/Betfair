using System.Diagnostics;

namespace Betfair.Domain.Tests;

public class BackTrailingStopLossTests
{
    [Fact]
    public void InitiallyInOpenStatus()
    {
        var trade = BackTrailingStopLoss.Enter(Price.Of(3.0), 2);

        trade.IsOpen.Should().BeTrue();
    }

    [Theory]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(9)]
    public void IsClosedIfLayMovesOutMoreThanTheStopLoss(int stopTicks)
    {
        var entryPrice = Price.Of(3.0);
        var trade = BackTrailingStopLoss.Enter(entryPrice, stopTicks);

        var currentLay = entryPrice.AddTicks(stopTicks);
        trade.CurrentLayPrice(currentLay);

        trade.IsOpen.Should().BeFalse();
        trade.Result.Should().Be(currentLay.TicksBetween(entryPrice));
    }

    [Theory]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(9)]
    public void StaysOpenIfLayPriceDoesNotTriggerTheStopLoss(int stopTicks)
    {
        var entryPrice = Price.Of(3.0);
        var trade = BackTrailingStopLoss.Enter(entryPrice, stopTicks);

        var currentLay = entryPrice.AddTicks(stopTicks - 1);
        trade.CurrentLayPrice(currentLay);

        trade.IsOpen.Should().BeTrue();
    }

    [Theory]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(9)]
    public void StopIsMovedIfLayPriceComesIn(int stopTicks)
    {
        var entryPrice = Price.Of(3.0);
        var trade = BackTrailingStopLoss.Enter(entryPrice, stopTicks);

        // Move the stop as lay price comes in.
        var lay1 = entryPrice.AddTicks(-2);
        trade.CurrentLayPrice(lay1);
        trade.IsOpen.Should().BeTrue();

        // Trigger a stop
        var lay2 = lay1.AddTicks(stopTicks);
        trade.CurrentLayPrice(lay2);

        trade.IsOpen.Should().BeFalse();
        trade.Result.Should().Be(lay2.TicksBetween(entryPrice));
    }

    [Theory]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(9)]
    public void DoNothingIfTradeIsClosed(int stopTicks)
    {
        var entryPrice = Price.Of(3.0);
        var trade = BackTrailingStopLoss.Enter(entryPrice, stopTicks);

        var lay1 = entryPrice.AddTicks(stopTicks);
        trade.CurrentLayPrice(lay1);

        var lay2 = lay1.AddTicks(stopTicks);
        trade.CurrentLayPrice(lay2);

        trade.IsOpen.Should().BeFalse();
        trade.Result.Should().Be(lay1.TicksBetween(entryPrice));
    }

    [Fact]
    public void ProfileTest()
    {
        var entryPrice = Price.Of(110);
        for (int x = 0; x < 1000; x++)
        {
            var trade = BackTrailingStopLoss.Enter(entryPrice, 2);

            var currentLay = entryPrice.AddTicks(-2);
            for (int i = 0; i < 20; i++)
            {
                trade.CurrentLayPrice(currentLay);
                currentLay = currentLay.AddTicks(-2);
            }

            for (int i = 0; i < 50; i++)
                trade.CurrentLayPrice(currentLay);

            trade.CurrentLayPrice(currentLay.AddTicks(4));

            trade.IsOpen.Should().BeFalse();
            trade.Result.Should().Be(currentLay.TicksBetween(entryPrice) - 4);
        }
    }
}
