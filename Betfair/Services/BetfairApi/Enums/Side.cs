namespace Betfair.Services.BetfairApi.Enums
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    /// <summary>
    /// The side.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum Side
    {
        /// <summary>
        /// The back.
        /// </summary>
        BACK,

        /// <summary>
        /// The lay.
        /// </summary>
        LAY
    }
}