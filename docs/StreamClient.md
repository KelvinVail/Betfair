# StreamClient
The StreamClient is used to subscribe to Betfair market and order streams.

## Contents
- [StreamClient](#streamclient)
	- [Contents](#contents)
	- [Create a StreamClient](#create-a-streamclient)
	- [Subscribe to a Market Stream](#subscribe-to-a-market-stream)
	- [Subscribe to an Order Stream](#subscribe-to-an-order-stream)
	- [Consume Change Messages](#consume-change-messages)
		- [Back Pressure](#back-pressure)
		- [Consume the raw change messages](#consume-the-raw-change-messages)
	- [Unsubscribe from the stream](#unsubscribe-from-the-stream)

## Create a StreamClient
The StreamClient needs a [Credentials](/docs/Authentication.md) object to authenticate to Betfair.
```csharp
var credentials = new Credential([USERNAME], [PASSWORD], [APPKEY], [CERT]);
```

Create a StreamClient, wrapping it in a using block. Then authenticate the StreamClient. Authentication only needs to be performed once on StreamClient creation.
```csharp
using var streamClient = new StreamClient(credentials);
await streamClient.Authenticate();
```

To avoid a timeout you must subscribe to either a market or order stream within 15 seconds of authenticating the StreamClient.

## Subscribe to a Market Stream
To subscribe to a market stream we need to define which markets to subscribe to and what data we want returned in the stream. For more information on filters see [MarketFilter](/docs/MarketFilter.md) and [DataFilter](/docs/MarketFilter.md). In this example we are subscribing to a single market and requesting that only the best available prices are return in the data stream.

A recommended tip is to use the MarketFilter to subscribe to all markets you are interested in for the duration of your programs execution. This eliminates the need to close and recreate StreamClients for each new market you need. To improve speed, limit the DataFilter to only the data you need. 
```csharp
var marketFilter = new MarketFilter().WithMarketId("1.23456789");
var dataFilter = new DataFilter().WithBestPrices();
```

Use the filters to subscribe to a stream of market data.
```csharp
await streamClient.Subscribe(marketFilter, dataFilter);
```

## Subscribe to an Order Stream
To include updates to new and open orders in your stream, subscribe to the order stream.
```csharp
await streamClient.SubscribeToOrders();
```
The order stream can not be filtered, you will receive updates to all orders placed in your account.  
You will only receive updates to new orders or orders that are open at the time you subscribe to the order stream. Historical fully matched orders are not included in the order stream. If needed, you should use the BetfairApiClient to retrieve them. 

## Consume Change Messages
After you have subscribed to a market and/or an order stream you can iterate each [ChangeMessage](/docs/ChangeMessage.md) asynchronously as they become available on the stream. 
```csharp
await foreach (ChangeMessage change in streamClient.GetChanges())
{
	// Handle change..
}
```
The implementation of handling change messages is left to the reader. This is so that you can implement any strategy as you see fit as efficiently as you can. Without relying on an opinionated and bloated caching implementation. See the documentation on [ChangeMessage](/docs/ChangeMessage.md) for more information on the different messages you will receive.

### Back Pressure
The StreamClient takes changes off of the underlying TCP connection as quickly as it can, holding any messages in a cache that are unconsumed by you. Be aware of this if you plan to perform any long running operations between handling each message.

### Consume the raw change messages
The ```streamClient.GetChanges()``` method does have the very small overhead of deserializing a byte array into a class object. If you want to handle this yourself or need access to the raw byte arrays as they arrive on the stream, you can use the following method.
```csharp
await foreach (byte[] change in streamClient.ReadLines())
{
	// Handle change..
}
```
You will need to handle all connection issues and error messages if you use this method.  
Use a single method to take changes from the stream. Do Not combine ```GetChanges()``` with ```ReadLines()``` as they will compete to consume messages.

## Unsubscribe from the stream
To unsubscribe from the entire stream you must either close or dispose of the stream.
```csharp
streamClient.Close();
```
Once a StreamClient is closed you can not reuse it.  
You can not unsubscribe from components of your subscription. For example, if you are subscribed to two markets you can not unsubscribe from one and leave the other open. To stop the stream you must close it. 