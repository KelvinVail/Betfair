using Betfair.Core.Client;

namespace Betfair.Core.Login;

internal sealed class TokenProvider
{
    private readonly BetfairHttpClient _client;
    private readonly Credentials _credentials;

    public TokenProvider(BetfairHttpClient client, Credentials credentials)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _credentials = credentials ?? throw new ArgumentNullException(nameof(credentials));
    }

    public async Task<string> GetToken(CancellationToken cancellationToken)
    {
        using var request = _credentials.GetLoginRequest();

        var response = await _client.SendAsync(request, cancellationToken);
        var result = await JsonSerializer.DeserializeAsync<LoginResponse>(
            await response.Content.ReadAsStreamAsync(cancellationToken),
            StandardResolver.AllowPrivateExcludeNullCamelCase);

        if (LoginIsSuccess(result))
            return TokenFrom(result);

        throw Error(result);
    }

    private static bool LoginIsSuccess(LoginResponse result) =>
        result.Status.Equals("success", StringComparison.OrdinalIgnoreCase)
        || result.LoginStatus.Equals("Success", StringComparison.OrdinalIgnoreCase);

    private static string TokenFrom(LoginResponse result) =>
        !string.IsNullOrWhiteSpace(result.Token)
            ? result.Token
            : result.SessionToken;

    private static BetfairRequestException Error(LoginResponse result) =>
        new (
            !string.IsNullOrWhiteSpace(result.Error)
            ? result.Error
            : result.LoginStatus);
}
