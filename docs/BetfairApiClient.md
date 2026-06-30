# BetfairApiClient
The BetfairApiClient is used to interact with the Betfair REST API. It provides methods for querying markets, placing orders, and managing your account.

## Contents
- [BetfairApiClient](#betfairapiclient)
  - [Contents](#contents)
  - [Create a Client](#create-a-client)
  - [Querying Markets](#querying-markets)
    - [Event Types](#event-types)
    - [Events](#events)
    - [Competitions](#competitions)
    - [Countries](#countries)
    - [Market Types](#market-types)
    - [Time Ranges](#time-ranges)
    - [Venues](#venues)
    - [Market Catalogue](#market-catalogue)
    - [Market Book](#market-book)
    - [Runner Book](#runner-book)
    - [Market Status](#market-status)
    - [Market Profit and Loss](#market-profit-and-loss)
  - [Placing and Managing Orders](#placing-and-managing-orders)
    - [Place Orders](#place-orders)
    - [Update Orders](#update-orders)
    - [Replace Orders](#replace-orders)
    - [Cancel Orders](#cancel-orders)
    - [Current Orders](#current-orders)
    - [Cleared Orders](#cleared-orders)
  - [Account Operations](#account-operations)
    - [Account Funds](#account-funds)
    - [Account Details](#account-details)
    - [Account Statement](#account-statement)
    - [Currency Rates](#currency-rates)

## Create a Client
The BetfairApiClient needs a [Credentials](/docs/Authentication.md) object to authenticate to Betfair.
Wrap it in a using block as it implements IDisposable.
```csharp
var credentials = new Credentials([USERNAME], [PASSWORD], [APPKEY]);
using var client = new BetfairApiClient(credentials);
```

## Querying Markets

All query methods accept an optional `ApiMarketFilter` to narrow results. If no filter is provided, all available data is returned.

### Event Types
Returns a list of event types (i.e. Sports) associated with the markets selected by the filter.
```csharp
var eventTypes = await client.EventTypes();

// With a filter
var filter = new ApiMarketFilter().WithCountries(Country.UnitedKingdom);
var eventTypes = await client.EventTypes(filter);
```

### Events
Returns a list of events (i.e., Reading vs. Man United) associated with the markets selected by the filter.
```csharp
var events = await client.Events();
```

### Competitions
Returns a list of competitions (i.e., World Cup 2023) associated with the markets selected by the filter. Currently only Football markets have an associated competition.
```csharp
var competitions = await client.Competitions();
```

### Countries
Returns a list of countries associated with the markets selected by the filter.
```csharp
var countries = await client.Countries();
```

### Market Types
Returns a list of market types (i.e. MATCH_ODDS, NEXT_GOAL) associated with the markets selected by the filter.
```csharp
var marketTypes = await client.MarketTypes();
```

### Time Ranges
Returns a list of time ranges in the granularity specified associated with the markets selected by the filter.
```csharp
var timeRanges = await client.TimeRanges();
```

### Venues
Returns a list of venues (i.e. Cheltenham, Ascot) associated with the markets selected by the filter.
```csharp
var venues = await client.Venues();
```

### Market Catalogue
Returns a list of information about markets that does not change (or changes very rarely). Use this to retrieve the name of the market, the names of selections and other information.

```csharp
var filter = new ApiMarketFilter()
    .WithEventTypes(EventType.HorseRacing)
    .WithCountries(Country.UnitedKingdom)
    .WithMarketTypes(MarketType.Win)
    .FromMarketStart(DateTimeOffset.UtcNow)
    .ToMarketStart(DateTimeOffset.UtcNow.AddDays(1));

var query = new MarketCatalogueQuery()
    .Include(MarketProjection.Event)
    .Include(MarketProjection.MarketStartTime)
    .Include(MarketProjection.RunnerDescription)
    .OrderBy(MarketSort.FirstToStart)
    .Take(100);

var catalogues = await client.MarketCatalogue(filter, query);
```

The `MarketCatalogueQuery` controls what data is returned:
- `Include(MarketProjection)` — adds a projection (Event, EventType, Competition, MarketStartTime, MarketDescription, RunnerDescription, RunnerMetadata)
- `OrderBy(MarketSort)` — sets the sort order (FirstToStart, LastToStart, MinimumTraded, MaximumTraded, MinimumAvailable, MaximumAvailable)
- `Take(int)` — limits the number of results (max 1000)

### Market Book
Returns dynamic data about markets including prices, status of selections, traded volume, and order status.
```csharp
var books = await client.MarketBook(new[] { "1.23456789" });

// With a query
var query = new MarketBookQuery()
    .WithPriceProjection(priceProjection)
    .IncludeAllOrders();

var books = await client.MarketBook(new[] { "1.23456789" }, query);
```

The `MarketBookQuery` controls what data is returned:
- `WithPriceProjection(PriceProjection)` — controls price data returned
- `IncludeAllOrders()` / `ExecutableOrdersOnly()` / `ExecutionCompleteOrdersOnly()` — order filtering
- `NoMatchRollup()` / `RollupByPrice()` / `RollupByAveragePrice()` — match aggregation
- `IncludeOverallPositions()` — includes overall position for each selection
- `PartitionByStrategy()` — partitions matched amounts by strategy reference
- `WithCustomerStrategies(params string[])` — filters by strategy references
- `WithCurrency(string)` — sets the currency code
- `MatchedSince(DateTime)` — filters matches from a date
- `WithBets(params string[])` — filters by bet IDs

### Runner Book
Returns dynamic data about a specific runner in a market.
```csharp
var book = await client.RunnerBook("1.23456789", selectionId: 12345);
```

### Market Status
Returns just the status of a market (Active, Suspended, Closed, Inactive).
```csharp
var status = await client.MarketStatus("1.23456789");
```

### Market Profit and Loss
Retrieve profit and loss for a given list of OPEN markets. Only odds markets are supported.
```csharp
var pnl = await client.MarketProfitAndLoss(
    new List<string> { "1.23456789" },
    includeSettledBets: false,
    netOfCommission: true);
```

## Placing and Managing Orders

### Place Orders
Place new orders into a market. This operation is atomic — all orders will be placed or none will be placed.
```csharp
var result = await client.PlaceOrders(placeOrders);
```

### Update Orders
Update non-exposure changing fields on existing orders.
```csharp
var result = await client.UpdateOrders(updateOrders);
```

### Replace Orders
Logically a bulk cancel followed by a bulk place. The cancel is completed first then the new orders are placed atomically.
```csharp
var result = await client.ReplaceOrders(replaceOrders);
```

### Cancel Orders
Cancel all bets, all bets on a market, or specific bets on a market.
```csharp
var result = await client.CancelOrders(cancelOrders);
```

### Current Orders
Returns a list of your current orders. Optionally filter and sort using an `ApiOrderFilter`.
```csharp
var orders = await client.CurrentOrders();

// With a filter
var filter = new ApiOrderFilter()
    .WithMarketIds("1.23456789")
    .ExecutableOnly()
    .MostRecentFirst()
    .Take(50);

var orders = await client.CurrentOrders(filter);
```

The `ApiOrderFilter` supports:
- `WithBetIds(params string[])` — filter by bet IDs
- `WithMarketIds(params string[])` — filter by market IDs
- `WithOrderProjection(OrderProjection)` / `ExecutableOnly()` / `ExecutionCompleteOnly()` — order status filter
- `WithCustomerOrderRefs(params string[])` — filter by customer order references
- `WithCustomerStrategyRefs(params string[])` — filter by strategy references
- `WithDateRange(DateTimeOffset, DateTimeOffset)` — date range filter
- `OrderBy(OrderBy)` — sort criteria
- `SortBy(SortDir)` / `MostRecentFirst()` / `OldestFirst()` — sort direction
- `From(int)` — starting record (for pagination)
- `Take(int)` — number of records to return (max 1000)

### Cleared Orders
Returns a list of settled bets based on a query.
```csharp
var query = new ClearedOrdersQuery(BetStatus.Settled)
    .WithMarketIds("1.23456789");

var cleared = await client.ClearedOrders(query);
```

## Account Operations

### Account Funds
Returns the available to bet amount, exposure and commission information.
```csharp
var funds = await client.AccountFunds();
```

### Account Details
Returns your account details including discount rate and Betfair point balance.
```csharp
var details = await client.AccountDetails();
```

### Account Statement
Creates a fluent builder for account statement requests.
```csharp
var statement = client.AccountStatement();
```

### Currency Rates
Returns a list of currency rates. Rates are updated once every hour.
```csharp
var rates = await client.CurrencyRates();

// From a specific currency
var rates = await client.CurrencyRates("GBP");
```

## Class
You can view the BetfairApiClient class [here](/src/Betfair/Api/BetfairApiClient.cs)
