namespace Betfair.Api.Betting.Endpoints.ListMarketBook.Responses;

/// <summary>
/// Exchange prices available for a runner.
/// </summary>
public class ExchangePrices
{
    /// <summary>
    /// Gets the prices available to back.
    /// </summary>
    [JsonPropertyName("availableToBack")]
    public List<PriceSize>? AvailableToBack { get; init; }

    /// <summary>
    /// Gets the prices available to lay.
    /// </summary>
    [JsonPropertyName("availableToLay")]
    public List<PriceSize>? AvailableToLay { get; init; }

    /// <summary>
    /// Gets the traded volume.
    /// </summary>
    [JsonPropertyName("tradedVolume")]
    public List<PriceSize>? TradedVolume { get; init; }
}