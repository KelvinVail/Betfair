# Price Projection

Complete guide to configuring price projections for market book requests.

## Contents
- [Price Projection Builder](#price-projection-builder)
- [Price Data Types](#price-data-types)
- [Best Offers Configuration](#best-offers-configuration)
- [Rollup Models](#rollup-models)
- [Virtual Prices](#virtual-prices)
- [Starting Prices](#starting-prices)
- [Common Patterns](#common-patterns)
- [Performance Considerations](#performance-considerations)

## Price Projection Builder

The `PriceProjectionBuilder` provides a fluent interface for configuring price projections:

### Basic Usage

```csharp
// Simple best prices
var projection = PriceProjectionBuilder.Create()
    .WithBestPrices()
    .WithBestPricesDepth(3);

// Comprehensive data
var fullProjection = PriceProjectionBuilder.Create()
    .ComprehensivePrices()
    .WithVirtualPrices()
    .WithBestPricesDepth(5);
```

### Method Chaining

```csharp
var projection = PriceProjectionBuilder.Create()
    .WithExchangePrices()
    .WithStartingPrices()
    .WithTradedVolume()
    .WithBestPricesDepth(5)
    .WithVirtualPrices()
    .WithRolloverStakes();
```

## Price Data Types

### Exchange Prices (EX_BEST_OFFERS)

Best available back and lay prices:

```csharp
var projection = PriceProjectionBuilder.Create()
    .WithBestPrices()
    .WithBestPricesDepth(3);

// Results in:
// - AvailableToBack: Top 3 back prices
// - AvailableToLay: Top 3 lay prices
```

### All Available Prices (EX_ALL_OFFERS)

Complete price ladder:

```csharp
var projection = PriceProjectionBuilder.Create()
    .WithExchangePrices();

// Results in:
// - Complete available to back ladder
// - Complete available to lay ladder
```

### Traded Volume (EX_TRADED)

Historical traded prices and volumes:

```csharp
var projection = PriceProjectionBuilder.Create()
    .WithTradedVolume();

// Results in:
// - TradedVolume: Array of price/volume pairs
```

### Starting Prices (SP_AVAILABLE, SP_TRADED)

Starting price information:

```csharp
var projection = PriceProjectionBuilder.Create()
    .WithStartingPrices();

// Results in:
// - StartingPrices.NearPrice
// - StartingPrices.FarPrice
// - StartingPrices.BackStakeTaken
// - StartingPrices.LayLiabilityTaken
```

## Best Offers Configuration

### Depth Configuration

```csharp
// Top 3 prices (default)
var basic = PriceProjectionBuilder.Create()
    .WithBestPrices()
    .WithBestPricesDepth(3);

// Top 10 prices
var deep = PriceProjectionBuilder.Create()
    .WithBestPrices()
    .WithBestPricesDepth(10);

// Single best price
var minimal = PriceProjectionBuilder.Create()
    .WithBestPrices()
    .WithBestPricesDepth(1);
```

### Exchange Best Offers Override

```csharp
var projection = PriceProjectionBuilder.Create()
    .WithBestPrices()
    .WithBestPricesDepth(5)
    .WithRollupModel(RollupModel.Stake, 100, 1000.0, 2);

// Equivalent to:
var manualProjection = new PriceProjection
{
    PriceData = new List<string> { "EX_BEST_OFFERS" },
    ExBestOffersOverrides = new ExBestOffersOverrides
    {
        BestPricesDepth = 5,
        RollupModel = "STAKE",
        RollupLimit = 100,
        RollupLiabilityThreshold = 1000.0,
        RollupLiabilityFactor = 2
    }
};
```

## Rollup Models

### Stake Rollup

Combines orders with similar prices based on stake:

```csharp
var projection = PriceProjectionBuilder.Create()
    .WithBestPrices()
    .WithRollupModel(
        RollupModel.Stake,
        rollupLimit: 100,           // Max orders to rollup
        threshold: 1000.0,          // Liability threshold
        factor: 2);                 // Rollup factor
```

### Price Rollup

Combines orders at the same price:

```csharp
var projection = PriceProjectionBuilder.Create()
    .WithBestPrices()
    .WithRollupModel(
        RollupModel.Price,
        rollupLimit: 50,
        threshold: 500.0,
        factor: 1);
```

### No Rollup

Returns individual orders without combining:

```csharp
var projection = PriceProjectionBuilder.Create()
    .WithBestPrices()
    .WithRollupModel(RollupModel.None);
```

## Virtual Prices

Virtual prices include Betfair's algorithmic prices:

```csharp
// Include virtual prices
var withVirtual = PriceProjectionBuilder.Create()
    .WithBestPrices()
    .WithVirtualPrices();

// Exclude virtual prices (default)
var withoutVirtual = PriceProjectionBuilder.Create()
    .WithBestPrices();
```

### Virtual Price Considerations

```csharp
// Virtual prices are useful for:
// - Markets with low liquidity
// - Getting price indication when no real money available
// - Understanding Betfair's price model

// Note: Virtual prices update ~150ms after real prices
```

## Starting Prices

### Available Starting Prices

```csharp
var projection = PriceProjectionBuilder.Create()
    .WithStartingPrices();

// Access starting price data:
foreach (var runner in marketBook.Runners)
{
    var sp = runner.ExchangePrices?.StartingPrices;
    if (sp != null)
    {
        Console.WriteLine($"SP Range: {sp.NearPrice} - {sp.FarPrice}");
        Console.WriteLine($"Back Stake: £{sp.BackStakeTaken}");
        Console.WriteLine($"Lay Liability: £{sp.LayLiabilityTaken}");
    }
}
```

### Starting Price Projection

```csharp
// For markets that will have starting prices
var spProjection = PriceProjectionBuilder.Create()
    .WithStartingPrices()
    .WithBestPrices()
    .WithBestPricesDepth(1);
```

## Common Patterns

### Minimal Data (Fast)

```csharp
// For high-frequency price monitoring
var minimal = PriceProjectionBuilder.Create()
    .WithBestPrices()
    .WithBestPricesDepth(1);

var query = new MarketBookQuery()
    .WithPriceProjection(minimal);
```

### Standard Trading Data

```csharp
// For most trading applications
var standard = PriceProjectionBuilder.Create()
    .WithBestPrices()
    .WithBestPricesDepth(3)
    .WithTradedVolume();

var query = new MarketBookQuery()
    .WithPriceProjection(standard)
    .WithCurrency("GBP");
```

### Comprehensive Analysis

```csharp
// For detailed market analysis
var comprehensive = PriceProjectionBuilder.Create()
    .ComprehensivePrices()
    .WithVirtualPrices()
    .WithBestPricesDepth(10)
    .WithStartingPrices()
    .WithRolloverStakes();

var query = new MarketBookQuery()
    .WithPriceProjection(comprehensive)
    .IncludeOverallPositions();
```

### Ladder View

```csharp
// For displaying full price ladder
var ladder = PriceProjectionBuilder.Create()
    .WithExchangePrices()
    .WithTradedVolume();

var query = new MarketBookQuery()
    .WithPriceProjection(ladder);
```

### Starting Price Markets

```csharp
// For BSP markets
var bsp = PriceProjectionBuilder.Create()
    .WithStartingPrices()
    .WithBestPrices()
    .WithBestPricesDepth(3);

var query = new MarketBookQuery()
    .WithPriceProjection(bsp);
```

## Performance Considerations

### Data Size vs Speed

```csharp
// Fastest - minimal data
var fast = PriceProjectionBuilder.Create()
    .WithBestPrices()
    .WithBestPricesDepth(1);

// Moderate - standard trading data
var moderate = PriceProjectionBuilder.Create()
    .WithBestPrices()
    .WithBestPricesDepth(3)
    .WithTradedVolume();

// Slowest - comprehensive data
var comprehensive = PriceProjectionBuilder.Create()
    .ComprehensivePrices()
    .WithVirtualPrices()
    .WithBestPricesDepth(10);
```

### Batch Optimization

```csharp
// Optimize for multiple markets
async Task<Dictionary<string, MarketBook>> GetOptimizedMarketBooks(
    IEnumerable<string> marketIds)
{
    // Use minimal projection for batch requests
    var projection = PriceProjectionBuilder.Create()
        .WithBestPrices()
        .WithBestPricesDepth(3);

    var query = new MarketBookQuery()
        .WithPriceProjection(projection);

    var books = await client.MarketBook(marketIds, query);
    return books.ToDictionary(b => b.MarketId!, b => b);
}
```

### Conditional Projections

```csharp
PriceProjection CreateProjectionForMarketType(string marketType)
{
    var builder = PriceProjectionBuilder.Create();

    switch (marketType)
    {
        case "MATCH_ODDS":
            return builder
                .WithBestPrices()
                .WithBestPricesDepth(3)
                .WithTradedVolume();

        case "OVER_UNDER_25":
            return builder
                .WithBestPrices()
                .WithBestPricesDepth(5)
                .WithVirtualPrices();

        case "CORRECT_SCORE":
            return builder
                .WithExchangePrices()
                .WithStartingPrices();

        default:
            return builder
                .WithBestPrices()
                .WithBestPricesDepth(1);
    }
}
```

### Memory Optimization

```csharp
// For long-running applications, avoid keeping large projections in memory
class PriceProjectionFactory
{
    public static PriceProjection CreateMinimal() =>
        PriceProjectionBuilder.Create()
            .WithBestPrices()
            .WithBestPricesDepth(1);

    public static PriceProjection CreateStandard() =>
        PriceProjectionBuilder.Create()
            .WithBestPrices()
            .WithBestPricesDepth(3)
            .WithTradedVolume();

    public static PriceProjection CreateComprehensive() =>
        PriceProjectionBuilder.Create()
            .ComprehensivePrices()
            .WithVirtualPrices()
            .WithBestPricesDepth(5);
}
```

## Advanced Configuration

### Custom Price Projection

```csharp
// Manual configuration for specific needs
var customProjection = new PriceProjection
{
    PriceData = new List<string> 
    { 
        "EX_BEST_OFFERS", 
        "EX_TRADED",
        "SP_AVAILABLE" 
    },
    ExBestOffersOverrides = new ExBestOffersOverrides
    {
        BestPricesDepth = 5,
        RollupModel = "STAKE",
        RollupLimit = 20,
        RollupLiabilityThreshold = 1000.0,
        RollupLiabilityFactor = 2
    },
    Virtualise = false,
    RolloverStakes = true
};
```

### Dynamic Projection Selection

```csharp
PriceProjection SelectProjectionByMarketActivity(double totalMatched)
{
    if (totalMatched > 100000) // High activity
    {
        return PriceProjectionBuilder.Create()
            .WithBestPrices()
            .WithBestPricesDepth(5)
            .WithTradedVolume();
    }
    else if (totalMatched > 10000) // Medium activity
    {
        return PriceProjectionBuilder.Create()
            .WithBestPrices()
            .WithBestPricesDepth(3)
            .WithVirtualPrices();
    }
    else // Low activity
    {
        return PriceProjectionBuilder.Create()
            .WithBestPrices()
            .WithBestPricesDepth(1)
            .WithVirtualPrices();
    }
}
```
