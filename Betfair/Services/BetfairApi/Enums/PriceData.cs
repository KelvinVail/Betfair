namespace Betfair.Services.BetfairApi.Enums
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    /// <summary>
    /// The price data.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum PriceData
    {
        /// <summary>
        /// The SP available.
        /// </summary>
        SP_AVAILABLE,

        /// <summary>
        /// The SP traded.
        /// </summary>
        SP_TRADED,

        /// <summary>
        /// The ex best offers.
        /// </summary>
        EX_BEST_OFFERS,

        /// <summary>
        /// The ex all offers.
        /// </summary>
        EX_ALL_OFFERS,

        /// <summary>
        /// The ex traded.
        /// </summary>
        EX_TRADED
    }
}