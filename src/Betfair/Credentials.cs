using System.Security.Cryptography.X509Certificates;
using Betfair.Errors;
using CSharpFunctionalExtensions;

namespace Betfair;

public sealed class Credentials : ValueObject
{
    private readonly string _username;
    private readonly string _password;
    private readonly string _appKey;

    private Credentials(string username, string password, string appKey)
    {
        _username = username;
        _password = password;
        _appKey = appKey;
    }

    internal X509Certificate2? Certificate { get; private set; }

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

    public static Result<Credentials, ErrorResult> CreateWithCert(
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

    internal HttpRequestMessage GetLoginRequest()
    {
        if (Certificate is not null)
            return GetNonInteractiveLogin();
        return GetInteractiveLogin();
    }

    private HttpRequestMessage GetInteractiveLogin()
    {
        var request = new HttpRequestMessage(
            HttpMethod.Post,
            new Uri("https://identitysso.betfair.com/api/login"));

        AddHeadersAndContent(request);

        return request;
    }

    private HttpRequestMessage GetNonInteractiveLogin()
    {
        var request = new HttpRequestMessage(
            HttpMethod.Post,
            new Uri("https://identitysso-cert.betfair.com/api/certlogin"));

        AddHeadersAndContent(request);

        return request;
    }

    private void AddHeadersAndContent(HttpRequestMessage request)
    {
        request.Headers.Add("X-Application", _appKey);
        request.Content = new FormUrlEncodedContent(
            new Dictionary<string, string> { { "username", _username }, { "password", _password } });
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return _username;
        yield return _password;
        yield return _appKey;
    }
}
