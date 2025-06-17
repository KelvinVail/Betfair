using Betfair.Api.Betting.Endpoints.ListMarketCatalogue.Enums;

namespace Betfair.Api.Betting.Endpoints.ListMarketCatalogue.Requests;

/// <summary>
/// Query parameters for market catalogue requests.
/// </summary>
public class MarketCatalogueQuery
{
    private HashSet<MarketProjection>? _projection;

    /// <summary>
    /// Gets the type and amount of data returned about the market.
    /// </summary>
    [JsonPropertyName("marketProjection")]
    public IReadOnlyCollection<MarketProjection>? MarketProjection => _projection;

    /// <summary>
    /// Gets the order of the results.
    /// </summary>
    [JsonPropertyName("sort")]
    public MarketSort? Sort { get; private set; }

    /// <summary>
    /// Gets the limit on the total number of results returned, must be greater than 0 and less than or equal to 1000.
    /// </summary>
    [JsonPropertyName("maxResults")]
    public int MaxResults { get; private set; } = 1000;

    /// <summary>
    /// Includes the specified market projection in the query.
    /// </summary>
    /// <param name="projection">The market projection to include.</param>
    /// <returns>The current <see cref="MarketCatalogueQuery"/> instance.</returns>
    public MarketCatalogueQuery Include(MarketProjection projection)
    {
        _projection ??= new ();
        _projection.Add(projection);
        return this;
    }

    /// <summary>
    /// Sets the order of the results.
    /// </summary>
    /// <param name="order">The sort order to apply.</param>
    /// <returns>The current <see cref="MarketCatalogueQuery"/> instance.</returns>
    public MarketCatalogueQuery OrderBy(MarketSort order)
    {
        Sort = order;
        return this;
    }

    /// <summary>
    /// Sets the maximum number of results to return.
    /// </summary>
    /// <param name="value">The maximum number of results (must be between 1 and 1000).</param>
    /// <returns>The current <see cref="MarketCatalogueQuery"/> instance.</returns>
    public MarketCatalogueQuery Take(int value)
    {
        MaxResults = value;
        return this;
    }
}
