namespace Betfair.Stream.Messages;

internal class Authentication(int id, string sessionToken, string appKey)
    : MessageBase("authentication", id)
{
    [JsonPropertyName("session")]
    public string Session { get; } = sessionToken;

    [JsonPropertyName("appKey")]
    public string AppKey { get; } = appKey;
}
