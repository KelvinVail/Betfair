namespace Betfair.Services.BetfairApi.Enums
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    /// <summary>
    /// The order type.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum OrderType
    {
        /// <summary>
        /// Limit: normal exchange limit order for immediate execution.
        /// </summary>
        LIMIT, 

        /// <summary>
        /// Limit on close: limit order for the auction (SP).
        /// </summary>
        LIMIT_ON_CLOSE, 

        /// <summary>
        /// Market on close: market order for the auction (SP).
        /// </summary>
        MARKET_ON_CLOSE 
    }
}