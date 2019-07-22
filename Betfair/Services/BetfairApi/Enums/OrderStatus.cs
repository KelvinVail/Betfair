namespace Betfair.Services.BetfairApi.Enums
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    /// <summary>
    /// The order status.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum OrderStatus
    {
        /// <summary>
        /// Execution complete indicates the order has been fully matched.
        /// </summary>
        EXECUTION_COMPLETE,

        /// <summary>
        /// Executable indicates some or all of the order is unmatched.
        /// </summary>
        EXECUTABLE
    }
}