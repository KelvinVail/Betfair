# OrderFilter
Used to shape and filter the order data returned on the stream.

## Chaining order filters
The OrderFilter class can be used to build up a list of options to be used in the order stream.

The following example will filter the order stream to only those strategy refs specified,
and then aggregate the order for each strategy ref.
```csharp
var orderFilter = new OrderFilter().WithStrategyRefs("myRef", "otherRef").WithOrderPerStrategy();
```

## Aggregated Positions (Default)
Returns changes to the overall order position aggregated per runner.
This is the default if the OrderFilter is not used or if Detailed Positions are not asked for.
```csharp
var orderFilter = new OrderFilter().WithAggregatedPositions();
```

## Detailed Positions
Returns changes to each individual order.
```csharp
var orderFilter = new OrderFilter().WithDetailedPositions();
```

## Orders per Strategy
If aggregated orders are asked for, this splits that aggregation per strategy reference.
Strategy references can be specified when placing orders with Betfair.
```csharp
var orderFilter = new OrderFilter().WithOrderPerStrategy();
```

## Filter Strategy References
Returns only the changes to orders that have these specified strategy references.
Strategy references can be specified when placing orders with Betfair.
```csharp
var orderFilter = new OrderFilter().WithStrategyRefs("myRef");
```
