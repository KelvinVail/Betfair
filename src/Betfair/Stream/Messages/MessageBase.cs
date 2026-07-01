namespace Betfair.Stream.Messages;

internal abstract class MessageBase(string op, int id)
{
    [JsonPropertyName("op")]
    public string Op { get; } = op;

    [JsonPropertyName("id")]
    public int Id { get; internal set; } = id;
}
