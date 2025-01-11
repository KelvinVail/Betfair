namespace Betfair.Extensions.Markets;

internal class OrderCache
{
    private readonly PriceLadderDictionary _matchedBacks = [];

    private OrderCache()
    {
    }

    public double IfWin { get; private set; }

    public double IfLose { get; private set; }

    internal static OrderCache Create() =>
        new();

    internal void UpdateMatchedBacks(double price, double size)
    {
        _matchedBacks.Update(price, Math.Round((price * size) - size, 2));
        IfLose = -size;

        IfWin = _matchedBacks.Values.Sum();
    }

    internal void UpdateMatchedLays(double price, double size)
    {
    }
}
