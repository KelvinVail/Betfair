using Betfair.Api.Requests;
using Betfair.Api.Requests.Account;
using Betfair.Api.Requests.Markets;
using Betfair.Api.Requests.Orders;
using Betfair.Api.Requests.Orders.Filters;
using Betfair.Api.Requests.Orders.Queries;
using Betfair.Api.Responses;
using Betfair.Api.Responses.Account;
using Betfair.Api.Responses.Markets;
using Betfair.Api.Responses.Orders;
using Betfair.Core.Client;
using Betfair.Core.Enums;
using Betfair.Core.Login;

namespace Betfair.Api;

public class BetfairApiClient : IDisposable
{
    private const string _betting = "https://api.betfair.com/exchange/betting/rest/v1.0";
    private const string _account = "https://api.betfair.com/exchange/account/rest/v1.0";
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

    /// <summary>
    /// Returns a list of Event Types (i.e. Sports) associated with the markets selected by the MarketFilter.
    /// </summary>
    /// <param name="filter">The filter to select desired markets. All markets that match the criteria in the filter are selected.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>An array of <see cref="EventTypeResult"/>.</returns>
    public virtual Task<EventTypeResult[]> EventTypes(
        ApiMarketFilter? filter = null,
        CancellationToken cancellationToken = default)
    {
        filter ??= new ApiMarketFilter();
        var request = new EventTypesRequest { Filter = filter };
        return _client.PostAsync<EventTypeResult[]>(new Uri($"{_betting}/listEventTypes/"), request, cancellationToken);
    }

    /// <summary>
    /// Returns a list of Events (i.e, Reading vs. Man United) associated with the markets selected by the MarketFilter.
    /// </summary>
    /// <param name="filter">The filter to select desired markets. All markets that match the criteria in the filter are selected.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>An array of <see cref="EventResult"/>.</returns>
    public virtual Task<EventResult[]> Events(
        ApiMarketFilter? filter = null,
        CancellationToken cancellationToken = default)
    {
        filter ??= new ApiMarketFilter();
        var request = new EventsRequest { Filter = filter };
        return _client.PostAsync<EventResult[]>(new Uri($"{_betting}/listEvents/"), request, cancellationToken);
    }

    /// <summary>
    /// Returns a list of Competitions (i.e., World Cup 2013) associated with the markets selected by the
    /// MarketFilter. Currently only Football markets have an associated competition.
    /// </summary>
    /// <param name="filter">The filter to select desired markets. All markets that match the criteria in the filter are selected.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>An array of <see cref="CompetitionResult"/>.</returns>
    public virtual Task<CompetitionResult[]> Competitions(
        ApiMarketFilter? filter = null,
        CancellationToken cancellationToken = default)
    {
        filter ??= new ApiMarketFilter();
        var request = new CompetitionsRequest { Filter = filter };
        return _client.PostAsync<CompetitionResult[]>(new Uri($"{_betting}/listCompetitions/"), request, cancellationToken);
    }

    /// <summary>
    /// Returns a list of Countries associated with the markets selected by the MarketFilter.
    /// </summary>
    /// <param name="filter">The filter to select desired markets. All markets that match the criteria in the filter are selected.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>An array of <see cref="CountryCodeResult"/>.</returns>
    public virtual Task<CountryCodeResult[]> Countries(
        ApiMarketFilter? filter = null,
        CancellationToken cancellationToken = default)
    {
        filter ??= new ApiMarketFilter();
        var request = new CountriesRequest { Filter = filter };
        return _client.PostAsync<CountryCodeResult[]>(new Uri($"{_betting}/listCountries/"), request, cancellationToken);
    }

    /// <summary>
    /// Returns a list of Market Types (i.e. MATCH_ODDS, NEXT_GOAL) associated with the markets selected by the MarketFilter.
    /// </summary>
    /// <param name="filter">The filter to select desired markets. All markets that match the criteria in the filter are selected.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>An array of <see cref="MarketTypeResult"/>.</returns>
    public virtual Task<MarketTypeResult[]> MarketTypes(
        ApiMarketFilter? filter = null,
        CancellationToken cancellationToken = default)
    {
        filter ??= new ApiMarketFilter();
        var request = new MarketTypesRequest { Filter = filter };
        return _client.PostAsync<MarketTypeResult[]>(new Uri($"{_betting}/listMarketTypes/"), request, cancellationToken);
    }

