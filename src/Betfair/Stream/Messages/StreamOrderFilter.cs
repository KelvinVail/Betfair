﻿namespace Betfair.Stream.Messages;

/// <summary>
/// Used to shape and filter the order data returned on the stream.
/// </summary>
public class StreamOrderFilter
{
    private HashSet<string>? _strategyRefs;

    [JsonPropertyName("includeOverallPosition")]
    public bool? IncludeOverallPosition { get; private set; } = null;

    [JsonPropertyName("partitionMatchedByStrategyRef")]
    public bool? PartitionMatchedByStrategyRef { get; private set; } = null;

    [JsonPropertyName("customerStrategyRefs")]
    public IReadOnlyCollection<string>? CustomerStrategyRefs => _strategyRefs;

    /// <summary>
    /// Returns aggregated order data per runner. e.g. (Back / Lays).
    /// </summary>
    /// <returns>This <see cref="StreamOrderFilter"/>.</returns>
    public StreamOrderFilter WithAggregatedPositions()
    {
        IncludeOverallPosition = true;
        return this;
    }

    /// <summary>
    /// Returns data for each individual order per runner.
    /// </summary>
    /// <returns>This <see cref="StreamOrderFilter"/>.</returns>
    public StreamOrderFilter WithDetailedPositions()
    {
        IncludeOverallPosition = false;
        return this;
    }

    /// <summary>
    /// Returns orders aggregated per runner, per strategy reference
    /// if aggregated positions have been request.
    /// Has no effect if detailed permissions have been requested.
    /// </summary>
    /// <returns>This <see cref="StreamOrderFilter"/>.</returns>
    public StreamOrderFilter WithOrdersPerStrategy()
    {
        PartitionMatchedByStrategyRef = true;
        return this;
    }

    /// <summary>
    /// Restricts to specified customerStrategyRefs (specified in placeOrders).
    /// This will filter orders and StrategyMatchChanges accordingly
    /// (Note: overall position is not filtered).
    /// </summary>
    /// <param name="strategyRefs">A set of strategy references (specified in placeOrders).</param>
    /// <returns>This <see cref="StreamOrderFilter"/>.</returns>
    public StreamOrderFilter WithStrategyRefs(params string[] strategyRefs)
    {
        _strategyRefs ??=[];

        foreach (var strategyRef in strategyRefs.Where(x => x is not null))
            _strategyRefs.Add(strategyRef);

        return this;
    }
}
