using Betfair.Api.Requests;
using Betfair.Api.Requests.Orders;
using Betfair.Api.Responses;
using Betfair.Api.Responses.Orders;
using Betfair.Core.Client;
using Betfair.Core.Login;
using Betfair.Stream.Messages;
using Betfair.Stream.Responses;

namespace Betfair;

[JsonSourceGenerationOptions(GenerationMode = JsonSourceGenerationMode.Metadata, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull, PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]

// Login
[JsonSerializable(typeof(Authentication), GenerationMode = JsonSourceGenerationMode.Serialization)]
[JsonSerializable(typeof(LoginResponse))]

// Stream Messages
[JsonSerializable(typeof(DataFilter), GenerationMode = JsonSourceGenerationMode.Serialization)]
[JsonSerializable(typeof(MarketSubscription), GenerationMode = JsonSourceGenerationMode.Serialization)]
[JsonSerializable(typeof(OrderFilter), GenerationMode = JsonSourceGenerationMode.Serialization)]
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
[JsonSerializable(typeof(MarketStatus[]))]
[JsonSerializable(typeof(Runner))]
[JsonSerializable(typeof(MarketCatalogue[]))]

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

// Error Responses
[JsonSerializable(typeof(BadRequestResponse))]
[JsonSerializable(typeof(BadRequestDetail))]
[JsonSerializable(typeof(BadRequestErrorCode))]
internal partial class SerializerContext : JsonSerializerContext
{
}