namespace Betfair.Services.BetfairApi.Enums
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    /// <summary>
    /// The match projection.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum MatchProjection
    {
        /// <summary>
        /// The none.
        /// </summary>
        NONE,

        /// <summary>
        /// The n o_ rollup.
        /// </summary>
        NO_ROLLUP,

        /// <summary>
        /// The rolled up by price.
        /// </summary>
        ROLLED_UP_BY_PRICE,

        /// <summary>
        /// The rolled up by average price.
        /// </summary>
        ROLLED_UP_BY_AVG_PRICE
    }
}