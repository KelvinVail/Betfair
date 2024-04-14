namespace Betfair.Api.Requests.OrderDtos;

public class PlaceInstruction
{
    /// <summary>
    /// Gets or sets the Selection Id.
    /// </summary>
    public long SelectionId { get; set; }

    /// <summary>
    /// Gets or sets the order Side.
    /// Back or Lay.
    /// </summary>
    public string Side { get; set; } = "BACK";

    /// <summary>
    /// Gets or sets the Order Type.
    /// LIMIT, LIMIT_ON_CLOSE or MARKET_ON_CLOSE.
    /// </summary>
    public string OrderType { get; set; } = "LIMIT";

    /// <summary>
    /// Gets or sets the Limit Order.
    /// </summary>
    public LimitOrder? LimitOrder { get; set; }

    /// <summary>
    /// Gets or sets the Handicap.
    /// </summary>
    public double? Handicap { get; set; }

    /// <summary>
    /// Gets or sets the Customer Order Ref.
    /// </summary>
    public string? CustomerOrderRef { get; set; }
}