    /// <summary>
    /// Returns a list of Time Ranges in the granularity specified in the request (i.e. 3PM to 4PM, Aug 14th to Aug 15th) associated with the markets selected by the MarketFilter.
    /// </summary>
    /// <param name="filter">The filter to select desired markets. All markets that match the criteria in the filter are selected.</param>
    /// <param name="granularity">The granularity of time periods that correspond to markets selected by the market filter.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>An array of <see cref="TimeRangeResult"/>.</returns>
    public virtual Task<TimeRangeResult[]> TimeRanges(
        ApiMarketFilter? filter = null,
        TimeGranularity granularity = TimeGranularity.Days,
        CancellationToken cancellationToken = default)
    {
        filter ??= new ApiMarketFilter();
        var request = new TimeRangesRequest { Filter = filter, Granularity = granularity.ToString().ToUpperInvariant() };
        return _client.PostAsync<TimeRangeResult[]>(new Uri($"{_betting}/listTimeRanges/"), request, cancellationToken);
    }

    /// <summary>
    /// Returns a list of Venues (i.e. Cheltenham, Ascot) associated with the markets selected by the MarketFilter.
    /// </summary>
    /// <param name="filter">The filter to select desired markets. All markets that match the criteria in the filter are selected.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>An array of <see cref="VenueResult"/>.</returns>
    public virtual Task<VenueResult[]> Venues(
        ApiMarketFilter? filter = null,
        CancellationToken cancellationToken = default)
    {
        filter ??= new ApiMarketFilter();
        var request = new VenuesRequest { Filter = filter };
        return _client.PostAsync<VenueResult[]>(new Uri($"{_betting}/listVenues/"), request, cancellationToken);
    }

    /// <summary>
    /// Place new orders into market. LIMIT orders below the minimum bet size are allowed if
    /// there is an unmatched bet at the same price in the market. This operation is atomic in that
    /// all orders will be placed or none will be placed.
    /// </summary>
    /// <param name="placeOrders">The place orders request containing market id and instructions.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A <see cref="PlaceExecutionReport"/>.</returns>
    public virtual Task<PlaceExecutionReport> PlaceOrders(
        PlaceOrders placeOrders,
        CancellationToken cancellationToken = default) =>
        _client.PostAsync<PlaceExecutionReport>(new Uri($"{_betting}/placeOrders/"), placeOrders, cancellationToken);

    /// <summary>
    /// Update non-exposure changing fields.
    /// </summary>
    /// <param name="updateOrders">The update orders request containing market id and instructions.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>An <see cref="UpdateExecutionReport"/>.</returns>
    public virtual Task<UpdateExecutionReport> UpdateOrders(
        UpdateOrders updateOrders,
        CancellationToken cancellationToken = default) =>
        _client.PostAsync<UpdateExecutionReport>(new Uri($"{_betting}/updateOrders/"), updateOrders, cancellationToken);

    /// <summary>
    /// This operation is logically a bulk cancel followed by a bulk place. The cancel is completed first then
    /// the new orders are placed. The new orders will be placed atomically in that they will all be placed or none
    /// will be placed. In the case where the new orders cannot be placed the cancellations will not be rolled back.
    /// </summary>
    /// <param name="replaceOrders">The replace orders request containing market id and instructions.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A <see cref="ReplaceExecutionReport"/>.</returns>
    public virtual Task<ReplaceExecutionReport> ReplaceOrders(
        ReplaceOrders replaceOrders,
        CancellationToken cancellationToken = default) =>
        _client.PostAsync<ReplaceExecutionReport>(new Uri($"{_betting}/replaceOrders/"), replaceOrders, cancellationToken);

    /// <summary>
    /// Cancel all bets OR cancel all bets on a market OR fully or partially cancel particular bets on a market.
    /// </summary>
    /// <param name="cancelOrders">The cancel orders request.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A <see cref="CancelExecutionReport"/>.</returns>
    public virtual Task<CancelExecutionReport> CancelOrders(
        CancelOrders cancelOrders,
        CancellationToken cancellationToken = default) =>
        _client.PostAsync<CancelExecutionReport>(new Uri($"{_betting}/cancelOrders/"), cancelOrders, cancellationToken);

    /// <summary>
    /// Returns a list of your current orders.
    /// Optionally you can filter and sort your current orders using the various parameters,
    /// setting none of the parameters will return all of your current orders up to a maximum of 1000 bets,
    /// ordered BY_BET and sorted EARLIEST_TO_LATEST. To retrieve more than 1000 orders,
    /// you need to make use of the from and take parameters.
    /// </summary>
    /// <param name="filter">The filter to select and sort desired orders. All orders that match the criteria in the filter are selected.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A <see cref="CurrentOrderSummaryReport"/>.</returns>
    public virtual Task<CurrentOrderSummaryReport> CurrentOrders(
        ApiOrderFilter? filter = null,
        CancellationToken cancellationToken = default)
    {
        filter ??= new ApiOrderFilter();
        var request = new CurrentOrdersRequest
        {
            BetIds = filter.BetIds?.ToList(),
            MarketIds = filter.MarketIds?.ToList(),
            OrderProjection = filter.OrderProjection,
            CustomerOrderRefs = filter.CustomerOrderRefs?.ToList(),
            CustomerStrategyRefs = filter.CustomerStrategyRefs?.ToList(),
            DateRange = filter.DateRange,
            OrderBy = filter.OrderByValue,
            SortDir = filter.SortDir,
            FromRecord = filter.FromRecord,
            RecordCount = filter.RecordCount,
        };

        return _client.PostAsync<CurrentOrderSummaryReport>(new Uri($"{_betting}/listCurrentOrders/"), request, cancellationToken);
    }

