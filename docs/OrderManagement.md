# Order Management

Complete guide to placing, managing, and tracking orders with the Betfair API.

## Contents
- [Placing Orders](#placing-orders)
- [Order Types](#order-types)
- [Cancelling Orders](#cancelling-orders)
- [Replacing Orders](#replacing-orders)
- [Updating Orders](#updating-orders)
- [Listing Orders](#listing-orders)
- [Order History](#order-history)
- [Error Handling](#error-handling)

## Placing Orders

### Basic Limit Order

```csharp
var placeOrders = new PlaceOrders("1.123456789");
placeOrders.Instructions.Add(new PlaceInstruction
{
    SelectionId = 47972,
    Side = Side.Back,
    OrderType = OrderType.Limit,
    LimitOrder = new LimitOrder
    {
        Size = 10.0,     // £10 stake
        Price = 2.5,     // At odds of 2.5
        PersistenceType = PersistenceType.Lapse
    },
    CustomerOrderRef = "my-ref-123"
});

var result = await client.PlaceOrders(placeOrders);
```

### Multiple Orders

```csharp
var placeOrders = new PlaceOrders("1.123456789");

// Back bet
placeOrders.Instructions.Add(new PlaceInstruction
{
    SelectionId = 47972,
    Side = Side.Back,
    OrderType = OrderType.Limit,
    LimitOrder = new LimitOrder { Size = 10.0, Price = 2.5, PersistenceType = PersistenceType.Lapse }
});

// Lay bet
placeOrders.Instructions.Add(new PlaceInstruction
{
    SelectionId = 47973,
    Side = Side.Lay,
    OrderType = OrderType.Limit,
    LimitOrder = new LimitOrder { Size = 5.0, Price = 3.0, PersistenceType = PersistenceType.Persist }
});

var result = await client.PlaceOrders(placeOrders);
```

### Asynchronous Placement

```csharp
var placeOrders = new PlaceOrders("1.123456789")
{
    Async = true  // Orders placed asynchronously
};

placeOrders.Instructions.Add(new PlaceInstruction
{
    SelectionId = 47972,
    Side = Side.Back,
    OrderType = OrderType.Limit,
    LimitOrder = new LimitOrder { Size = 10.0, Price = 2.5, PersistenceType = PersistenceType.Lapse },
    CustomerOrderRef = "async-bet-123"
});

var result = await client.PlaceOrders(placeOrders);
// Orders will have status PENDING and no bet ID initially
```

## Order Types

### Limit Orders
Standard orders at a specific price:

```csharp
LimitOrder = new LimitOrder
{
    Size = 10.0,
    Price = 2.5,
    PersistenceType = PersistenceType.Lapse,
    TimeInForce = TimeInForce.FillOrKill,
    MinFillSize = 5.0,
    BetTargetType = BetTargetType.BackBaccaratPlayer,
    BetTargetSize = 20.0
}
```

### Market on Close Orders
Bet at starting price:

```csharp
OrderType = OrderType.MarketOnClose,
MarketOnClose = new MarketOnCloseOrder
{
    Liability = 10.0  // Maximum liability for lay bets
}
```

### Limit on Close Orders
Bet at starting price with a limit:

```csharp
OrderType = OrderType.LimitOnClose,
LimitOnClose = new LimitOnCloseOrder
{
    Liability = 10.0,
    Price = 2.0  // Won't accept worse than 2.0
}
```

## Cancelling Orders

### Cancel All Orders on Market

```csharp
var cancelOrders = new CancelOrders
{
    MarketId = "1.123456789"
};

var result = await client.CancelOrders(cancelOrders);
```

### Cancel Specific Orders

```csharp
var cancelOrders = new CancelOrders
{
    MarketId = "1.123456789"
};

cancelOrders.Instructions.Add(new CancelInstruction
{
    BetId = "123456789"
});

cancelOrders.Instructions.Add(new CancelInstruction
{
    BetId = "987654321",
    SizeReduction = 5.0  // Partially cancel £5
});

var result = await client.CancelOrders(cancelOrders);
```

### Cancel All Orders (All Markets)

```csharp
var cancelAll = new CancelOrders(); // No market ID = cancel all
var result = await client.CancelOrders(cancelAll);
```

## Replacing Orders

Replace existing orders with new ones:

```csharp
var replaceOrders = new ReplaceOrders("1.123456789");

replaceOrders.Instructions.Add(new ReplaceInstruction
{
    BetId = "123456789",
    NewPrice = 3.0  // Change price from old to 3.0
});

var result = await client.ReplaceOrders(replaceOrders);
```

## Updating Orders

Update non-exposure changing fields:

```csharp
var updateOrders = new UpdateOrders("1.123456789");

updateOrders.Instructions.Add(new UpdateInstruction
{
    BetId = "123456789",
    NewPersistenceType = PersistenceType.Persist
});

var result = await client.UpdateOrders(updateOrders);
```

## Listing Orders

### Current Unmatched Orders

```csharp
var filter = new ApiOrderFilter()
    .WithMarketIds("1.123456789")
    .ExecutableOnly()
    .MostRecentFirst()
    .Take(100);

var orders = await client.CurrentOrders(filter);

foreach (var order in orders.CurrentOrders ?? [])
{
    Console.WriteLine($"Bet ID: {order.BetId}");
    Console.WriteLine($"Selection: {order.SelectionId}");
    Console.WriteLine($"Side: {order.Side}");
    Console.WriteLine($"Price: {order.PriceSize?.Price}");
    Console.WriteLine($"Size: {order.PriceSize?.Size}");
    Console.WriteLine($"Matched: £{order.SizeMatched:F2}");
    Console.WriteLine();
}
```

### Filter by Strategy Reference

```csharp
var filter = new ApiOrderFilter()
    .WithCustomerStrategyRefs("my-strategy")
    .ExecutableOnly();

var orders = await client.CurrentOrders(filter);
```

### Filter by Date Range

```csharp
var filter = new ApiOrderFilter()
    .WithDateRange(
        DateTimeOffset.UtcNow.AddDays(-7),
        DateTimeOffset.UtcNow)
    .Take(500);

var orders = await client.CurrentOrders(filter);
```

## Order History

### Cleared Orders (Betting History)

```csharp
var query = new ClearedOrdersQuery()
    .LastWeek()
    .SettledOnly()
    .WithMarkets("1.123456789")
    .BackBetsOnly()
    .GroupBy(GroupBy.Market)
    .IncludeItemDescriptions()
    .Take(500);

var clearedOrders = await client.ClearedOrders(query);

foreach (var order in clearedOrders.ClearedOrders ?? [])
{
    Console.WriteLine($"Bet ID: {order.BetId}");
    Console.WriteLine($"Market: {order.ItemDescription?.MarketName}");
    Console.WriteLine($"Selection: {order.ItemDescription?.SelectionName}");
    Console.WriteLine($"Stake: £{order.PriceSize?.Size:F2}");
    Console.WriteLine($"Odds: {order.PriceSize?.Price}");
    Console.WriteLine($"Profit/Loss: £{order.Profit:F2}");
    Console.WriteLine($"Settled: {order.SettledDate}");
    Console.WriteLine();
}
```

### Filter by Event Type

```csharp
var query = new ClearedOrdersQuery()
    .WithEventTypes(EventType.HorseRacing.ToString())
    .LastMonth()
    .SettledOnly();

var clearedOrders = await client.ClearedOrders(query);
```

### Filter by Bet Status

```csharp
// Settled bets only
var settledQuery = new ClearedOrdersQuery().SettledOnly();

// Cancelled bets only
var cancelledQuery = new ClearedOrdersQuery().CancelledOnly();

// Voided bets only
var voidedQuery = new ClearedOrdersQuery().VoidedOnly();
```

## Error Handling

### Handling Execution Reports

```csharp
var result = await client.PlaceOrders(placeOrders);

if (result.Status == ExecutionReportStatus.Success)
{
    Console.WriteLine("All orders placed successfully");
    foreach (var report in result.InstructionReports ?? [])
    {
        if (report.Status == InstructionReportStatus.Success)
        {
            Console.WriteLine($"Order placed: Bet ID {report.BetId}");
        }
        else
        {
            Console.WriteLine($"Order failed: {report.ErrorCode}");
        }
    }
}
else
{
    Console.WriteLine($"Request failed: {result.ErrorCode} - {result.ErrorMessage}");
}
```

### Common Error Codes

| Error Code | Description |
|------------|-------------|
| INVALID_MARKET_ID | Market ID is invalid or market is closed |
| INSUFFICIENT_FUNDS | Not enough funds in account |
| INVALID_ODDS | Price is outside valid range |
| DUPLICATE_TRANSACTION | Duplicate customer reference |
| MARKET_SUSPENDED | Market is currently suspended |
| BET_TAKEN_OR_LAPSED | Bet has already been taken or lapsed |

### Retry Logic

```csharp
async Task<PlaceExecutionReport> PlaceOrderWithRetry(PlaceOrders placeOrders, int maxRetries = 3)
{
    for (int attempt = 1; attempt <= maxRetries; attempt++)
    {
        try
        {
            var result = await client.PlaceOrders(placeOrders);
            
            if (result.Status == ExecutionReportStatus.Success)
                return result;
                
            if (result.ErrorCode == ExecutionReportErrorCode.MarketSuspended)
            {
                await Task.Delay(1000 * attempt); // Wait before retry
                continue;
            }
            
            return result; // Don't retry for other errors
        }
        catch (HttpRequestException) when (attempt < maxRetries)
        {
            await Task.Delay(1000 * attempt);
        }
    }
    
    throw new InvalidOperationException("Max retries exceeded");
}
```

## Best Practices

1. **Always check execution reports** - Even successful requests can have failed individual instructions
2. **Use customer references** - For tracking and deduplication
3. **Handle market suspension** - Markets can be suspended during events
4. **Monitor your exposure** - Check account funds before placing large orders
5. **Use appropriate persistence types** - Lapse for in-play, Persist for pre-match
6. **Implement retry logic** - For transient network errors
7. **Log all operations** - For audit and debugging purposes
