using System.Collections;

namespace Betfair.Domain;

public sealed class Ladder : IEnumerable<KeyValuePair<Price, Size>>
{
    private readonly Dictionary<Price, Size> _ladder = new (350);

    private Ladder()
    {
    }

    public static Ladder Create() =>
        new ();

    public Size Size(Price price)
    {
        if (price is null) return Domain.Size.Of(0);
        if (!_ladder.ContainsKey(price)) return Domain.Size.Of(0);

        return _ladder[price];
    }

    public void AddOrUpdate(Price price, Size size)
    {
        if (_ladder.ContainsKey(price))
            _ladder.Remove(price);

        _ladder.Add(price, size);
    }

    public IEnumerator<KeyValuePair<Price, Size>> GetEnumerator() =>
        _ladder.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}