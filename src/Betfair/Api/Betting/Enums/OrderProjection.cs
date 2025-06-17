namespace Betfair.Api.Betting.Enums;

/// <summary>
/// Order projection options for market book requests.
/// </summary>
[JsonConverter(typeof(SnakeCaseEnumJsonConverter<OrderProjection>))]
public enum OrderProjection
{
    /// <summary>
    /// EXECUTABLE and EXECUTION_COMPLETE orders.
    /// </summary>
    All,

    /// <summary>
    /// An order that has a remaining unmatched portion.
    /// This is either a fully unmatched or partially matched bet (order).
    /// </summary>
    Executable,

    /// <summary>
    /// An order that does not have any remaining unmatched portion.
    /// This is a fully matched bet (order).
    /// </summary>
    ExecutionComplete,
}
