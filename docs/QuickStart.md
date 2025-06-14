# Quick Start Guide

Get up and running with the Betfair library in minutes.

## Installation

Install the package from NuGet:

```bash
dotnet add package Betfair
```

## Prerequisites

Before you start, you'll need:
1. A Betfair account
2. A Betfair Application Key ([Get one here](https://docs.developer.betfair.com/display/1smk3cen4v3lu3yomq5qye0ni/Application+Keys))
3. Your Betfair username and password

## Your First API Call

Let's start by getting today's horse racing markets:

```csharp
using Betfair.Api;
using Betfair.Api.Requests;
using Betfair.Core.Login;
using Betfair.Core.Enums;

// Create credentials
var credentials = new Credentials("YOUR_USERNAME", "YOUR_PASSWORD", "YOUR_APP_KEY");

// Create API client
using var client = new BetfairApiClient(credentials);

// Get today's UK horse racing
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
    .Take(10);

var markets = await client.MarketCatalogue(filter, query);

foreach (var market in markets)
{
    Console.WriteLine($"{market.Event?.Name} - {market.MarketName}");
    Console.WriteLine($"Start Time: {market.MarketStartTime}");
    Console.WriteLine();
}
```

## Your First Stream Subscription

Now let's subscribe to live market data:

```csharp
using Betfair.Stream;
using Betfair.Stream.Messages;

// Create credentials (same as above)
var credentials = new Credentials("YOUR_USERNAME", "YOUR_PASSWORD", "YOUR_APP_KEY");

// Create subscription
using var subscription = new Subscription(credentials);

// Subscribe to a specific market
var marketFilter = new StreamMarketFilter().WithMarketIds("1.123456789");
var dataFilter = new DataFilter().WithBestPrices();

await subscription.Subscribe(marketFilter, dataFilter);

// Read changes
await foreach (var change in subscription.ReadLines(default))
{
    switch (change.Operation)
    {
        case "connection":
            Console.WriteLine($"Connected: {change.ConnectionId}");
            break;
        
        case "status":
            Console.WriteLine($"Status: {change.StatusCode}");
            break;
        
        case "mcm":
            Console.WriteLine("Market change received");
            if (change.MarketChanges != null)
            {
                foreach (var marketChange in change.MarketChanges)
                {
                    Console.WriteLine($"Market {marketChange.Id}: Total Matched £{marketChange.TotalMatched:F2}");
                }
            }
            break;
    }
}
```

## Common Patterns

### Finding Markets

```csharp
// Today's football matches
var footballFilter = new ApiMarketFilter()
    .WithEventTypes(EventType.Soccer)
    .WithMarketTypes(MarketType.MatchOdds)
    .FromMarketStart(DateTimeOffset.UtcNow)
    .ToMarketStart(DateTimeOffset.UtcNow.AddDays(1));

// Next 2 hours of tennis
var tennisFilter = new ApiMarketFilter()
    .WithEventTypes(EventType.Tennis)
    .FromMarketStart(DateTimeOffset.UtcNow)
    .ToMarketStart(DateTimeOffset.UtcNow.AddHours(2));

// Specific market by ID
var specificFilter = new ApiMarketFilter()
    .WithMarketIds("1.123456789");
```

### Getting Prices

```csharp
var marketIds = new[] { "1.123456789" };
var priceQuery = new MarketBookQuery()
    .WithPriceProjection(PriceProjectionBuilder.Create()
        .WithBestPrices()
        .WithBestPricesDepth(3)
        .WithTradedVolume())
    .WithCurrency("GBP");

var marketBooks = await client.MarketBook(marketIds, priceQuery);

foreach (var book in marketBooks)
{
    Console.WriteLine($"Market: {book.MarketId}");
    foreach (var runner in book.Runners ?? [])
    {
        var bestBack = runner.ExchangePrices?.AvailableToBack?.FirstOrDefault();
        var bestLay = runner.ExchangePrices?.AvailableToLay?.FirstOrDefault();
        
        Console.WriteLine($"  Runner {runner.SelectionId}:");
        Console.WriteLine($"    Best Back: {bestBack?.Price} @ £{bestBack?.Size}");
        Console.WriteLine($"    Best Lay: {bestLay?.Price} @ £{bestLay?.Size}");
    }
}
```

### Placing a Bet

```csharp
var placeOrders = new PlaceOrders("1.123456789");
placeOrders.Instructions.Add(new PlaceInstruction
{
    SelectionId = 47972,
    Side = Side.Back,
    OrderType = OrderType.Limit,
    LimitOrder = new LimitOrder
    {
        Size = 2.0,      // £2 stake
        Price = 3.5,     // At odds of 3.5
        PersistenceType = PersistenceType.Lapse
    },
    CustomerOrderRef = "my-bet-ref-123"
});

var result = await client.PlaceOrders(placeOrders);

if (result.Status == ExecutionReportStatus.Success)
{
    Console.WriteLine("Bet placed successfully!");
    foreach (var report in result.InstructionReports ?? [])
    {
        Console.WriteLine($"Bet ID: {report.BetId}");
    }
}
else
{
    Console.WriteLine($"Failed to place bet: {result.ErrorCode}");
}
```

## Next Steps

- Read the [Authentication Guide](/docs/Authentication.md) for certificate setup
- Learn about [Market Filters](/docs/StreamMarketFilter.md) for stream subscriptions
- Explore [Order Management](/docs/OrderManagement.md) for advanced betting operations
- Check out [Examples](/docs/Examples.md) for more comprehensive scenarios

## Getting Help

- Check the [API Documentation](/docs/BetfairApiClient.md)
- Review [Change Messages](/docs/ChangeMessage.md) for stream data
- Visit the [GitHub Discussions](https://github.com/KelvinVail/Betfair/discussions) for community support
