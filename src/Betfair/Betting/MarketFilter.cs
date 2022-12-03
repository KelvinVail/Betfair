using System.Globalization;
using Utf8Json;
using Utf8Json.Resolvers;

namespace Betfair.Betting;

public class MarketFilter
{
    private readonly Filter _filter = new ();

    public string ToJsonString() =>
        $"\"filter\":{Serialize()}";

    public MarketFilter WithEventType(EventType eventType)
    {
        if (eventType is null) return this;
        _filter.EventTypeIds ??= new List<int>();

        _filter.EventTypeIds.Add(
            int.Parse(eventType.Id, NumberStyles.Integer, CultureInfo.InvariantCulture));
        return this;
    }

    public MarketFilter WithMarketType(MarketType marketType)
    {
        if (marketType is null) return this;
        _filter.MarketTypeCodes ??= new List<string>();

        _filter.MarketTypeCodes.Add(marketType.Id);
        return this;
    }

    public MarketFilter WithCountryCode(string countryCode)
    {
        if (countryCode is null) return this;
        _filter.MarketCountries ??= new List<string>();

        _filter.MarketCountries.Add(countryCode);
        return this;
    }

    private string Serialize() =>
        JsonSerializer.ToJsonString(
            _filter,
            StandardResolver.AllowPrivateExcludeNullCamelCase);

    // ReSharper disable CollectionNeverQueried.Local
    private sealed class Filter
    {
        public List<int>? EventTypeIds { get; internal set;  }

        public List<string>? MarketTypeCodes { get; internal set; }

        public List<string>? MarketCountries { get; internal set; }
    }
}
