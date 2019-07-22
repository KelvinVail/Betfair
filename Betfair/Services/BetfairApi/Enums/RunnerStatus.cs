namespace Betfair.Services.BetfairApi.Enums
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    /// <summary>
    /// The runner status.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum RunnerStatus
    {
        /// <summary>
        /// The active.
        /// </summary>
        ACTIVE,

        /// <summary>
        /// The winner.
        /// </summary>
        WINNER,

        /// <summary>
        /// The loser.
        /// </summary>
        LOSER,

        /// <summary>
        /// The remove d_ vacant.
        /// </summary>
        REMOVED_VACANT,

        /// <summary>
        /// The removed.
        /// </summary>
        REMOVED
    }
}