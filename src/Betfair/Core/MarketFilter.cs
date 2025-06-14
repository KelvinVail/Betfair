namespace Betfair.Core;

public abstract class MarketFilter<T>
    where T : MarketFilter<T>, new()
{
    /// <summary>
    /// Restrict markets by any text associated with the market such as the Name, Event, Competition, etc.
    /// You can include a wildcard (*) character as long as it is not the first character.
    /// </summary>
    [JsonPropertyName("textQuery")]
    public string? TextQuery { get; private set; }

    /// <summary>
    /// Restrict markets by the Exchange where the market operates.
    /// Note: This is not currently in use and only entities for the current exchange will be returned.
    /// </summary>
    [JsonPropertyName("exchangeIds")]
    public HashSet<string>? ExchangeIds { get; private set; }

    /// <summary>
    /// Restrict markets by event type associated with the market. (i.e., Football, Hockey, etc)
    /// </summary>
    [JsonPropertyName("eventTypeIds")]
    public HashSet<string>? EventTypeIds { get; private set; }

    /// <summary>
    /// Restrict markets by the event id associated with the market.
    /// </summary>
    [JsonPropertyName("eventIds")]
    public HashSet<string>? EventIds { get; private set; }

    /// <summary>
    /// Restrict markets by the competitions associated with the market.
    /// </summary>
    [JsonPropertyName("competitionIds")]
    public HashSet<string>? CompetitionIds { get; private set; }

    /// <summary>
    /// Restrict markets by the market id associated with the market.
    /// </summary>
    [JsonPropertyName("marketIds")]
    public HashSet<string>? MarketIds { get; private set; }

    /// <summary>
    /// Restrict markets by the venue associated with the market. Currently only Horse Racing markets have venues.
    /// </summary>
    [JsonPropertyName("venues")]
    public HashSet<string>? Venues { get; private set; }

    /// <summary>
    /// Restrict to bsp markets only, if True or non-bsp markets if False. If not specified then returns both BSP and non-BSP markets.
    /// </summary>
    [JsonPropertyName("bspOnly")]
    public bool? BspOnly { get; private set; }

    /// <summary>
    /// Restrict to markets that will turn in play if True or will not turn in play if false. If not specified, returns both.
    /// </summary>
    [JsonPropertyName("turnInPlayEnabled")]
    public bool? TurnInPlayEnabled { get; private set; }

    /// <summary>
    /// Restrict to markets that are currently in play if True or not in play if false. If not specified, returns both.
    /// </summary>
    [JsonPropertyName("inPlayOnly")]
    public bool? InPlayOnly { get; private set; }

    /// <summary>
    /// Restrict to markets that match the betting type of the market (i.e. Odds, Asian Handicap Singles, Asian Handicap Doubles or Line).
    /// </summary>
    [JsonPropertyName("marketBettingTypes")]
    public HashSet<string>? MarketBettingTypes { get; private set; }

    /// <summary>
    /// Restrict to markets that are in the specified country or countries.
    /// </summary>
    [JsonPropertyName("marketCountries")]
    public HashSet<string>? MarketCountries { get; private set; }

    /// <summary>
    /// Restrict to markets that match the type of the market (i.e., MATCH_ODDS, HALF_TIME_SCORE).
    /// You should use this instead of relying on the market name as the market type codes are the same in all locales.
    /// </summary>
    [JsonPropertyName("marketTypeCodes")]
    public HashSet<string>? MarketTypeCodes { get; private set; }

    /// <summary>
    /// Restrict to markets that I have one or more orders in these status.
    /// </summary>
    [JsonPropertyName("withOrders")]
    public HashSet<string>? WithOrdersFilter { get; private set; }

    /// <summary>
    /// Restrict by race type.
    /// </summary>
    [JsonPropertyName("raceTypes")]
    public HashSet<string>? RaceTypes { get; private set; }

    // Legacy properties for backward compatibility
    [JsonPropertyName("marketTypes")]
    public HashSet<string>? MarketTypes { get; private set; }

    [JsonPropertyName("countryCodes")]
    public HashSet<string>? CountryCodes { get; private set; }

    /// <summary>
    /// Restrict markets by any text associated with the market such as the Name, Event, Competition, etc.
    /// </summary>
    /// <param name="textQuery">The text query to search for.</param>
    /// <returns>This <typeparamref name="T"/>.</returns>
    public T WithTextQuery(string textQuery)
    {
        TextQuery = textQuery;
        return This();
    }

    /// <summary>
    /// Restrict markets by the Exchange where the market operates.
    /// </summary>
    /// <param name="exchangeIds">Exchange IDs to include.</param>
    /// <returns>This <typeparamref name="T"/>.</returns>
    public T WithExchangeIds(params string[] exchangeIds)
    {
        if (exchangeIds is null) return This();

        ExchangeIds ??= [];
        foreach (var exchangeId in exchangeIds.Where(x => x is not null))
            ExchangeIds.Add(exchangeId);

        return This();
    }

    /// <summary>
    /// Restrict markets by event type associated with the market.
    /// </summary>
    /// <param name="eventTypes">Event Types to include.</param>
    /// <returns>This <typeparamref name="T"/>.</returns>
    public T WithEventTypes(params EventType[] eventTypes) =>
        eventTypes is null ? This() : WithEventTypes(eventTypes.Where(x => x is not null).Select(x => x.Id.ToString()).ToArray());

    /// <inheritdoc cref="MarketFilter{T}.WithEventTypes(EventType[])"/>
    public T WithEventTypes(params string[] eventTypes)
    {
        if (eventTypes is null) return This();

        EventTypeIds ??= [];
        foreach (var eventType in eventTypes.Where(x => x is not null))
            EventTypeIds.Add(eventType);

        return This();
    }

    /// <inheritdoc cref="MarketFilter{T}.WithEventTypes(EventType[])"/>
    public T WithEventTypes(params int[] eventTypes)
    {
        if (eventTypes is null) return This();

        EventTypeIds ??= [];
        foreach (var eventType in eventTypes)
            EventTypeIds.Add(eventType.ToString());

        return This();
    }

    /// <summary>
    /// Restrict markets by the event id associated with the market.
    /// </summary>
    /// <param name="eventIds">Event IDs to include.</param>
    /// <returns>This <typeparamref name="T"/>.</returns>
    public T WithEventIds(params string[] eventIds)
    {
        if (eventIds is null) return This();

        EventIds ??= [];
        foreach (var eventId in eventIds.Where(x => x is not null))
            EventIds.Add(eventId);

        return This();
    }

    /// <summary>
    /// Restrict markets by the competitions associated with the market.
    /// </summary>
    /// <param name="competitionIds">Competition IDs to include.</param>
    /// <returns>This <typeparamref name="T"/>.</returns>
    public T WithCompetitionIds(params string[] competitionIds)
    {
        if (competitionIds is null) return This();

        CompetitionIds ??= [];
        foreach (var competitionId in competitionIds.Where(x => x is not null))
            CompetitionIds.Add(competitionId);

        return This();
    }

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

        MarketTypes ??= [];
        MarketTypeCodes ??= [];
        foreach (var marketType in marketTypes.Where(x => x is not null))
        {
            MarketTypes.Add(marketType);
            MarketTypeCodes.Add(marketType);
        }

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

        MarketIds ??= [];
        foreach (var marketId in marketIds.Where(x => x is not null))
            MarketIds.Add(marketId);

        return This();
    }

    /// <summary>
    /// Restrict markets by the venue associated with the market.
    /// </summary>
    /// <param name="venues">Venues to include.</param>
    /// <returns>This <typeparamref name="T"/>.</returns>
    public T WithVenues(params string[] venues)
    {
        if (venues is null) return This();

        Venues ??= [];
        foreach (var venue in venues.Where(x => x is not null))
            Venues.Add(venue);

        return This();
    }

    /// <summary>
    /// Restrict to BSP markets only.
    /// </summary>
    /// <param name="bspOnly">True for BSP markets only, false for non-BSP markets only, null for both.</param>
    /// <returns>This <typeparamref name="T"/>.</returns>
    public T WithBspOnly(bool? bspOnly)
    {
        BspOnly = bspOnly;
        return This();
    }

    /// <summary>
    /// Restrict to markets that will turn in play.
    /// </summary>
    /// <param name="turnInPlayEnabled">True for markets that turn in play, false for markets that don't, null for both.</param>
    /// <returns>This <typeparamref name="T"/>.</returns>
    public T WithTurnInPlayEnabled(bool? turnInPlayEnabled)
    {
        TurnInPlayEnabled = turnInPlayEnabled;
        return This();
    }

    /// <summary>
    /// Restrict to markets that are currently in play.
    /// </summary>
    /// <param name="inPlayOnly">True for in-play markets only, false for pre-play markets only, null for both.</param>
    /// <returns>This <typeparamref name="T"/>.</returns>
    public T WithInPlayOnly(bool? inPlayOnly)
    {
        InPlayOnly = inPlayOnly;
        return This();
    }

    /// <summary>
    /// Restrict to markets that match the betting type.
    /// </summary>
    /// <param name="marketBettingTypes">Market betting types to include.</param>
    /// <returns>This <typeparamref name="T"/>.</returns>
    public T WithMarketBettingTypes(params string[] marketBettingTypes)
    {
        if (marketBettingTypes is null) return This();

        MarketBettingTypes ??= [];
        foreach (var bettingType in marketBettingTypes.Where(x => x is not null))
            MarketBettingTypes.Add(bettingType);

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

        MarketCountries ??= [];
        CountryCodes ??= [];
        foreach (var isoCode in isoCodes.Where(x => x is not null))
        {
            MarketCountries.Add(isoCode);
            CountryCodes.Add(isoCode);
        }

        return This();
    }

    /// <summary>
    /// Restrict to markets that have orders in the specified status.
    /// </summary>
    /// <param name="orderStatuses">Order statuses to include.</param>
    /// <returns>This <typeparamref name="T"/>.</returns>
    public T WithOrders(params string[] orderStatuses)
    {
        if (orderStatuses is null) return This();

        WithOrdersFilter ??= [];
        foreach (var status in orderStatuses.Where(x => x is not null))
            WithOrdersFilter.Add(status);

        return This();
    }

    /// <summary>
    /// Restrict by race type.
    /// </summary>
    /// <param name="raceTypes">Race types to include.</param>
    /// <returns>This <typeparamref name="T"/>.</returns>
    public T WithRaceTypes(params string[] raceTypes)
    {
        if (raceTypes is null) return This();

        RaceTypes ??= [];
        foreach (var raceType in raceTypes.Where(x => x is not null))
            RaceTypes.Add(raceType);

        return This();
    }

    private T This() => (this as T)!;
}
