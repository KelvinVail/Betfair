using Betfair.Api.Requests;
using Betfair.Api.Responses;
using Betfair.Core.Client;
using Betfair.Core.Login;
using Betfair.Stream.Messages;
using Betfair.Stream.Responses;

namespace Betfair;

public static class SerializerContextExtensions
{
    private static readonly Dictionary<Type, JsonTypeInfo> _internalTypes = new ()
    {
        { typeof(LoginResponse), SerializerContext.Default.LoginResponse },
        { typeof(BadRequestResponse), SerializerContext.Default.BadRequestResponse },
        { typeof(BadRequestDetail), SerializerContext.Default.BadRequestDetail },
        { typeof(BadRequestErrorCode), SerializerContext.Default.BadRequestErrorCode },
    };

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
        { typeof(MarketCatalogue[]), SerializerContext.Default.MarketCatalogueArray },
        { typeof(MarketDescription), SerializerContext.Default.MarketDescription },
        { typeof(MarketEvent), SerializerContext.Default.MarketEvent },
        { typeof(MarketStatus[]), SerializerContext.Default.MarketStatusArray },
        { typeof(Runner), SerializerContext.Default.Runner },
        { typeof(MarketCatalogueRequest), SerializerContext.Default.MarketCatalogueRequest },
        { typeof(MarketBookRequest), SerializerContext.Default.MarketBookRequest },
    };

    public static JsonTypeInfo GetContext<T>([NotNull] this T obj)
        where T : class => GetTypeInfo(obj.GetType());

    public static JsonTypeInfo<T> GetTypeInfo<T>()
        where T : class => (JsonTypeInfo<T>)GetTypeInfo(typeof(T));

    internal static JsonTypeInfo GetInternalContext<T>([NotNull] this T obj)
        where T : class => GetTypeInfoWithFallback(obj.GetType(), GetInternalTypeInfo);

    internal static JsonTypeInfo<T> GetInternalTypeInfo<T>()
        where T : class => (JsonTypeInfo<T>)GetTypeInfoWithFallback(typeof(T), GetInternalTypeInfo);

    private static JsonTypeInfo GetTypeInfo(Type type) => GetTypeInfoFromDictionary(type, _typeInfo);

    private static JsonTypeInfo GetInternalTypeInfo(Type type) => GetTypeInfoFromDictionary(type, _internalTypes);

    private static JsonTypeInfo GetTypeInfoWithFallback(Type type, Func<Type, JsonTypeInfo> primaryGetter)
    {
        try
        {
            return primaryGetter(type);
        }
        catch (InvalidOperationException)
        {
            return GetTypeInfo(type);
        }
    }

    private static JsonTypeInfo GetTypeInfoFromDictionary(Type type, Dictionary<Type, JsonTypeInfo> dictionary)
    {
        if (dictionary.TryGetValue(type, out var value))
            return value;

        throw new InvalidOperationException($"Type {type} is not supported.");
    }
}