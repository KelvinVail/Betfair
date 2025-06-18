namespace Betfair.Api.Accounts.Endpoints.ListCurrencyRates.Requests;

internal class CurrencyRatesRequest
{
    [JsonPropertyName("fromCurrency")]
    public string? FromCurrency { get; set; }
}
