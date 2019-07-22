namespace Betfair.Services.BetfairApi.Enums
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    /// <summary>
    /// The market status.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum MarketStatus
    {
        /// <summary>
        /// Indicates the market is inactive.
        /// </summary>
        INACTIVE,

        /// <summary>
        /// Indicates the market is open for trading.
        /// </summary>
        OPEN,

        /// <summary>
        /// Indicates the market trading has been suspended.
        /// </summary>
        SUSPENDED,

        /// <summary>
        /// Indicates the market has been closed and settled.
        /// </summary>
        CLOSED
    }
}