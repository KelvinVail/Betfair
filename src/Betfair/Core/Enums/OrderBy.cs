namespace Betfair.Core.Enums;

/// <summary>
/// Specifies how the results will be ordered.
/// If no value is passed in, it defaults to BY_BET.
/// Also acts as a filter such that only orders with a valid value in the field being ordered by will be returned
/// (i.e. BY_VOID_TIME returns only voided orders,
/// BY_SETTLED_TIME (applies to partially settled markets) returns only settled orders
/// and BY_MATCH_TIME returns only orders with a matched date (voided, settled, matched orders)).
/// Note that specifying an orderBy parameter defines the context of the date filter applied by the dateRange parameter (placed, matched, voided or settled date)
/// - see the dateRange parameter description (above) for more information.
/// See also the OrderBy type definition.
/// </summary>
[JsonConverter(typeof(OrderByJsonConverter))]
public enum OrderBy
{
    /// <summary>
    /// Order by market id, then placed time, then bet id.
    /// </summary>
    Market,

    /// <summary>
    /// Order by time of last matched fragment (if any), then placed time, then bet id. Filters out orders which have no matched date.
    /// The dateRange filter (if specified) is applied to the matched date.
    /// </summary>
    MatchTime,

    /// <summary>
    /// Order by placed time, then bet id.
    /// The dateRange filter (if specified) is applied to the placed date.
    /// </summary>
    PlaceTime,

    /// <summary>
    /// Order by time of last settled fragment (if any due to partial market settlement), then by last match time, then placed time, then bet id.
    /// Filters out orders which have not been settled.
    /// The dateRange filter (if specified) is applied to the settled date.
    /// </summary>
    SettledTime,

    /// <summary>
    /// Order by time of last voided fragment (if any), then by last match time, then placed time, then bet id.
    /// Filters out orders which have not been voided.
    /// The dateRange filter (if specified) is applied to the voided date.
    /// </summary>
    VoidTime,
}
