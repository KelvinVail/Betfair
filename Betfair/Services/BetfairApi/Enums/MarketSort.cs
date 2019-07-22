namespace Betfair.Services.BetfairApi.Enums
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    /// <summary>
    /// The order of the results. Will default to RANK if not passed.
    /// RANK is an assigned priority that is determined by our Market
    /// Operations team in our back-end system. A result's overall rank
    /// is derived from the ranking given to the flowing attributes for the
    /// result. EventType, Competition, StartTime, MarketType,
    /// MarketId. For example, EventType is ranked by the most
    /// popular sports types and marketTypes are ranked in the
    /// following order: ODDS ASIAN LINE RANGE If all other
    /// dimensions of the result are equal, then the results are ranked
    /// in MarketId order.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum MarketSort
    {
        /// <summary>
        /// Minimum traded volume.
        /// </summary>
        MINIMUM_TRADED,

        /// <summary>
        /// Maximum traded volume.
        /// </summary>
        MAXIMUM_TRADED,

        /// <summary>
        /// Minimum available to match.
        /// </summary>
        MINIMUM_AVAILABLE,

        /// <summary>
        /// Maximum available to match.
        /// </summary>
        MAXIMUM_AVAILABLE,

        /// <summary>
        /// The closest markets based on their expected start time.
        /// </summary>
        FIRST_TO_START,

        /// <summary>
        /// The most distant markets based on their expected start time.
        /// </summary>
        LAST_TO_START
    }
}