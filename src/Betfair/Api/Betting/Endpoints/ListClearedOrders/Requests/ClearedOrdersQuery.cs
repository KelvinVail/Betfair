using System.Globalization;
using Betfair.Api.Betting.Endpoints.ListClearedOrders.Enums;
using Betfair.Api.Betting.Enums;

namespace Betfair.Api.Betting.Endpoints.ListClearedOrders.Requests;

/// <summary>
/// Fluent query builder for cleared orders requests.
/// </summary>
public class ClearedOrdersQuery
{
    private HashSet<string>? _eventTypeIds;
    private HashSet<string>? _eventIds;
    private HashSet<string>? _marketIds;
    private HashSet<long>? _runnerIds;
    private HashSet<string>? _betIds;
    private HashSet<string>? _customerOrderRefs;
    private HashSet<string>? _customerStrategyRefs;

    /// <summary>
    /// Gets the bet status filter.
    /// </summary>
    public BetStatus BetStatus { get; private set; } = BetStatus.Settled;

    /// <summary>
    /// Gets the event type IDs filter.
    /// </summary>
    public IReadOnlyCollection<string>? EventTypeIds => _eventTypeIds;

    /// <summary>
    /// Gets the event IDs filter.
    /// </summary>
    public IReadOnlyCollection<string>? EventIds => _eventIds;

    /// <summary>
    /// Gets the market IDs filter.
    /// </summary>
    public IReadOnlyCollection<string>? MarketIds => _marketIds;

    /// <summary>
    /// Gets the runner IDs filter.
    /// </summary>
    public IReadOnlyCollection<long>? RunnerIds => _runnerIds;

    /// <summary>
    /// Gets the bet IDs filter.
    /// </summary>
    public IReadOnlyCollection<string>? BetIds => _betIds;

    /// <summary>
    /// Gets the customer order references filter.
    /// </summary>
    public IReadOnlyCollection<string>? CustomerOrderRefs => _customerOrderRefs;

    /// <summary>
    /// Gets the customer strategy references filter.
    /// </summary>
    public IReadOnlyCollection<string>? CustomerStrategyRefs => _customerStrategyRefs;

    /// <summary>
    /// Gets the side filter.
    /// </summary>
    public Side? Side { get; private set; }

    /// <summary>
    /// Gets the settled date range filter.
    /// </summary>
    public DateRange? SettledDateRange { get; private set; }

    /// <summary>
    /// Gets the group by option.
    /// </summary>
    public GroupBy? GroupByOption { get; private set; }

    /// <summary>
    /// Gets a value indicating whether to include item descriptions.
    /// </summary>
    public bool IncludeItemDescription { get; private set; }

    /// <summary>
    /// Gets the locale.
    /// </summary>
    public string? Locale { get; private set; }

    /// <summary>
    /// Gets the starting record index.
    /// </summary>
    public int FromRecord { get; private set; }

    /// <summary>
    /// Gets the number of records to return.
    /// </summary>
    public int RecordCount { get; private set; } = 1000;

    /// <summary>
    /// Sets the bet status filter.
    /// </summary>
    /// <param name="betStatus">The bet status to filter by.</param>
    /// <returns>This <see cref="ClearedOrdersQuery"/>.</returns>
    public ClearedOrdersQuery WithBetStatus(BetStatus betStatus)
    {
        BetStatus = betStatus;
        return this;
    }

    /// <summary>
    /// Adds event type IDs to the filter.
    /// </summary>
    /// <param name="eventTypeIds">The event type IDs to add.</param>
    /// <returns>This <see cref="ClearedOrdersQuery"/>.</returns>
    public ClearedOrdersQuery WithEventTypes(params string[] eventTypeIds)
    {
        if (eventTypeIds?.Length > 0)
        {
            _eventTypeIds ??= new HashSet<string>();
            foreach (var id in eventTypeIds.Where(x => !string.IsNullOrWhiteSpace(x)))
            {
                _eventTypeIds.Add(id);
            }
        }

        return this;
    }

