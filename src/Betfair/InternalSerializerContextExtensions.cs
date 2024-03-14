using Betfair.Api.Requests;
using Betfair.Api.Responses;
using Betfair.Core.Client;
using Betfair.Core.Login;
using Betfair.Stream.Messages;
using Betfair.Stream.Responses;

namespace Betfair;

internal static class InternalSerializerContextExtensions
{
    private static readonly Dictionary<Type, JsonTypeInfo> _typeInfo = new ()
    {
        { typeof(Authentication), SerializerContext.Default.Authentication },
        { typeof(DataFilter), SerializerContext.Default.DataFilter },
        { typeof(MarketSubscription), SerializerContext.Default.MarketSubscription },
        { typeof(OrderFilter), SerializerContext.Default.OrderFilter },
        { typeof(OrderSubscription), SerializerContext.Default.OrderSubscription },
        { typeof(StreamMarketFilter), SerializerContext.Default.StreamMarketFilter },
        { typeof(ChangeMessage), SerializerContext.Default.ChangeMessage },
        { typeof(MarketChange), SerializerContext.Default.MarketChange },
        { typeof(MarketDefinition), SerializerContext.Default.MarketDefinition },
        { typeof(OrderChange), SerializerContext.Default.OrderChange },
        { typeof(OrderRunnerChange), SerializerContext.Default.OrderRunnerChange },
        { typeof(RunnerChange), SerializerContext.Default.RunnerChange },
        { typeof(RunnerDefinition), SerializerContext.Default.RunnerDefinition },
        { typeof(UnmatchedOrder), SerializerContext.Default.UnmatchedOrder },
        { typeof(ApiMarketFilter), SerializerContext.Default.ApiMarketFilter },
        { typeof(DateRange), SerializerContext.Default.DateRange },
        { typeof(MarketCatalogueQuery), SerializerContext.Default.MarketCatalogueQuery },
        { typeof(Competition), SerializerContext.Default.Competition },
        { typeof(LadderDescription), SerializerContext.Default.LadderDescription },
        { typeof(MarketCatalogue), SerializerContext.Default.MarketCatalogue },
        { typeof(MarketDescription), SerializerContext.Default.MarketDescription },
        { typeof(MarketEvent), SerializerContext.Default.MarketEvent },
        { typeof(MarketStatus), SerializerContext.Default.MarketStatus },
        { typeof(Runner), SerializerContext.Default.Runner },
        { typeof(LoginResponse), SerializerContext.Default.LoginResponse },
        { typeof(BadRequestResponse), SerializerContext.Default.BadRequestResponse },
        { typeof(BadRequestDetail), SerializerContext.Default.BadRequestDetail },
        { typeof(BadRequestErrorCode), SerializerContext.Default.BadRequestErrorCode },
    };

    public static JsonTypeInfo GetInternalContext<T>(this T obj)
        where T : class
    {
        if (_typeInfo.TryGetValue(obj.GetType(), out var value))
            return value;

        throw new InvalidOperationException($"Type {typeof(T)} is not supported.");
    }

    public static JsonTypeInfo<T> GetTypeInfo<T>()
        where T : class
    {
        if (_typeInfo.TryGetValue(typeof(T), out var value))
            return (JsonTypeInfo<T>)value;

        throw new InvalidOperationException($"Type {typeof(T)} is not supported.");
    }
}
