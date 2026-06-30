# Subscription
The Subscription is used to subscribe to Betfair market and order streams.

## Contents
- [Subscription](#subscription)
	- [Contents](#contents)
	- [Create a Subscription](#create-a-subscription)
	- [Subscribe to a Market Stream](#subscribe-to-a-market-stream)
	- [Subscribe to an Order Stream](#subscribe-to-an-order-stream)
	- [Conflate the Subscription](#conflate-the-subscription)
	- [Consume Change Messages](#consume-change-messages)
		- [Raw Byte Access](#raw-byte-access)
		- [Back Pressure](#back-pressure)
	- [Market Cache](#market-cache)
	- [Reconnection](#reconnection)
	- [Unsubscribe from the stream](#unsubscribe-from-the-stream)

## Create a Subscription
The Subscription needs a [Credentials](/docs/Authentication.md) object to authenticate to Betfair.  
Create a Subscription, wrapping it in a using block.
```csharp
var credentials = new Credentials([USERNAME], [PASSWORD], [APPKEY]);
using var subscription = new Subscription(credentials);
```

## Subscribe to a Market Stream
To subscribe to a market stream we need to define which markets to subscribe to and what data we want returned in the stream.
For more information on filters see [StreamMarketFilter](/docs/StreamMarketFilter.md) and [DataFilter](/docs/DataFilter.md).
In this example we are subscribing to a single market and requesting that only the best available prices are returned in the data stream.

The DataFilter is optional, if it is not included Best Available Prices will be returned.

A recommended tip is to use the StreamMarketFilter to subscribe to all markets you are interested in for the duration of your programs execution. This eliminates the need to close and recreate Subscriptions for each new market you need. To improve speed, limit the DataFilter to only the data you need. 
```csharp
var marketFilter = new StreamMarketFilter().WithMarketIds("1.23456789");
var dataFilter = new DataFilter().WithBestPrices();
```

Use the filters to subscribe to a stream of market data.
```csharp
await subscription.Subscribe(marketFilter, dataFilter);
```

## Subscribe to an Order Stream
To include updates to new and open orders in your stream, subscribe to the order stream.
```csharp
await subscription.SubscribeToOrders();
```
The order stream can optionally by shaped and filtered using the [StreamOrderFilter](/docs/OrderFilter.md).
```csharp
var orderFilter = new StreamOrderFilter().WithStrategyRefs("myRef");
await subscription.SubscribeToOrders(orderFilter);
```

You will only receive updates to new orders or orders that are open at the time you subscribe to the order stream. Historical fully matched orders are not included in the order stream. If needed, you should use the BetfairApiClient to retrieve them. 

## Conflate the Subscription
Either the market or order subscription can be conflated. Data will be rolled up and sent on each increment of the interval specified.
```csharp
var conflate = TimeSpan.FromSeconds(1);
await subscription.Subscribe(marketFilter, dataFilter, conflate: conflate);
await subscription.SubscribeToOrders(conflate: conflate);
```

## Consume Change Messages
After you have subscribed to a market and/or an order stream you can iterate each [ChangeMessage](/docs/ChangeMessage.md) asynchronously as they become available on the stream. 
```csharp
await foreach (ChangeMessage change in subscription.ReadLines(default))
{
	// Handle change..
}
```
The implementation of handling change messages is left to the reader. This is so that you can implement any strategy as you see fit as efficiently as you can. Without relying on an opinionated and bloated caching implementation. See the documentation on [ChangeMessage](/docs/ChangeMessage.md) for more information on the different messages you will receive.

### Raw Byte Access
If you need access to the raw stream bytes (for logging, recording, or custom parsing) you can use `ReadRawLines` instead:
```csharp
await foreach (ReadOnlyMemory<byte> rawMessage in subscription.ReadRawLines(default))
{
	// Each element is one newline-delimited message before JSON deserialization.
}
```

### Back Pressure
The Subscription takes changes off of the underlying TCP connection as quickly as it can, holding any messages in a cache that are unconsumed by you. Be aware of this if you plan to perform any long running operations between handling each message.

## Market Cache
For latency-sensitive use cases, the Subscription provides a zero-allocation market cache that processes raw stream bytes directly into an in-memory market state without intermediate deserialization. See [MarketCache](/docs/MarketCache.md) for full documentation.

```csharp
await subscription.RunMarketCache(marketFilter, dataFilter, onUpdate: processor =>
{
    foreach (var (id, market) in processor.Markets)
    {
        // React to price changes with minimal latency
    }
});
```

## Reconnection
The Subscription automatically reconnects and resubscribes if the stream connection is lost. On reconnection it will use the last received `initialClk` and `clk` tokens to resume the stream from where it left off, minimizing data loss.

You can also provide resume tokens manually when subscribing:
```csharp
await subscription.Subscribe(marketFilter, dataFilter, initialClk: savedInitialClk, clk: savedClk);
```

## Unsubscribe from the stream
To unsubscribe from the entire stream you must dispose of the subscription.
```csharp
subscription.Dispose();
```
Once a Subscription is disposed of, you can not reuse it.
You can not unsubscribe from components of your subscription. For example, if you are subscribed to two markets you can not unsubscribe from one and leave the other open. To stop the stream you must dispose of it. 
