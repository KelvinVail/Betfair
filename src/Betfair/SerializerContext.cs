using Betfair.Api.Requests;
using Betfair.Api.Requests.Account;
using Betfair.Api.Requests.Markets;
using Betfair.Api.Requests.Orders;
using Betfair.Api.Responses;
using Betfair.Api.Responses.Account;
using Betfair.Api.Responses.Markets;
using Betfair.Api.Responses.Orders;
using Betfair.Core.Client;
using Betfair.Core.Login;
using Betfair.Stream.Messages;
using Betfair.Stream.Responses;
using RunnerResponse = Betfair.Api.Responses.RunnerResponse;

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
[JsonSerializable(typeof(TransferFundsRequest), GenerationMode = JsonSourceGenerationMode.Serialization)]
[JsonSerializable(typeof(TransferResponse))]
[ExcludeFromCodeCoverage]
internal partial class SerializerContext : JsonSerializerContext
{
}