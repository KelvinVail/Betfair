using System.Text.Json.Serialization;

namespace Betfair.Stream.Messages;

internal abstract class MessageBase
{
    protected MessageBase(string operation, int id)
    {
        Op = operation;
        Id = id;
    }

    [JsonPropertyName("op")]
    public string Op { get; }

    [JsonPropertyName("id")]
    public int Id { get; }
}
