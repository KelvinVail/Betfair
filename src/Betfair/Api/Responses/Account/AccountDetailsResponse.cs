﻿namespace Betfair.Api.Responses.Account;

/// <summary>
/// Account details response.
/// </summary>
public class AccountDetailsResponse
{
    /// <summary>
    /// Gets the currency code.
    /// </summary>
    public string? CurrencyCode { get; internal set; }

    /// <summary>
    /// Gets the first name.
    /// </summary>
    public string? FirstName { get; internal set; }

    /// <summary>
    /// Gets the last name.
    /// </summary>
    public string? LastName { get; internal set; }

    /// <summary>
    /// Gets the locale code.
    /// </summary>
    public string? LocaleCode { get; internal set; }

    /// <summary>
    /// Gets the region.
    /// </summary>
    public string? Region { get; internal set; }

    /// <summary>
    /// Gets the timezone.
    /// </summary>
    public string? Timezone { get; internal set; }

    /// <summary>
    /// Gets the discount rate.
    /// </summary>
    public double DiscountRate { get; internal set; }

    /// <summary>
    /// Gets the points balance.
    /// </summary>
    public int PointsBalance { get; internal set; }

    /// <summary>
    /// Gets the country code.
    /// </summary>
    public string? CountryCode { get; internal set; }
}
