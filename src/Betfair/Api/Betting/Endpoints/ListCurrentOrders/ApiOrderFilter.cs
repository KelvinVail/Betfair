using Betfair.Api.Betting.Endpoints.ListCurrentOrders.Enums;
using Betfair.Api.Betting.Enums;

namespace Betfair.Api.Betting.Endpoints.ListCurrentOrders;

/// <summary>
/// Filter for current orders API requests.
/// </summary>
public class ApiOrderFilter
{
    internal HashSet<string>? BetIds { get; private set; }

    internal HashSet<string>? MarketIds { get; private set; }

    internal OrderProjection? OrderProjection { get; private set; }

    internal HashSet<string>? CustomerOrderRefs { get; private set; }

    internal HashSet<string>? CustomerStrategyRefs { get; private set; }

    internal DateRange? DateRange { get; private set; }

    internal OrderBy? OrderByValue { get; private set; }

    internal SortDir? SortDir { get; private set; }

    internal int FromRecord { get; private set; } = 0;

    internal int RecordCount { get; private set; } = 1000;

    /// <summary>
    /// Optionally restricts the results to the specified bet IDs.
    /// A maximum of 250 betId's, or a combination of 250 betId's & marketId's are permitted.
    /// </summary>
    /// <param name="betIds">The bet IDs to include.</param>
    /// <returns>This <see cref="ApiOrderFilter"/>.</returns>
    public ApiOrderFilter WithBetIds(params string[] betIds)
    {
        if (betIds is null) return this;

        BetIds ??= [];
        foreach (var betId in betIds.Where(x => x is not null))
            BetIds.Add(betId);

        return this;
    }

    /// <summary>
    /// Optionally restricts the results to the specified market IDs.
    /// A maximum of 250 marketId's, or a combination of 250 marketId's & betId's are permitted.
    /// </summary>
    /// <param name="marketIds">The market IDs to include.</param>
    /// <returns>This <see cref="ApiOrderFilter"/>.</returns>
    public ApiOrderFilter WithMarketIds(params string[] marketIds)
    {
        if (marketIds is null) return this;

        MarketIds ??= [];
        foreach (var marketId in marketIds.Where(x => x is not null))
            MarketIds.Add(marketId);

        return this;
    }

    /// <summary>
    /// Optionally restricts the results to the specified order status.
    /// </summary>
    /// <param name="orderStatus">The order projection.</param>
    /// <returns>This <see cref="ApiOrderFilter"/>.</returns>
    public ApiOrderFilter WithOrderProjection(OrderProjection orderStatus)
    {
        OrderProjection = orderStatus;
        return this;
    }

    /// <summary>
    /// Optionally restricts the results to the specified customer order references.
    /// </summary>
    /// <param name="customerOrderRefs">The customer order references to include.</param>
    /// <returns>This <see cref="ApiOrderFilter"/>.</returns>
    public ApiOrderFilter WithCustomerOrderRefs(params string[] customerOrderRefs)
    {
        if (customerOrderRefs is null) return this;

        CustomerOrderRefs ??= [];
        foreach (var customerOrderRef in customerOrderRefs.Where(x => x is not null))
            CustomerOrderRefs.Add(customerOrderRef);

        return this;
    }

    /// <summary>
    /// Optionally restricts the results to the specified customer strategy references.
    /// </summary>
    /// <param name="customerStrategyRefs">The customer strategy references to include.</param>
    /// <returns>This <see cref="ApiOrderFilter"/>.</returns>
    public ApiOrderFilter WithCustomerStrategyRefs(params string[] customerStrategyRefs)
    {
        if (customerStrategyRefs is null) return this;

        CustomerStrategyRefs ??= [];
        foreach (var customerStrategyRef in customerStrategyRefs.Where(x => x is not null))
            CustomerStrategyRefs.Add(customerStrategyRef);

        return this;
    }

