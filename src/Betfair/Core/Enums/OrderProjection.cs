namespace Betfair.Core.Enums;

/// <summary>
/// Order projection options for market book requests.
/// </summary>
[JsonConverter(typeof(SnakeCaseEnumJsonConverter<OrderProjection>))]
public enum OrderProjection
{
    /// <summary>
    /// Return all orders.
    /// </summary>
    All,

    /// <summary>
    /// Return only executable orders.
    /// </summary>
    Executable,

    /// <summary>
    /// Return only execution complete orders.
    /// </summary>
    ExecutionComplete,
}
