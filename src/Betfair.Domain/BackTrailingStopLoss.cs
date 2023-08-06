namespace Betfair.Domain;

public class BackTrailingStopLoss
{
    private readonly Price _entryPrice;
    private readonly int _stopTicks;
    private Price _stop;

    private BackTrailingStopLoss(
        Price price,
        int stopTicks)
    {
        IsOpen = true;
        _entryPrice = price;
        _stopTicks = stopTicks;
        _stop = price.AddTicks(stopTicks);
    }

    public bool IsOpen { get; private set; }

    public int Result { get; private set; }

    public static BackTrailingStopLoss Enter(
        double decimalOdds,
        int stopTicks) =>
        new (Price.Of(decimalOdds), stopTicks);

    public void CurrentLayPrice(double price)
    {
        if (!IsOpen) return;

        var currentLay = Price.Of(price);
        if (_stop.TicksBetween(currentLay) >= 0)
        {
            Result = currentLay.TicksBetween(_entryPrice);
            IsOpen = false;
        }

        _stop = currentLay.AddTicks(_stopTicks);
    }
}
