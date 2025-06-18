﻿namespace Betfair.Api.Accounts.Endpoints.GetAccountFunds.Responses;

/// <summary>
/// Account funds response.
/// </summary>
public class AccountFundsResponse
{
    /// <summary>
    /// Gets the amount available to bet.
    /// </summary>
    public double AvailableToBetBalance { get; init; }

    /// <summary>
    /// Gets the current exposure.
    /// </summary>
    public double Exposure { get; init; }

    /// <summary>
    /// Gets the sum of retained commission.
    /// </summary>
    public double RetainedCommission { get; init; }

    /// <summary>
    /// Gets the exposure limit.
    /// </summary>
    public double ExposureLimit { get; init; }

    /// <summary>
    /// Gets the user discount rate.
    /// Please note: Betfair AUS/NZ customers should not rely on this to determine their discount rates which are now applied at the account level.
    /// </summary>
    public double DiscountRate { get; init; }

    /// <summary>
    /// Gets the Betfair points balance.
    /// </summary>
    public int PointsBalance { get; init; }
}
