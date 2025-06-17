﻿namespace Betfair.Api.Accounts.Endpoints.GetAccountFunds.Responses;

/// <summary>
/// Account funds response.
/// </summary>
public class AccountFundsResponse
{
    /// <summary>
    /// Gets the available to bet amount.
    /// </summary>
    public double AvailableToBetBalance { get; init; }

    /// <summary>
    /// Gets the exposure.
    /// </summary>
    public double Exposure { get; init; }

    /// <summary>
    /// Gets the retained commission.
    /// </summary>
    public double RetainedCommission { get; init; }

    /// <summary>
    /// Gets the exposure limit.
    /// </summary>
    public double ExposureLimit { get; init; }

    /// <summary>
    /// Gets the discount rate.
    /// </summary>
    public double DiscountRate { get; init; }

    /// <summary>
    /// Gets the points balance.
    /// </summary>
    public int PointsBalance { get; init; }

    /// <summary>
    /// Gets the wallet.
    /// </summary>
    public string? Wallet { get; init; }
}
