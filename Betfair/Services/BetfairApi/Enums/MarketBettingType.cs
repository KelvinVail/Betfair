namespace Betfair.Services.BetfairApi.Enums
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    /// <summary>
    /// The market betting type.
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public enum MarketBettingType
    {
        /// <summary>
        /// The odds.
        /// </summary>
        ODDS,

        /// <summary>
        /// The line.
        /// </summary>
        LINE,

        /// <summary>
        /// The range.
        /// </summary>
        RANGE,

        /// <summary>
        /// The asian handicap double line.
        /// </summary>
        ASIAN_HANDICAP_DOUBLE_LINE,

        /// <summary>
        /// The asian handicap single line.
        /// </summary>
        ASIAN_HANDICAP_SINGLE_LINE,

        /// <summary>
        /// The fixed odds.
        /// </summary>
        FIXED_ODDS
    }
}