namespace Betfair.Stream.Messages;

[JsonSerializable(typeof(MessageBase))]
internal abstract class MessageBase
{
    protected MessageBase(string op, int id)
    {
        Op = op;
        Id = id;
    }

    [JsonPropertyName("op")]
    public string Op { get; }

    [JsonPropertyName("id")]
    public int Id { get; }
}
