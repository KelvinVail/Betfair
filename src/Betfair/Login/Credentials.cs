using System.Security.Cryptography.X509Certificates;

namespace Betfair.Login;

public sealed class Credentials : ValueObject
{
    private const string _apiLogin = "https://identitysso.betfair.com/api/login";
    private const string _certLogin = "https://identitysso-cert.betfair.com/api/certlogin";
    private readonly string _username;
    private readonly string _password;

    private Credentials(string username, string password, string appKey)
    {
        _username = username;
        _password = password;
        AppKey = appKey;
    }

    internal X509Certificate2? Certificate { get; private set; }

    internal string AppKey { get; }

    public static Result<Credentials, ErrorResult> Create(
        string username,
        string password,
        string appKey)
    {
        if (string.IsNullOrWhiteSpace(username))
            return ErrorResult.Empty(nameof(username));
        if (string.IsNullOrWhiteSpace(password))
            return ErrorResult.Empty(nameof(password));
        if (string.IsNullOrWhiteSpace(appKey))
            return ErrorResult.Empty(nameof(appKey));

        return new Credentials(username, password, appKey);
    }

    public static Result<Credentials, ErrorResult> Create(
        string username,
        string password,
        string appKey,
        X509Certificate2 certificate)
    {
        var cred = Create(username, password, appKey);
        if (cred.IsFailure) return cred.Error;
        if (certificate is null) return ErrorResult.Empty(nameof(certificate));
        cred.Value.Certificate = certificate;

        return cred;
    }

    internal void AddHeaders(StreamContent request, string sessionToken)
    {
        request.Headers.Add("Content-Type", "application/json");
        request.Headers.Add("X-Application", AppKey);
        request.Headers.Add("X-Authentication", sessionToken);
    }

    internal HttpRequestMessage GetLoginRequest() =>
        Certificate is not null ? CertLogin() : ApiLogin();

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return _username;
        yield return _password;
        yield return AppKey;
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
        request.Headers.Add("X-Application", AppKey);
        request.Content = new FormUrlEncodedContent(
            new Dictionary<string, string> { { "username", _username }, { "password", _password } });
    }
}
