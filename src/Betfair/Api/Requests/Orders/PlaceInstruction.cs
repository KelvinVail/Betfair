using Betfair.Core;

namespace Betfair.Api.Requests.Orders;

public class PlaceInstruction
{
    /// <summary>
    /// Gets or sets the Selection ID.
    /// </summary>
    public long SelectionId { get; set; }

    /// <summary>
    /// Gets or sets the order Side.
    /// Back or Lay.
    /// </summary>
    public Side Side { get; set; } = Side.Back;

    /// <summary>
    /// Gets or sets the Order Type.
    /// LIMIT, LIMIT_ON_CLOSE or MARKET_ON_CLOSE.
    /// </summary>
    public OrderType OrderType { get; set; } = OrderType.Limit;

    /// <summary>
    /// Gets or sets the Limit Order.
    /// </summary>
    public LimitOrder? LimitOrder { get; set; }

    /// <summary>
    /// Gets or sets the Limit On Close order.
    /// </summary>
    public LimitOnCloseOrder? LimitOnClose { get; set; }

    /// <summary>
    /// Gets or sets the Market On Close order.
    /// </summary>
    public MarketOnCloseOrder? MarketOnClose { get; set; }

    /// <summary>
    /// Gets or sets the Handicap.
    /// </summary>
    public double? Handicap { get; set; }

    /// <summary>
    /// Gets or sets the Customer Order Ref.
    /// An optional reference customers can set to identify instructions.
    /// No validation will be done on uniqueness and the string is limited to 32 characters.
    /// If an empty string is provided it will be treated as null.
    /// </summary>
    public string? CustomerOrderRef { get; set; }
}
