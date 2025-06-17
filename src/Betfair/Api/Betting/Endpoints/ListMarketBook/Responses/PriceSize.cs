namespace Betfair.Api.Betting.Endpoints.ListMarketBook.Responses;

/// <summary>
/// Price and size.
/// </summary>
public class PriceSize
{
    /// <summary>
    /// Gets the price.
    /// </summary>
    [JsonPropertyName("price")]
    public double Price { get; init; }

    /// <summary>
    /// Gets the size.
    /// </summary>
    [JsonPropertyName("size")]
    public double Size { get; init; }
}
