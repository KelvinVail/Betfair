namespace Betfair.Api.Betting.Endpoints.ListTimeRanges;

/// <summary>
/// Time range result.
/// </summary>
public class TimeRangeResult
{
    /// <summary>
    /// Gets the time range.
    /// </summary>
    [JsonPropertyName("timeRange")]
    public TimeRange? TimeRange { get; init; }

    /// <summary>
    /// Gets the count of markets associated with this time range.
    /// </summary>
    [JsonPropertyName("marketCount")]
    public int MarketCount { get; init; }
}
