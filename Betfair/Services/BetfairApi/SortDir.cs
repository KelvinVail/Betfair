namespace Betfair.Services.BetfairApi
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    [JsonConverter(typeof(StringEnumConverter))]
    public enum SortDir
    {
        EARLIEST_TO_LATEST,
        LATEST_TO_EARLIEST
    }
}