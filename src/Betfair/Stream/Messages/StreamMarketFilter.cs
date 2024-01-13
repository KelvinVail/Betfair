using Betfair.Core;

namespace Betfair.Stream.Messages;

public class StreamMarketFilter : MarketFilter<StreamMarketFilter>
{
    public HashSet<string>? BettingTypes { get; private set; }

    public bool? TurnInPlayEnabled { get; private set; }

    public HashSet<string>? Venues { get; private set; }

    public HashSet<string>? EventIds { get; private set; }

    public bool? BspMarket { get; private set; }

    public StreamMarketFilter WithInPlayMarketsOnly()
    {
        TurnInPlayEnabled = true;
        return this;
    }

    public StreamMarketFilter WithVenue(string venue)
    {
        Venues ??= new HashSet<string>();
        Venues.Add(venue);
        return this;
    }

    public StreamMarketFilter WithEventId(string eventId)
    {
        EventIds ??= new HashSet<string>();
        EventIds.Add(eventId);
        return this;
    }

    public StreamMarketFilter WithBspMarketsOnly()
    {
        BspMarket = true;
        return this;
    }

    public StreamMarketFilter WithBettingType(string bettingType)
    {
        BettingTypes ??= new HashSet<string>();
        BettingTypes.Add(bettingType);
        return this;
    }
}