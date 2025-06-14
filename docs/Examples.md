# Examples

Comprehensive examples demonstrating common use cases with the Betfair library.

## Contents
- [Getting Started](#getting-started)
- [Market Discovery](#market-discovery)
- [Live Price Monitoring](#live-price-monitoring)
- [Automated Betting](#automated-betting)
- [Portfolio Management](#portfolio-management)
- [Data Analysis](#data-analysis)
- [Advanced Scenarios](#advanced-scenarios)

## Getting Started

### Simple Market Lookup

```csharp
using Betfair.Api;
using Betfair.Api.Requests;
using Betfair.Core.Login;
using Betfair.Core.Enums;

class Program
{
    static async Task Main(string[] args)
    {
        var credentials = new Credentials("username", "password", "appkey");
        using var client = new BetfairApiClient(credentials);

        // Find today's Premier League matches
        var filter = new ApiMarketFilter()
            .WithEventTypes(EventType.Soccer)
            .WithMarketTypes(MarketType.MatchOdds)
            .FromMarketStart(DateTimeOffset.UtcNow)
            .ToMarketStart(DateTimeOffset.UtcNow.AddDays(1));

        var query = new MarketCatalogueQuery()
            .Include(MarketProjection.Event)
            .Include(MarketProjection.MarketStartTime)
            .Include(MarketProjection.RunnerDescription)
            .OrderBy(MarketSort.FirstToStart)
            .Take(10);

        var markets = await client.MarketCatalogue(filter, query);

        foreach (var market in markets)
        {
            Console.WriteLine($"{market.Event?.Name} - {market.MarketName}");
            Console.WriteLine($"Start: {market.MarketStartTime:HH:mm}");
            
            foreach (var runner in market.Runners ?? [])
            {
                Console.WriteLine($"  {runner.RunnerName}");
            }
            Console.WriteLine();
        }
    }
}
```

### Basic Stream Subscription

```csharp
using Betfair.Stream;
using Betfair.Stream.Messages;

class StreamExample
{
    static async Task Main(string[] args)
    {
        var credentials = new Credentials("username", "password", "appkey");
        using var subscription = new Subscription(credentials);

        // Subscribe to a specific market
        var marketFilter = new StreamMarketFilter()
            .WithMarketIds("1.123456789");
        
        var dataFilter = new DataFilter()
            .WithBestPrices()
            .WithTradedVolume();

        await subscription.Subscribe(marketFilter, dataFilter);

        // Process changes
        await foreach (var change in subscription.ReadLines(default))
        {
            if (change.Operation == "mcm" && change.MarketChanges != null)
            {
                foreach (var marketChange in change.MarketChanges)
                {
                    Console.WriteLine($"Market {marketChange.Id}: £{marketChange.TotalMatched:F2} matched");
                    
                    foreach (var runnerChange in marketChange.RunnerChanges ?? [])
                    {
                        if (runnerChange.Ltp.HasValue)
                        {
                            Console.WriteLine($"  Runner {runnerChange.Id}: LTP {runnerChange.Ltp}");
                        }
                    }
                }
            }
        }
    }
}
```

## Market Discovery

### Horse Racing Scanner

```csharp
class HorseRacingScanner
{
    private readonly BetfairApiClient _client;

    public HorseRacingScanner(BetfairApiClient client)
    {
        _client = client;
    }

    public async Task<List<RaceInfo>> GetTodaysRaces()
    {
        var filter = new ApiMarketFilter()
            .WithEventTypes(EventType.HorseRacing)
            .WithCountries(Country.UnitedKingdom, Country.Ireland)
            .WithMarketTypes(MarketType.Win)
            .FromMarketStart(DateTimeOffset.UtcNow)
            .ToMarketStart(DateTimeOffset.UtcNow.AddDays(1));

        var query = new MarketCatalogueQuery()
            .Include(MarketProjection.Event)
            .Include(MarketProjection.MarketStartTime)
            .Include(MarketProjection.MarketDescription)
            .Include(MarketProjection.RunnerDescription)
            .OrderBy(MarketSort.FirstToStart)
            .Take(200);

        var markets = await _client.MarketCatalogue(filter, query);

        return markets.Select(m => new RaceInfo
        {
            MarketId = m.MarketId!,
            RaceName = m.Event?.Name ?? "Unknown",
            Venue = m.Event?.Venue ?? "Unknown",
            StartTime = m.MarketStartTime,
            RunnerCount = m.Runners?.Count ?? 0,
            Runners = m.Runners?.Select(r => new RunnerInfo
            {
                SelectionId = r.SelectionId,
                Name = r.RunnerName ?? "Unknown",
                Handicap = r.Handicap
            }).ToList() ?? new List<RunnerInfo>()
        }).ToList();
    }
}

public class RaceInfo
{
    public string MarketId { get; set; } = "";
    public string RaceName { get; set; } = "";
    public string Venue { get; set; } = "";
    public DateTimeOffset StartTime { get; set; }
    public int RunnerCount { get; set; }
    public List<RunnerInfo> Runners { get; set; } = new();
}

public class RunnerInfo
{
    public long SelectionId { get; set; }
    public string Name { get; set; } = "";
    public double? Handicap { get; set; }
}
```

### Football Match Finder

```csharp
class FootballMatchFinder
{
    private readonly BetfairApiClient _client;

    public FootballMatchFinder(BetfairApiClient client)
    {
        _client = client;
    }

    public async Task<List<MatchInfo>> GetTodaysMatches(string competition = "")
    {
        var filter = new ApiMarketFilter()
            .WithEventTypes(EventType.Soccer)
            .WithMarketTypes(MarketType.MatchOdds)
            .FromMarketStart(DateTimeOffset.UtcNow)
            .ToMarketStart(DateTimeOffset.UtcNow.AddDays(1));

        var query = new MarketCatalogueQuery()
            .Include(MarketProjection.Event)
            .Include(MarketProjection.Competition)
            .Include(MarketProjection.MarketStartTime)
            .Include(MarketProjection.RunnerDescription)
            .OrderBy(MarketSort.FirstToStart)
            .Take(100);

        var markets = await _client.MarketCatalogue(filter, query);

        var matches = markets
            .Where(m => string.IsNullOrEmpty(competition) || 
                       m.Competition?.Name?.Contains(competition, StringComparison.OrdinalIgnoreCase) == true)
            .Select(m => new MatchInfo
            {
                MarketId = m.MarketId!,
                HomeTeam = m.Runners?.ElementAtOrDefault(0)?.RunnerName ?? "Unknown",
                AwayTeam = m.Runners?.ElementAtOrDefault(1)?.RunnerName ?? "Unknown",
                Competition = m.Competition?.Name ?? "Unknown",
                StartTime = m.MarketStartTime,
                HomeSelectionId = m.Runners?.ElementAtOrDefault(0)?.SelectionId ?? 0,
                AwaySelectionId = m.Runners?.ElementAtOrDefault(1)?.SelectionId ?? 0,
                DrawSelectionId = m.Runners?.ElementAtOrDefault(2)?.SelectionId ?? 0
            }).ToList();

        return matches;
    }
}

public class MatchInfo
{
    public string MarketId { get; set; } = "";
    public string HomeTeam { get; set; } = "";
    public string AwayTeam { get; set; } = "";
    public string Competition { get; set; } = "";
    public DateTimeOffset StartTime { get; set; }
    public long HomeSelectionId { get; set; }
    public long AwaySelectionId { get; set; }
    public long DrawSelectionId { get; set; }
}
```

## Live Price Monitoring

### Price Alert System

```csharp
class PriceAlertSystem
{
    private readonly BetfairApiClient _client;
    private readonly Dictionary<string, PriceAlert> _alerts = new();

    public PriceAlertSystem(BetfairApiClient client)
    {
        _client = client;
    }

    public void AddAlert(string marketId, long selectionId, double targetPrice, bool isBack)
    {
        var key = $"{marketId}_{selectionId}_{(isBack ? "back" : "lay")}";
        _alerts[key] = new PriceAlert
        {
            MarketId = marketId,
            SelectionId = selectionId,
            TargetPrice = targetPrice,
            IsBack = isBack,
            Created = DateTimeOffset.UtcNow
        };
    }

    public async Task MonitorPrices(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested && _alerts.Any())
        {
            var marketIds = _alerts.Values.Select(a => a.MarketId).Distinct().ToArray();
            
            var query = new MarketBookQuery()
                .WithPriceProjection(PriceProjectionBuilder.Create()
                    .WithBestPrices()
                    .WithBestPricesDepth(1));

            var marketBooks = await _client.MarketBook(marketIds, query);

            foreach (var book in marketBooks)
            {
                CheckAlertsForMarket(book);
            }

            await Task.Delay(2000, cancellationToken);
        }
    }

    private void CheckAlertsForMarket(MarketBook book)
    {
        foreach (var runner in book.Runners ?? [])
        {
            var backPrice = runner.ExchangePrices?.AvailableToBack?.FirstOrDefault()?.Price;
            var layPrice = runner.ExchangePrices?.AvailableToLay?.FirstOrDefault()?.Price;

            CheckAlert($"{book.MarketId}_{runner.SelectionId}_back", backPrice);
            CheckAlert($"{book.MarketId}_{runner.SelectionId}_lay", layPrice);
        }
    }

    private void CheckAlert(string key, double? currentPrice)
    {
        if (!_alerts.TryGetValue(key, out var alert) || !currentPrice.HasValue)
            return;

        bool triggered = alert.IsBack 
            ? currentPrice >= alert.TargetPrice
            : currentPrice <= alert.TargetPrice;

        if (triggered)
        {
            Console.WriteLine($"ALERT: {alert.MarketId} Selection {alert.SelectionId} " +
                            $"{(alert.IsBack ? "Back" : "Lay")} price reached {currentPrice}");
            
            _alerts.Remove(key);
        }
    }
}

public class PriceAlert
{
    public string MarketId { get; set; } = "";
    public long SelectionId { get; set; }
    public double TargetPrice { get; set; }
    public bool IsBack { get; set; }
    public DateTimeOffset Created { get; set; }
}
```

### Market Depth Analyzer

```csharp
class MarketDepthAnalyzer
{
    private readonly BetfairApiClient _client;

    public MarketDepthAnalyzer(BetfairApiClient client)
    {
        _client = client;
    }

    public async Task<MarketDepthInfo> AnalyzeMarketDepth(string marketId)
    {
        var query = new MarketBookQuery()
            .WithPriceProjection(PriceProjectionBuilder.Create()
                .WithBestPrices()
                .WithBestPricesDepth(10)
                .WithTradedVolume());

        var marketBooks = await _client.MarketBook(new[] { marketId }, query);
        var book = marketBooks.FirstOrDefault();

        if (book?.Runners == null)
            return new MarketDepthInfo { MarketId = marketId };

        var analysis = new MarketDepthInfo
        {
            MarketId = marketId,
            TotalMatched = book.TotalMatched,
            Status = book.Status ?? "UNKNOWN",
            RunnerAnalysis = new List<RunnerDepthInfo>()
        };

        foreach (var runner in book.Runners)
        {
            var runnerInfo = new RunnerDepthInfo
            {
                SelectionId = runner.SelectionId,
                LastTradedPrice = runner.LastPriceTraded,
                TotalMatched = runner.TotalMatched,
                BackDepth = CalculateDepth(runner.ExchangePrices?.AvailableToBack),
                LayDepth = CalculateDepth(runner.ExchangePrices?.AvailableToLay),
                TradedVolume = runner.ExchangePrices?.TradedVolume?.Sum(tv => tv.Size) ?? 0
            };

            analysis.RunnerAnalysis.Add(runnerInfo);
        }

        return analysis;
    }

    private double CalculateDepth(IEnumerable<PriceSize>? prices)
    {
        return prices?.Sum(p => p.Size) ?? 0;
    }
}

public class MarketDepthInfo
{
    public string MarketId { get; set; } = "";
    public double TotalMatched { get; set; }
    public string Status { get; set; } = "";
    public List<RunnerDepthInfo> RunnerAnalysis { get; set; } = new();
}

public class RunnerDepthInfo
{
    public long SelectionId { get; set; }
    public double? LastTradedPrice { get; set; }
    public double TotalMatched { get; set; }
    public double BackDepth { get; set; }
    public double LayDepth { get; set; }
    public double TradedVolume { get; set; }
}
```

## Automated Betting

### Simple Lay the Favourite Strategy

```csharp
class LayTheFavouriteStrategy
{
    private readonly BetfairApiClient _client;
    private readonly double _maxStake;
    private readonly double _minOdds;
    private readonly double _maxOdds;

    public LayTheFavouriteStrategy(BetfairApiClient client, double maxStake = 10.0, 
                                  double minOdds = 1.5, double maxOdds = 3.0)
    {
        _client = client;
        _maxStake = maxStake;
        _minOdds = minOdds;
        _maxOdds = maxOdds;
    }

    public async Task<bool> ExecuteStrategy(string marketId)
    {
        // Get market prices
        var query = new MarketBookQuery()
            .WithPriceProjection(PriceProjectionBuilder.Create()
                .WithBestPrices()
                .WithBestPricesDepth(1));

        var marketBooks = await _client.MarketBook(new[] { marketId }, query);
        var book = marketBooks.FirstOrDefault();

        if (book?.Status != "OPEN" || book.Runners == null)
            return false;

        // Find favourite (lowest back price)
        var favourite = book.Runners
            .Where(r => r.Status == "ACTIVE")
            .Where(r => r.ExchangePrices?.AvailableToBack?.Any() == true)
            .OrderBy(r => r.ExchangePrices!.AvailableToBack!.First().Price)
            .FirstOrDefault();

        if (favourite?.ExchangePrices?.AvailableToBack == null)
            return false;

        var bestBackPrice = favourite.ExchangePrices.AvailableToBack.First().Price;
        var bestLayPrice = favourite.ExchangePrices.AvailableToLay?.FirstOrDefault()?.Price;

        // Check if price is within our range
        if (bestBackPrice < _minOdds || bestBackPrice > _maxOdds || bestLayPrice == null)
            return false;

        // Calculate stake (liability = stake * (odds - 1))
        var liability = _maxStake * (bestLayPrice.Value - 1);
        
        // Check we have enough funds
        var funds = await _client.AccountFunds();
        if (funds.AvailableToBetBalance < liability)
            return false;

        // Place lay bet
        var placeOrders = new PlaceOrders(marketId);
        placeOrders.Instructions.Add(new PlaceInstruction
        {
            SelectionId = favourite.SelectionId,
            Side = Side.Lay,
            OrderType = OrderType.Limit,
            LimitOrder = new LimitOrder
            {
                Size = _maxStake,
                Price = bestLayPrice.Value,
                PersistenceType = PersistenceType.Lapse
            },
            CustomerOrderRef = $"lay-fav-{DateTimeOffset.UtcNow.Ticks}"
        });

        var result = await _client.PlaceOrders(placeOrders);
        
        if (result.Status == ExecutionReportStatus.Success)
        {
            Console.WriteLine($"Laid favourite at {bestLayPrice} for £{_maxStake}");
            return true;
        }

        Console.WriteLine($"Failed to place bet: {result.ErrorCode}");
        return false;
    }
}
```

### Arbitrage Detector

```csharp
class ArbitrageDetector
{
    private readonly BetfairApiClient _client;

    public ArbitrageDetector(BetfairApiClient client)
    {
        _client = client;
    }

    public async Task<List<ArbitrageOpportunity>> FindArbitrageOpportunities(IEnumerable<string> marketIds)
    {
        var opportunities = new List<ArbitrageOpportunity>();

        var query = new MarketBookQuery()
            .WithPriceProjection(PriceProjectionBuilder.Create()
                .WithBestPrices()
                .WithBestPricesDepth(3));

        var marketBooks = await _client.MarketBook(marketIds, query);

        foreach (var book in marketBooks)
        {
            if (book.Status != "OPEN" || book.Runners?.Count != 2)
                continue;

            var runner1 = book.Runners[0];
            var runner2 = book.Runners[1];

            var r1Back = runner1.ExchangePrices?.AvailableToBack?.FirstOrDefault()?.Price;
            var r1Lay = runner1.ExchangePrices?.AvailableToLay?.FirstOrDefault()?.Price;
            var r2Back = runner2.ExchangePrices?.AvailableToBack?.FirstOrDefault()?.Price;
            var r2Lay = runner2.ExchangePrices?.AvailableToLay?.FirstOrDefault()?.Price;

            if (r1Back.HasValue && r2Lay.HasValue)
            {
                var profit = CalculateArbitrageProfit(r1Back.Value, r2Lay.Value);
                if (profit > 0)
                {
                    opportunities.Add(new ArbitrageOpportunity
                    {
                        MarketId = book.MarketId!,
                        BackSelection = runner1.SelectionId,
                        BackPrice = r1Back.Value,
                        LaySelection = runner2.SelectionId,
                        LayPrice = r2Lay.Value,
                        ProfitPercentage = profit
                    });
                }
            }

            if (r2Back.HasValue && r1Lay.HasValue)
            {
                var profit = CalculateArbitrageProfit(r2Back.Value, r1Lay.Value);
                if (profit > 0)
                {
                    opportunities.Add(new ArbitrageOpportunity
                    {
                        MarketId = book.MarketId!,
                        BackSelection = runner2.SelectionId,
                        BackPrice = r2Back.Value,
                        LaySelection = runner1.SelectionId,
                        LayPrice = r1Lay.Value,
                        ProfitPercentage = profit
                    });
                }
            }
        }

        return opportunities.OrderByDescending(o => o.ProfitPercentage).ToList();
    }

    private double CalculateArbitrageProfit(double backPrice, double layPrice)
    {
        // Simplified arbitrage calculation
        var backImpliedProb = 1.0 / backPrice;
        var layImpliedProb = 1.0 / layPrice;
        var totalImpliedProb = backImpliedProb + layImpliedProb;
        
        return totalImpliedProb < 1.0 ? (1.0 - totalImpliedProb) * 100 : 0;
    }
}

public class ArbitrageOpportunity
{
    public string MarketId { get; set; } = "";
    public long BackSelection { get; set; }
    public double BackPrice { get; set; }
    public long LaySelection { get; set; }
    public double LayPrice { get; set; }
    public double ProfitPercentage { get; set; }
}
```

## Portfolio Management

### Position Tracker

```csharp
class PositionTracker
{
    private readonly BetfairApiClient _client;
    private readonly Dictionary<string, MarketPosition> _positions = new();

    public PositionTracker(BetfairApiClient client)
    {
        _client = client;
    }

    public async Task UpdatePositions()
    {
        // Get current orders
        var currentOrders = await _client.CurrentOrders();
        
        // Get cleared orders from last 7 days
        var clearedQuery = new ClearedOrdersQuery()
            .WithDateRange(DateTimeOffset.UtcNow.AddDays(-7), DateTimeOffset.UtcNow)
            .Take(1000);
        var clearedOrders = await _client.ClearedOrders(clearedQuery);

        _positions.Clear();

        // Process current orders
        foreach (var order in currentOrders.CurrentOrders ?? [])
        {
            if (order.MarketId == null) continue;
            
            if (!_positions.ContainsKey(order.MarketId))
                _positions[order.MarketId] = new MarketPosition { MarketId = order.MarketId };

            var position = _positions[order.MarketId];
            position.UnmatchedOrders.Add(new OrderInfo
            {
                BetId = order.BetId ?? "",
                SelectionId = order.SelectionId,
                Side = order.Side?.ToString() ?? "",
                Price = order.PriceSize?.Price ?? 0,
                Size = order.PriceSize?.Size ?? 0,
                SizeMatched = order.SizeMatched,
                SizeRemaining = order.SizeRemaining
            });
        }

        // Process cleared orders
        foreach (var order in clearedOrders.ClearedOrders ?? [])
        {
            if (order.MarketId == null) continue;
            
            if (!_positions.ContainsKey(order.MarketId))
                _positions[order.MarketId] = new MarketPosition { MarketId = order.MarketId };

            var position = _positions[order.MarketId];
            position.SettledOrders.Add(new SettledOrderInfo
            {
                BetId = order.BetId ?? "",
                SelectionId = order.SelectionId,
                Side = order.Side?.ToString() ?? "",
                Price = order.PriceSize?.Price ?? 0,
                Size = order.PriceSize?.Size ?? 0,
                Profit = order.Profit,
                Commission = order.Commission,
                SettledDate = order.SettledDate
            });

            position.TotalProfit += order.Profit;
            position.TotalCommission += order.Commission;
        }
    }

    public void PrintPositions()
    {
        foreach (var position in _positions.Values)
        {
            Console.WriteLine($"Market: {position.MarketId}");
            Console.WriteLine($"Total P&L: £{position.TotalProfit:F2}");
            Console.WriteLine($"Commission: £{position.TotalCommission:F2}");
            Console.WriteLine($"Unmatched Orders: {position.UnmatchedOrders.Count}");
            Console.WriteLine($"Settled Orders: {position.SettledOrders.Count}");
            Console.WriteLine();
        }
    }
}

public class MarketPosition
{
    public string MarketId { get; set; } = "";
    public List<OrderInfo> UnmatchedOrders { get; set; } = new();
    public List<SettledOrderInfo> SettledOrders { get; set; } = new();
    public double TotalProfit { get; set; }
    public double TotalCommission { get; set; }
}

public class OrderInfo
{
    public string BetId { get; set; } = "";
    public long SelectionId { get; set; }
    public string Side { get; set; } = "";
    public double Price { get; set; }
    public double Size { get; set; }
    public double SizeMatched { get; set; }
    public double SizeRemaining { get; set; }
}

public class SettledOrderInfo : OrderInfo
{
    public double Profit { get; set; }
    public double Commission { get; set; }
    public DateTime SettledDate { get; set; }
}
```

## Data Analysis

### Performance Analytics

```csharp
class PerformanceAnalytics
{
    private readonly BetfairApiClient _client;

    public PerformanceAnalytics(BetfairApiClient client)
    {
        _client = client;
    }

    public async Task<PerformanceReport> GenerateReport(int days = 30)
    {
        var query = new ClearedOrdersQuery()
            .WithDateRange(DateTimeOffset.UtcNow.AddDays(-days), DateTimeOffset.UtcNow)
            .SettledOnly()
            .Take(1000);

        var clearedOrders = await _client.ClearedOrders(query);
        var orders = clearedOrders.ClearedOrders ?? [];

        var report = new PerformanceReport
        {
            Period = $"Last {days} days",
            TotalBets = orders.Count,
            TotalStake = orders.Sum(o => o.PriceSize?.Size ?? 0),
            TotalProfit = orders.Sum(o => o.Profit),
            TotalCommission = orders.Sum(o => o.Commission),
            WinningBets = orders.Count(o => o.Profit > 0),
            LosingBets = orders.Count(o => o.Profit < 0),
            BreakEvenBets = orders.Count(o => o.Profit == 0)
        };

        report.WinRate = report.TotalBets > 0 ? (double)report.WinningBets / report.TotalBets * 100 : 0;
        report.AverageOdds = orders.Where(o => o.PriceSize?.Price > 0).Average(o => o.PriceSize!.Price);
        report.ROI = report.TotalStake > 0 ? report.TotalProfit / report.TotalStake * 100 : 0;

        // Group by event type
        report.EventTypeBreakdown = orders
            .Where(o => !string.IsNullOrEmpty(o.ItemDescription?.EventTypeId))
            .GroupBy(o => o.ItemDescription!.EventTypeId!)
            .Select(g => new EventTypePerformance
            {
                EventTypeId = g.Key,
                BetCount = g.Count(),
                TotalProfit = g.Sum(o => o.Profit),
                TotalStake = g.Sum(o => o.PriceSize?.Size ?? 0)
            })
            .OrderByDescending(e => e.TotalProfit)
            .ToList();

        return report;
    }
}

public class PerformanceReport
{
    public string Period { get; set; } = "";
    public int TotalBets { get; set; }
    public double TotalStake { get; set; }
    public double TotalProfit { get; set; }
    public double TotalCommission { get; set; }
    public int WinningBets { get; set; }
    public int LosingBets { get; set; }
    public int BreakEvenBets { get; set; }
    public double WinRate { get; set; }
    public double AverageOdds { get; set; }
    public double ROI { get; set; }
    public List<EventTypePerformance> EventTypeBreakdown { get; set; } = new();
}

public class EventTypePerformance
{
    public string EventTypeId { get; set; } = "";
    public int BetCount { get; set; }
    public double TotalProfit { get; set; }
    public double TotalStake { get; set; }
    public double ROI => TotalStake > 0 ? TotalProfit / TotalStake * 100 : 0;
}
```

### Market Efficiency Analyzer

```csharp
class MarketEfficiencyAnalyzer
{
    private readonly BetfairApiClient _client;

    public MarketEfficiencyAnalyzer(BetfairApiClient client)
    {
        _client = client;
    }

    public async Task<EfficiencyReport> AnalyzeMarketEfficiency(string marketId, TimeSpan duration)
    {
        var priceHistory = new List<PriceSnapshot>();
        var startTime = DateTimeOffset.UtcNow;

        while (DateTimeOffset.UtcNow - startTime < duration)
        {
            var query = new MarketBookQuery()
                .WithPriceProjection(PriceProjectionBuilder.Create()
                    .WithBestPrices()
                    .WithBestPricesDepth(1)
                    .WithTradedVolume());

            var marketBooks = await _client.MarketBook(new[] { marketId }, query);
            var book = marketBooks.FirstOrDefault();

            if (book?.Runners != null)
            {
                var snapshot = new PriceSnapshot
                {
                    Timestamp = DateTimeOffset.UtcNow,
                    TotalMatched = book.TotalMatched,
                    Runners = book.Runners.Select(r => new RunnerSnapshot
                    {
                        SelectionId = r.SelectionId,
                        BackPrice = r.ExchangePrices?.AvailableToBack?.FirstOrDefault()?.Price,
                        LayPrice = r.ExchangePrices?.AvailableToLay?.FirstOrDefault()?.Price,
                        LastTradedPrice = r.LastPriceTraded,
                        TotalMatched = r.TotalMatched
                    }).ToList()
                };

                priceHistory.Add(snapshot);
            }

            await Task.Delay(5000); // Sample every 5 seconds
        }

        return AnalyzePriceHistory(priceHistory);
    }

    private EfficiencyReport AnalyzePriceHistory(List<PriceSnapshot> history)
    {
        var report = new EfficiencyReport
        {
            SampleCount = history.Count,
            Duration = history.Count > 1 ? history.Last().Timestamp - history.First().Timestamp : TimeSpan.Zero
        };

        if (history.Count < 2) return report;

        // Calculate price volatility
        foreach (var runner in history.First().Runners)
        {
            var runnerPrices = history
                .SelectMany(h => h.Runners.Where(r => r.SelectionId == runner.SelectionId))
                .Where(r => r.BackPrice.HasValue)
                .Select(r => r.BackPrice!.Value)
                .ToList();

            if (runnerPrices.Count > 1)
            {
                var mean = runnerPrices.Average();
                var variance = runnerPrices.Sum(p => Math.Pow(p - mean, 2)) / runnerPrices.Count;
                var volatility = Math.Sqrt(variance) / mean * 100;

                report.RunnerVolatility[runner.SelectionId] = volatility;
            }
        }

        // Calculate spread analysis
        var spreads = history
            .SelectMany(h => h.Runners)
            .Where(r => r.BackPrice.HasValue && r.LayPrice.HasValue)
            .Select(r => (r.LayPrice!.Value - r.BackPrice!.Value) / r.BackPrice.Value * 100)
            .ToList();

        if (spreads.Any())
        {
            report.AverageSpread = spreads.Average();
            report.MinSpread = spreads.Min();
            report.MaxSpread = spreads.Max();
        }

        return report;
    }
}

public class EfficiencyReport
{
    public int SampleCount { get; set; }
    public TimeSpan Duration { get; set; }
    public Dictionary<long, double> RunnerVolatility { get; set; } = new();
    public double AverageSpread { get; set; }
    public double MinSpread { get; set; }
    public double MaxSpread { get; set; }
}

public class PriceSnapshot
{
    public DateTimeOffset Timestamp { get; set; }
    public double TotalMatched { get; set; }
    public List<RunnerSnapshot> Runners { get; set; } = new();
}

public class RunnerSnapshot
{
    public long SelectionId { get; set; }
    public double? BackPrice { get; set; }
    public double? LayPrice { get; set; }
    public double? LastTradedPrice { get; set; }
    public double TotalMatched { get; set; }
}
```

## Advanced Scenarios

### Multi-Market Strategy

```csharp
class MultiMarketStrategy
{
    private readonly BetfairApiClient _client;
    private readonly Dictionary<string, MarketState> _marketStates = new();

    public MultiMarketStrategy(BetfairApiClient client)
    {
        _client = client;
    }

    public async Task ExecuteStrategy(List<string> marketIds)
    {
        // Initialize market states
        foreach (var marketId in marketIds)
        {
            _marketStates[marketId] = new MarketState { MarketId = marketId };
        }

        var cancellationToken = new CancellationTokenSource(TimeSpan.FromHours(2)).Token;

        while (!cancellationToken.IsCancellationRequested)
        {
            await UpdateMarketStates(marketIds);
            await ExecuteTradingLogic();
            await Task.Delay(10000, cancellationToken); // Check every 10 seconds
        }
    }

    private async Task UpdateMarketStates(List<string> marketIds)
    {
        var query = new MarketBookQuery()
            .WithPriceProjection(PriceProjectionBuilder.Create()
                .WithBestPrices()
                .WithBestPricesDepth(3)
                .WithTradedVolume());

        var marketBooks = await _client.MarketBook(marketIds, query);

        foreach (var book in marketBooks)
        {
            if (book.MarketId != null && _marketStates.ContainsKey(book.MarketId))
            {
                var state = _marketStates[book.MarketId];
                state.LastUpdate = DateTimeOffset.UtcNow;
                state.Status = book.Status ?? "UNKNOWN";
                state.TotalMatched = book.TotalMatched;

                state.Runners.Clear();
                foreach (var runner in book.Runners ?? [])
                {
                    state.Runners[runner.SelectionId] = new RunnerState
                    {
                        SelectionId = runner.SelectionId,
                        Status = runner.Status ?? "UNKNOWN",
                        LastTradedPrice = runner.LastPriceTraded,
                        TotalMatched = runner.TotalMatched,
                        BestBackPrice = runner.ExchangePrices?.AvailableToBack?.FirstOrDefault()?.Price,
                        BestLayPrice = runner.ExchangePrices?.AvailableToLay?.FirstOrDefault()?.Price
                    };
                }
            }
        }
    }

    private async Task ExecuteTradingLogic()
    {
        // Example: Look for correlated movements across markets
        var activeMarkets = _marketStates.Values
            .Where(m => m.Status == "OPEN" && m.TotalMatched > 1000)
            .ToList();

        foreach (var market in activeMarkets)
        {
            // Check if favourite has drifted significantly
            var favourite = market.Runners.Values
                .Where(r => r.BestBackPrice.HasValue)
                .OrderBy(r => r.BestBackPrice)
                .FirstOrDefault();

            if (favourite?.BestBackPrice > 2.0 && favourite.TotalMatched > 500)
            {
                // Consider laying the favourite
                await ConsiderLayBet(market.MarketId, favourite.SelectionId, favourite.BestLayPrice ?? 0);
            }
        }
    }

    private async Task<bool> ConsiderLayBet(string marketId, long selectionId, double price)
    {
        if (price < 1.5 || price > 4.0) return false;

        var funds = await _client.AccountFunds();
        var maxStake = Math.Min(10.0, funds.AvailableToBetBalance / 10); // Risk 10% of balance

        if (maxStake < 2.0) return false;

        var placeOrders = new PlaceOrders(marketId);
        placeOrders.Instructions.Add(new PlaceInstruction
        {
            SelectionId = selectionId,
            Side = Side.Lay,
            OrderType = OrderType.Limit,
            LimitOrder = new LimitOrder
            {
                Size = maxStake,
                Price = price,
                PersistenceType = PersistenceType.Lapse
            },
            CustomerOrderRef = $"multi-market-{DateTimeOffset.UtcNow.Ticks}"
        });

        var result = await _client.PlaceOrders(placeOrders);
        return result.Status == ExecutionReportStatus.Success;
    }
}

public class MarketState
{
    public string MarketId { get; set; } = "";
    public DateTimeOffset LastUpdate { get; set; }
    public string Status { get; set; } = "";
    public double TotalMatched { get; set; }
    public Dictionary<long, RunnerState> Runners { get; set; } = new();
}

public class RunnerState
{
    public long SelectionId { get; set; }
    public string Status { get; set; } = "";
    public double? LastTradedPrice { get; set; }
    public double TotalMatched { get; set; }
    public double? BestBackPrice { get; set; }
    public double? BestLayPrice { get; set; }
}
```

### Risk Management System

```csharp
class RiskManagementSystem
{
    private readonly BetfairApiClient _client;
    private readonly RiskSettings _settings;

    public RiskManagementSystem(BetfairApiClient client, RiskSettings settings)
    {
        _client = client;
        _settings = settings;
    }

    public async Task<bool> ValidateBet(PlaceOrders placeOrders)
    {
        // Check account balance
        var funds = await _client.AccountFunds();
        if (funds.AvailableToBetBalance < _settings.MinimumBalance)
        {
            Console.WriteLine("Risk check failed: Insufficient balance");
            return false;
        }

        // Calculate total liability
        var totalLiability = 0.0;
        foreach (var instruction in placeOrders.Instructions)
        {
            if (instruction.Side == Side.Lay && instruction.LimitOrder != null)
            {
                totalLiability += instruction.LimitOrder.Size * (instruction.LimitOrder.Price - 1);
            }
            else if (instruction.Side == Side.Back && instruction.LimitOrder != null)
            {
                totalLiability += instruction.LimitOrder.Size;
            }
        }

        // Check single bet limit
        if (totalLiability > _settings.MaxSingleBetLiability)
        {
            Console.WriteLine($"Risk check failed: Single bet liability £{totalLiability:F2} exceeds limit £{_settings.MaxSingleBetLiability:F2}");
            return false;
        }

        // Check daily limit
        var todaysBets = await GetTodaysBets();
        var todaysLiability = todaysBets.Sum(b => CalculateLiability(b));

        if (todaysLiability + totalLiability > _settings.MaxDailyLiability)
        {
            Console.WriteLine($"Risk check failed: Daily liability would exceed limit");
            return false;
        }

        // Check market exposure
        var marketExposure = await CalculateMarketExposure(placeOrders.MarketId);
        if (marketExposure + totalLiability > _settings.MaxMarketExposure)
        {
            Console.WriteLine($"Risk check failed: Market exposure would exceed limit");
            return false;
        }

        return true;
    }

    private async Task<List<CurrentOrder>> GetTodaysBets()
    {
        var filter = new ApiOrderFilter()
            .WithDateRange(DateTime.Today, DateTime.Today.AddDays(1))
            .Take(1000);

        var orders = await _client.CurrentOrders(filter);
        return orders.CurrentOrders?.ToList() ?? new List<CurrentOrder>();
    }

    private double CalculateLiability(CurrentOrder order)
    {
        if (order.Side == "LAY" && order.PriceSize != null)
        {
            return order.PriceSize.Size * (order.PriceSize.Price - 1);
        }
        return order.PriceSize?.Size ?? 0;
    }

    private async Task<double> CalculateMarketExposure(string marketId)
    {
        var pnl = await _client.MarketProfitAndLoss(new List<string> { marketId });
        var market = pnl.FirstOrDefault();

        if (market?.ProfitAndLosses == null) return 0;

        return market.ProfitAndLosses
            .Select(p => Math.Min(p.IfWin ?? 0, p.IfLose ?? 0))
            .Where(loss => loss < 0)
            .Sum(loss => Math.Abs(loss));
    }
}

public class RiskSettings
{
    public double MinimumBalance { get; set; } = 50.0;
    public double MaxSingleBetLiability { get; set; } = 100.0;
    public double MaxDailyLiability { get; set; } = 500.0;
    public double MaxMarketExposure { get; set; } = 200.0;
    public double MaxExposurePercentage { get; set; } = 20.0; // % of balance
}
```
```
