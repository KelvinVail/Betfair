namespace Betfair.Api.Accounts.Endpoints.GetAccountFunds;

internal class AccountFundsRequest
{
    [JsonPropertyName("wallet")]
    public string? Wallet { get; set; }
}
