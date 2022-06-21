namespace Betfair.RestApi;

public sealed class SessionService
{
    private readonly HttpClient _client;
    private readonly Credential _credential;

    public SessionService(HttpClient client, Credential credential)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _credential = credential ?? throw new ArgumentNullException(nameof(credential));
    }
}