using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Betfair.Betting
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Side
    {
        [DataMember(Name = "BACK", EmitDefaultValue = false)]
        Back,

        [DataMember(Name = "LAY", EmitDefaultValue = false)]
        Lay,
    }
}