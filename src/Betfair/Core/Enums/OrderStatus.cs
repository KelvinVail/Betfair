namespace Betfair.Core.Enums;

/// <summary>
/// Optionally restricts the results to the specified order status.
/// </summary>
[JsonConverter(typeof(UpperCaseEnumJsonConverter<OrderStatus>))]
public enum OrderStatus
{
    /// <summary>
    /// EXECUTABLE and EXECUTION_COMPLETE orders.
    /// </summary>
    All,

    /// <summary>
    /// An order that has a remaining unmatched portion. This is either a fully unmatched or partially matched bet (order).
    /// </summary>
    Executable,

    /// <summary>
    /// An order that does not have any remaining unmatched portion.  This is a fully matched bet (order).
    /// </summary>
    ExecutionComplete,
}
