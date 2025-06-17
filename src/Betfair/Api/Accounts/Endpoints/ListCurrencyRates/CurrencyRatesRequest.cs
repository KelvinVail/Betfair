namespace Betfair.Api.Accounts.Endpoints.ListCurrencyRates;

internal class CurrencyRatesRequest
{
    [JsonPropertyName("fromCurrency")]
    public string? FromCurrency { get; set; }
}
