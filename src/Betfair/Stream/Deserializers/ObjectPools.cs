using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using Betfair.Stream.Responses;

namespace Betfair.Stream.Deserializers;

/// <summary>
/// Object pools for reducing GC pressure during deserialization.
/// </summary>
internal static class ObjectPools
{
    // Object pools for reducing GC pressure
    private static readonly ConcurrentQueue<List<MarketChange>> _marketChangeListPool = new();
    private static readonly ConcurrentQueue<List<OrderChange>> _orderChangeListPool = new();
    private static readonly ConcurrentQueue<List<RunnerChange>> _runnerChangeListPool = new();
    private static readonly ConcurrentQueue<List<List<double>>> _doubleArrayListPool = new();
    private static readonly ConcurrentQueue<List<double>> _doubleListPool = new();
    private static readonly ConcurrentQueue<List<string>> _stringListPool = new();
    private static readonly ConcurrentQueue<List<RunnerDefinition>> _runnerDefinitionListPool = new();

    /// <summary>
    /// Gets a pooled list for MarketChange objects.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static List<MarketChange> GetMarketChangeList()
    {
        if (_marketChangeListPool.TryDequeue(out var list))
        {
            list.Clear();
            return list;
        }
        return new List<MarketChange>();
    }

    /// <summary>
    /// Returns a MarketChange list to the pool.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void ReturnMarketChangeList(List<MarketChange> list)
    {
        if (list.Count < 1000) // Prevent memory bloat
        {
            _marketChangeListPool.Enqueue(list);
        }
    }

    /// <summary>
    /// Gets a pooled list for OrderChange objects.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static List<OrderChange> GetOrderChangeList()
    {
        if (_orderChangeListPool.TryDequeue(out var list))
        {
            list.Clear();
            return list;
        }
        return new List<OrderChange>();
    }

    /// <summary>
    /// Returns an OrderChange list to the pool.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void ReturnOrderChangeList(List<OrderChange> list)
    {
        if (list.Count < 1000) // Prevent memory bloat
        {
            _orderChangeListPool.Enqueue(list);
        }
    }

    /// <summary>
    /// Gets a pooled list for RunnerChange objects.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static List<RunnerChange> GetRunnerChangeList()
    {
        if (_runnerChangeListPool.TryDequeue(out var list))
        {
            list.Clear();
            return list;
        }
        return new List<RunnerChange>();
    }

    /// <summary>
    /// Returns a RunnerChange list to the pool.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void ReturnRunnerChangeList(List<RunnerChange> list)
    {
        if (list.Count < 1000) // Prevent memory bloat
        {
            _runnerChangeListPool.Enqueue(list);
        }
    }

    /// <summary>
    /// Gets a pooled list for double arrays.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static List<List<double>> GetDoubleArrayList()
    {
        if (_doubleArrayListPool.TryDequeue(out var list))
        {
            list.Clear();
            return list;
        }
        return new List<List<double>>();
    }

    /// <summary>
    /// Returns a double array list to the pool.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void ReturnDoubleArrayList(List<List<double>> list)
    {
        if (list.Count < 1000) // Prevent memory bloat
        {
            _doubleArrayListPool.Enqueue(list);
        }
    }

    /// <summary>
    /// Gets a pooled list for double values.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static List<double> GetDoubleList()
    {
        if (_doubleListPool.TryDequeue(out var list))
        {
            list.Clear();
            return list;
        }
        return new List<double>();
    }

    /// <summary>
    /// Returns a double list to the pool.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void ReturnDoubleList(List<double> list)
    {
        if (list.Count < 1000) // Prevent memory bloat
        {
            _doubleListPool.Enqueue(list);
        }
    }

    /// <summary>
    /// Gets a pooled list for string values.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static List<string> GetStringList()
    {
        if (_stringListPool.TryDequeue(out var list))
        {
            list.Clear();
            return list;
        }
        return new List<string>();
    }

    /// <summary>
    /// Returns a string list to the pool.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void ReturnStringList(List<string> list)
    {
        if (list.Count < 1000) // Prevent memory bloat
        {
            _stringListPool.Enqueue(list);
        }
    }

    /// <summary>
    /// Gets a pooled list for RunnerDefinition objects.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static List<RunnerDefinition> GetRunnerDefinitionList()
    {
        if (_runnerDefinitionListPool.TryDequeue(out var list))
        {
            list.Clear();
            return list;
        }
        return new List<RunnerDefinition>();
    }

    /// <summary>
    /// Returns a RunnerDefinition list to the pool.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void ReturnRunnerDefinitionList(List<RunnerDefinition> list)
    {
        if (list.Count < 1000) // Prevent memory bloat
        {
            _runnerDefinitionListPool.Enqueue(list);
        }
    }
}
