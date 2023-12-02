namespace Betfair.Stream.Messages;

internal class Authentication : MessageBase
{
    public Authentication(int id, string sessionToken, string appKey)
        : base("authentication", id)
    {
        Session = sessionToken;
        AppKey = appKey;
    }

    public string Session { get; }

    public string AppKey { get; }
}
