namespace Betfair.Domain.Tests;

public class BackTrailingStopLossTests
{
    [Fact]
    public void InitiallyInOpenStatus()
    {
        var trade = BackTrailingStopLoss.Enter(3.0, 2);

        trade.IsOpen.Should().BeTrue();
    }

    [Theory]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(9)]
    public void IsClosedIfLayMovesOutMoreThanTheStopLoss(int stopTicks)
    {
        var entryPrice = Price.Of(3.0);
        var trade = BackTrailingStopLoss.Enter(entryPrice.DecimalOdds, stopTicks);

        var currentLay = entryPrice.AddTicks(stopTicks);
        trade.CurrentLayPrice(currentLay.DecimalOdds);

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
        var trade = BackTrailingStopLoss.Enter(entryPrice.DecimalOdds, stopTicks);

        var currentLay = entryPrice.AddTicks(stopTicks - 1);
        trade.CurrentLayPrice(currentLay.DecimalOdds);

        trade.IsOpen.Should().BeTrue();
    }

    [Theory]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(9)]
    public void StopIsMovedIfLayPriceComesIn(int stopTicks)
    {
        var entryPrice = Price.Of(3.0);
        var trade = BackTrailingStopLoss.Enter(entryPrice.DecimalOdds, stopTicks);

        // Move the stop as lay price comes in.
        var lay1 = entryPrice.AddTicks(-2);
        trade.CurrentLayPrice(lay1.DecimalOdds);
        trade.IsOpen.Should().BeTrue();

        // Trigger a stop
        var lay2 = lay1.AddTicks(stopTicks);
        trade.CurrentLayPrice(lay2.DecimalOdds);

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
        var trade = BackTrailingStopLoss.Enter(entryPrice.DecimalOdds, stopTicks);

        var lay1 = entryPrice.AddTicks(stopTicks);
        trade.CurrentLayPrice(lay1.DecimalOdds);

        var lay2 = lay1.AddTicks(stopTicks);
        trade.CurrentLayPrice(lay2.DecimalOdds);

        trade.IsOpen.Should().BeFalse();
        trade.Result.Should().Be(lay1.TicksBetween(entryPrice));
    }
}