    /// <summary>
    /// Adds event IDs to the filter.
    /// </summary>
    /// <param name="eventIds">The event IDs to add.</param>
    /// <returns>This <see cref="ClearedOrdersQuery"/>.</returns>
    public ClearedOrdersQuery WithEvents(params string[] eventIds)
    {
        if (eventIds?.Length > 0)
        {
            _eventIds ??= new HashSet<string>();
            foreach (var id in eventIds.Where(x => !string.IsNullOrWhiteSpace(x)))
            {
                _eventIds.Add(id);
            }
        }

        return this;
    }

    /// <summary>
    /// Adds market IDs to the filter.
    /// </summary>
    /// <param name="marketIds">The market IDs to add.</param>
    /// <returns>This <see cref="ClearedOrdersQuery"/>.</returns>
    public ClearedOrdersQuery WithMarkets(params string[] marketIds)
    {
        if (marketIds?.Length > 0)
        {
            _marketIds ??= new HashSet<string>();
            foreach (var id in marketIds.Where(x => !string.IsNullOrWhiteSpace(x)))
            {
                _marketIds.Add(id);
            }
        }

        return this;
    }

    /// <summary>
    /// Adds runner IDs to the filter.
    /// </summary>
    /// <param name="runnerIds">The runner IDs to add.</param>
    /// <returns>This <see cref="ClearedOrdersQuery"/>.</returns>
    public ClearedOrdersQuery WithRunners(params long[] runnerIds)
    {
        if (runnerIds?.Length > 0)
        {
            _runnerIds ??= new HashSet<long>();
            foreach (var id in runnerIds)
            {
                _runnerIds.Add(id);
            }
        }

        return this;
    }

    /// <summary>
    /// Adds bet IDs to the filter.
    /// </summary>
    /// <param name="betIds">The bet IDs to add.</param>
    /// <returns>This <see cref="ClearedOrdersQuery"/>.</returns>
    public ClearedOrdersQuery WithBets(params string[] betIds)
    {
        if (betIds?.Length > 0)
        {
            _betIds ??= new HashSet<string>();
            foreach (var id in betIds.Where(x => !string.IsNullOrWhiteSpace(x)))
            {
                _betIds.Add(id);
            }
        }

        return this;
    }

    /// <summary>
    /// Adds customer order references to the filter.
    /// </summary>
    /// <param name="customerOrderRefs">The customer order references to add.</param>
    /// <returns>This <see cref="ClearedOrdersQuery"/>.</returns>
    public ClearedOrdersQuery WithCustomerOrderRefs(params string[] customerOrderRefs)
    {
        if (customerOrderRefs?.Length > 0)
        {
            _customerOrderRefs ??= new HashSet<string>();
            foreach (var orderRef in customerOrderRefs.Where(x => !string.IsNullOrWhiteSpace(x)))
            {
                _customerOrderRefs.Add(orderRef);
            }
        }

        return this;
    }

    /// <summary>
    /// Adds customer strategy references to the filter.
    /// </summary>
    /// <param name="customerStrategyRefs">The customer strategy references to add.</param>
    /// <returns>This <see cref="ClearedOrdersQuery"/>.</returns>
    public ClearedOrdersQuery WithCustomerStrategyRefs(params string[] customerStrategyRefs)
    {
        if (customerStrategyRefs?.Length > 0)
        {
            _customerStrategyRefs ??= new HashSet<string>();
            foreach (var strategyRef in customerStrategyRefs.Where(x => !string.IsNullOrWhiteSpace(x)))
            {
                _customerStrategyRefs.Add(strategyRef);
            }
        }

        return this;
    }

    /// <summary>
    /// Sets the side filter.
    /// </summary>
    /// <param name="side">The side to filter by.</param>
    /// <returns>This <see cref="ClearedOrdersQuery"/>.</returns>
    public ClearedOrdersQuery WithSide(Side side)
    {
        Side = side;
        return this;
    }

