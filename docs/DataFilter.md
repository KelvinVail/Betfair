# DataFilter
Use the DataFilter object to restrict data received in a Betfair stream. 

## Chaining data filters
The DataFilter class can be used to build up a list of data required in your Betfair stream.

The following example will return market definitions and best available prices in the Betfair stream.
```csharp
var dataFilter = new DataFilter().WithMarketDefinition().WithBestPrices();
```

## Available Stream Data
### Market Definitions
Include the definition of all markets you subscribe to in the initial image. You will then receive updates to any fields on each market definition when they change.
```csharp
var dataFilter = new DataFilter().WithMarketDefinition();
```

### Best Available Prices
Include the best available back on lay prices. The number of ladder levels to receive can be set using ```.WithLadderLevels()```, if undefined the default is 3 ladder levels.