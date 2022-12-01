using Betfair.Client;

namespace Betfair.Account;

public sealed class AccountClient
{
    private const string _accountFunds = "https://api.betfair.com/exchange/account/rest/v1.0/getAccountFunds/";
    private readonly BetfairHttpClient _client;

    public AccountClient(BetfairHttpClient client) =>
        _client = client;

    public async Task<Result<AccountFunds, ErrorResult>> AccountFunds(
        string sessionToken,
        CancellationToken cancellationToken) =>
        await _client.Post<AccountFunds>(
            new Uri(_accountFunds),
            sessionToken,
            Maybe<object>.None,
            cancellationToken);
}
