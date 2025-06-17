using Betfair.Api.Accounts.Endpoints.GetAccountDetails.Requests;
using Betfair.Api.Accounts.Endpoints.GetAccountDetails.Responses;
using Betfair.Api.Accounts.Endpoints.GetAccountFunds.Requests;
using Betfair.Api.Accounts.Endpoints.GetAccountFunds.Responses;
using Betfair.Api.Accounts.Endpoints.GetAccountStatement.Requests;
using Betfair.Api.Accounts.Endpoints.GetAccountStatement.Responses;
using Betfair.Api.Accounts.Endpoints.ListCurrencyRates.Requests;
using Betfair.Api.Accounts.Endpoints.ListCurrencyRates.Responses;
using Betfair.Api.Betting;
using Betfair.Api.Betting.Endpoints.CancelOrders.Requests;
using Betfair.Api.Betting.Endpoints.CancelOrders.Responses;
using Betfair.Api.Betting.Endpoints.ListClearedOrders.Requests;
using Betfair.Api.Betting.Endpoints.ListClearedOrders.Responses;
using Betfair.Api.Betting.Endpoints.ListCompetitions.Requests;
using Betfair.Api.Betting.Endpoints.ListCompetitions.Responses;
using Betfair.Api.Betting.Endpoints.ListCountries.Requests;
using Betfair.Api.Betting.Endpoints.ListCountries.Responses;
using Betfair.Api.Betting.Endpoints.ListCurrentOrders.Requests;
using Betfair.Api.Betting.Endpoints.ListCurrentOrders.Responses;
using Betfair.Api.Betting.Endpoints.ListEvents.Requests;
using Betfair.Api.Betting.Endpoints.ListEvents.Responses;
using Betfair.Api.Betting.Endpoints.ListEventTypes.Requests;
using Betfair.Api.Betting.Endpoints.ListEventTypes.Responses;
using Betfair.Api.Betting.Endpoints.ListMarketBook.Requests;
using Betfair.Api.Betting.Endpoints.ListMarketBook.Responses;
using Betfair.Api.Betting.Endpoints.ListMarketCatalogue.Requests;
using Betfair.Api.Betting.Endpoints.ListMarketCatalogue.Responses;
using Betfair.Api.Betting.Endpoints.ListMarketProfitAndLoss.Requests;
using Betfair.Api.Betting.Endpoints.ListMarketProfitAndLoss.Responses;
using Betfair.Api.Betting.Endpoints.ListMarketTypes.Requests;
using Betfair.Api.Betting.Endpoints.ListMarketTypes.Responses;
using Betfair.Api.Betting.Endpoints.ListRunnerBook.Requests;
using Betfair.Api.Betting.Endpoints.ListTimeRanges.Requests;
using Betfair.Api.Betting.Endpoints.ListTimeRanges.Responses;
using Betfair.Api.Betting.Endpoints.ListVenues.Requests;
using Betfair.Api.Betting.Endpoints.ListVenues.Responses;
using Betfair.Api.Betting.Endpoints.PlaceOrders.Requests;
using Betfair.Api.Betting.Endpoints.PlaceOrders.Responses;
using Betfair.Api.Betting.Endpoints.ReplaceOrders.Requests;
using Betfair.Api.Betting.Endpoints.ReplaceOrders.Responses;
using Betfair.Api.Betting.Endpoints.UpdateOrders.Requests;
using Betfair.Api.Betting.Endpoints.UpdateOrders.Responses;
using Betfair.Core.Authentication;
using Betfair.Core.Client;
using Betfair.Stream.Messages;
using Betfair.Stream.Responses;
using RunnerResponse = Betfair.Api.Betting.Endpoints.ListMarketCatalogue.Responses.RunnerResponse;

namespace Betfair;

