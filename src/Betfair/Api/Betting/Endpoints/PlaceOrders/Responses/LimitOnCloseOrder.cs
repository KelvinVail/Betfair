namespace Betfair.Api.Betting.Endpoints.PlaceOrders.Responses;

/// <summary>
/// Place a new LIMIT_ON_CLOSE bet.
/// </summary>
public class LimitOnCloseOrder
{
    /// <summary>
    /// Gets or sets the liability of the order.
    /// The size of the bet. See Min BSP Liability.
    /// </summary>
    public double Liability { get; set; }

    /// <summary>
    /// Gets or sets the price of the order.
    /// The limit price of the bet if LOC.
    /// </summary>
    public double Price { get; set; }
}

