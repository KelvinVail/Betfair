using Betfair.Api.Betting.Endpoints.PlaceOrders.Requests;

namespace Betfair.Api.Betting.Endpoints.PlaceOrders.Responses;

public class PlaceInstructionReport
{
    /// <summary>
    /// Gets or sets the status of the instruction report.
    /// Whether the command succeeded or failed.
    /// </summary>
    public string Status { get; init; } = string.Empty;

    /// <summary>
    /// Gets or sets the error code, if any, of the instruction report.
    /// Cause of failure, or null if the command succeeds.
    /// </summary>
    public string? ErrorCode { get; init; }

    /// <summary>
    /// Gets or sets the ID of the bet.
    /// The bet ID of the new bet. Will be null on failure or if the order was placed asynchronously.
    /// </summary>
    public string? BetId { get; init; }

    /// <summary>
    /// Gets or sets the date and time the bet was placed.
    /// Will be null if the order was placed asynchronously.
    /// </summary>
    public DateTimeOffset? PlacedDate { get; init; }

    /// <summary>
    /// Gets or sets the average price matched for the bet.
    /// Will be null if the order was placed asynchronously.
    /// This value is not meaningful for activity on LINE markets and is not guaranteed to be returned or maintained for these markets.
    /// </summary>
    public double? AveragePriceMatched { get; init; }

    /// <summary>
    /// Gets or sets the size matched for the bet.
    /// Will be null if the order was placed asynchronously.
    /// </summary>
    public double? SizeMatched { get; init; }

    /// <summary>
    /// Gets or sets the status of the order.
    /// The status of the order, if the instruction succeeded. If the instruction is unsuccessful, no value is provided.
    /// Please note: by default, this field is not returned for MARKET_ON_CLOSE and LIMIT_ON_CLOSE orders
    /// </summary>
    public string? OrderStatus { get; init; }

    /// <summary>
    /// Gets or sets the place instruction associated with the report.
    /// The instruction that was requested.
    /// </summary>
    public PlaceInstruction? Instruction { get; init; }
}
