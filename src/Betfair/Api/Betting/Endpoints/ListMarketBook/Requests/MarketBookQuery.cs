using Betfair.Api.Betting.Endpoints.ListMarketBook.Enums;
using Betfair.Api.Betting.Endpoints.ListMarketBook.Responses;
using Betfair.Api.Betting.Enums;

namespace Betfair.Api.Betting.Endpoints.ListMarketBook.Requests;

/// <summary>
/// Fluent query builder for market book requests.
/// </summary>
public class MarketBookQuery
{
    private HashSet<string>? _customerStrategyRefs;
    private HashSet<string>? _betIds;

    /// <summary>
    /// Gets the price projection.
    /// </summary>
    public PriceProjection? PriceProjection { get; private set; }

    /// <summary>
    /// Gets the order projection.
    /// </summary>
    public OrderProjection? OrderProjection { get; private set; }

    /// <summary>
    /// Gets the match projection.
    /// </summary>
    public MatchProjection? MatchProjection { get; private set; }

    /// <summary>
    /// Gets a value indicating whether to include overall position.
    /// </summary>
    public bool? IncludeOverallPosition { get; private set; }

    /// <summary>
    /// Gets a value indicating whether to partition matched by strategy reference.
    /// </summary>
    public bool? PartitionMatchedByStrategyRef { get; private set; }

    /// <summary>
    /// Gets the customer strategy references filter.
    /// </summary>
    public IReadOnlyCollection<string>? CustomerStrategyRefs => _customerStrategyRefs;

    /// <summary>
    /// Gets the currency code.
    /// </summary>
    public string? CurrencyCode { get; private set; }

    /// <summary>
    /// Gets the locale.
    /// </summary>
    public string? Locale { get; private set; }

    /// <summary>
    /// Gets the matched since date filter.
    /// </summary>
    public DateTime? MatchedSinceDate { get; private set; }

    /// <summary>
    /// Gets the bet IDs filter.
    /// </summary>
    public IReadOnlyCollection<string>? BetIds => _betIds;

    /// <summary>
    /// Sets the price projection.
    /// </summary>
    /// <param name="priceProjection">The price projection to use.</param>
    /// <returns>This <see cref="MarketBookQuery"/>.</returns>
    public MarketBookQuery WithPriceProjection(PriceProjection priceProjection)
    {
        PriceProjection = priceProjection;
        return this;
    }

    /// <summary>
    /// Sets the order projection.
    /// </summary>
    /// <param name="orderProjection">The order projection to use.</param>
    /// <returns>This <see cref="MarketBookQuery"/>.</returns>
    public MarketBookQuery WithOrderProjection(OrderProjection orderProjection)
    {
        OrderProjection = orderProjection;
        return this;
    }

    /// <summary>
    /// Sets the match projection.
    /// </summary>
    /// <param name="matchProjection">The match projection to use.</param>
    /// <returns>This <see cref="MarketBookQuery"/>.</returns>
    public MarketBookQuery WithMatchProjection(MatchProjection matchProjection)
    {
        MatchProjection = matchProjection;
        return this;
    }

    /// <summary>
    /// Enables including overall position in the response.
    /// </summary>
    /// <returns>This <see cref="MarketBookQuery"/>.</returns>
    public MarketBookQuery IncludeOverallPositions()
    {
        IncludeOverallPosition = true;
        return this;
    }

    /// <summary>
    /// Enables partitioning matched amounts by strategy reference.
    /// </summary>
    /// <returns>This <see cref="MarketBookQuery"/>.</returns>
    public MarketBookQuery PartitionByStrategy()
    {
        PartitionMatchedByStrategyRef = true;
        return this;
    }

    /// <summary>
    /// Adds customer strategy references to the filter.
    /// </summary>
    /// <param name="customerStrategyRefs">The customer strategy references to add.</param>
    /// <returns>This <see cref="MarketBookQuery"/>.</returns>
    public MarketBookQuery WithCustomerStrategies(params string[] customerStrategyRefs)
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
    /// Sets the currency code for the response.
    /// </summary>
    /// <param name="currencyCode">The currency code to use.</param>
    /// <returns>This <see cref="MarketBookQuery"/>.</returns>
    public MarketBookQuery WithCurrency(string currencyCode)
    {
        CurrencyCode = currencyCode;
        return this;
    }

    /// <summary>
    /// Sets the locale for the response.
    /// </summary>
    /// <param name="locale">The locale to use.</param>
    /// <returns>This <see cref="MarketBookQuery"/>.</returns>
    public MarketBookQuery WithLocale(string locale)
    {
        Locale = locale;
        return this;
    }

    /// <summary>
    /// Sets the matched since date filter.
    /// </summary>
    /// <param name="matchedSince">The date to filter matches from.</param>
    /// <returns>This <see cref="MarketBookQuery"/>.</returns>
    public MarketBookQuery MatchedSince(DateTime matchedSince)
    {
        MatchedSinceDate = matchedSince;
        return this;
    }

    /// <summary>
    /// Adds bet IDs to the filter.
    /// </summary>
    /// <param name="betIds">The bet IDs to add.</param>
    /// <returns>This <see cref="MarketBookQuery"/>.</returns>
    public MarketBookQuery WithBets(params string[] betIds)
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
    /// Configures the query to include all orders.
    /// </summary>
    /// <returns>This <see cref="MarketBookQuery"/>.</returns>
    public MarketBookQuery IncludeAllOrders()
    {
        OrderProjection = Betting.Enums.OrderProjection.All;
        return this;
    }

    /// <summary>
    /// Configures the query to include only executable orders.
    /// </summary>
    /// <returns>This <see cref="MarketBookQuery"/>.</returns>
    public MarketBookQuery ExecutableOrdersOnly()
    {
        OrderProjection = Betting.Enums.OrderProjection.Executable;
        return this;
    }

    /// <summary>
    /// Configures the query to include only execution complete orders.
    /// </summary>
    /// <returns>This <see cref="MarketBookQuery"/>.</returns>
    public MarketBookQuery ExecutionCompleteOrdersOnly()
    {
        OrderProjection = Betting.Enums.OrderProjection.ExecutionComplete;
        return this;
    }

    /// <summary>
    /// Configures the query to not roll up matches.
    /// </summary>
    /// <returns>This <see cref="MarketBookQuery"/>.</returns>
    public MarketBookQuery NoMatchRollup()
    {
        MatchProjection = Enums.MatchProjection.NoRollup;
        return this;
    }

    /// <summary>
    /// Configures the query to roll up matches by price.
    /// </summary>
    /// <returns>This <see cref="MarketBookQuery"/>.</returns>
    public MarketBookQuery RollupByPrice()
    {
        MatchProjection = Enums.MatchProjection.RolledUpByPrice;
        return this;
    }

    /// <summary>
    /// Configures the query to roll up matches by average price.
    /// </summary>
    /// <returns>This <see cref="MarketBookQuery"/>.</returns>
    public MarketBookQuery RollupByAveragePrice()
    {
        MatchProjection = Enums.MatchProjection.RolledUpByAvgPrice;
        return this;
    }
}
