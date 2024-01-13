# DataFilter
Use the DataFilter object to restrict data received in a Betfair stream. 

## Contents
- [DataFilter](#datafilter)
  - [Contents](#contents)
  - [Chaining data filters](#chaining-data-filters)
  - [Available Stream Data](#available-stream-data)
    - [Market Definitions](#market-definitions)
    - [Best Available Prices](#best-available-prices)
    - [Best Available Virtual Prices](#best-available-virtual-prices)
    - [Full Offers Ladder](#full-offers-ladder)
    - [Full Traded Ladder](#full-traded-ladder)
    - [Traded Volume](#traded-volume)
    - [Last Traded Price](#last-traded-price)
    - [Starting Price Ladder](#starting-price-ladder)
    - [Starting Price Projection](#starting-price-projection)
    - [Ladder Levels](#ladder-levels)

## Chaining data filters
The DataFilter class can be used to build up a list of data required in your Betfair stream.

The following example will return market definitions and best available prices in the Betfair stream.
```csharp
var dataFilter = new DataFilter().WithMarketDefinition().WithBestPrices();
```

## Available Stream Data
### Market Definitions
Includes the definition of all markets you subscribe to in the initial image. You will then receive updates to any fields on each market definition when they change.
```csharp
var dataFilter = new DataFilter().WithMarketDefinition();
```

### Best Available Prices
Includes the best available back and lay prices not including virtual prices. The number of ladder levels to receive can be set using ```.WithLadderLevels()```,
if undefined the default is 3 ladder levels.
```csharp
var dataFilter = new DataFilter().WithBestPrices();
```

### Best Available Virtual Prices
Includes the best available back and lay prices including virtual prices. The number of ladder levels to receive can be set using ```.WithLadderLevels()```,
if undefined the default is 3 ladder levels.  
The virtual price stream is updated ~150 m/s after non-virtual prices.
```csharp
var dataFilter = new DataFilter().WithBestPricesIncludingVirtual();
```

### Full Offers Ladder
Includes the full available to BACK/LAY ladder.
```csharp
var dataFilter = new DataFilter().WithFullOffersLadder();
```

### Full Traded Ladder
Includes the full traded ladder.  This is the amount traded at any price on any selection in the market.
```csharp
var dataFilter = new DataFilter().WithFullTradedLadder();
```

### Traded Volume
Includes market and runner level traded volume.
```csharp
var dataFilter = new DataFilter().WithTradedVolume();
```

### Last Traded Price
Includes the "Last Price Matched" on a selection.
```csharp
var dataFilter = new DataFilter().WithLastTradedPrice();
```

### Starting Price Ladder
Include the starting price ladder.
```csharp
var dataFilter = new DataFilter().WithStartingPriceLadder();
```

### Starting Price Projection
Includes starting price projection prices.
```csharp
var dataFilter = new DataFilter().WithStartingPriceProjection();
```

### Ladder Levels
For depth-based ladders the number of levels to send (1 to 10). 1 is best price to back or lay etc.
```csharp
var dataFilter = new DataFilter().WithLadderLevels();
```