using System.Collections;

namespace Betfair.Extensions.Markets;

public sealed class PriceLadderDictionary : IReadOnlyDictionary<double, double>
{
    private readonly Dictionary<double, double> _ladder = [];

    public IEnumerable<double> Keys => _ladder.Keys;

    public IEnumerable<double> Values => _ladder.Values;

    public int Count => _ladder.Count;

    public double this[double key] => _ladder[key];

    public IEnumerator<KeyValuePair<double, double>> GetEnumerator() => _ladder.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public bool ContainsKey(double key) => _ladder.ContainsKey(key);

    public bool TryGetValue(double key, out double value) => _ladder.TryGetValue(key, out value!);

    internal void Update(double price, double size) =>
        _ladder[price] = size;
}
