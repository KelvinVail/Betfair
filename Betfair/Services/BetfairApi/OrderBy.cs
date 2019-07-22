namespace Betfair.Services.BetfairApi
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    [JsonConverter(typeof(StringEnumConverter))]
    public enum OrderBy
    {
        BY_BET,
        BY_MARKET,
        BY_MATCH_TIME,
        BY_PLACE_TIME,
        BY_SETTLED_TIME,
        BY_VOID_TIME
    }
}