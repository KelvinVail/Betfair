using System.Net.Http.Headers;
using Betfair.Errors;
using Betfair.Login;
using CSharpFunctionalExtensions;
using Utf8Json;
using Utf8Json.Resolvers;

namespace Betfair.Client;

public sealed class BetfairHttpClient : HttpClient
{
    private static readonly BetfairClientHandler _handler = new ();
    private readonly Credentials _credentials;

    public BetfairHttpClient(Credentials credentials)
        : base(_handler)
    {
        _credentials = credentials ?? throw new ArgumentNullException(nameof(credentials));
        Configure();
    }

    public BetfairHttpClient(Credentials credentials, HttpMessageHandler handler)
        : base(handler)
    {
        _credentials = credentials;
        Configure();
    }

    public async Task<Result<string, ErrorResult>> Login(
        CancellationToken cancellationToken)
    {
        using var request = _credentials.GetLoginRequest();

        var response = await SendAsync(request, cancellationToken);
        var result = await JsonSerializer.DeserializeAsync<Response>(
            response.Content.ReadAsStream(),
            StandardResolver.CamelCase);

        if (LoginIsSuccess(result))
            return TokenFrom(result);

        return Error(result);
    }

    private static bool LoginIsSuccess(Response result) =>
        result.Status.Equals("success", StringComparison.OrdinalIgnoreCase)
        || result.LoginStatus.Equals("Success", StringComparison.OrdinalIgnoreCase);

    private static string TokenFrom(Response result) =>
        !string.IsNullOrWhiteSpace(result.Token)
            ? result.Token
            : result.SessionToken;

    private static ErrorResult Error(Response result) =>
        ErrorResult.Create(!string.IsNullOrWhiteSpace(result.Error)
            ? result.Error
            : result.LoginStatus);

    private void Configure()
    {
        Timeout = TimeSpan.FromSeconds(30);
        DefaultRequestHeaders.Accept
            .Add(new MediaTypeWithQualityHeaderValue("application/json"));
        DefaultRequestHeaders
            .Add("Connection", "keep-alive");
        DefaultRequestHeaders.AcceptEncoding
            .Add(new StringWithQualityHeaderValue("gzip"));

        if (_credentials.Certificate is not null)
            _handler.ClientCertificates.Add(_credentials.Certificate);
    }
}
