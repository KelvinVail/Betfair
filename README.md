![.NET Core](https://github.com/KelvinVail/Betfair/workflows/.NET%20Core/badge.svg)
[![GitHub license](https://img.shields.io/github/license/mashape/apistatus.svg)](https://github.com/kelvinvail/Betfair/blob/master/LICENSE)
[![NuGet downloads](https://img.shields.io/nuget/v/Betfair.svg)](https://www.nuget.org/packages/Betfair/)

# Betfair
Fast and simple classes for interacting with the Betfair API and Stream.

[Full documentation](/docs/README.md).

## Installation

Available on [NuGet](https://www.nuget.org/packages/Betfair/)
```bash
dotnet add package Betfair
```
or
```powershell
PM> Install-Package Betfair
```

## Subscribe to a Market Stream
Create a Subscription. Then use a MarketFilter to start a stream.  
[Full subscription documentation](/docs/Subscription.md).

```csharp
var credentials = new Credential("USERNAME", "PASSWORD", "APP_KEY");
using var subscription = new Subscription(credentials);

await subscription.Subscribe(new StreamMarketFilter().WithMarketIds("MARKET_ID"))
await foreach (var change in subscription.ReadLines())
{
	// Handle changes
}
```