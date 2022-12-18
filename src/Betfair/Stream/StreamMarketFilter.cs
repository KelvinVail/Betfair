#nullable enable
namespace Betfair.Stream;

public class StreamMarketFilter
{
    public HashSet<string>? CountryCodes { get; private set; }

    public HashSet<string>? BettingTypes { get; private set; }

    public bool? TurnInPlayEnabled { get; private set; }

    public HashSet<string>? MarketTypes { get; private set; }

    public HashSet<string>? Venues { get; private set; }

    public HashSet<string>? MarketIds { get; private set; }

    public HashSet<string>? EventTypeIds { get; private set; }

    public HashSet<string>? EventIds { get; private set; }

    public bool? BspMarket { get; private set; }

    public StreamMarketFilter WithMarketId(string marketId)
    {
        MarketIds ??= new HashSet<string>();
        MarketIds.Add(marketId);
        return this;
    }

    public StreamMarketFilter WithCountryCode(string countryCode)
    {
        CountryCodes ??= new HashSet<string>();
        CountryCodes.Add(countryCode);
        return this;
    }

    public StreamMarketFilter WithInPlayMarketsOnly()
    {
        TurnInPlayEnabled = true;
        return this;
    }

    public StreamMarketFilter WithMarketType(string marketType)
    {
        MarketTypes ??= new HashSet<string>();
        MarketTypes.Add(marketType);
        return this;
    }

    public StreamMarketFilter WithVenue(string venue)
    {
        Venues ??= new HashSet<string>();
        Venues.Add(venue);
        return this;
    }

    public StreamMarketFilter WithEventTypeId(string eventTypeId)
    {
        EventTypeIds ??= new HashSet<string>();
        EventTypeIds.Add(eventTypeId);
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