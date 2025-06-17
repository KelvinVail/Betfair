using Betfair.Api.Betting.Endpoints.ListCurrentOrders.Enums;
using Betfair.Api.Betting.Enums;

namespace Betfair.Api.Betting.Endpoints.ListCurrentOrders.Responses;

/// <summary>
/// Current order.
/// </summary>
public class CurrentOrder
{
    /// <summary>
    /// Gets the bet ID of the original place order.
    /// </summary>
    public string BetId { get; init; } = string.Empty;

    /// <summary>
    /// Gets the market id the order is for.
    /// </summary>
    public string MarketId { get; init; } = string.Empty;

    /// <summary>
    /// Gets the selection id the order is for.
    /// </summary>
    public long SelectionId { get; init; }

    /// <summary>
    /// Gets the handicap associated with the runner in case of Asian handicap markets, null otherwise.
    /// </summary>
    public double? Handicap { get; init; }

    /// <summary>
    /// Gets the price and size.
    /// </summary>
    public PriceSize PriceSize { get; init; }

    /// <summary>
    /// Gets the BSP liability.
    /// Not to be confused with size.
    /// This is the liability of a given BSP bet.
    /// </summary>
    public double BspLiability { get; init; }

    /// <summary>
    /// Gets the side.
    /// BACK/LAY.
    /// </summary>
    public Side Side { get; init; }

    /// <summary>
    /// Gets the status.
    /// Either EXECUTABLE (an unmatched amount remains) or EXECUTION_COMPLETE (no unmatched amount remains).
    /// </summary>
    public OrderStatus Status { get; init; }

    /// <summary>
    /// Gets the persistence type.
    /// What to do with the order at turn-in-play.
    /// </summary>
    public PersistenceType PersistenceType { get; init; }

    /// <summary>
    /// Gets the order type.
    /// BSP Order type.
    /// </summary>
    public OrderType OrderType { get; init; }

    /// <summary>
    /// Gets the date, to the second, the bet was placed.
    /// </summary>
    public DateTimeOffset PlacedDate { get; init; }

    /// <summary>
    /// Gets the date, to the second, of the last matched bet fragment (where applicable).
    /// </summary>
    public DateTimeOffset MatchedDate { get; init; }

    /// <summary>
    /// Gets the average price matched at.
    /// Voided match fragments are removed from this average calculation.
    /// The price is automatically adjusted in the event of non runners being declared with applicable reduction factors.
    /// Please note: This value is not meaningful for activity on LINE markets and is not guaranteed to be returned or maintained for these markets.
    /// </summary>
    public double AveragePriceMatched { get; init; }

    /// <summary>
    /// Gets the current amount of this bet that was matched.
    /// </summary>
    public double SizeMatched { get; init; }

    /// <summary>
    /// Gets the current amount of this bet that is unmatched.
    /// </summary>
    public double SizeRemaining { get; init; }

    /// <summary>
    /// Gets the current amount of this bet that was lapsed.
    /// </summary>
    public double SizeLapsed { get; init; }

    /// <summary>
    /// Gets the current amount of this bet that was cancelled.
    /// </summary>
    public double SizeCancelled { get; init; }

    /// <summary>
    /// Gets the current amount of this bet that was voided.
    /// </summary>
    public double SizeVoided { get; init; }

    /// <summary>
    /// Gets the regulator authorisation code.
    /// </summary>
    public string? RegulatorAuthCode { get; init; }

    /// <summary>
    /// Gets the regulator code.
    /// </summary>
    public string? RegulatorCode { get; init; }

    /// <summary>
    /// Gets the order reference defined by the customer for this bet.
    /// </summary>
    public string? CustomerOrderRef { get; init; }

    /// <summary>
    /// Gets the strategy reference defined by the customer for this bet.
    /// </summary>
    public string? CustomerStrategyRef { get; init; }
}

