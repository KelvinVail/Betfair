# Order Cache
The Order Cache is a zero-allocation, ultra-low-latency system for maintaining live order state directly from stream bytes. It processes raw Betfair order stream data (`ocm` messages) without intermediate JSON deserialization objects, making it ideal for latency-sensitive trading applications that need to track order positions in real time.

## Contents
- [Order Cache](#order-cache)
  - [Contents](#contents)
  - [Quick Start](#quick-start)
  - [How It Works](#how-it-works)
  - [Accessing Order Data](#accessing-order-data)
    - [OrderCacheProcessor](#ordercacheprocessor)
    - [OrderCache](#ordercache-1)
    - [OrderRunnerCache](#orderrunnercache)
    - [UnmatchedOrderCache](#unmatchedordercache)
    - [StrategyMatchCache](#strategymatchcache)
  - [Order Filters](#order-filters)
  - [Combined Market and Order Cache](#combined-market-and-order-cache)
  - [Resume Tokens](#resume-tokens)
  - [Callback on Update](#callback-on-update)
  - [Performance](#performance)

## Quick Start
```csharp
var credentials = new Credentials([USERNAME], [PASSWORD], [APPKEY]);
using var subscription = new Subscription(credentials);

var orderFilter = new StreamOrderFilter().WithDetailedPositions();

await subscription.RunOrderCache(orderFilter, onUpdate: processor =>
{
    var market = processor.GetMarket("1.23456789");
    if (market is null) return;

    foreach (var runner in market.RunnerSpan)
    {
        // Aggregated positions
        var matchedBackAt2 = runner.MatchedBacks[2.0];

        // Individual orders
        foreach (var order in runner.OrderSpan)
        {
            var betId = order.BetId;
            var price = order.Price;
            var remaining = order.SizeRemaining;
        }
    }
});
```

## How It Works
When you call `subscription.RunOrderCache(...)`, the Subscription:
1. Subscribes to the order stream with your filter settings.
2. Reads raw bytes directly from the TCP pipeline.
3. Passes each message to `OrderCacheProcessor.Process()` which parses and applies updates in-place.
4. Optionally invokes your callback after each message is processed.

On steady-state delta messages (after the initial image), **zero heap allocations occur**. This is achieved through:
- Direct `Utf8JsonReader` parsing without intermediate objects
- Byte-based identity comparison for BetIds and strategy references (no string allocation on lookup)
- Cached UTF-8 bytes for string fields with lazy decoding (only allocates a string when you read the property, and only on first access)
- Byte-sized enums for categorical fields (`Side`, `OrderStatus`, `PersistenceType`, `OrderType`)
- Object pooling for `UnmatchedOrderCache` instances on full image resets

## Accessing Order Data

### OrderCacheProcessor
The processor is available on the Subscription via `subscription.OrderProcessor`. It maintains the state of all subscribed order markets.

| Property | Type | Description |
|----------|------|-------------|
| `Markets` | `IReadOnlyDictionary<string, OrderCache>` | All order caches keyed by market ID |
| `Clock` | `string?` | Last clock token (allocates on access) |
| `ClockBytes` | `ReadOnlySpan<byte>` | Last clock token as raw bytes (zero-allocation) |
| `InitialClock` | `string?` | Initial clock token (allocates on access) |
| `InitialClockBytes` | `ReadOnlySpan<byte>` | Initial clock token as raw bytes (zero-allocation) |
| `PublishTime` | `long` | Last publish time from the stream |
| `LastProcessingTime` | `TimeSpan` | Processing duration of the last message |

| Method | Description |
|--------|-------------|
| `GetMarket(string marketId)` | Gets an order cache by market ID, or null if not present |
| `Process(ReadOnlySpan<byte> data)` | Processes a single line of raw stream bytes |

### OrderCache
Each market in the processor has its own `OrderCache` containing live order state.

| Property | Type | Description |
|----------|------|-------------|
| `MarketId` | `string` | The market ID (e.g. "1.241629436") |
| `AccountId` | `long?` | The account ID associated with these orders |
| `Closed` | `bool?` | Whether the market is closed for orders |
| `IsImage` | `bool` | Whether the last update was a full image |
| `PublishTime` | `long` | Last publish time from the stream |
| `RunnerCount` | `int` | Number of runners with order data |
| `RunnerSpan` | `ReadOnlySpan<OrderRunnerCache>` | All runners as a span (zero-allocation iteration) |

| Method | Description |
|--------|-------------|
| `GetRunner(long selectionId)` | Gets a runner by selection ID, or null if not found |

For best performance, use `RunnerSpan` to iterate runners without allocating a dictionary.

### OrderRunnerCache
Each runner maintains aggregated positions, individual orders, and per-strategy data.

| Property | Type | Description |
|----------|------|-------------|
| `SelectionId` | `long` | The selection (runner) ID |
| `Handicap` | `double` | The handicap value for this runner |
| `MatchedBacks` | `PriceLadder` | Aggregated matched backs (price → size) |
| `MatchedLays` | `PriceLadder` | Aggregated matched lays (price → size) |
| `OrderCount` | `int` | Number of individual orders |
| `OrderSpan` | `ReadOnlySpan<UnmatchedOrderCache>` | All orders as a span (zero-allocation) |
| `Orders` | `IReadOnlyDictionary<string, UnmatchedOrderCache>` | Orders keyed by BetId (allocates on access) |
| `StrategyMatches` | `IReadOnlyDictionary<string, StrategyMatchCache>` | Per-strategy data (allocates on access) |

| Method | Description |
|--------|-------------|
| `GetOrder(string betId)` | Gets an order by BetId, or null if not found |
| `GetOrderByBytes(ReadOnlySpan<byte> betIdBytes)` | Gets an order by UTF-8 BetId bytes (zero-allocation) |

For best performance, use `OrderSpan` to iterate orders without allocating.

### UnmatchedOrderCache
Each individual order maintains its full state, updated in-place from stream deltas.

| Property | Type | Description |
|----------|------|-------------|
| `BetId` | `string` | The bet ID (lazily decoded from UTF-8 bytes) |
| `BetIdBytes` | `ReadOnlySpan<byte>` | Raw UTF-8 bytes of the BetId (zero-allocation) |
| `Price` | `double` | Original placed price (NaN if not set) |
| `Size` | `double` | Original placed size (NaN if not set) |
| `BspLiability` | `double` | BSP liability (NaN if not a BSP order) |
| `Side` | `Side` | Back or Lay |
| `Status` | `OrderStatus` | Executable or ExecutionComplete |
| `PersistenceType` | `PersistenceType` | Lapse, Persist, or MarketOnClose |
| `OrderType` | `OrderType` | Limit, LimitOnClose, or MarketOnClose |
| `PlacedDate` | `long` | Placed date (epoch millis) |
| `MatchedDate` | `long` | Matched date (epoch millis, 0 if not matched) |
| `CancelledDate` | `long` | Cancelled date (epoch millis, 0 if not cancelled) |
| `LapsedDate` | `long` | Lapsed date (epoch millis, 0 if not lapsed) |
| `LapsedStatusReasonCode` | `string?` | Lapse reason (lazily decoded) |
| `AveragePriceMatched` | `double` | Average matched price (NaN if not matched) |
| `SizeMatched` | `double` | Amount matched (NaN if not set) |
| `SizeRemaining` | `double` | Amount remaining (NaN if not set) |
| `SizeLapsed` | `double` | Amount lapsed (NaN if not set) |
| `SizeCancelled` | `double` | Amount cancelled (NaN if not set) |
| `SizeVoided` | `double` | Amount voided (NaN if not set) |
| `RegulatorAuthCode` | `string?` | Regulator auth code (lazily decoded) |
| `RegulatorCode` | `string?` | Regulator code (lazily decoded) |
| `OrderReference` | `string?` | Customer order reference (lazily decoded) |
| `StrategyReference` | `string?` | Customer strategy reference (lazily decoded) |

Scalar `double` values use `double.NaN` as a sentinel instead of `Nullable<double>` to eliminate boxing overhead. Categorical fields use enums parsed directly from UTF-8 bytes. String fields are lazily decoded from cached UTF-8 bytes — no allocation occurs if you don't read the property, and the string is only created once on first access.

### StrategyMatchCache
Per-strategy matched position data (when `partitionMatchedByStrategyRef` is enabled).

| Property | Type | Description |
|----------|------|-------------|
| `StrategyRef` | `string` | The strategy reference (lazily decoded) |
| `StrategyRefBytes` | `ReadOnlySpan<byte>` | Raw UTF-8 bytes (zero-allocation) |
| `MatchedBacks` | `PriceLadder` | Matched backs for this strategy (price → size) |
| `MatchedLays` | `PriceLadder` | Matched lays for this strategy (price → size) |

## Order Filters
The `StreamOrderFilter` class controls what data the order stream returns:

```csharp
// Aggregated positions (default) — just matched back/lay totals per runner
var filter = new StreamOrderFilter().WithAggregatedPositions();

// Detailed positions — individual order objects per runner
var filter = new StreamOrderFilter().WithDetailedPositions();

// Per-strategy aggregation
var filter = new StreamOrderFilter().WithOrdersPerStrategy();

// Filter to specific strategy references
var filter = new StreamOrderFilter().WithStrategyRefs("myRef", "otherRef");
```

See the [Order Filter documentation](/docs/OrderFilter.md) for more details.

## Combined Market and Order Cache
Use `RunMarketAndOrderCaches` to subscribe to both market prices and orders on a single TCP connection. Both processors receive every message and filter on their respective operation types (`mcm` / `ocm`).

```csharp
await subscription.RunMarketAndOrderCaches(
    marketFilter: new StreamMarketFilter().WithMarketIds("1.23456789"),
    dataFilter: new DataFilter().WithBestPrices().WithLastTradedPrice(),
    orderFilter: new StreamOrderFilter().WithDetailedPositions(),
    onUpdate: (markets, orders) =>
    {
        var market = markets.GetMarket("1.23456789");
        var orderMarket = orders.GetMarket("1.23456789");
        if (market is null) return;

        foreach (var runner in market.RunnerSpan)
        {
            var bestBack = runner.BestAvailableToBack.GetPrice(0);
            var orderRunner = orderMarket?.GetRunner(runner.SelectionId);

            // Correlate market prices with order positions
            if (orderRunner != null)
            {
                var myPosition = orderRunner.MatchedBacks[bestBack];
            }
        }
    });
```

This gives you correlated market and order state on every tick with minimal latency.

## Resume Tokens
The processor exposes `Clock` and `InitialClock` tokens that can be saved and used to resume a stream after a disconnection. The Subscription handles this automatically on reconnection, but you can access the tokens directly:

```csharp
// Save tokens for manual resume
string? clk = subscription.OrderProcessor.Clock;
string? initialClk = subscription.OrderProcessor.InitialClock;

// Use zero-allocation span access in hot paths
ReadOnlySpan<byte> clockBytes = subscription.OrderProcessor.ClockBytes;
```

## Callback on Update
The `onUpdate` callback is invoked after each message is fully processed. This is the recommended way to react to order changes with minimal latency:

```csharp
await subscription.RunOrderCache(orderFilter, onUpdate: processor =>
{
    // This runs on the stream-reading thread — keep it fast!
    foreach (var (_, market) in processor.Markets)
    {
        if (market.Closed == true) continue;

        foreach (var runner in market.RunnerSpan)
        {
            foreach (var order in runner.OrderSpan)
            {
                if (order.Status == OrderStatus.Executable)
                {
                    // React to live order updates...
                }
            }
        }
    }
});
```

Keep the callback lightweight. Heavy work (logging, network calls, order placement) should be offloaded to another thread or channel.

## Performance
Benchmarked on AMD Ryzen 7 5800X, .NET 10.0, processing 266 real order stream messages:

| Scenario | Mean | Allocated |
|----------|------|-----------|
| System.Text.Json deserialization (baseline) | 1,330 μs | 1,175 KB |
| OrderCache — full stream (image + deltas) | 700 μs | 349 KB |
| **OrderCache — deltas only (steady-state)** | **598 μs** | **0 B** |
| Zero-copy pipeline — deltas only | 731 μs | 3 KB |
| Zero-copy pipeline — full stream | 807 μs | 352 KB |

The delta-only path achieves **zero heap allocation** — no GC pressure during normal operation. The initial image allocates ~349 KB to create the order objects and their byte identity arrays, which are then reused for the lifetime of the stream via object pooling.

## Class
You can view the OrderCacheProcessor class [here](/src/Betfair/Stream/OrderCache/OrderCacheProcessor.cs)
