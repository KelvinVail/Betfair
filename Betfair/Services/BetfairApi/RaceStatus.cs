namespace Betfair.Services.BetfairApi
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    [JsonConverter(typeof(StringEnumConverter))]
    public enum RaceStatus
    {
        DORMANT,
        DELAYED,
        PARADING,
        GOINGDOWN,
        GOINGBEHIND,
        APPROACHING,
        GOINGINTRAPS,
        HARERUNNING,
        ATTHEPOST,
        OFF,
        FINISHED,
        FINALRESULT,
        FALSESTART,
        PHOTOGRAPH,
        RESULT,
        WEIGHEDIN,
        RACEVOID,
        NORACE,
        RERUN,
        ABANDONED
    }
}