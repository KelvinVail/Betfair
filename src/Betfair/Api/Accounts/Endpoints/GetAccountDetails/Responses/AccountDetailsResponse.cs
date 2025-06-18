namespace Betfair.Api.Accounts.Endpoints.GetAccountDetails.Responses;

/// <summary>
/// Account details response.
/// </summary>
public class AccountDetailsResponse
{
    /// <summary>
    /// Gets the default user currency code.
    /// </summary>
    public string? CurrencyCode { get; init; }

    /// <summary>
    /// Gets the first name.
    /// </summary>
    public string? FirstName { get; init; }

    /// <summary>
    /// Gets the last name.
    /// </summary>
    public string? LastName { get; init; }

    /// <summary>
    /// Gets the locale code.
    /// </summary>
    public string? LocaleCode { get; init; }

    /// <summary>
    /// Gets the region based on users zip/postcode (ISO 3166-1 alpha-3 format).
    /// Defaults to GBR if zip/postcode cannot be identified.
    /// </summary>
    public string? Region { get; init; }

    /// <summary>
    /// Gets the timezone.
    /// </summary>
    public string? Timezone { get; init; }

    /// <summary>
    /// Gets the user discount rate.
    /// Please note: Betfair AUS/NZ customers should not rely on this to determine their discount rates which are now applied at the account level.
    /// </summary>
    public double DiscountRate { get; init; }

    /// <summary>
    /// Gets the Betfair points balance.
    /// </summary>
    public int PointsBalance { get; init; }

    /// <summary>
    /// Gets the customer's country of residence (ISO 2 Char format).
    /// </summary>
    public string? CountryCode { get; init; }
}
