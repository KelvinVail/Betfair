![.NET Core](https://github.com/KelvinVail/Betfair/workflows/.NET%20Core/badge.svg)
[![GitHub license](https://img.shields.io/github/license/mashape/apistatus.svg)](https://github.com/kelvinvail/Betfair/blob/master/LICENSE)


# Betfair
A dotnet core package for interacting with Betfair

## Betfair.Stream
[![NuGet downloads](https://img.shields.io/nuget/v/Betfair.svg)](https://www.nuget.org/packages/Betfair/)

## Installation

Available on [NuGet](https://www.nuget.org/packages/Betfair/)
```bash
dotnet add package Betfair
```
or
```powershell
PM> Install-Package Betfair
```

## Getting Started
Register and obtain an app key from the [Betfair Developer Program](https://developer.betfair.com/).

### Create a credential object

```csharp
var credentials = new Credential([USERNAME], [PASSWORD], [APPKEY]);
```
It is recommended by Betfair that you use a certificate for non-interactive bot login.
[Learn more here](https://docs.developer.betfair.com/display/1smk3cen4v3lu3yomq5qye0ni/Non-Interactive+%28bot%29+login).

To use a certificate to login:
```csharp
var cert = X509Certificate2.CreateFrom...;
var credentials = new Credential([USERNAME], [PASSWORD], [APPKEY], cert);
```

### Subscribe to a Market Stream
Create a stream client. Then use a MarketFilter and DataFilter to start a stream.

```csharp
using var streamClient = new StreamClient(credentials);
await streamClient.Authenticate();
await streamClient.Subscribe(
	new MarketFilter().WithMarketId("MARKET_ID"),
	new DataFilter().WithBestPrices());

await foreach (var change in streamClient.GetChanges())
{
	// Do stuff..
}
```
