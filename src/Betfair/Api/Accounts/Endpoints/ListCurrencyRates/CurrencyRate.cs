namespace Betfair.Api.Accounts.Endpoints.ListCurrencyRates;

/// <summary>
/// Currency rate.
/// </summary>
public class CurrencyRate
{
    /// <summary>
    /// Gets the three-letter ISO 4217 code.
    /// </summary>
    public string? CurrencyCode { get; init; }

    /// <summary>
    /// Gets the exchange rate for the currency specified in the request.
    /// </summary>
    public double Rate { get; init; }
}
