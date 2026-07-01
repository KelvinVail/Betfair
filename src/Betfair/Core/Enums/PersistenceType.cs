using Betfair.Api.Betting.Enums;

namespace Betfair.Core.Enums;

/// <summary>
/// Defines what to do with the order at turn-in-play.
/// </summary>
[JsonConverter(typeof(SnakeCaseEnumJsonConverter<PersistenceType>))]
public enum PersistenceType
{
    /// <summary>Unknown or not yet set.</summary>
    Unknown = 0,

    /// <summary>
    /// Lapse (cancel) the order automatically when the market is turned in play if the bet is unmatched.
    /// </summary>
    Lapse,

    /// <summary>
    /// Persist the unmatched order to in-play.
    /// </summary>
    Persist,

    /// <summary>
    /// Put the order into the auction (SP) at turn-in-play.
    /// </summary>
    MarketOnClose,
}
