# Market Cache
The Market Cache is a zero-allocation, ultra-low-latency system for maintaining live market state directly from stream bytes. It processes raw Betfair stream data without intermediate JSON deserialization objects, making it ideal for latency-sensitive trading applications.

## Contents
- [Market Cache](#market-cache)
  - [Contents](#contents)
  - [Quick Start](#quick-start)
  - [How It Works](#how-it-works)
  - [Accessing Market Data](#accessing-market-data)
    - [MarketCacheProcessor](#marketcacheprocessor)
    - [MarketCache](#marketcache-1)
    - [RunnerCache](#runnercache)
    - [Market Definition](#market-definition)
  - [Price Ladders](#price-ladders)
    - [PriceLadder](#priceladder)
    - [PositionLadder](#positionladder)
  - [Resume Tokens](#resume-tokens)
  - [Callback on Update](#callback-on-update)

## Quick Start
```csharp
var credentials = new Credentials([USERNAME], [PASSWORD], [APPKEY]);
using var subscription = new Subscription(credentials);

var marketFilter = new StreamMarketFilter().WithMarketIds("1.23456789");
var dataFilter = new DataFilter().WithBestPrices().WithLastTradedPrice();

await subscription.RunMarketCache(marketFilter, dataFilter, onUpdate: processor =>
{
    var market = processor.GetMarket("1.23456789");
    if (market is null) return;

    foreach (var runner in market.RunnerSpan)
    {
        var bestBack = runner.BestAvailableToBack.GetPrice(0);
        var bestLay = runner.BestAvailableToLay.GetPrice(0);
        // React to price updates...
    }
});
```

## How It Works
When you call `subscription.RunMarketCache(...)`, the Subscription:
1. Subscribes to the market stream with your filter and data settings.
2. Reads raw bytes directly from the TCP pipeline.
3. Passes each message to `MarketCacheProcessor.Process()` which parses and applies updates in-place.
4. Optionally invokes your callback after each message is processed.

This avoids allocating `ChangeMessage` objects, JSON strings, and list instances on every tick. On steady-state delta messages, zero heap allocations occur.

## Accessing Market Data

### MarketCacheProcessor
The processor is available on the Subscription via `subscription.MarketProcessor`. It maintains the state of all subscribed markets.

| Property | Type | Description |
|----------|------|-------------|
| `Markets` | `IReadOnlyDictionary<string, MarketCache>` | All market caches keyed by market ID |
| `Clock` | `string?` | Last clock token (allocates on access) |
| `ClockBytes` | `ReadOnlySpan<byte>` | Last clock token as raw bytes (zero-allocation) |
| `InitialClock` | `string?` | Initial clock token (allocates on access) |
| `InitialClockBytes` | `ReadOnlySpan<byte>` | Initial clock token as raw bytes (zero-allocation) |
| `PublishTime` | `long` | Last publish time from the stream |
| `LastProcessingTime` | `TimeSpan` | Processing duration of the last message |

| Method | Description |
|--------|-------------|
| `GetMarket(string marketId)` | Gets a market cache by ID, or null if not present |
| `Process(ReadOnlySpan<byte> data)` | Processes a single line of raw stream bytes |

### MarketCache
Each market in the processor has its own `MarketCache` containing live state.

| Property | Type | Description |
|----------|------|-------------|
| `MarketId` | `string` | The market ID (e.g. "1.241629436") |
| `TotalMatched` | `double?` | Total matched volume on this market |
| `IsImage` | `bool` | Whether the last update was a full image |
| `PublishTime` | `long` | Last publish time from the stream |
| `Definition` | `MarketDefinitionCache` | Market definition (status, venue, in-play, etc.) |
| `RunnerCount` | `int` | Number of runners in this market |
| `RunnerSpan` | `ReadOnlySpan<RunnerCache>` | All runners as a span (zero-allocation iteration) |
| `Runners` | `IReadOnlyDictionary<long, RunnerCache>` | All runners keyed by selection ID (allocates on access) |

| Method | Description |
|--------|-------------|
| `GetRunner(long selectionId)` | Gets a runner by selection ID, or null if not found |

For best performance, use `RunnerSpan` to iterate runners without allocating a dictionary.

### RunnerCache
Each runner maintains its own set of price ladders and scalar values.

| Property | Type | Description |
|----------|------|-------------|
| `SelectionId` | `long` | The selection (runner) ID |
| `Handicap` | `double` | The handicap value for this runner |
| `LastTradedPrice` | `double` | Last price matched (NaN if not set) |
| `TotalMatched` | `double` | Total volume matched on this runner (NaN if not set) |
| `StartingPriceNear` | `double` | Starting price near (NaN if not set) |
| `StartingPriceFar` | `double` | Starting price far (NaN if not set) |
| `AvailableToBack` | `PriceLadder` | Full depth available to back (price to size) |
| `AvailableToLay` | `PriceLadder` | Full depth available to lay (price to size) |
| `BestAvailableToBack` | `PositionLadder` | Best available to back (position-indexed) |
| `BestAvailableToLay` | `PositionLadder` | Best available to lay (position-indexed) |
| `BestDisplayAvailableToBack` | `PositionLadder` | Best display available to back (includes virtual) |
| `BestDisplayAvailableToLay` | `PositionLadder` | Best display available to lay (includes virtual) |
| `Traded` | `PriceLadder` | Traded volume (price to cumulative size) |
| `StartingPriceBack` | `PriceLadder` | Starting price back ladder |
| `StartingPriceLay` | `PriceLadder` | Starting price lay ladder |

Scalar values use `double.NaN` as a sentinel instead of `Nullable<double>` to eliminate wrapping overhead. Use the `Has*` properties to check if values have been set:
- `HasLastTradedPrice`
- `HasTotalMatched`
- `HasStartingPriceNear`
- `HasStartingPriceFar`

### Market Definition
The `MarketDefinitionCache` provides metadata about the market, updated in-place from the stream.

| Property | Type | Description |
|----------|------|-------------|
| `Status` | `string?` | Market status (OPEN, SUSPENDED, CLOSED) |
| `InPlay` | `bool?` | Whether the market is currently in-play |
| `BetDelay` | `int?` | Bet delay in seconds |
| `BettingType` | `string?` | Betting type (ODDS, LINE, etc.) |
| `MarketType` | `string?` | Market type (WIN, MATCH_ODDS, etc.) |
| `Venue` | `string?` | Venue name |
| `CountryCode` | `string?` | Country ISO code |
| `EventId` | `string?` | Event ID |
| `EventTypeId` | `string?` | Event type ID |
| `NumberOfWinners` | `int?` | Number of winners |
| `NumberOfActiveRunners` | `int?` | Number of active runners |
| `TurnInPlayEnabled` | `bool?` | Whether the market will turn in-play |
| `BspMarket` | `bool?` | Whether it is a BSP market |
| `CrossMatching` | `bool?` | Whether cross-matching is enabled |
| `RunnersVoidable` | `bool?` | Whether runners are voidable |
| `Runners` | `IReadOnlyList<RunnerDefinitionCache>` | Runner definitions |

Each `RunnerDefinitionCache` contains:
- `SelectionId` — the runner's selection ID
- `Status` — runner status (ACTIVE, REMOVED, WINNER, LOSER)
- `SortPriority` — display sort priority
- `Handicap` — handicap value
- `AdjustmentFactor` — BSP adjustment factor
- `BspLiability` — BSP liability

## Price Ladders

### PriceLadder
A dictionary-based ladder mapping price to size. Used for full-depth data (AvailableToBack, AvailableToLay, Traded, StartingPriceBack, StartingPriceLay).

```csharp
var runner = market.GetRunner(selectionId);

// Look up size at a specific price
double sizeAt2_5 = runner.AvailableToBack[2.5];

// Iterate all levels
foreach (var entry in runner.Traded)
{
    double price = entry.Key;
    double size = entry.Value;
}

// Check how many levels are populated
int depth = runner.AvailableToBack.Count;
```

### PositionLadder
A fixed-size, position-indexed ladder for Best Available data (up to 10 positions). Provides O(1) access with excellent cache locality.

```csharp
var runner = market.GetRunner(selectionId);

// Get best back price and size (position 0)
double bestBackPrice = runner.BestAvailableToBack.GetPrice(0);
double bestBackSize = runner.BestAvailableToBack.GetSize(0);

// Get second-best lay price
double secondLayPrice = runner.BestAvailableToLay.GetPrice(1);

// Check how many positions are populated
int count = runner.BestAvailableToBack.Count;
```

## Resume Tokens
The processor exposes `Clock` and `InitialClock` tokens that can be saved and used to resume a stream after a disconnection. The Subscription handles this automatically on reconnection, but you can also access the tokens directly:

```csharp
// Save tokens for manual resume
string? clk = subscription.MarketProcessor.Clock;
string? initialClk = subscription.MarketProcessor.InitialClock;

// Use zero-allocation span access in hot paths
ReadOnlySpan<byte> clockBytes = subscription.MarketProcessor.ClockBytes;
```

## Callback on Update
The `onUpdate` callback is invoked after each message is fully processed. This is the recommended way to react to price changes with minimal latency:

```csharp
await subscription.RunMarketCache(marketFilter, dataFilter, onUpdate: processor =>
{
    // This runs on the stream-reading thread — keep it fast!
    var market = processor.GetMarket("1.23456789");
    if (market?.Definition.InPlay == true)
    {
        // Market is in-play, check prices...
    }
});
```

Keep the callback lightweight. Heavy work (logging, network calls, order placement) should be offloaded to another thread or channel.

## Class
You can view the MarketCacheProcessor class [here](/src/Betfair/Stream/MarketCache/MarketCacheProcessor.cs)
