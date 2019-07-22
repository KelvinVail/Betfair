namespace Betfair.Services.BetfairApi
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    [JsonConverter(typeof(StringEnumConverter))]
    public enum MatchProjection
    {
        NONE,
        NO_ROLLUP,
        ROLLED_UP_BY_PRICE,
        ROLLED_UP_BY_AVG_PRICE
    }
}