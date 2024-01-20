using Betfair.Core.Client;

namespace Betfair.Core.Login;

internal sealed class TokenProvider
{
    private const string _apiLogin = "https://identitysso.betfair.com/api/login";
    private const string _certLogin = "https://identitysso-cert.betfair.com/api/certlogin";
    private readonly BetfairHttpClient _client;
    private readonly Credentials _credentials;

    internal TokenProvider(BetfairHttpClient client, Credentials credentials)
    {
        _client = client;
        _credentials = credentials;
    }

    internal async Task<string> GetToken(CancellationToken cancellationToken)
    {
        using var request = GetLoginRequest();

        var response = await GetLoginResponse(request, cancellationToken);

        if (!LoginIsSuccess(response)) throw Error(response);

        return TokenFrom(response);
    }

    private static bool LoginIsSuccess(LoginResponse result) =>
        result.Status.Equals("success", StringComparison.OrdinalIgnoreCase)
        || result.LoginStatus.Equals("Success", StringComparison.OrdinalIgnoreCase);

    private static string TokenFrom(LoginResponse result) =>
        !string.IsNullOrWhiteSpace(result.Token)
            ? result.Token
            : result.SessionToken;

    private static HttpRequestException Error(LoginResponse result) =>
        new (
            !string.IsNullOrWhiteSpace(result.Error)
            ? result.Error
            : result.LoginStatus);

    private HttpRequestMessage GetLoginRequest() =>
        _credentials.Certificate is not null ? CertLogin() : ApiLogin();

    private async Task<LoginResponse> GetLoginResponse(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var response = await _client.SendAsync(request, cancellationToken);
        return await JsonSerializer.DeserializeAsync<LoginResponse>(
            await response.Content.ReadAsStreamAsync(cancellationToken),
            StandardResolver.AllowPrivateExcludeNullCamelCase);
    }

    private HttpRequestMessage ApiLogin()
    {
        var request = new HttpRequestMessage(
            HttpMethod.Post,
            new Uri(_apiLogin));

        AddHeadersAndContent(request);

        return request;
    }

    private HttpRequestMessage CertLogin()
    {
        var request = new HttpRequestMessage(
            HttpMethod.Post,
            new Uri(_certLogin));

        AddHeadersAndContent(request);

        return request;
    }

    private void AddHeadersAndContent(HttpRequestMessage request)
    {
        request.Headers.Add("X-Application", _credentials.AppKey);
        request.Content = new FormUrlEncodedContent(
            new Dictionary<string, string> { { "username", _credentials.Username }, { "password", _credentials.Password } });
    }
}
