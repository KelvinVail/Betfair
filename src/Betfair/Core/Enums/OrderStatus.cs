using Betfair.Api.Betting.Enums;

namespace Betfair.Core.Enums;

[JsonConverter(typeof(SnakeCaseEnumJsonConverter<OrderStatus>))]
public enum OrderStatus
{
    /// <summary>Unknown or not yet set.</summary>
    Unknown = 0,

    /// <summary>
    /// An asynchronous order is yet to be processed.
    /// </summary>
    Pending,

    /// <summary>
    /// An order that does not have any remaining unmatched portion.
    /// </summary>
    ExecutionComplete,

    /// <summary>
    /// An order that has a remaining unmatched portion.
    /// </summary>
    Executable,

    /// <summary>
    /// The order is no longer available for execution due to its time in force constraint.
    /// </summary>
    Expired,
}
