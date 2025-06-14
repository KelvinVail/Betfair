namespace Betfair.Core.Enums;

/// <summary>
/// Runner status values.
/// </summary>
[JsonConverter(typeof(SnakeCaseEnumJsonConverter<RunnerStatus>))]
public enum RunnerStatus
{
    /// <summary>
    /// Active runner.
    /// </summary>
    Active,

    /// <summary>
    /// Winner.
    /// </summary>
    Winner,

    /// <summary>
    /// Loser.
    /// </summary>
    Loser,

    /// <summary>
    /// Placed (applies to horse racing).
    /// </summary>
    Placed,

    /// <summary>
    /// Removed from market.
    /// </summary>
    Removed,

    /// <summary>
    /// Hidden from market.
    /// </summary>
    Hidden,
}
