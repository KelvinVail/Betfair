namespace Betfair.Core;

public abstract class MarketFilter<T>
    where T : MarketFilter<T>, new()
{
    public HashSet<string>? MarketTypes { get; private set; }

    public HashSet<string>? MarketTypeCodes { get; private set; }

    public HashSet<int>? EventTypeIds { get; private set; }

    public HashSet<string>? MarketIds { get; private set; }

    public HashSet<string>? MarketCountries { get; private set; }

    public HashSet<string>? CountryCodes { get; private set; }

    public T WithMarketType(MarketType marketType) =>
        marketType is null ? This() : WithMarketType(marketType.Id);

    public T WithMarketType(string marketType)
    {
        if (marketType is null) return This();

        MarketTypes ??= new ();
        MarketTypes.Add(marketType);

        MarketTypeCodes ??= new ();
        MarketTypeCodes.Add(marketType);

        return This();
    }

    public T WithEventType(EventType eventType) =>
        eventType is null ? This() : WithEventType(eventType.Id);

    public T WithEventType(int eventType)
    {
        EventTypeIds ??= new ();
        EventTypeIds.Add(eventType);
        return This();
    }

    public T WithMarketId(string marketId)
    {
        if (marketId is null) return This();

        MarketIds ??= new ();
        MarketIds.Add(marketId);
        return This();
    }

    public T WithCountry(Country country) =>
        country is null ? This() : WithCountry(country.Id);

    public T WithCountry(string isoCode)
    {
        if (isoCode is null) return This();

        MarketCountries ??= new ();
        MarketCountries.Add(isoCode);

        CountryCodes ??= new ();
        CountryCodes.Add(isoCode);

        return This();
    }

    private T This() => (this as T) !;
}
