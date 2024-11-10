namespace Betfair.Extensions.Markets;

public sealed class PriceSize
{
    internal PriceSize(double price, double size)
    {
        Price = Price.Of(price);
        Size = size;
    }

    public Price Price { get; }

    public double Size { get; }

    public override string ToString() => $"{Price}:{Size}";
}
