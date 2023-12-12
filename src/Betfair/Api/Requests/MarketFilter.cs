namespace Betfair.Api.Requests;

public class MarketFilter
{
    internal Filter Filter { get; } = new ();

    public string ToJsonString() => Serialize();

    public MarketFilter WithEventType(EventType eventType)
    {
        if (eventType is null) return this;
        Filter.EventTypeIds ??= new HashSet<int>();

        //Filter.EventTypeIds.Add(
        //    int.Parse(eventType.Id, NumberStyles.Integer, CultureInfo.InvariantCulture));
        return this;
    }

    public MarketFilter WithMarketType(MarketType marketType)
    {
        if (marketType is null) return this;
        Filter.MarketTypeCodes ??= new List<string>();

        Filter.MarketTypeCodes.Add(marketType.Id);
        return this;
    }

    public MarketFilter WithCountryCode(string countryCode)
    {
        if (countryCode is null) return this;
        Filter.MarketCountries ??= new List<string>();

        Filter.MarketCountries.Add(countryCode);
        return this;
    }

    private string Serialize() =>
        JsonSerializer.ToJsonString(
            Filter,
            StandardResolver.AllowPrivateExcludeNullCamelCase);
}
