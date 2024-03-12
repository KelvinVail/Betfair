using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace Betfair.Stream.Messages;

[JsonSourceGenerationOptions(GenerationMode = JsonSourceGenerationMode.Metadata)]
[JsonSerializable(typeof(Authentication))]
[JsonSerializable(typeof(DataFilter))]
[JsonSerializable(typeof(MarketSubscription))]
[JsonSerializable(typeof(OrderFilter))]
[JsonSerializable(typeof(OrderSubscription))]
[JsonSerializable(typeof(StreamMarketFilter))]
internal partial class SerializerContext : JsonSerializerContext
{
}

internal static class ContextSwitch
{
    public static JsonTypeInfo GetContext<T>(T obj)
    {
        JsonTypeInfo jsonTypeInfo = obj switch
        {
            Authentication => SerializerContext.Default.Authentication,
            DataFilter => SerializerContext.Default.DataFilter,
            MarketSubscription => SerializerContext.Default.MarketSubscription,
            OrderFilter => SerializerContext.Default.OrderFilter,
            OrderSubscription => SerializerContext.Default.OrderSubscription,
            StreamMarketFilter => SerializerContext.Default.StreamMarketFilter,
            _ => throw new InvalidOperationException("Unsupported type")
        };

        return jsonTypeInfo;
    }
}