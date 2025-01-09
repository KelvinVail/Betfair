namespace Betfair.Extensions.Markets;

internal class OrderCache
{
    private OrderCache()
    {
    }

    public double IfWin { get; private set; }

    public double IfLose { get; private set; }

    internal static OrderCache Create() =>
        new();

    internal void UpdateMatchedBacks(double price, double size)
    {
        IfWin = Math.Round((price * size) - size, 2);
        IfLose = size;
    }

    internal void UpdateMatchedLays(double price, double size)
    {
    }
}
