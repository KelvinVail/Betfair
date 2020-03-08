namespace Betfair.Sports
{
    using System.Runtime.Serialization;

    [DataContract]
    public enum CatalogueSort
    {
        [EnumMember(Value = "MINIMUM_TRADED")]
        MinimumTraded,

        [EnumMember(Value = "MAXIMUM_TRADED")]
        MaximumTraded,

        [EnumMember(Value = "MINIMUM_AVAILABLE")]
        MinimumAvailable,

        [EnumMember(Value = "MAXIMUM_AVAILABLE")]
        MaximumAvailable,

        [EnumMember(Value = "FIRST_TO_START")]
        FirstToStart,

        [EnumMember(Value = "LAST_TO_START")]
        LastToStart,
    }
}