namespace Betfair.Api.Betting.Endpoints.ListTimeRanges;

/// <summary>
/// Time range.
/// </summary>
public class TimeRange
{
    /// <summary>
    /// Gets the start time (format: ISO 8601).
    /// </summary>
    [JsonPropertyName("from")]
    public DateTimeOffset From { get; init; }

    /// <summary>
    /// Gets the end time (format: ISO 8601).
    /// </summary>
    [JsonPropertyName("to")]
    public DateTimeOffset To { get; init; }
}
