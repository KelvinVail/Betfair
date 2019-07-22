namespace Betfair.Services.BetfairApi
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    [JsonConverter(typeof(StringEnumConverter))]
    public enum IncludeItem
    {
        ALL,
        DEPOSITS_WITHDRAWALS,
        EXCHANGE,
        POKER_ROOM
    }
}