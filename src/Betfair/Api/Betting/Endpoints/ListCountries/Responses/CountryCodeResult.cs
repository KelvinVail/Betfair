namespace Betfair.Api.Betting.Endpoints.ListCountries.Responses;

/// <summary>
/// Country code result.
/// </summary>
public class CountryCodeResult
{
    /// <summary>
    /// Gets the ISO-2 code for the event.  A list of ISO-2 codes is available via http://en.wikipedia.org/wiki/ISO_3166-1_alpha-2.
    /// </summary>
    public string? CountryCode { get; init; }

    /// <summary>
    /// Gets the count of markets associated with this country.
    /// </summary>
    public int MarketCount { get; init; }
}
