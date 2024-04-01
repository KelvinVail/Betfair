namespace Betfair.Stream.Responses;

public class ConnectionMessage
{
    [JsonPropertyName("op")]
    [DataMember(Name = "op")]
    public string? Operation { get; set; }

    [JsonPropertyName("connectionId")]
    [DataMember(Name = "connectionId")]
    public string? ConnectionId { get; set; }
}
