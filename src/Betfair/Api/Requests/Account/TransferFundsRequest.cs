﻿namespace Betfair.Api.Requests.Account;

internal class TransferFundsRequest
{
    [JsonPropertyName("from")]
    public string? From { get; set; }

    [JsonPropertyName("to")]
    public string? To { get; set; }

    [JsonPropertyName("amount")]
    public double Amount { get; set; }
}
