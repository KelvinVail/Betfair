namespace Betfair.Betting
{
    using System.Runtime.Serialization;
    using System.Text.Json.Serialization;

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Side
    {
        [DataMember(Name = "BACK", EmitDefaultValue = false)]
        Back,

        [DataMember(Name = "LAY", EmitDefaultValue = false)]
        Lay,
    }
}