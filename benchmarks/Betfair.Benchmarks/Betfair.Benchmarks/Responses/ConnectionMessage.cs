using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using MemoryPack;

namespace Betfair.Benchmarks.Responses;

[MemoryPackable]
public partial class ConnectionMessage
{
    [JsonPropertyName("op")]
    [DataMember(Name = "op")]
    public string? Operation { get; set; }

    [JsonPropertyName("connectionId")]
    [DataMember(Name = "connectionId")]
    public string? ConnectionId { get; set; }
}
