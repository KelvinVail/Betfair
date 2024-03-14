using Betfair.Core.Client;

namespace Betfair.Core.Login;

internal class TokenProvider
{
    private const string _apiLogin = "https://identitysso.betfair.com/api/login";
    private const string _certLogin = "https://identitysso-cert.betfair.com/api/certlogin";
    private readonly BetfairHttpClient? _client;
    private readonly Credentials? _credentials;

    internal TokenProvider(BetfairHttpClient client, Credentials credentials)
    {
        _client = client;
        _credentials = credentials;
    }

    internal TokenProvider()
    {
    }

    internal virtual async Task<string> GetToken(CancellationToken cancellationToken)
    {
        using var request = GetLoginRequest();
        var response = await GetLoginResponse(request, cancellationToken);
        if (!LoginIsSuccess(response)) throw new HttpRequestException(response.Error);

        return response.Token;
    }

    private static bool LoginIsSuccess(MergedResponse result) =>
        result.Status.Equals("success", StringComparison.OrdinalIgnoreCase);

    private HttpRequestMessage GetLoginRequest() =>
        _credentials!.Certificate is not null ? CertLogin() : ApiLogin();

    private async Task<MergedResponse> GetLoginResponse(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var response = await _client!.SendAsync(request, cancellationToken);
        var result = await JsonSerializer.DeserializeAsync(
            await response.Content.ReadAsStreamAsync(cancellationToken),
            SerializerContextExtensions.GetInternalTypeInfo<LoginResponse>(),
            cancellationToken);
        return new MergedResponse(result!);
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
        request.Headers.Add("X-Application", _credentials!.AppKey);
        request.Content = new FormUrlEncodedContent(
            new Dictionary<string, string> { { "username", _credentials.Username }, { "password", _credentials.Password } });
    }

    private sealed class MergedResponse(LoginResponse response)
    {
        public string Token { get; } = response.Token + response.SessionToken;

        public string Status { get; } = response.Status + response.LoginStatus;

        public string Error { get; } = response.Error + response.LoginStatus;
    }
}
