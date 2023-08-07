using System.Diagnostics.CodeAnalysis;

namespace Betfair.Domain;

public class BackTrailingStopLoss
{
    private readonly int _entryTick;
    private readonly int _stopTicks;
    private int _stopTick;
    private bool _open = true;

    private BackTrailingStopLoss(
        int tick,
        int stopTicks)
    {
        _entryTick = tick;
        _stopTicks = stopTicks;
        _stopTick = tick + stopTicks;
    }

    public bool IsOpen => _open;

    public int Result { get; private set; }

    public static BackTrailingStopLoss Enter(
        [NotNull]Price price,
        int stopTicks) =>
        new (price.Tick, stopTicks);

    public void CurrentLayPrice([NotNull]Price currentLay)
    {
        if (!_open) return;
        var currentLayTick = currentLay.Tick;

        var newStop = currentLayTick + _stopTicks;
        if (newStop == _stopTick) return;
        if (newStop < _stopTick) _stopTick = newStop;

        if (StopIsTriggeredBy(currentLayTick))
            CloseTheTrade(currentLayTick);
    }

    public override string ToString() => $"Stop: {_stopTick}";

    private bool StopIsTriggeredBy(int currentTick) => currentTick >= _stopTick;

    private void CloseTheTrade(int currentTick)
    {
        Result = _entryTick - currentTick;
        _open = false;
    }
}
