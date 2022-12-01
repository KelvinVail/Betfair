using Betfair.Client;
using Utf8Json;
using Utf8Json.Resolvers;

namespace Betfair.Login;

public class TokenProvider
{
    private readonly BetfairHttpClient _client;
    private readonly Credentials _credentials;

    public TokenProvider(BetfairHttpClient client, Credentials credentials)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _credentials = credentials ?? throw new ArgumentNullException(nameof(credentials));
    }

    public async Task<Result<string, ErrorResult>> GetToken(CancellationToken cancellationToken)
    {
        using var request = _credentials.GetLoginRequest();

        var response = await _client.SendAsync(request, cancellationToken);
        var result = await JsonSerializer.DeserializeAsync<LoginResponse>(
            await response.Content.ReadAsStreamAsync(cancellationToken),
            StandardResolver.CamelCase);

        if (LoginIsSuccess(result))
            return TokenFrom(result);

        return Error(result);
    }

    private static bool LoginIsSuccess(LoginResponse result) =>
        result.Status.Equals("success", StringComparison.OrdinalIgnoreCase)
        || result.LoginStatus.Equals("Success", StringComparison.OrdinalIgnoreCase);

    private static string TokenFrom(LoginResponse result) =>
        !string.IsNullOrWhiteSpace(result.Token)
            ? result.Token
            : result.SessionToken;

    private static ErrorResult Error(LoginResponse result) =>
        ErrorResult.Create(!string.IsNullOrWhiteSpace(result.Error)
            ? result.Error
            : result.LoginStatus);
}
