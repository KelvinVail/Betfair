# StreamMarketFilter
Use the StreamMarketFilter to define which markets to subscribe to in a Betfair market stream.

- [StreamMarketFilter](#streammarketfilter)
  - [Chaining market filters](#chaining-market-filters)
  - [Available Market Filters](#available-market-filters)
    - [MarketId](#marketid)
    - [Event Type (Horse Racing, Soccer...)](#event-type-horse-racing-soccer)
    - [Market Type (Win, Place, Match Odds...)](#market-type-win-place-match-odds)
    - [Country](#country)
    - [Venue (Lingfield, Wolverhampton...)](#venue-lingfield-wolverhampton)
    - [Events (Lingfield 14th Jan, Kelso 14th Jan...)](#events-lingfield-14th-jan-kelso-14th-jan)
    - [Turn In Play](#turn-in-play)
    - [Betfair Starting Price Markets (BSP)](#betfair-starting-price-markets-bsp)
    - [Betting Types (Asian Handicap Singles, Asian Handicap Doubles...)](#betting-types-asian-handicap-singles-asian-handicap-doubles)

## Chaining market filters
The StreamMarketFilter class can be used to build up a list of criteria to filter the markets you subscribe
to in the Betfair stream. Only markets that meet all your filtered requirements will be subscribed to.

The following example will subscribe to all UK horse racing win markets in the Betfair stream.
```csharp
var marketFilter = new StreamMarketFilter()
    .WithCountries(Country.UnitedKingdom)
    .WithEventTypes(EventType.HorseRacing)
    .WithMarketTypes(MarketType.Win);
```

## Available Market Filters
### MarketId
Restricts the subscription to a defined set of MarketIds.
```csharp
var marketFilter = new StreamMarketFilter().WithMarketIds("1.2345", "1.9876");
```

### Event Type (Horse Racing, Soccer...)
Restrict markets by event type associated with the market. (i.e., HorseRacing, Soccer, Tennis).
```csharp
var marketFilter = new StreamMarketFilter().WithEventTypes(EventType.HorseRacing, EventType.Soccer);
```

A set of popular event types have been provided for convenience. However, this is not a complete list.
If you want to subscribe to an event type that is not available in the helper class you can pass the event type id value defined by Betfair into the filter. (i.e., "1" for Soccer, "7" for Horse Racing, etc)
<!-- For a full list of event types available see the ```ApiClient.GetEventTypes()``` method. -->
```csharp
var marketFilter = new StreamMarketFilter().WithEventTypes(7, 1);
```

### Market Type (Win, Place, Match Odds...)
Restrict the subscription to markets that match the type of the market (i.e., Win, MatchOdds, HalfTimeScore).
```csharp
var marketFilter = new StreamMarketFilter().WithMarketTypes(MarketType.Win, MarketType.Place);
```

A set of popular market types have been provided for convenience. However, this is not a complete list.
If you want to subscribe to a market type that is not available in the helper class you can pass the string value defined by Betfair into the filter.
<!-- For a full list of market types available see the ```ApiClient.GetMarketTypes()``` method. -->
```csharp
var marketFilter = new StreamMarketFilter().WithMarketTypes("ROOKIE_OF_THE_YEAR");
```

### Country
Restrict to markets that are in the specified country or countries.
```csharp
var marketFilter = new StreamMarketFilter().WithCountries(Country.UnitedKingdom, Country.Ireland);
```

The Country helper class should contain a complete list of countries. However, you can also pass the
2 digit country iso code into the filter. If you enter an invalid iso code the default iso code of GB will be used.
```csharp
var marketFilter = new StreamMarketFilter().WithCountries("GB", "IE");
```

### Venue (Lingfield, Wolverhampton...)
Restrict markets by the venue associated with the market. Currently, only Horse Racing markets have venues.
```csharp
var marketFilter = new StreamMarketFilter().WithVenues(Venue.Kelso, Venue.Wolverhampton);
```

A set of popular venues have been provided for convenience. However, this is not a complete list.
If you want to subscribe to a venue that is not available in the helper class you can pass the string value defined by Betfair into the filter.
<!-- For a full list of venues available see the ```ApiClient.GetVenues()``` method. -->
```csharp
var marketFilter = new StreamMarketFilter().WithVenues("Santa Anita Park", "Golden Gate Fields");
```

### Events (Lingfield 14th Jan, Kelso 14th Jan...)
Restrict markets by the event ID associated with the market.
<!-- For a full list of events available see the ```ApiClient.GetEvents()``` method. -->
```csharp
var marketFilter = new StreamMarketFilter().WithEventIds("32935031", "32935029");
```

### Turn In Play
To restrict the subscription to only markets that will turn in play:
```csharp
var marketFilter = new StreamMarketFilter().WithInPlayMarketsOnly();
```
To exclude markets that will turn in play:
```csharp
var marketFilter = new StreamMarketFilter().ExcludeInPlayMarkets();
```
If you do not define either of these option you will subscribe to both in play and non in play markets.

### Betfair Starting Price Markets (BSP)
To restrict the subscription to BSP markets only:
```csharp
var marketFilter = new StreamMarketFilter().WithBspMarketsOnly();
```
To exclude all BSP markets:
```csharp
var marketFilter = new StreamMarketFilter().ExcludeBspMarkets();
```
If you do not define either of these option you will subscribe to both bsp and non-bsp markets.

### Betting Types (Asian Handicap Singles, Asian Handicap Doubles...)
Restrict to markets that match the betting type of the market (i.e. Odds, Asian Handicap Singles, or Asian Handicap Doubles).
```csharp
var marketFilter = new StreamMarketFilter().WithBettingTypes(
    BettingType.AsianHandicapSingles,
    BettingType.AsianHandicapDoubles);
```