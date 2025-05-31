﻿namespace Betfair.Api.Responses.Account;

/// <summary>
/// Account funds response.
/// </summary>
public class AccountFundsResponse
{
    /// <summary>
    /// Gets the available to bet amount.
    /// </summary>
    public double AvailableToBetBalance { get; internal set; }

    /// <summary>
    /// Gets the exposure.
    /// </summary>
    public double Exposure { get; internal set; }

    /// <summary>
    /// Gets the retained commission.
    /// </summary>
    public double RetainedCommission { get; internal set; }

    /// <summary>
    /// Gets the exposure limit.
    /// </summary>
    public double ExposureLimit { get; internal set; }

    /// <summary>
    /// Gets the discount rate.
    /// </summary>
    public double DiscountRate { get; internal set; }

    /// <summary>
    /// Gets the points balance.
    /// </summary>
    public int PointsBalance { get; internal set; }

    /// <summary>
    /// Gets the wallet.
    /// </summary>
    public string? Wallet { get; internal set; }
}
