﻿namespace Betfair.Api.Responses.Account;

/// <summary>
/// Currency rate.
/// </summary>
public class CurrencyRate
{
    /// <summary>
    /// Gets the currency code.
    /// </summary>
    public string? CurrencyCode { get; internal set; }

    /// <summary>
    /// Gets the rate.
    /// </summary>
    public double Rate { get; internal set; }
}
