namespace Betfair.Extensions.Markets;

internal class OrderCache
{
    private readonly Dictionary<double, IfWinIfLose> _matchedBacks = [];
    private readonly Dictionary<double, IfWinIfLose> _matchedLays = [];

    private OrderCache()
    {
    }

    public double IfWin { get; private set; }

    public double IfLose { get; private set; }

    internal static OrderCache Create() => new ();

    internal void UpdateMatchedBacks(double price, double size)
    {
        if (_matchedBacks.Remove(price, out var previous))
        {
            IfWin -= previous.IfWin;
            IfLose -= previous.IfLose;
        }

        var ifWin = (price * size) - size;
        var ifLose = -size;
        var roundedIfWin = Math.Round(ifWin, 2, MidpointRounding.ToEven);
        _matchedBacks[price] = new IfWinIfLose(roundedIfWin, ifLose);

        IfWin += roundedIfWin;
        IfLose += ifLose;
    }

    internal void UpdateMatchedLays(double price, double size)
    {
        if (_matchedLays.Remove(price, out var previous))
        {
            IfWin -= previous.IfWin;
            IfLose -= previous.IfLose;
        }

        var ifWin = -((price * size) - size);
        var roundedIfWin = Math.Round(ifWin, 2, MidpointRounding.ToEven);
        _matchedLays[price] = new IfWinIfLose(roundedIfWin, size);

        IfWin += roundedIfWin;
        IfLose += size;
    }

    private readonly struct IfWinIfLose(double ifWin, double ifLose)
    {
        public double IfWin { get; } = ifWin;

        public double IfLose { get; } = ifLose;
    }
}
