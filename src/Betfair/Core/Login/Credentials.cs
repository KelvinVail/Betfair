using System.Security.Cryptography.X509Certificates;

namespace Betfair.Core.Login;

/// <summary>
/// Used to store all information need to authenticate to Betfair.
/// </summary>
public sealed class Credentials
{
    private const string _apiLogin = "https://identitysso.betfair.com/api/login";
    private const string _certLogin = "https://identitysso-cert.betfair.com/api/certlogin";
    private readonly string _username;
    private readonly string _password;

    /// <summary>
    /// Initializes a new instance of the <see cref="Credentials"/> class.
    /// Used to store all information need to authenticate to Betfair.
    /// </summary>
    /// <param name="username">Your Betfair username.</param>
    /// <param name="password">Your Betfair password.</param>
    /// <param name="appKey">Your Betfair app key.</param>
    /// <param name="cert">An optional certificate. If provided the library will use the cert to authenticate to Betfair.</param>
    /// <exception cref="ArgumentNullException">Will throw if username, password or appKey is null, empty or whitespace.</exception>
    public Credentials(
        string username,
        string password,
        string appKey,
        X509Certificate2? cert = null)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentNullException(nameof(username));
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentNullException(nameof(password));
        if (string.IsNullOrWhiteSpace(appKey))
            throw new ArgumentNullException(nameof(appKey));

        _username = username;
        _password = password;
        AppKey = appKey;
        Certificate = cert;
    }

    internal X509Certificate2? Certificate { get; private set; }

    internal string AppKey { get; }

    internal HttpRequestMessage GetLoginRequest() =>
        Certificate is not null ? CertLogin() : ApiLogin();

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
