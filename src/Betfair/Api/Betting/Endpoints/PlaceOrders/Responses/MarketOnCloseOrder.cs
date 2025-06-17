namespace Betfair.Api.Betting.Endpoints.PlaceOrders.Responses;

/// <summary>
/// Place a new MARKET_ON_CLOSE bet.
/// </summary>
public class MarketOnCloseOrder
{
    /// <summary>
    /// Gets or sets the liability of the order.
    /// The size of the bet.
    /// </summary>
    public double Liability { get; set; }
}

