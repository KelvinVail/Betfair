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

    /// <summary>
    /// Restrict to markets that match the type of the market.
    /// </summary>
    /// <param name="marketTypes">Market Types to include.</param>
    /// <returns>This <typeparamref name="T"/>.</returns>
    public T IncludeMarketTypes(params MarketType[] marketTypes) =>
        marketTypes is null ? This() : IncludeMarketTypes(marketTypes.Where(x => x is not null).Select(x => x.Id).ToArray());

    /// <inheritdoc cref="MarketFilter{T}.IncludeMarketTypes"/>
    public T IncludeMarketTypes(params string[] marketTypes)
    {
        if (marketTypes is null) return This();

        MarketTypes ??=[];
        MarketTypeCodes ??=[];
        foreach (var marketType in marketTypes.Where(x => x is not null))
        {
            MarketTypes.Add(marketType);
            MarketTypeCodes.Add(marketType);
        }

        return This();
    }

    /// <summary>
    /// Restrict markets by event type associated with the market.
    /// </summary>
    /// <param name="eventTypes">Event Types to include.</param>
    /// <returns>This <typeparamref name="T"/>.</returns>
    public T IncludeEventTypes(params EventType[] eventTypes) =>
        eventTypes is null ? This() : IncludeEventTypes(eventTypes.Where(x => x is not null).Select(x => x.Id).ToArray());

    /// <inheritdoc cref="MarketFilter{T}.IncludeEventTypes"/>
    public T IncludeEventTypes(params int[] eventTypes)
    {
        if (eventTypes is null) return This();

        EventTypeIds ??=[];
        foreach (var eventType in eventTypes)
            EventTypeIds.Add(eventType);

        return This();
    }

    /// <summary>
    /// Restrict markets by market id associated with the market.
    /// </summary>
    /// <param name="marketIds">MarketIds to include.</param>
    /// <returns>This <typeparamref name="T"/>.</returns>
    public T IncludeMarketIds(params string[] marketIds)
    {
        if (marketIds is null) return This();

        MarketIds ??=[];
        foreach (var marketId in marketIds.Where(x => x is not null))
            MarketIds.Add(marketId);

        return This();
    }

    /// <summary>
    /// Restrict to markets that are in the specified country or countries.
    /// </summary>
    /// <param name="countries">Countries to include.</param>
    /// <returns>This <typeparamref name="T"/>.</returns>
    public T IncludeCountries(params Country[] countries) =>
        countries is null ? This() : IncludeCountries(countries.Where(x => x is not null).Select(x => x.Id).ToArray());

    /// <inheritdoc cref="MarketFilter{T}.IncludeCountries"/>
    public T IncludeCountries(params string[] isoCodes)
    {
        if (isoCodes is null) return This();

        MarketCountries ??=[];
        CountryCodes ??=[];
        foreach (var isoCode in isoCodes.Where(x => x is not null))
        {
            MarketCountries.Add(isoCode);
            CountryCodes.Add(isoCode);
        }

        return This();
    }

    private T This() => (this as T) !;
}
