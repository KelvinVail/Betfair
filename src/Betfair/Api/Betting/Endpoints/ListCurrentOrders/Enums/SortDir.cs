using Betfair.Api.Betting.Enums;

namespace Betfair.Api.Betting.Endpoints.ListCurrentOrders.Enums;

/// <summary>
/// Specifies the direction the results will be sorted in.
/// If no value is passed in, it defaults to EARLIEST_TO_LATEST.
/// </summary>
[JsonConverter(typeof(SnakeCaseEnumJsonConverter<SortDir>))]
public enum SortDir
{
    /// <summary>
    /// Order from the earliest value to latest e.g. lowest betId is first in the results.
    /// </summary>
    EarliestToLatest,

    /// <summary>
    /// Order from the latest value to the earliest e.g. highest betId is first in the results.
    /// </summary>
    LatestToEarliest,
}
