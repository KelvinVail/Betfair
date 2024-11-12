using System.Collections;

namespace Betfair.Extensions.Markets;

public sealed class LevelLadder : IReadOnlyDictionary<int, PriceSize>
{
    private readonly Dictionary<int, double> _priceLadder = [];
    private readonly Dictionary<int, double> _sizeLadder = [];

    public IEnumerable<int> Keys => _priceLadder.Keys;

    public IEnumerable<PriceSize> Values => AllKeyValues().Values;

    public int Count => _priceLadder.Count;

    public IEnumerator<KeyValuePair<int, PriceSize>> GetEnumerator() =>
        _priceLadder.Keys.Select(key => new KeyValuePair<int, PriceSize>(key, new PriceSize(_priceLadder[key], _sizeLadder[key]))).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() =>
        GetEnumerator();

    public bool ContainsKey(int key) =>
        _priceLadder.ContainsKey(key);

    public bool TryGetValue(int key, out PriceSize value) =>
        AllKeyValues().TryGetValue(key, out value!);

    public PriceSize this[int key] =>
        AllKeyValues()[key];

    internal void Update(int level, double price, double size)
    {
        _priceLadder[level] = price;
        _sizeLadder[level] = size;
    }

    private Dictionary<int, PriceSize> AllKeyValues() =>
        _priceLadder.Keys.ToDictionary(key => key, key => new PriceSize(_priceLadder[key], _sizeLadder[key]));
}
