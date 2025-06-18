namespace Betfair.Api.Accounts.Endpoints.GetAccountFunds.Requests;

internal class AccountFundsRequest
{
    [JsonPropertyName("wallet")]
    public string Wallet { get; } = "UK";
}