    /// <summary>
    /// Returns a list of settled bets based on the specified query.
    /// </summary>
    /// <param name="query">The query parameters for filtering cleared orders.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A <see cref="ClearedOrderSummaryReport"/>.</returns>
    public virtual Task<ClearedOrderSummaryReport> ClearedOrders(
        ClearedOrdersQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        var request = new ClearedOrdersRequest
        {
            BetStatus = query.BetStatus.ToString().ToUpperInvariant(),
            EventTypeIds = query.EventTypeIds?.ToList(),
            EventIds = query.EventIds?.ToList(),
            MarketIds = query.MarketIds?.ToList(),
            RunnerIds = query.RunnerIds?.ToList(),
            BetIds = query.BetIds?.ToList(),
            CustomerOrderRefs = query.CustomerOrderRefs?.ToList(),
            CustomerStrategyRefs = query.CustomerStrategyRefs?.ToList(),
            Side = query.Side?.ToString().ToUpperInvariant(),
            SettledDateRange = query.SettledDateRange,
            GroupBy = query.GroupByOption?.ToString().ToUpperInvariant(),
            IncludeItemDescription = query.IncludeItemDescription,
            Locale = query.Locale,
            FromRecord = query.FromRecord,
            RecordCount = query.RecordCount,
        };
        return _client.PostAsync<ClearedOrderSummaryReport>(new Uri($"{_betting}/listClearedOrders/"), request, cancellationToken);
    }

    /// <summary>
    /// Returns a list of information about markets that does not change (or changes very rarely). You use
    /// listMarketCatalogue to retrieve the name of the market, the names of selections and other information about
    /// markets.
    /// </summary>
    /// <param name="filter">The filter to select desired markets. All markets that match the criteria in the filter are selected.</param>
    /// <param name="query">The type and amount of data returned about the market.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>An array of <see cref="MarketCatalogue"/>.</returns>
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
    /// <param name="cancellationToken">Cancellation Token.</param>
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

    /// <summary>
    /// Returns a list of dynamic data about markets. Dynamic data includes prices, the status of the
    /// market, the status of selections, the traded volume, and the status of any orders you have placed in the
    /// market.
    /// </summary>
    /// <param name="marketIds">One or more market IDs.</param>
    /// <param name="query">The query parameters for the market book request.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>An array of <see cref="MarketBook"/>.</returns>
    public virtual Task<MarketBook[]> MarketBook(
        IEnumerable<string> marketIds,
        MarketBookQuery? query = null,
        CancellationToken cancellationToken = default)
    {
        query ??= new MarketBookQuery();
        var request = new MarketBookRequest
        {
            MarketIds = marketIds.ToList(),
            PriceProjection = query.PriceProjection,
            OrderProjection = query.OrderProjection?.ToString().ToUpperInvariant(),
            MatchProjection = query.MatchProjection?.ToString().ToUpperInvariant(),
            IncludeOverallPosition = query.IncludeOverallPosition,
            PartitionMatchedByStrategyRef = query.PartitionMatchedByStrategyRef,
            CustomerStrategyRefs = query.CustomerStrategyRefs?.ToList(),
            CurrencyCode = query.CurrencyCode,
            Locale = query.Locale,
            MatchedSince = query.MatchedSinceDate,
            BetIds = query.BetIds?.ToList(),
        };
        return _client.PostAsync<MarketBook[]>(new Uri($"{_betting}/listMarketBook/"), request, cancellationToken);
    }

