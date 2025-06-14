using System.Collections.Concurrent;
using System.Reflection;

namespace Betfair;

[ExcludeFromCodeCoverage]
public static class SerializerContextExtensions
{
    private static readonly ConcurrentDictionary<Type, JsonTypeInfo> _typeInfoCache = new ();

    static SerializerContextExtensions()
    {
        // Pre-populate cache with all available JsonTypeInfo properties from SerializerContext.Default
        PopulateTypeInfoCache();
    }

    /// <summary>
    /// Gets the JsonTypeInfo for the specified object instance.
    /// </summary>
    /// <typeparam name="T">The type of the object.</typeparam>
    /// <param name="obj">The object instance.</param>
    /// <returns>The JsonTypeInfo for the object's type.</returns>
    public static JsonTypeInfo GetContext<T>([NotNull] this T obj)
        where T : class => GetTypeInfo(obj.GetType());

    /// <summary>
    /// Gets the strongly-typed JsonTypeInfo for the specified type.
    /// </summary>
    /// <typeparam name="T">The type to get JsonTypeInfo for.</typeparam>
    /// <returns>The strongly-typed JsonTypeInfo for the specified type.</returns>
    public static JsonTypeInfo<T> GetTypeInfo<T>()
        where T : class => (JsonTypeInfo<T>)GetTypeInfo(typeof(T));

    /// <summary>
    /// Gets the JsonTypeInfo for the specified object instance, with internal type support.
    /// </summary>
    /// <typeparam name="T">The type of the object.</typeparam>
    /// <param name="obj">The object instance.</param>
    /// <returns>The JsonTypeInfo for the object's type.</returns>
    internal static JsonTypeInfo GetInternalContext<T>([NotNull] this T obj)
        where T : class => GetTypeInfo(obj.GetType());

    /// <summary>
    /// Gets the strongly-typed JsonTypeInfo for the specified type, with internal type support.
    /// </summary>
    /// <typeparam name="T">The type to get JsonTypeInfo for.</typeparam>
    /// <returns>The strongly-typed JsonTypeInfo for the specified type.</returns>
    internal static JsonTypeInfo<T> GetInternalTypeInfo<T>()
        where T : class => (JsonTypeInfo<T>)GetTypeInfo(typeof(T));

    /// <summary>
    /// Automatically discovers and caches all JsonTypeInfo properties from SerializerContext.Default.
    /// </summary>
    private static void PopulateTypeInfoCache()
    {
        var contextType = typeof(SerializerContext);
        var defaultInstance = SerializerContext.Default;

        // Get all public properties that return JsonTypeInfo or JsonTypeInfo<T>
        var properties = contextType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => typeof(JsonTypeInfo).IsAssignableFrom(p.PropertyType));

        foreach (var property in properties)
        {
            try
            {
                var jsonTypeInfo = (JsonTypeInfo)property.GetValue(defaultInstance)!;
                var type = jsonTypeInfo.Type;
                _typeInfoCache.TryAdd(type, jsonTypeInfo);
            }
            catch (Exception ex) when (ex is TargetInvocationException or ArgumentException or InvalidOperationException)
            {
                // Skip properties that can't be accessed or don't have valid JsonTypeInfo
            }
        }
    }

    /// <summary>
    /// Gets the JsonTypeInfo for the specified type from the cache.
    /// </summary>
    /// <param name="type">The type to get JsonTypeInfo for.</param>
    /// <returns>The JsonTypeInfo for the specified type.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the type is not supported.</exception>
    private static JsonTypeInfo GetTypeInfo(Type type)
    {
        if (_typeInfoCache.TryGetValue(type, out var value))
            return value;

        throw new InvalidOperationException($"Type {type} is not supported.");
    }
}