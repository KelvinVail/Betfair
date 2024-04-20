﻿// ReSharper disable InconsistentNaming
namespace Betfair.Core;

/// <summary>
/// Defines what to do with the order at turn-in-play.
/// </summary>
[JsonConverter(typeof(UpperCaseEnumJsonConverter<PersistenceType>))]
[SuppressMessage("Naming", "CA1707:Identifiers should not contain underscores", Justification = "The underscores are required by Betfair.")]
public enum PersistenceType
{
    /// <summary>
    /// Lapse (cancel) the order automatically when the market is turned in play if the bet is unmatched
    /// </summary>
    Lapse,

    /// <summary>
    /// Persist the unmatched order to in-play.
    /// The bet will be placed automatically into the in-play market at the start of the event.
    /// Once in play, the bet won't be cancelled by Betfair if a material event takes place
    /// and will be available until matched or cancelled by the user.
    /// </summary>
    Persist,

    /// <summary>
    /// Put the order into the auction (SP) at turn-in-play.
    /// </summary>
    Market_On_Close,
}
