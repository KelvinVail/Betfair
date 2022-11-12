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

    public BetfairHttpClient()
        : base(_handler) =>
        Configure();

    public BetfairHttpClient(HttpMessageHandler handler)
        : base(handler) =>
        Configure();

    public async Task<Result<string, ErrorResult>> Login(
        Credentials credentials,
        CancellationToken cancellationToken)
    {
        if (credentials is null) return ErrorResult.Empty(nameof(credentials));
        using var request = credentials.GetLoginRequest();

        var response = await SendAsync(request, cancellationToken);
        var result = await JsonSerializer.DeserializeAsync<Response>(
            response.Content.ReadAsStream(),
            StandardResolver.CamelCase);

        if (result.Status.Equals("success", StringComparison.OrdinalIgnoreCase))
            return result.Token;

        return ErrorResult.Create(result.Error);
    }

    private void Configure()
    {
        Timeout = TimeSpan.FromSeconds(30);
        DefaultRequestHeaders.Accept
            .Add(new MediaTypeWithQualityHeaderValue("application/json"));
        DefaultRequestHeaders
            .Add("Connection", "keep-alive");
        DefaultRequestHeaders.AcceptEncoding
            .Add(new StringWithQualityHeaderValue("gzip"));
    }
}
