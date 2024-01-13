using Betfair.Core;

namespace Betfair.Stream.Messages;

public sealed class StreamMarketFilter : MarketFilter<StreamMarketFilter>
{
    public HashSet<string>? BettingTypes { get; private set; }

    public bool? TurnInPlayEnabled { get; private set; }

    public HashSet<string>? Venues { get; private set; }

    public HashSet<string>? EventIds { get; private set; }

    public bool? BspMarket { get; private set; }

    public StreamMarketFilter IncludeInPlayMarketsOnly()
    {
        TurnInPlayEnabled = true;
        return this;
    }

    public StreamMarketFilter WithVenue(string venue)
    {
        Venues ??=[];
        Venues.Add(venue);
        return this;
    }

    public StreamMarketFilter WithEventId(string eventId)
    {
        EventIds ??=[];
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

    /// <summary>
    /// Restrict to markets that match the betting type of the market (i.e. Odds, Asian Handicap Singles, or Asian Handicap Doubles).
    /// </summary>
    /// <param name="bettingTypes">The betting type to include.</param>
    /// <returns>This <see cref="StreamMarketFilter"/>.</returns>
    public StreamMarketFilter IncludeBettingTypes(params string[] bettingTypes)
    {
        if (bettingTypes is null) return this;

        BettingTypes ??=[];
        foreach (var bettingType in bettingTypes.Where(x => x is not null))
            BettingTypes.Add(bettingType);

        return this;
    }
}