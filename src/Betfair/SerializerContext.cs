using Betfair.Api.Requests;
using Betfair.Api.Responses;
using Betfair.Stream.Messages;
using Betfair.Stream.Responses;

namespace Betfair;

[JsonSourceGenerationOptions(GenerationMode = JsonSourceGenerationMode.Serialization)]
[JsonSerializable(typeof(Authentication))]
[JsonSerializable(typeof(DataFilter))]
[JsonSerializable(typeof(MarketSubscription))]
[JsonSerializable(typeof(OrderFilter))]
[JsonSerializable(typeof(OrderSubscription))]
[JsonSerializable(typeof(StreamMarketFilter))]
[JsonSerializable(typeof(MessageBase))]

[JsonSerializable(typeof(ChangeMessage))]
[JsonSerializable(typeof(MarketChange))]
[JsonSerializable(typeof(MarketDefinition))]
[JsonSerializable(typeof(OrderChange))]
[JsonSerializable(typeof(OrderRunnerChange))]
[JsonSerializable(typeof(RunnerChange))]
[JsonSerializable(typeof(RunnerDefinition))]
[JsonSerializable(typeof(UnmatchedOrder))]

[JsonSerializable(typeof(ApiMarketFilter))]
[JsonSerializable(typeof(DateRange))]
[JsonSerializable(typeof(MarketCatalogueQuery))]

[JsonSerializable(typeof(Competition))]
[JsonSerializable(typeof(LadderDescription))]
[JsonSerializable(typeof(MarketCatalogue))]
[JsonSerializable(typeof(MarketDescription))]
[JsonSerializable(typeof(MarketEvent))]
[JsonSerializable(typeof(MarketStatus))]
[JsonSerializable(typeof(Runner))]
internal partial class SerializerContext : JsonSerializerContext
{
}