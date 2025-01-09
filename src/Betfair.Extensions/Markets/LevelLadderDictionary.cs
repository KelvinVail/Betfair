using System.Collections;

namespace Betfair.Extensions.Markets;

public sealed class LevelLadderDictionary : IReadOnlyDictionary<int, (double Price, double Size)>
{
    private readonly Dictionary<int, (double Price, double Size)> _ladder = [];

    public IEnumerable<int> Keys => _ladder.Keys;

    public IEnumerable<(double Price, double Size)> Values => _ladder.Values;

    public int Count => _ladder.Count;

    public (double Price, double Size) this[int key] => _ladder[key];

    public IEnumerator<KeyValuePair<int, (double Price, double Size)>> GetEnumerator() => _ladder.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public bool ContainsKey(int key) => _ladder.ContainsKey(key);

    public bool TryGetValue(int key, out (double Price, double Size) value) => _ladder.TryGetValue(key, out value!);

    internal void Update(int level, double price, double size) =>
        _ladder[level] = (price, size);
}
