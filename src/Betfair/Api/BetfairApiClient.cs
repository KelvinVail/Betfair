using Betfair.Api.Requests;
using Betfair.Api.Requests.Markets;
using Betfair.Api.Requests.Orders;
using Betfair.Api.Responses;
using Betfair.Api.Responses.Markets;
using Betfair.Api.Responses.Orders;
using Betfair.Core.Client;
using Betfair.Core.Login;

namespace Betfair.Api;

public class BetfairApiClient : IDisposable
{
    private const string _betting = "https://api.betfair.com/exchange/betting/rest/v1.0";
    private readonly BetfairHttpClient _httpClient;
    private readonly HttpAdapter _client;
    private readonly bool _disposeHttpClient = true;
    private bool _disposedValue;

    [ExcludeFromCodeCoverage]
    public BetfairApiClient(Credentials credentials)
    {
        ArgumentNullException.ThrowIfNull(credentials);
        _httpClient = new BetfairHttpClient(credentials.Certificate);
        var tokenProvider = new TokenProvider(_httpClient, credentials);
        _client = BetfairHttpFactory.Create(credentials, tokenProvider, _httpClient);
    }

    internal BetfairApiClient(HttpAdapter adapter)
    {
        _disposeHttpClient = false;
        _httpClient = null!;
        _client = adapter;
    }

    public virtual Task<PlaceExecutionReport> PlaceOrders(
        PlaceOrders placeOrders,
        CancellationToken cancellationToken = default) =>
        _client.PostAsync<PlaceExecutionReport>(new Uri($"{_betting}/placeOrders/"), placeOrders, cancellationToken);

    public virtual Task<UpdateExecutionReport> UpdateOrders(
        UpdateOrders updateOrders,
        CancellationToken cancellationToken = default) =>
        _client.PostAsync<UpdateExecutionReport>(new Uri($"{_betting}/updateOrders/"), updateOrders, cancellationToken);

    public virtual Task<ReplaceExecutionReport> ReplaceOrders(
        ReplaceOrders replaceOrders,
        CancellationToken cancellationToken = default) =>
        _client.PostAsync<ReplaceExecutionReport>(new Uri($"{_betting}/replaceOrders/"), replaceOrders, cancellationToken);

    public virtual Task<CancelExecutionReport> CancelOrders(
        CancelOrders cancelOrders,
        CancellationToken cancellationToken = default) =>
        _client.PostAsync<CancelExecutionReport>(new Uri($"{_betting}/cancelOrders/"), cancelOrders, cancellationToken);

    public virtual async Task<MarketCatalogue[]> MarketCatalogue(
        ApiMarketFilter? filter = null,
        MarketCatalogueQuery? query = null,
        CancellationToken cancellationToken = default)
    {
        filter ??= new ApiMarketFilter();
        query ??= new MarketCatalogueQuery();
        var request = new MarketCatalogueRequest
        {
            Filter = filter,
            MarketProjection = query.MarketProjection?.ToList(),
            Sort = query.Sort,
            MaxResults = query.MaxResults,
        };

        return await _client.PostAsync<MarketCatalogue[]>(
            new Uri($"{_betting}/listMarketCatalogue/"), request, cancellationToken);
    }

    /// <summary>
    /// Retrieve profit and loss for a given list of OPEN markets.
    /// The values are calculated using matched bets and optionally settled bets.
    /// Only odds (MarketBettingType = ODDS) markets are implemented, markets of other types are silently ignored.
    /// To retrieve your profit and loss for CLOSED markets, please use the ClearedOrders request.
    /// </summary>
    /// <param name="marketIds">List of markets to calculate profit and loss.</param>
    /// <param name="includeSettledBets">Option to include settled bets (partially settled markets only). Defaults to false if not specified.</param>
    /// <param name="includeBspBets">Option to include BSP bets. Defaults to false if not specified.</param>
    /// <param name="netOfCommission">Option to return profit and loss net of users current commission rate for this market including any special tariffs. Defaults to false if not specified.</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <returns>A list of <see cref="MarketProfitAndLoss"/>.</returns>
    public virtual async Task<IEnumerable<MarketProfitAndLoss>> MarketProfitAndLoss(
        List<string> marketIds,
        bool includeSettledBets = false,
        bool includeBspBets = false,
        bool netOfCommission = false,
        CancellationToken cancellationToken = default)
    {
        var request = new MarketProfitAndLossRequest
        {
            MarketIds = marketIds,
            IncludeSettledBets = includeSettledBets,
            IncludeBspBets = includeBspBets,
            NetOfCommission = netOfCommission,
        };

        return await _client.PostAsync<IEnumerable<MarketProfitAndLoss>>(new Uri($"{_betting}/listMarketProfitAndLoss/"), request, cancellationToken);
    }

    public virtual async Task<string> MarketStatus(
        string marketId,
        CancellationToken cancellationToken)
    {
        var response = await _client.PostAsync<MarketStatus[]>(
            new Uri($"{_betting}/listMarketBook/"),
            new MarketBookRequest { MarketIds = new List<string> { marketId } },
            cancellationToken);

        return response?.FirstOrDefault()?.Status ?? "NONE";
    }

    [ExcludeFromCodeCoverage]
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    [ExcludeFromCodeCoverage]
    protected virtual void Dispose(bool disposing)
    {
        if (_disposedValue) return;
        if (disposing && _disposeHttpClient) _httpClient.Dispose();

        _disposedValue = true;
    }
}
