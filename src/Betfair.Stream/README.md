# Betfair.Stream

## Example use
```csharp
using var client = new StreamClient();
await client.Authenticate("YOUR_APP_KEY", "SESSION_TOKEN");
await client.Subscribe(
	new MarketFilter().WithMarketId("MARKET_ID"),
	new DataFilter().WithBestPrices());

await foreach (var change in client.GetChanges())
{
	// Do stuff..
}
```

