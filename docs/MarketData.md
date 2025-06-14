# Market Data

Complete guide to retrieving and working with market data from the Betfair API.

## Contents
- [Market Discovery](#market-discovery)
- [Market Catalogue](#market-catalogue)
- [Market Book (Live Prices)](#market-book-live-prices)
- [Price Projection](#price-projection)
- [Market Status](#market-status)
- [Runner Book](#runner-book)
- [Filtering and Querying](#filtering-and-querying)
- [Best Practices](#best-practices)

## Market Discovery

### List Event Types (Sports)

```csharp
// Get all available sports
var eventTypes = await client.EventTypes();

foreach (var eventType in eventTypes)
{
    Console.WriteLine($"{eventType.EventType.Name} (ID: {eventType.EventType.Id})");
    Console.WriteLine($"Market Count: {eventType.MarketCount}");
}
```

### List Events

```csharp
// Get today's football events
var filter = new ApiMarketFilter()
    .WithEventTypes(EventType.Soccer)
    .FromMarketStart(DateTimeOffset.UtcNow)
    .ToMarketStart(DateTimeOffset.UtcNow.AddDays(1));

var events = await client.Events(filter);

foreach (var eventResult in events)
{
    Console.WriteLine($"{eventResult.Event.Name}");
    Console.WriteLine($"Open Date: {eventResult.Event.OpenDate}");
    Console.WriteLine($"Market Count: {eventResult.MarketCount}");
}
```

### List Competitions

```csharp
// Get football competitions
var filter = new ApiMarketFilter()
    .WithEventTypes(EventType.Soccer);

var competitions = await client.Competitions(filter);

foreach (var comp in competitions)
{
    Console.WriteLine($"{comp.Competition.Name}");
    Console.WriteLine($"Region: {comp.CompetitionRegion}");
    Console.WriteLine($"Market Count: {comp.MarketCount}");
}
```

### List Market Types

```csharp
// Get available market types for horse racing
var filter = new ApiMarketFilter()
    .WithEventTypes(EventType.HorseRacing);

var marketTypes = await client.MarketTypes(filter);

foreach (var marketType in marketTypes)
{
    Console.WriteLine($"{marketType.MarketType} - {marketType.MarketCount} markets");
}
```

### List Countries

```csharp
var countries = await client.Countries();

foreach (var country in countries)
{
    Console.WriteLine($"{country.CountryCode} - {country.MarketCount} markets");
}
```

### List Venues

```csharp
// Get horse racing venues
var filter = new ApiMarketFilter()
    .WithEventTypes(EventType.HorseRacing)
    .WithCountries(Country.UnitedKingdom);

var venues = await client.Venues(filter);

foreach (var venue in venues)
{
    Console.WriteLine($"{venue.Venue} - {venue.MarketCount} markets");
}
```

## Market Catalogue

### Basic Market Search

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

foreach (var market in markets)
{
    Console.WriteLine($"Market: {market.MarketName}");
    Console.WriteLine($"Event: {market.Event?.Name}");
    Console.WriteLine($"Start: {market.MarketStartTime}");
    Console.WriteLine($"Runners: {market.Runners?.Count}");
    
    if (market.Runners != null)
    {
        foreach (var runner in market.Runners)
        {
            Console.WriteLine($"  {runner.RunnerName} (ID: {runner.SelectionId})");
        }
    }
    Console.WriteLine();
}
```

### Advanced Market Search

```csharp
var filter = new ApiMarketFilter()
    .WithEventTypes(EventType.HorseRacing)
    .WithCountries(Country.UnitedKingdom, Country.Ireland)
    .WithMarketTypes(MarketType.Win)
    .WithVenues(Venue.Cheltenham, Venue.Ascot)
    .FromMarketStart(DateTimeOffset.UtcNow)
    .ToMarketStart(DateTimeOffset.UtcNow.AddHours(4))
    .WithInPlayMarketsOnly();

var query = new MarketCatalogueQuery()
    .Include(MarketProjection.Event)
    .Include(MarketProjection.MarketStartTime)
    .Include(MarketProjection.MarketDescription)
    .Include(MarketProjection.RunnerDescription)
    .Include(MarketProjection.RunnerMetadata)
    .OrderBy(MarketSort.FirstToStart)
    .Take(100);

var markets = await client.MarketCatalogue(filter, query);
```

### Using Helper Methods

```csharp
// Today's UK/Irish horse racing card
var filter = new ApiMarketFilter().TodaysCard();

var query = new MarketCatalogueQuery()
    .Include(MarketProjection.Event)
    .Include(MarketProjection.MarketStartTime)
    .Include(MarketProjection.RunnerDescription)
    .OrderBy(MarketSort.FirstToStart)
    .Take(200);

var markets = await client.MarketCatalogue(filter, query);
```

## Market Book (Live Prices)

### Basic Price Data

```csharp
var marketIds = new[] { "1.123456789" };

var query = new MarketBookQuery()
    .WithPriceProjection(PriceProjectionBuilder.Create()
        .WithBestPrices()
        .WithBestPricesDepth(3))
    .WithCurrency("GBP");

var marketBooks = await client.MarketBook(marketIds, query);

foreach (var book in marketBooks)
{
    Console.WriteLine($"Market: {book.MarketId}");
    Console.WriteLine($"Status: {book.Status}");
    Console.WriteLine($"Total Matched: £{book.TotalMatched:F2}");
    Console.WriteLine($"Last Match: {book.LastMatchTime}");
    
    foreach (var runner in book.Runners ?? [])
    {
        Console.WriteLine($"\nRunner {runner.SelectionId}:");
        Console.WriteLine($"Status: {runner.Status}");
        Console.WriteLine($"Last Price: {runner.LastPriceTraded}");
        Console.WriteLine($"Total Matched: £{runner.TotalMatched:F2}");
        
        // Best available prices
        var bestBack = runner.ExchangePrices?.AvailableToBack?.FirstOrDefault();
        var bestLay = runner.ExchangePrices?.AvailableToLay?.FirstOrDefault();
        
        Console.WriteLine($"Best Back: {bestBack?.Price} @ £{bestBack?.Size}");
        Console.WriteLine($"Best Lay: {bestLay?.Price} @ £{bestLay?.Size}");
    }
}
```

### Comprehensive Price Data

```csharp
var query = new MarketBookQuery()
    .WithPriceProjection(PriceProjectionBuilder.Create()
        .ComprehensivePrices()
        .WithVirtualPrices()
        .WithBestPricesDepth(5)
        .WithTradedVolume()
        .WithStartingPrices())
    .IncludeOverallPositions()
    .WithCurrency("GBP");

var marketBooks = await client.MarketBook(marketIds, query);

foreach (var book in marketBooks)
{
    foreach (var runner in book.Runners ?? [])
    {
        Console.WriteLine($"Runner {runner.SelectionId}:");
        
        // Available to back
        Console.WriteLine("Available to Back:");
        foreach (var price in runner.ExchangePrices?.AvailableToBack ?? [])
        {
            Console.WriteLine($"  {price.Price} @ £{price.Size}");
        }
        
        // Available to lay
        Console.WriteLine("Available to Lay:");
        foreach (var price in runner.ExchangePrices?.AvailableToLay ?? [])
        {
            Console.WriteLine($"  {price.Price} @ £{price.Size}");
        }
        
        // Traded volume
        Console.WriteLine("Traded Volume:");
        foreach (var traded in runner.ExchangePrices?.TradedVolume ?? [])
        {
            Console.WriteLine($"  {traded.Price} - £{traded.Size}");
        }
        
        // Starting prices
        if (runner.ExchangePrices?.StartingPrices != null)
        {
            var sp = runner.ExchangePrices.StartingPrices;
            Console.WriteLine($"Starting Price: {sp.NearPrice} - {sp.FarPrice}");
            Console.WriteLine($"SP Backed: £{sp.BackStakeTaken}");
            Console.WriteLine($"SP Laid: £{sp.LayLiabilityTaken}");
        }
    }
}
```

### Monitor Price Changes

```csharp
async Task MonitorPrices(string marketId, CancellationToken cancellationToken)
{
    var lastPrices = new Dictionary<long, double>();
    
    while (!cancellationToken.IsCancellationRequested)
    {
        var marketBooks = await client.MarketBook(new[] { marketId });
        var book = marketBooks.FirstOrDefault();
        
        if (book?.Runners != null)
        {
            foreach (var runner in book.Runners)
            {
                var currentPrice = runner.ExchangePrices?.AvailableToBack?.FirstOrDefault()?.Price ?? 0;
                
                if (lastPrices.TryGetValue(runner.SelectionId, out var lastPrice) && 
                    Math.Abs(currentPrice - lastPrice) > 0.01)
                {
                    Console.WriteLine($"Price change - Runner {runner.SelectionId}: {lastPrice} -> {currentPrice}");
                }
                
                lastPrices[runner.SelectionId] = currentPrice;
            }
        }
        
        await Task.Delay(2000, cancellationToken); // Check every 2 seconds
    }
}
```

## Price Projection

### Price Projection Builder

```csharp
// Best prices only
var basicProjection = PriceProjectionBuilder.Create()
    .WithBestPrices()
    .WithBestPricesDepth(3);

// Comprehensive data
var fullProjection = PriceProjectionBuilder.Create()
    .ComprehensivePrices()
    .WithVirtualPrices()
    .WithBestPricesDepth(10)
    .WithTradedVolume()
    .WithStartingPrices()
    .WithRolloverStakes();

// Custom projection
var customProjection = PriceProjectionBuilder.Create()
    .WithExchangePrices()
    .WithStartingPrices()
    .WithBestPricesDepth(5)
    .WithRollupModel(RollupModel.Stake, 100, 1000.0, 2);
```

### Exchange Best Offers Override

```csharp
var projection = new PriceProjection
{
    PriceData = new List<string> { "EX_BEST_OFFERS" },
    ExBestOffersOverrides = new ExBestOffersOverrides
    {
        BestPricesDepth = 5,
        RollupModel = "STAKE",
        RollupLimit = 100,
        RollupLiabilityThreshold = 1000.0,
        RollupLiabilityFactor = 2
    },
    Virtualise = true,
    RolloverStakes = true
};
```

## Market Status

### Check Market Status

```csharp
var status = await client.MarketStatus("1.123456789");
Console.WriteLine($"Market Status: {status}");

// Possible statuses: OPEN, SUSPENDED, CLOSED
```

### Market Status Monitoring

```csharp
async Task MonitorMarketStatus(string marketId, CancellationToken cancellationToken)
{
    var lastStatus = "";
    
    while (!cancellationToken.IsCancellationRequested)
    {
        var currentStatus = await client.MarketStatus(marketId);
        
        if (currentStatus != lastStatus)
        {
            Console.WriteLine($"Market {marketId} status changed: {lastStatus} -> {currentStatus}");
            lastStatus = currentStatus;
        }
        
        await Task.Delay(5000, cancellationToken);
    }
}
```

## Runner Book

### Get Single Runner Data

```csharp
var runnerBook = await client.RunnerBook("1.123456789", 47972);

if (runnerBook != null)
{
    var runner = runnerBook.Runners?.FirstOrDefault();
    if (runner != null)
    {
        Console.WriteLine($"Runner {runner.SelectionId}:");
        Console.WriteLine($"Status: {runner.Status}");
        Console.WriteLine($"Last Price: {runner.LastPriceTraded}");
        Console.WriteLine($"Total Matched: £{runner.TotalMatched:F2}");
        
        var bestBack = runner.ExchangePrices?.AvailableToBack?.FirstOrDefault();
        Console.WriteLine($"Best Back: {bestBack?.Price} @ £{bestBack?.Size}");
    }
}
```

## Filtering and Querying

### Complex Market Filter

```csharp
var filter = new ApiMarketFilter()
    .WithEventTypes(EventType.Soccer, EventType.Tennis)
    .WithMarketTypes(MarketType.MatchOdds, MarketType.OverUnder25Goals)
    .WithCountries(Country.UnitedKingdom, Country.Germany, Country.Spain)
    .FromMarketStart(DateTimeOffset.UtcNow)
    .ToMarketStart(DateTimeOffset.UtcNow.AddDays(7))
    .WithInPlayMarketsOnly()
    .ExcludeBspMarkets();
```

### Market Catalogue Query Options

```csharp
var query = new MarketCatalogueQuery()
    .Include(MarketProjection.Event)
    .Include(MarketProjection.Competition)
    .Include(MarketProjection.MarketStartTime)
    .Include(MarketProjection.MarketDescription)
    .Include(MarketProjection.RunnerDescription)
    .Include(MarketProjection.RunnerMetadata)
    .OrderBy(MarketSort.MaximumTraded)
    .Take(1000)
    .WithLocale("en-GB");
```

### Market Book Query Options

```csharp
var query = new MarketBookQuery()
    .WithPriceProjection(projection)
    .ExecutableOrdersOnly()
    .RollupByPrice()
    .IncludeOverallPositions()
    .PartitionByStrategy()
    .WithCustomerStrategies("strategy1", "strategy2")
    .WithCurrency("EUR")
    .WithLocale("de-DE")
    .MatchedSince(DateTime.UtcNow.AddHours(-1))
    .WithBets("bet1", "bet2");
```

## Best Practices

### 1. Efficient Market Discovery

```csharp
// Cache event types and market types
private static readonly Dictionary<string, string> _eventTypeCache = new();
private static readonly Dictionary<string, string> _marketTypeCache = new();

async Task<string> GetEventTypeName(string eventTypeId)
{
    if (!_eventTypeCache.ContainsKey(eventTypeId))
    {
        var eventTypes = await client.EventTypes();
        foreach (var et in eventTypes)
        {
            _eventTypeCache[et.EventType.Id] = et.EventType.Name;
        }
    }
    
    return _eventTypeCache.GetValueOrDefault(eventTypeId, "Unknown");
}
```

### 2. Rate Limiting

```csharp
private readonly SemaphoreSlim _marketDataSemaphore = new(5, 5); // Max 5 concurrent calls

async Task<MarketBook[]> GetMarketBookThrottled(IEnumerable<string> marketIds)
{
    await _marketDataSemaphore.WaitAsync();
    try
    {
        return await client.MarketBook(marketIds);
    }
    finally
    {
        _marketDataSemaphore.Release();
    }
}
```

### 3. Error Handling

```csharp
async Task<MarketBook[]?> GetMarketBookSafely(IEnumerable<string> marketIds)
{
    try
    {
        return await client.MarketBook(marketIds);
    }
    catch (HttpRequestException ex)
    {
        Console.WriteLine($"Failed to get market book: {ex.Message}");
        return null;
    }
}
```

### 4. Data Validation

```csharp
bool IsValidMarketData(MarketBook book)
{
    if (book.Status != "OPEN") return false;
    if (book.Runners?.Any() != true) return false;
    
    foreach (var runner in book.Runners)
    {
        if (runner.Status != "ACTIVE") continue;
        
        var bestBack = runner.ExchangePrices?.AvailableToBack?.FirstOrDefault();
        var bestLay = runner.ExchangePrices?.AvailableToLay?.FirstOrDefault();
        
        if (bestBack?.Price <= 1.0 || bestLay?.Price <= 1.0) return false;
        if (bestBack?.Size <= 0 || bestLay?.Size <= 0) return false;
    }
    
    return true;
}
```

### 5. Batch Processing

```csharp
async Task<Dictionary<string, MarketBook>> GetMultipleMarketBooks(IEnumerable<string> marketIds)
{
    const int batchSize = 40; // Betfair limit
    var results = new Dictionary<string, MarketBook>();
    
    var batches = marketIds.Chunk(batchSize);
    
    foreach (var batch in batches)
    {
        var books = await client.MarketBook(batch);
        foreach (var book in books)
        {
            if (book.MarketId != null)
            {
                results[book.MarketId] = book;
            }
        }
        
        await Task.Delay(100); // Small delay between batches
    }
    
    return results;
}
```
