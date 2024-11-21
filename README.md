![.NET Core](https://github.com/KelvinVail/Betfair/workflows/.NET%20Core/badge.svg)
[![GitHub license](https://img.shields.io/github/license/mashape/apistatus.svg)](https://github.com/kelvinvail/Betfair/blob/master/LICENSE)
[![NuGet downloads](https://img.shields.io/nuget/v/Betfair.svg)](https://www.nuget.org/packages/Betfair/)

[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=KelvinVail_Betfair&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=KelvinVail_Betfair)
[![Bugs](https://sonarcloud.io/api/project_badges/measure?project=KelvinVail_Betfair&metric=bugs)](https://sonarcloud.io/summary/new_code?id=KelvinVail_Betfair)
[![Code Smells](https://sonarcloud.io/api/project_badges/measure?project=KelvinVail_Betfair&metric=code_smells)](https://sonarcloud.io/summary/new_code?id=KelvinVail_Betfair)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=KelvinVail_Betfair&metric=coverage)](https://sonarcloud.io/summary/new_code?id=KelvinVail_Betfair)
[![Duplicated Lines (%)](https://sonarcloud.io/api/project_badges/measure?project=KelvinVail_Betfair&metric=duplicated_lines_density)](https://sonarcloud.io/summary/new_code?id=KelvinVail_Betfair)

[![Mutation testing badge](https://img.shields.io/endpoint?style=flat&url=https%3A%2F%2Fbadge-api.stryker-mutator.io%2Fgithub.com%2FKelvinVail%2FBetfair%2Fmaster)](https://dashboard.stryker-mutator.io/reports/github.com/KelvinVail/Betfair/master)

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

## How to Subscribe to a Market Stream
Create a Subscription. Then use a MarketFilter to start a stream.  
[Full subscription documentation](/docs/Subscription.md).

```csharp
var credentials = new Credentials("USERNAME", "PASSWORD", "APP_KEY");
using var subscription = new Subscription(credentials);

await subscription.Subscribe(new StreamMarketFilter().WithMarketIds("MARKET_ID"));
await foreach (var change in subscription.ReadLines(default))
{
	// Handle changes
}
```

## How to List Today's Horse Races
```csharp
var credentials = new Credentials("USERNAME", "PASSWORD", "APP_KEY");

using var client = new BetfairApiClient(credentials);

var filter = new ApiMarketFilter()
    .WithMarketTypes(MarketType.Win)
    .WithCountries(Country.UnitedKingdom, Country.Ireland)
    .WithEventTypes(EventType.HorseRacing)
    .FromMarketStart(DateTimeOffset.UtcNow)
    .ToMarketStart(DateTimeOffset.UtcNow.AddDays(1));

var query = new MarketCatalogueQuery()
    .Include(MarketProjection.Event)
    .Include(MarketProjection.MarketStartTime)
    .Include(MarketProjection.MarketDescription)
    .Include(MarketProjection.RunnerDescription)
    .OrderBy(MarketSort.FirstToStart)
    .Take(200);

var marketCatalogues = await client.MarketCatalogue(filter, query);
```
Or use the helper extension.
```csharp
var filter = new ApiMarketFilter()
    .TodaysCard();

var query = new MarketCatalogueQuery()
    .Include(MarketProjection.Event)
    .Include(MarketProjection.MarketStartTime)
    .Include(MarketProjection.MarketDescription)
    .Include(MarketProjection.RunnerDescription)
    .OrderBy(MarketSort.FirstToStart)
    .Take(200);

var marketCatalogues = await client.MarketCatalogue(filter, query);
```