    /// <summary>
    /// Optionally restricts the results to be from/to the specified date,
    /// these dates are contextual to the orders being returned
    /// and therefore the dates used to filter on will change to placed, matched, voided or settled dates depending on the orderBy.
    /// This date is inclusive, i.e. if an order was placed on exactly this date (to the millisecond) then it will be included in the results.
    /// If the from date is later than the to date, no results will be returned.
    /// </summary>
    /// <param name="from">Include orders from this date.</param>
    /// <param name="to">Include orders to this date.</param>
    /// <returns>This <see cref="ApiOrderFilter"/>.</returns>
    public ApiOrderFilter WithDateRange(DateTimeOffset from, DateTimeOffset to)
    {
        DateRange ??= new DateRange();
        DateRange.From = from.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ", DateTimeFormatInfo.InvariantInfo);
        DateRange.To = to.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ", DateTimeFormatInfo.InvariantInfo);

        return this;
    }

    /// <summary>
    /// Specifies how the results will be ordered.
    /// If no value is passed in, it defaults to BY_BET.
    /// Also acts as a filter such that only orders with a valid value in the field being ordered by will be returned
    /// (i.e. BY_VOID_TIME returns only voided orders,
    /// BY_SETTLED_TIME (applies to partially settled markets) returns only settled orders
    /// and BY_MATCH_TIME returns only orders with a matched date (voided, settled, matched orders)).
    /// Note that specifying an orderBy parameter defines the context of the date filter applied by the dateRange parameter
    /// (placed, matched, voided or settled date).
    /// </summary>
    /// <param name="orderBy">The order by criteria.</param>
    /// <returns>This <see cref="ApiOrderFilter"/>.</returns>
    public ApiOrderFilter OrderBy(OrderBy orderBy)
    {
        OrderByValue = orderBy;
        return this;
    }

    /// <summary>
    /// Specifies the direction the results will be sorted in. If no value is passed in, it defaults to EARLIEST_TO_LATEST.
    /// </summary>
    /// <param name="sortDir">The sort direction.</param>
    /// <returns>This <see cref="ApiOrderFilter"/>.</returns>
    public ApiOrderFilter SortBy(SortDir sortDir)
    {
        SortDir = sortDir;
        return this;
    }

    /// <summary>
    /// Specifies the first record that will be returned.
    /// Records start at index zero, not at index one.
    /// </summary>
    /// <param name="fromRecord">The starting record number.</param>
    /// <returns>This <see cref="ApiOrderFilter"/>.</returns>
    public ApiOrderFilter From(int fromRecord)
    {
        FromRecord = fromRecord;
        return this;
    }

    /// <summary>
    /// Specifies how many records will be returned from the index position 'fromRecord'.
    /// Note that there is a page size limit of 1000.
    /// A value of zero indicates that you would like all records (including and from 'fromRecord') up to the limit.
    /// </summary>
    /// <param name="recordCount">The number of records to return.</param>
    /// <returns>This <see cref="ApiOrderFilter"/>.</returns>
    public ApiOrderFilter Take(int recordCount)
    {
        RecordCount = recordCount;
        return this;
    }

    /// <summary>
    /// Filters to only executable orders.
    /// </summary>
    /// <returns>This <see cref="ApiOrderFilter"/>.</returns>
    public ApiOrderFilter ExecutableOnly()
    {
        OrderProjection = Betting.Enums.OrderProjection.Executable;
        return this;
    }

    /// <summary>
    /// Filters to only execution complete orders.
    /// </summary>
    /// <returns>This <see cref="ApiOrderFilter"/>.</returns>
    public ApiOrderFilter ExecutionCompleteOnly()
    {
        OrderProjection = Betting.Enums.OrderProjection.ExecutionComplete;
        return this;
    }

    /// <summary>
    /// Orders results by most recent first.
    /// </summary>
    /// <returns>This <see cref="ApiOrderFilter"/>.</returns>
    public ApiOrderFilter MostRecentFirst()
    {
        SortBy(Enums.SortDir.LatestToEarliest);
        return this;
    }

    /// <summary>
    /// Orders results by oldest first.
    /// </summary>
    /// <returns>This <see cref="ApiOrderFilter"/>.</returns>
    public ApiOrderFilter OldestFirst()
    {
        SortBy(Enums.SortDir.EarliestToLatest);
        return this;
    }
}
