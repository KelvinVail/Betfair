using Betfair.Api.Requests;
using Betfair.Api.Responses;
using Betfair.Stream.Messages;
using Betfair.Stream.Responses;

namespace Betfair;

internal static class SerializerContextSwitch
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

            ChangeMessage => SerializerContext.Default.ChangeMessage,
            MarketChange => SerializerContext.Default.MarketChange,
            MarketDefinition => SerializerContext.Default.MarketDefinition,
            OrderChange => SerializerContext.Default.OrderChange,
            OrderRunnerChange => SerializerContext.Default.OrderRunnerChange,
            RunnerChange => SerializerContext.Default.RunnerChange,
            RunnerDefinition => SerializerContext.Default.RunnerDefinition,
            UnmatchedOrder => SerializerContext.Default.UnmatchedOrder,

            ApiMarketFilter => SerializerContext.Default.ApiMarketFilter,
            DateRange => SerializerContext.Default.DateRange,
            MarketCatalogueQuery => SerializerContext.Default.MarketCatalogueQuery,

            Competition => SerializerContext.Default.Competition,
            LadderDescription => SerializerContext.Default.LadderDescription,
            MarketCatalogue => SerializerContext.Default.MarketCatalogue,
            MarketDescription => SerializerContext.Default.MarketDescription,
            MarketEvent => SerializerContext.Default.MarketEvent,
            MarketStatus => SerializerContext.Default.MarketStatus,
            Runner => SerializerContext.Default.Runner,

            _ => throw new InvalidOperationException("Unsupported type")
        };

        return jsonTypeInfo;
    }
}