[JsonSourceGenerationOptions(GenerationMode = JsonSourceGenerationMode.Metadata, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull, PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]

// Login
[JsonSerializable(typeof(Authentication), GenerationMode = JsonSourceGenerationMode.Serialization)]
[JsonSerializable(typeof(LoginResponse))]

// Stream Messages
[JsonSerializable(typeof(DataFilter), GenerationMode = JsonSourceGenerationMode.Serialization)]
[JsonSerializable(typeof(MarketSubscription), GenerationMode = JsonSourceGenerationMode.Serialization)]
[JsonSerializable(typeof(StreamOrderFilter), GenerationMode = JsonSourceGenerationMode.Serialization)]
[JsonSerializable(typeof(OrderSubscription), GenerationMode = JsonSourceGenerationMode.Serialization)]
[JsonSerializable(typeof(StreamMarketFilter), GenerationMode = JsonSourceGenerationMode.Serialization)]
[JsonSerializable(typeof(MessageBase), GenerationMode = JsonSourceGenerationMode.Serialization)]

// Stream Responses
[JsonSerializable(typeof(ChangeMessage))]
[JsonSerializable(typeof(MarketChange))]
[JsonSerializable(typeof(MarketDefinition))]
[JsonSerializable(typeof(OrderChange))]
[JsonSerializable(typeof(OrderRunnerChange))]
[JsonSerializable(typeof(RunnerChange))]
[JsonSerializable(typeof(RunnerDefinition))]
[JsonSerializable(typeof(UnmatchedOrder))]

[JsonSerializable(typeof(ApiMarketFilter), GenerationMode = JsonSourceGenerationMode.Serialization)]
[JsonSerializable(typeof(DateRange), GenerationMode = JsonSourceGenerationMode.Serialization)]

// Market Catalogue Requests
[JsonSerializable(typeof(MarketCatalogueQuery), GenerationMode = JsonSourceGenerationMode.Serialization)]
[JsonSerializable(typeof(MarketCatalogueRequest), GenerationMode = JsonSourceGenerationMode.Serialization)]

// Market Catalogue Responses
[JsonSerializable(typeof(Competition))]
[JsonSerializable(typeof(LadderDescription))]
[JsonSerializable(typeof(MarketCatalogue))]
[JsonSerializable(typeof(MarketDescription))]
[JsonSerializable(typeof(MarketEvent))]
[JsonSerializable(typeof(RunnerResponse))]
[JsonSerializable(typeof(MarketCatalogue[]))]
[JsonSerializable(typeof(MarketCatalogue))]

[JsonSerializable(typeof(MarketBookRequest), GenerationMode = JsonSourceGenerationMode.Serialization)]

// Place Order Requests
[JsonSerializable(typeof(PlaceOrders))]
[JsonSerializable(typeof(PlaceInstruction))]
[JsonSerializable(typeof(LimitOrder))]

// Place Order Responses
[JsonSerializable(typeof(PlaceExecutionReport))]

// Update Order Requests
[JsonSerializable(typeof(UpdateOrders))]
[JsonSerializable(typeof(UpdateInstruction))]

// Update Order Responses
[JsonSerializable(typeof(UpdateExecutionReport))]

// Replace Order Responses
[JsonSerializable(typeof(ReplaceOrders))]
[JsonSerializable(typeof(ReplaceExecutionReport))]

// Cancel Order Responses
[JsonSerializable(typeof(CancelOrders))]
[JsonSerializable(typeof(CancelExecutionReport))]

// Market Profit and Loss
[JsonSerializable(typeof(MarketProfitAndLossRequest), GenerationMode = JsonSourceGenerationMode.Serialization)]
[JsonSerializable(typeof(MarketProfitAndLoss))]
[JsonSerializable(typeof(MarketProfitAndLoss[]))]
[JsonSerializable(typeof(List<MarketProfitAndLoss>))]
[JsonSerializable(typeof(IEnumerable<MarketProfitAndLoss>))]
[JsonSerializable(typeof(RunnerProfitAndLoss))]

// Error Responses
[JsonSerializable(typeof(BadRequestResponse))]
[JsonSerializable(typeof(BadRequestDetail))]
[JsonSerializable(typeof(BadRequestErrorCode))]

// Event Types Requests
[JsonSerializable(typeof(EventTypesRequest), GenerationMode = JsonSourceGenerationMode.Serialization)]

// Event Types
[JsonSerializable(typeof(EventType))]
[JsonSerializable(typeof(EventTypeResult))]
[JsonSerializable(typeof(EventTypeResult[]))]

// Events Requests
[JsonSerializable(typeof(EventsRequest), GenerationMode = JsonSourceGenerationMode.Serialization)]

// Events
[JsonSerializable(typeof(Event))]
[JsonSerializable(typeof(EventResult))]
[JsonSerializable(typeof(EventResult[]))]

// Competitions
[JsonSerializable(typeof(CompetitionsRequest), GenerationMode = JsonSourceGenerationMode.Serialization)]
[JsonSerializable(typeof(CompetitionResult))]
[JsonSerializable(typeof(CompetitionResult[]))]

// Countries
[JsonSerializable(typeof(CountriesRequest), GenerationMode = JsonSourceGenerationMode.Serialization)]
[JsonSerializable(typeof(CountryCodeResult))]
[JsonSerializable(typeof(CountryCodeResult[]))]

// Market Types
[JsonSerializable(typeof(MarketTypesRequest), GenerationMode = JsonSourceGenerationMode.Serialization)]
[JsonSerializable(typeof(MarketTypeResult))]
[JsonSerializable(typeof(MarketTypeResult[]))]

// Time Ranges
[JsonSerializable(typeof(TimeRangesRequest), GenerationMode = JsonSourceGenerationMode.Serialization)]
[JsonSerializable(typeof(TimeRangeResult))]
[JsonSerializable(typeof(TimeRangeResult[]))]

// Venues
[JsonSerializable(typeof(VenuesRequest), GenerationMode = JsonSourceGenerationMode.Serialization)]
[JsonSerializable(typeof(VenueResult))]
[JsonSerializable(typeof(VenueResult[]))]

// Current Orders
[JsonSerializable(typeof(CurrentOrdersRequest), GenerationMode = JsonSourceGenerationMode.Serialization)]
[JsonSerializable(typeof(CurrentOrderSummaryReport))]
[JsonSerializable(typeof(CurrentOrder))]
[JsonSerializable(typeof(PriceSize))]

// Cleared Orders
[JsonSerializable(typeof(ClearedOrdersRequest), GenerationMode = JsonSourceGenerationMode.Serialization)]
[JsonSerializable(typeof(ClearedOrderSummaryReport))]
[JsonSerializable(typeof(ClearedOrder))]
[JsonSerializable(typeof(ItemDescription))]

// Market Book
[JsonSerializable(typeof(PriceProjection), GenerationMode = JsonSourceGenerationMode.Serialization)]
[JsonSerializable(typeof(ExBestOffersOverrides), GenerationMode = JsonSourceGenerationMode.Serialization)]
[JsonSerializable(typeof(RunnerBookRequest), GenerationMode = JsonSourceGenerationMode.Serialization)]
[JsonSerializable(typeof(MarketBook[]))]

// Account API
[JsonSerializable(typeof(AccountFundsRequest), GenerationMode = JsonSourceGenerationMode.Serialization)]
[JsonSerializable(typeof(AccountFundsResponse))]
[JsonSerializable(typeof(AccountDetailsRequest), GenerationMode = JsonSourceGenerationMode.Serialization)]
[JsonSerializable(typeof(AccountDetailsResponse))]
[JsonSerializable(typeof(AccountStatementRequest), GenerationMode = JsonSourceGenerationMode.Serialization)]
[JsonSerializable(typeof(AccountStatementReport))]
[JsonSerializable(typeof(StatementItem))]
[JsonSerializable(typeof(StatementLegacyData))]
[JsonSerializable(typeof(CurrencyRatesRequest), GenerationMode = JsonSourceGenerationMode.Serialization)]
[JsonSerializable(typeof(CurrencyRate))]
[JsonSerializable(typeof(CurrencyRate[]))]
[ExcludeFromCodeCoverage]
internal partial class SerializerContext : JsonSerializerContext
{
}