    /// <summary>
    /// Sets the settled date range filter.
    /// </summary>
    /// <param name="from">The start date.</param>
    /// <param name="to">The end date.</param>
    /// <returns>This <see cref="ClearedOrdersQuery"/>.</returns>
    public ClearedOrdersQuery WithSettledDateRange(DateTimeOffset from, DateTimeOffset to)
    {
        SettledDateRange = new DateRange
        {
            From = from.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture),
            To = to.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture),
        };
        return this;
    }

    /// <summary>
    /// Sets the group by option.
    /// </summary>
    /// <param name="groupBy">The group by option.</param>
    /// <returns>This <see cref="ClearedOrdersQuery"/>.</returns>
    public ClearedOrdersQuery GroupBy(GroupBy groupBy)
    {
        GroupByOption = groupBy;
        return this;
    }

    /// <summary>
    /// Enables including item descriptions in the response.
    /// </summary>
    /// <returns>This <see cref="ClearedOrdersQuery"/>.</returns>
    public ClearedOrdersQuery IncludeItemDescriptions()
    {
        IncludeItemDescription = true;
        return this;
    }

    /// <summary>
    /// Sets the locale for the response.
    /// </summary>
    /// <param name="locale">The locale to use.</param>
    /// <returns>This <see cref="ClearedOrdersQuery"/>.</returns>
    public ClearedOrdersQuery WithLocale(string locale)
    {
        Locale = locale;
        return this;
    }

    /// <summary>
    /// Sets the starting record index for pagination.
    /// </summary>
    /// <param name="fromRecord">The starting record index.</param>
    /// <returns>This <see cref="ClearedOrdersQuery"/>.</returns>
    public ClearedOrdersQuery From(int fromRecord)
    {
        FromRecord = Math.Max(0, fromRecord);
        return this;
    }

    /// <summary>
    /// Sets the number of records to return.
    /// </summary>
    /// <param name="recordCount">The number of records to return.</param>
    /// <returns>This <see cref="ClearedOrdersQuery"/>.</returns>
    public ClearedOrdersQuery Take(int recordCount)
    {
        RecordCount = Math.Max(1, Math.Min(1000, recordCount));
        return this;
    }

    /// <summary>
    /// Filters to only back bets.
    /// </summary>
    /// <returns>This <see cref="ClearedOrdersQuery"/>.</returns>
    public ClearedOrdersQuery BackBetsOnly()
    {
        Side = Betting.Enums.Side.Back;
        return this;
    }

    /// <summary>
    /// Filters to only lay bets.
    /// </summary>
    /// <returns>This <see cref="ClearedOrdersQuery"/>.</returns>
    public ClearedOrdersQuery LayBetsOnly()
    {
        Side = Betting.Enums.Side.Lay;
        return this;
    }

    /// <summary>
    /// Filters to settled bets only.
    /// </summary>
    /// <returns>This <see cref="ClearedOrdersQuery"/>.</returns>
    public ClearedOrdersQuery SettledOnly()
    {
        BetStatus = BetStatus.Settled;
        return this;
    }

    /// <summary>
    /// Filters to cancelled bets only.
    /// </summary>
    /// <returns>This <see cref="ClearedOrdersQuery"/>.</returns>
    public ClearedOrdersQuery CancelledOnly()
    {
        BetStatus = BetStatus.Cancelled;
        return this;
    }

    /// <summary>
    /// Filters to bets settled in the last week.
    /// </summary>
    /// <returns>This <see cref="ClearedOrdersQuery"/>.</returns>
    public ClearedOrdersQuery LastWeek()
    {
        var now = DateTimeOffset.UtcNow;
        return WithSettledDateRange(now.AddDays(-7), now);
    }

    /// <summary>
    /// Filters to bets settled in the last month.
    /// </summary>
    /// <returns>This <see cref="ClearedOrdersQuery"/>.</returns>
    public ClearedOrdersQuery LastMonth()
    {
        var now = DateTimeOffset.UtcNow;
        return WithSettledDateRange(now.AddDays(-30), now);
    }

    /// <summary>
    /// Filters to bets settled today.
    /// </summary>
    /// <returns>This <see cref="ClearedOrdersQuery"/>.</returns>
    public ClearedOrdersQuery Today()
    {
        var today = DateTimeOffset.UtcNow.Date;
        return WithSettledDateRange(today, today.AddDays(1));
    }

    /// <summary>
    /// Filters to bets settled yesterday.
    /// </summary>
    /// <returns>This <see cref="ClearedOrdersQuery"/>.</returns>
    public ClearedOrdersQuery Yesterday()
    {
        var yesterday = DateTimeOffset.UtcNow.Date.AddDays(-1);
        return WithSettledDateRange(yesterday, yesterday.AddDays(1));
    }
}
