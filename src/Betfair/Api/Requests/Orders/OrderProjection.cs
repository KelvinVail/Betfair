﻿namespace Betfair.Api.Requests.Orders;

/// <summary>
/// Order projection for current orders.
/// </summary>
public enum OrderProjection
{
    /// <summary>
    /// All orders.
    /// </summary>
    All,

    /// <summary>
    /// Executable orders only.
    /// </summary>
    Executable,

    /// <summary>
    /// Execution complete orders only.
    /// </summary>
    ExecutionComplete
}
