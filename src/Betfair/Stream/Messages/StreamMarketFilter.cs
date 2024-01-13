using Betfair.Core;

namespace Betfair.Stream.Messages;

public sealed class StreamMarketFilter : MarketFilter<StreamMarketFilter>
{
    public HashSet<string>? BettingTypes { get; private set; }

    public bool? TurnInPlayEnabled { get; private set; }

    public HashSet<string>? Venues { get; private set; }

    public HashSet<string>? EventIds { get; private set; }

    public bool? BspMarket { get; private set; }

    /// <summary>
    /// Restrict to markets that will turn in play.
    /// </summary>
    /// <returns>This <see cref="StreamMarketFilter"/>.</returns>
    public StreamMarketFilter IncludeInPlayMarketsOnly()
    {
        TurnInPlayEnabled = true;
        return this;
    }

    /// <summary>
    /// Restrict to markets that will not turn in play.
    /// </summary>
    /// <returns>This <see cref="StreamMarketFilter"/>.</returns>
    public StreamMarketFilter ExcludeInPlayMarkets()
    {
        TurnInPlayEnabled = false;
        return this;
    }

    /// <summary>
    /// Restrict markets by the venue associated with the market. Currently, only Horse Racing markets have venues.
    /// </summary>
    /// <param name="venues">List of Venue to include.</param>
    /// <returns>This <see cref="StreamMarketFilter"/>.</returns>
    public StreamMarketFilter IncludeVenues(params Venue[] venues) =>
        venues is null ? this : IncludeVenues(venues.Where(x => x is not null).Select(x => x.Id).ToArray());

    /// <inheritdoc cref="StreamMarketFilter.IncludeVenues"/>
    public StreamMarketFilter IncludeVenues(params string[] venues)
    {
        if (venues is null) return this;

        Venues ??=[];
        foreach (var venue in venues.Where(x => x is not null))
            Venues.Add(venue);

        return this;
    }

    /// <summary>
    /// Restrict markets by the event id associated with the market.
    /// </summary>
    /// <param name="eventIds">List of EventIds to include.</param>
    /// <returns>This <see cref="StreamMarketFilter"/>.</returns>
    public StreamMarketFilter IncludeEventIds(params string[] eventIds)
    {
        if (eventIds is null) return this;

        EventIds ??=[];
        foreach (var eventId in eventIds.Where(x => x is not null))
            EventIds.Add(eventId);

        return this;
    }

    /// <summary>
    /// Restrict to Bsp markets only.
    /// </summary>
    /// <returns>This <see cref="StreamMarketFilter"/>.</returns>
    public StreamMarketFilter IncludeBspMarketsOnly()
    {
        BspMarket = true;
        return this;
    }

    /// <summary>
    /// Restrict to non-bsp markets.
    /// </summary>
    /// <returns>This <see cref="StreamMarketFilter"/>.</returns>
    public StreamMarketFilter ExcludeBspMarkets()
    {
        BspMarket = false;
        return this;
    }

    /// <summary>
    /// Restrict to markets that match the betting type of the market (i.e. Odds, Asian Handicap Singles, or Asian Handicap Doubles).
    /// </summary>
    /// <param name="bettingTypes">The betting type to include.</param>
    /// <returns>This <see cref="StreamMarketFilter"/>.</returns>
    public StreamMarketFilter IncludeBettingTypes(params BettingType[] bettingTypes) =>
        bettingTypes is null ? this : IncludeBettingTypes(bettingTypes.Where(x => x is not null).Select(x => x.Id).ToArray());

    /// <inheritdoc cref="StreamMarketFilter.IncludeBettingTypes"/>
    public StreamMarketFilter IncludeBettingTypes(params string[] bettingTypes)
    {
        if (bettingTypes is null) return this;

        BettingTypes ??=[];
        foreach (var bettingType in bettingTypes.Where(x => x is not null))
            BettingTypes.Add(bettingType);

        return this;
    }
}