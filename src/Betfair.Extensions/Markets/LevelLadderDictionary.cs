using System.Collections;

namespace Betfair.Extensions.Markets;

public sealed class LevelLadderDictionary : IReadOnlyDictionary<int, PriceSize>
{
    private readonly Dictionary<int, PriceSize> _ladder = [];

    public IEnumerable<int> Keys => _ladder.Keys;

    public IEnumerable<PriceSize> Values => _ladder.Values;

    public int Count => _ladder.Count;

    public PriceSize this[int key] => _ladder[key];

    public IEnumerator<KeyValuePair<int, PriceSize>> GetEnumerator() => _ladder.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public bool ContainsKey(int key) => _ladder.ContainsKey(key);

    public bool TryGetValue(int key, out PriceSize value) => _ladder.TryGetValue(key, out value!);

    internal void Update(int level, double price, double size) =>
        _ladder[level] = new PriceSize(price, size);
}
