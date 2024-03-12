using System.Text.Json.Serialization;

namespace Betfair.Stream.Messages;

[JsonSerializable(typeof(Authentication))]
internal class Authentication : MessageBase
{
    [JsonConstructor]
    public Authentication(int id, string sessionToken, string appKey)
        : base("authentication", id)
    {
        Session = sessionToken;
        AppKey = appKey;
    }

    [JsonPropertyName("session")]
    public string Session { get; }

    [JsonPropertyName("appKey")]
    public string AppKey { get; }
}
