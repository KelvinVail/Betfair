namespace Betfair.RestApi;

public sealed class SessionService
{
    private readonly HttpClient _client;

    public SessionService(HttpClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }
}