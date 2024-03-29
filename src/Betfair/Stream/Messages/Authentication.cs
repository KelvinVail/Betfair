﻿namespace Betfair.Stream.Messages;

internal class Authentication : MessageBase
{
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
