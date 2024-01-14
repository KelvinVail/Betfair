# StreamMarketFilter
Use the StreamMarketFilter to define which markets to subscribe to in a Betfair market stream.

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

### Event Type
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

### Market Type
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