    /// <summary>
    /// Returns a list of dynamic data about a market and a specified runner. Dynamic data includes prices, the status of the
    /// market, the status of selections, the traded volume, and the status of any orders you have placed in the
    /// market.
    /// </summary>
    /// <param name="marketId">The market ID.</param>
    /// <param name="selectionId">The selection ID.</param>
    /// <param name="handicap">The handicap associated with the runner in case of Asian handicap markets.</param>
    /// <param name="query">The query parameters for the runner book request.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A <see cref="MarketBook"/>.</returns>
    public virtual async Task<MarketBook?> RunnerBook(
        string marketId,
        long selectionId,
        double? handicap = null,
        MarketBookQuery? query = null,
        CancellationToken cancellationToken = default)
    {
        query ??= new MarketBookQuery();
        var request = new RunnerBookRequest
        {
            MarketId = marketId,
            SelectionId = selectionId,
            Handicap = handicap,
            PriceProjection = query.PriceProjection,
            OrderProjection = query.OrderProjection?.ToString().ToUpperInvariant(),
            MatchProjection = query.MatchProjection?.ToString().ToUpperInvariant(),
            IncludeOverallPosition = query.IncludeOverallPosition,
            PartitionMatchedByStrategyRef = query.PartitionMatchedByStrategyRef,
            CustomerStrategyRefs = query.CustomerStrategyRefs?.ToList(),
            CurrencyCode = query.CurrencyCode,
            Locale = query.Locale,
            MatchedSince = query.MatchedSinceDate,
            BetIds = query.BetIds?.ToList(),
        };
        var response = await _client.PostAsync<MarketBook[]>(new Uri($"{_betting}/listRunnerBook/"), request, cancellationToken);
        return response?.FirstOrDefault();
    }

    /// <summary>
    /// Returns the status of a market.
    /// </summary>
    /// <param name="marketId">The market ID to get the status for.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The market status as a string.</returns>
    public virtual async Task<string> MarketStatus(
        string marketId,
        CancellationToken cancellationToken)
    {
        var response = await _client.PostAsync<MarketBook[]>(
            new Uri($"{_betting}/listMarketBook/"),
            new MarketBookRequest { MarketIds = [marketId] },
            cancellationToken);

        return response?.FirstOrDefault()?.Status ?? "NONE";
    }

    // Account API Endpoints

    /// <summary>
    /// Get available to bet amount.
    /// </summary>
    /// <param name="wallet">Name of the wallet. Defaults to UK wallet if not specified.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>An <see cref="AccountFundsResponse"/>.</returns>
    public virtual Task<AccountFundsResponse> AccountFunds(
        Wallet wallet = Wallet.Uk,
        CancellationToken cancellationToken = default)
    {
        var request = new AccountFundsRequest { Wallet = wallet.ToString().ToUpperInvariant() };
        return _client.PostAsync<AccountFundsResponse>(new Uri($"{_account}/getAccountFunds/"), request, cancellationToken);
    }

    /// <summary>
    /// Get Account details.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>An <see cref="AccountDetailsResponse"/>.</returns>
    public virtual Task<AccountDetailsResponse> AccountDetails(
        CancellationToken cancellationToken = default)
    {
        var request = new AccountDetailsRequest();
        return _client.PostAsync<AccountDetailsResponse>(new Uri($"{_account}/getAccountDetails/"), request, cancellationToken);
    }

    /// <summary>
    /// Get account statement.
    /// </summary>
    /// <param name="query">The query parameters for the account statement request.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>An <see cref="AccountStatementReport"/>.</returns>
    public virtual Task<AccountStatementReport> AccountStatement(
        AccountStatementQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        var request = new AccountStatementRequest
        {
            Locale = query.Locale,
            FromRecord = query.FromRecord,
            RecordCount = query.RecordCount,
            ItemDateRange = query.ItemDateRange,
            IncludeItem = query.IncludeItem.ToString().ToUpperInvariant(),
            Wallet = query.Wallet.ToString().ToUpperInvariant(),
        };
        return _client.PostAsync<AccountStatementReport>(new Uri($"{_account}/getAccountStatement/"), request, cancellationToken);
    }

    /// <summary>
    /// Returns a list of currency rates based on given currency.
    /// </summary>
    /// <param name="fromCurrency">The currency from which the rates are computed. Please note that EUR is not a valid from currency.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>An array of <see cref="CurrencyRate"/>.</returns>
    public virtual Task<CurrencyRate[]> CurrencyRates(
        string? fromCurrency = null,
        CancellationToken cancellationToken = default)
    {
        var request = new CurrencyRatesRequest { FromCurrency = fromCurrency };
        return _client.PostAsync<CurrencyRate[]>(new Uri($"{_account}/listCurrencyRates/"), request, cancellationToken);
    }

    /// <summary>
    /// Transfer funds between wallets.
    /// </summary>
    /// <param name="from">Source wallet.</param>
    /// <param name="toWallet">Destination wallet.</param>
    /// <param name="amount">The amount to transfer.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A <see cref="TransferResponse"/>.</returns>
    public virtual Task<TransferResponse> TransferFunds(
        Wallet from,
        Wallet toWallet,
        double amount,
        CancellationToken cancellationToken = default)
    {
        var request = new TransferFundsRequest
        {
            From = from.ToString().ToUpperInvariant(),
            To = toWallet.ToString().ToUpperInvariant(),
            Amount = amount,
        };
        return _client.PostAsync<TransferResponse>(new Uri($"{_account}/transferFunds/"), request, cancellationToken);
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
