namespace Betfair.Stream.Messages;

public class MarketFilter
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

    public MarketFilter WithMarketId(string marketId)
    {
        MarketIds ??= new HashSet<string>();
        MarketIds.Add(marketId);
        return this;
    }

    public MarketFilter WithCountryCode(string countryCode)
    {
        CountryCodes ??= new HashSet<string>();
        CountryCodes.Add(countryCode);
        return this;
    }

    public MarketFilter WithInPlayMarketsOnly()
    {
        TurnInPlayEnabled = true;
        return this;
    }

    public MarketFilter WithMarketType(string marketType)
    {
        MarketTypes ??= new HashSet<string>();
        MarketTypes.Add(marketType);
        return this;
    }

    public MarketFilter WithVenue(string venue)
    {
        Venues ??= new HashSet<string>();
        Venues.Add(venue);
        return this;
    }

    public MarketFilter WithEventTypeId(string eventTypeId)
    {
        EventTypeIds ??= new HashSet<string>();
        EventTypeIds.Add(eventTypeId);
        return this;
    }

    public MarketFilter WithEventId(string eventId)
    {
        EventIds ??= new HashSet<string>();
        EventIds.Add(eventId);
        return this;
    }

    public MarketFilter WithBspMarketsOnly()
    {
        BspMarket = true;
        return this;
    }

    public MarketFilter WithBettingType(string bettingType)
    {
        BettingTypes ??= new HashSet<string>();
        BettingTypes.Add(bettingType);
        return this;
    }
}