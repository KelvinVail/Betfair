# Betfair API Client

The `BetfairApiClient` provides access to all Betfair API endpoints for market data, order management, and account operations.

## Contents
- [Creating a Client](#creating-a-client)
- [Market Data Operations](#market-data-operations)
- [Order Management](#order-management)
- [Account Operations](#account-operations)
- [Error Handling](#error-handling)
- [Disposal](#disposal)

## Creating a Client

Create a client using your Betfair credentials:

```csharp
var credentials = new Credentials("USERNAME", "PASSWORD", "APP_KEY");
using var client = new BetfairApiClient(credentials);
```

For certificate-based authentication (recommended for bots):

```csharp
var cert = X509Certificate2.CreateFromPemFile("cert.pem", "key.pem");
var credentials = new Credentials("USERNAME", "PASSWORD", "APP_KEY", cert);
using var client = new BetfairApiClient(credentials);
```

## Market Data Operations

### List Event Types
Get available sports/event types:

```csharp
var eventTypes = await client.EventTypes();
foreach (var eventType in eventTypes)
{
    Console.WriteLine($"{eventType.EventType.Name} (ID: {eventType.EventType.Id})");
}
```

### List Events
Get events for specific sports:

```csharp
var filter = new ApiMarketFilter()
    .WithEventTypes(EventType.HorseRacing)
    .WithCountries(Country.UnitedKingdom);

var events = await client.Events(filter);
```

### List Market Catalogue
Get market information:

```csharp
var filter = new ApiMarketFilter()
    .WithEventTypes(EventType.Soccer)
    .WithMarketTypes(MarketType.MatchOdds)
    .FromMarketStart(DateTimeOffset.UtcNow)
    .ToMarketStart(DateTimeOffset.UtcNow.AddDays(1));

var query = new MarketCatalogueQuery()
    .Include(MarketProjection.Event)
    .Include(MarketProjection.MarketStartTime)
    .Include(MarketProjection.RunnerDescription)
    .OrderBy(MarketSort.FirstToStart)
    .Take(50);

var markets = await client.MarketCatalogue(filter, query);
```

### Get Market Book
Get live market data including prices:

```csharp
var marketIds = new[] { "1.123456789" };
var query = new MarketBookQuery()
    .WithPriceProjection(PriceProjectionBuilder.Create()
        .WithBestPrices()
        .WithBestPricesDepth(3))
    .IncludeOverallPositions()
    .WithCurrency("GBP");

var marketBooks = await client.MarketBook(marketIds, query);
```

## Order Management

### Place Orders
Place new orders on a market:

```csharp
var placeOrders = new PlaceOrders("1.123456789");
placeOrders.Instructions.Add(new PlaceInstruction
{
    SelectionId = 47972,
    Side = Side.Back,
    OrderType = OrderType.Limit,
    LimitOrder = new LimitOrder
    {
        Size = 10.0,
        Price = 2.5,
        PersistenceType = PersistenceType.Lapse
    }
});

var result = await client.PlaceOrders(placeOrders);
```

### Cancel Orders
Cancel existing orders:

```csharp
// Cancel all orders on a market
var cancelOrders = new CancelOrders { MarketId = "1.123456789" };
var result = await client.CancelOrders(cancelOrders);

// Cancel specific orders
var cancelSpecific = new CancelOrders { MarketId = "1.123456789" };
cancelSpecific.Instructions.Add(new CancelInstruction
{
    BetId = "123456789",
    SizeReduction = 5.0 // Partially cancel
});
```

### List Current Orders
Get your current unmatched orders:

```csharp
var filter = new ApiOrderFilter()
    .WithMarketIds("1.123456789")
    .ExecutableOnly()
    .Take(100);

var orders = await client.CurrentOrders(filter);
```

### List Cleared Orders
Get your betting history:

```csharp
var query = new ClearedOrdersQuery()
    .LastWeek()
    .SettledOnly()
    .WithMarkets("1.123456789")
    .Take(500);

var clearedOrders = await client.ClearedOrders(query);
```

## Account Operations

### Get Account Funds
Check your available balance:

```csharp
var funds = await client.AccountFunds();
Console.WriteLine($"Available: £{funds.AvailableToBetBalance:F2}");
Console.WriteLine($"Exposure: £{funds.Exposure:F2}");
```

### Get Account Details
Get your account information:

```csharp
var details = await client.AccountDetails();
Console.WriteLine($"Name: {details.FirstName} {details.LastName}");
Console.WriteLine($"Currency: {details.CurrencyCode}");
```

### Get Account Statement
Get your account transaction history:

```csharp
var query = new AccountStatementQuery()
    .LastMonth()
    .ExchangeOnly()
    .UkWallet()
    .Take(100);

var statement = await client.AccountStatement(query);
```

## Error Handling

Always wrap API calls in try-catch blocks:

```csharp
try
{
    var markets = await client.MarketCatalogue(filter, query);
}
catch (HttpRequestException ex)
{
    Console.WriteLine($"API Error: {ex.Message}");
}
catch (TaskCanceledException ex)
{
    Console.WriteLine($"Request timed out: {ex.Message}");
}
```

## Disposal

Always dispose of the client when finished:

```csharp
using var client = new BetfairApiClient(credentials);
// Use client...
// Automatically disposed at end of using block
```

## Class Reference

You can view the BetfairApiClient class [here](/src/Betfair/Api/BetfairApiClient.cs)
