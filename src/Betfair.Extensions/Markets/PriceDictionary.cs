using System.Collections;
using System.Diagnostics.CodeAnalysis;

#pragma warning disable CA1043

namespace Betfair.Extensions.Markets;

public class PriceDictionary : IReadOnlyDictionary<Price, double>
{
    private readonly Dictionary<int, double> _prices = PriceExtensions.ValidPrices.ToDictionary(p => (int)Math.Round(p * 100), _ => 0.0);

    public int Count => _prices.Count;

    public IEnumerable<Price> Keys => _prices.Keys.Select(k => Price.Of(k / 100.0));

    public IEnumerable<double> Values => _prices.Values;

    public double this[[NotNull]Price key] => _prices[key.Index];

    public bool ContainsKey([NotNull]Price key) => _prices.ContainsKey(key.Index);

    public bool TryGetValue([NotNull]Price key, out double value) => _prices.TryGetValue(key.Index, out value);

    public IEnumerator<KeyValuePair<Price, double>> GetEnumerator()
        {
            foreach (var kvp in _prices)
                yield return new KeyValuePair<Price, double>(Price.Of(kvp.Key / 100.0), kvp.Value);
        }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    internal void Update(Price key, double value) => _prices[key.Index] = value;

    // TODO: Calculate Weighted Price
}
