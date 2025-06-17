using Betfair.Api.Betting.Enums;

namespace Betfair.Api.Betting.Endpoints.ListTimeRanges.Responses;

/// <summary>
/// Time granularity for time range requests.
/// </summary>
[JsonConverter(typeof(SnakeCaseEnumJsonConverter<TimeGranularity>))]
public enum TimeGranularity
{
    /// <summary>
    /// Days granularity.
    /// </summary>
    Days,

    /// <summary>
    /// Hours granularity.
    /// </summary>
    Hours,

    /// <summary>
    /// Minutes granularity.
    /// </summary>
    Minutes,
}
