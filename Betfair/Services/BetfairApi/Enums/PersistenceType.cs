namespace Betfair.Services.BetfairApi.Enums
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    /// <summary>
    /// The persistence type.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum PersistenceType
    {
        /// <summary>
        /// Lapse: lapse the order at turn-in-play.
        /// </summary>
        LAPSE, 

        /// <summary>
        /// Persist: put the order into the auction (SP) at turn-in-play.
        /// </summary>
        PERSIST, 

        /// <summary>
        /// Market on close: put the order into the auction (SP) at turn-in-play.
        /// </summary>
        MARKET_ON_CLOSE 
    }
}