namespace Betfair.Api.Betting.Endpoints.ListCurrentOrders.Responses;

/// <summary>
/// Price and size.
/// </summary>
public class PriceSize
{
    /// <summary>
    /// Gets the price.
    /// </summary>
    public double Price { get; init; }

    /// <summary>
    /// Gets the size.
    /// </summary>
    public double Size { get; init; }
}
