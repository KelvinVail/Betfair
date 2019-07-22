namespace Betfair.Services.BetfairApi
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    [JsonConverter(typeof(StringEnumConverter))]
    public enum ResponseCode
    {
        OK,
        NO_NEW_UPDATES,
        NO_LIVE_DATA_AVAILABLE,
        SERVICE_UNAVAILABLE,
        UNEXPECTED_ERROR,
        LIVE_DATA_TEMPORARILY_UNAVAILABLE
    }
}