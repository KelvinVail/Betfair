namespace Betfair.Sports
{
    using System.Runtime.Serialization;

    [DataContract]
    public enum MarketProjection
    {
        [EnumMember(Value = "COMPETITION")]
        Competition,

        [EnumMember(Value = "EVENT")]
        Event,

        [EnumMember(Value = "EVENT_TYPE")]
        EventType,

        [EnumMember(Value = "MARKET_DESCRIPTION")]
        MarketDescription,

        [EnumMember(Value = "MARKET_START_TIME")]
        MarketStartTime,

        [EnumMember(Value = "RUNNER_DESCRIPTION")]
        RunnerDescription,

        [EnumMember(Value = "RUNNER_METADATA")]
        RunnerMetadata,
    }
}