namespace Betfair.Extensions.Markets;

internal class BestAvailable
{
    private readonly Dictionary<int, Price> _prices = new (3) { { 0, Price.None }, { 1, Price.None }, { 2, Price.None }, };
    private readonly Dictionary<int, double> _sizes = new (3) { { 0, 0 }, { 1, 0 }, { 2, 0 }, };

    internal void Update(int level, double price, double size)
    {
        _prices[level] = Price.Of(price);
        _sizes[level] = size;
    }

    internal Price PriceAt(int level) => _prices[level];

    internal double SizeAt(int level) => _sizes[level];
}
