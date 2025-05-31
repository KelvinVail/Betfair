﻿namespace Betfair.Api.Requests.Account;

internal class AccountFundsRequest
{
    [JsonPropertyName("wallet")]
    public string? Wallet { get; set; }
}
