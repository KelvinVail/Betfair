![.NET Core](https://github.com/KelvinVail/Betfair/workflows/.NET%20Core/badge.svg)
[![GitHub license](https://img.shields.io/github/license/mashape/apistatus.svg)](https://github.com/kelvinvail/Betfair/blob/master/LICENSE)
[![NuGet downloads](https://img.shields.io/nuget/v/Betfair.svg)](https://www.nuget.org/packages/Betfair/)

[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=KelvinVail_Betfair&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=KelvinVail_Betfair)
[![Bugs](https://sonarcloud.io/api/project_badges/measure?project=KelvinVail_Betfair&metric=bugs)](https://sonarcloud.io/summary/new_code?id=KelvinVail_Betfair)
[![Code Smells](https://sonarcloud.io/api/project_badges/measure?project=KelvinVail_Betfair&metric=code_smells)](https://sonarcloud.io/summary/new_code?id=KelvinVail_Betfair)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=KelvinVail_Betfair&metric=coverage)](https://sonarcloud.io/summary/new_code?id=KelvinVail_Betfair)
[![Duplicated Lines (%)](https://sonarcloud.io/api/project_badges/measure?project=KelvinVail_Betfair&metric=duplicated_lines_density)](https://sonarcloud.io/summary/new_code?id=KelvinVail_Betfair)

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
await foreach (var change in subscription.ReadLines(default))
{
	// Handle changes
}
```