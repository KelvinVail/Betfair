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
		- [Back Pressure](#back-pressure)
	- [Unsubscribe from the stream](#unsubscribe-from-the-stream)

## Create a Subscription
The Subscription needs a [Credentials](/docs/Authentication.md) object to authenticate to Betfair.  
Create a Subscription, wrapping it in a using block.
```csharp
var credentials = new Credential([USERNAME], [PASSWORD], [APPKEY]);
using var subscription = new Subscription(credentials);
```

## Subscribe to a Market Stream
To subscribe to a market stream we need to define which markets to subscribe to and what data we want returned in the stream.
For more information on filters see [StreamMarketFilter](/docs/StreamMarketFilter.md) and [DataFilter](/docs/MarketFilter.md).
In this example we are subscribing to a single market and requesting that only the best available prices are returned in the data stream.

The DataFilter is optional, if it is not included Best Available Prices will be returned.

A recommended tip is to use the StreamMarketFilter to subscribe to all markets you are interested in for the duration of your programs execution. This eliminates the need to close and recreate Subscriptions for each new market you need. To improve speed, limit the DataFilter to only the data you need. 
```csharp
var marketFilter = new StreamMarketFilter().WithMarketId("1.23456789");
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
The order stream can optionally by shaped and filtered using the [OrderFilter](/docs/OrderFilter.md).
```csharp
var orderFilter = new OrderFilter().WithStrategyRefs("myRef");
await subscription.SubscribeToOrders(orderFilter);
```

You will only receive updates to new orders or orders that are open at the time you subscribe to the order stream. Historical fully matched orders are not included in the order stream. If needed, you should use the BetfairApiClient to retrieve them. 

## Conflate the Subscription
Either the market or order subscription can be conflated. Data will be rolled up and sent on each increment of the interval specified.
```charp
var conflate = TimeSpan.FromSeconds(1);
await subscription.Subscribe(marketFilter, dataFilter, conflate: conflate);
await subscription.SubscribeToOrders(conflate: conflate);
```

## Consume Change Messages
After you have subscribed to a market and/or an order stream you can iterate each [ChangeMessage](/docs/ChangeMessage.md) asynchronously as they become available on the stream. 
```csharp
await foreach (ChangeMessage change in streamClient.ReadLines(default))
{
	// Handle change..
}
```
The implementation of handling change messages is left to the reader. This is so that you can implement any strategy as you see fit as efficiently as you can. Without relying on an opinionated and bloated caching implementation. See the documentation on [ChangeMessage](/docs/ChangeMessage.md) for more information on the different messages you will receive.

### Back Pressure
The StreamClient takes changes off of the underlying TCP connection as quickly as it can, holding any messages in a cache that are unconsumed by you. Be aware of this if you plan to perform any long running operations between handling each message.

## Unsubscribe from the stream
To unsubscribe from the entire stream you must dispose of the subscription.
```csharp
subscription.Dispose();
```
Once a Subscription is disposed of, you can not reuse it.
You can not unsubscribe from components of your subscription. For example, if you are subscribed to two markets you can not unsubscribe from one and leave the other open. To stop the stream you must dispose of it. 
