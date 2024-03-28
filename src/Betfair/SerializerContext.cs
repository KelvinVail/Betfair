using Betfair.Api.Requests;
using Betfair.Api.Responses;
using Betfair.Core.Client;
using Betfair.Core.Login;
using Betfair.Stream.Messages;
using Betfair.Stream.Responses;

namespace Betfair;

[JsonSourceGenerationOptions(GenerationMode = JsonSourceGenerationMode.Metadata, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
[JsonSerializable(typeof(Authentication), GenerationMode = JsonSourceGenerationMode.Serialization)]
[JsonSerializable(typeof(DataFilter), GenerationMode = JsonSourceGenerationMode.Serialization)]
[JsonSerializable(typeof(MarketSubscription), GenerationMode = JsonSourceGenerationMode.Serialization)]
[JsonSerializable(typeof(OrderFilter), GenerationMode = JsonSourceGenerationMode.Serialization)]
[JsonSerializable(typeof(OrderSubscription), GenerationMode = JsonSourceGenerationMode.Serialization)]
[JsonSerializable(typeof(StreamMarketFilter), GenerationMode = JsonSourceGenerationMode.Serialization)]
[JsonSerializable(typeof(MessageBase), GenerationMode = JsonSourceGenerationMode.Serialization)]

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
[JsonSerializable(typeof(MarketCatalogueQuery), GenerationMode = JsonSourceGenerationMode.Serialization)]
[JsonSerializable(typeof(MarketCatalogueRequest), GenerationMode = JsonSourceGenerationMode.Serialization)]
[JsonSerializable(typeof(MarketBookRequest), GenerationMode = JsonSourceGenerationMode.Serialization)]

[JsonSerializable(typeof(Competition))]
[JsonSerializable(typeof(LadderDescription))]
[JsonSerializable(typeof(MarketCatalogue))]
[JsonSerializable(typeof(MarketDescription))]
[JsonSerializable(typeof(MarketEvent))]
[JsonSerializable(typeof(MarketStatus[]))]
[JsonSerializable(typeof(Runner))]
[JsonSerializable(typeof(MarketCatalogue[]))]

[JsonSerializable(typeof(LoginResponse))]

[JsonSerializable(typeof(BadRequestResponse))]
[JsonSerializable(typeof(BadRequestDetail))]
[JsonSerializable(typeof(BadRequestErrorCode))]
internal partial class SerializerContext : JsonSerializerContext
{
}