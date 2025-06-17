﻿namespace Betfair.Api.Accounts.Endpoints.GetAccountDetails;

/// <summary>
/// Account details response.
/// </summary>
public class AccountDetailsResponse
{
    /// <summary>
    /// Gets the currency code.
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
    /// Gets the region.
    /// </summary>
    public string? Region { get; init; }

    /// <summary>
    /// Gets the timezone.
    /// </summary>
    public string? Timezone { get; init; }

    /// <summary>
    /// Gets the discount rate.
    /// </summary>
    public double DiscountRate { get; init; }

    /// <summary>
    /// Gets the points balance.
    /// </summary>
    public int PointsBalance { get; init; }

    /// <summary>
    /// Gets the country code.
    /// </summary>
    public string? CountryCode { get; init; }
}
