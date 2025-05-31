﻿namespace Betfair.Api.Requests.Account;

internal class CurrencyRatesRequest
{
    [JsonPropertyName("fromCurrency")]
    public string? FromCurrency { get; set; }
}
