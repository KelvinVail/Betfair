namespace Betfair.Core;

public abstract class MarketFilter<T>
    where T : MarketFilter<T>, new()
{
    [JsonPropertyName("marketTypes")]
    public HashSet<string>? MarketTypes { get; private set; }

    [JsonPropertyName("marketTypeCodes")]
    public HashSet<string>? MarketTypeCodes { get; private set; }

    [JsonPropertyName("eventTypeIds")]
    public HashSet<int>? EventTypeIds { get; private set; }

    [JsonPropertyName("marketIds")]
    public HashSet<string>? MarketIds { get; private set; }

    [JsonPropertyName("marketCountries")]
    public HashSet<string>? MarketCountries { get; private set; }

    [JsonPropertyName("countryCodes")]
    public HashSet<string>? CountryCodes { get; private set; }

    /// <summary>
    /// Restrict to markets that match the type of the market.
    /// </summary>
    /// <param name="marketTypes">Market Types to include.</param>
    /// <returns>This <typeparamref name="T"/>.</returns>
    public T WithMarketTypes(params MarketType[] marketTypes) =>
        marketTypes is null ? This() : WithMarketTypes(marketTypes.Where(x => x is not null).Select(x => x.Id).ToArray());

    /// <inheritdoc cref="MarketFilter{T}.WithMarketTypes(MarketType[])"/>
    public T WithMarketTypes(params string[] marketTypes)
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
    public T WithEventTypes(params EventType[] eventTypes) =>
        eventTypes is null ? This() : WithEventTypes(eventTypes.Where(x => x is not null).Select(x => x.Id).ToArray());

    /// <inheritdoc cref="MarketFilter{T}.WithEventTypes"/>
    public T WithEventTypes(params int[] eventTypes)
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
    public T WithMarketIds(params string[] marketIds)
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
    public T WithCountries(params Country[] countries) =>
        countries is null ? This() : WithCountries(countries.Where(x => x is not null).Select(x => x.Id).ToArray());

    /// <inheritdoc cref="MarketFilter{T}.WithCountries"/>
    public T WithCountries(params string[] isoCodes)
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
