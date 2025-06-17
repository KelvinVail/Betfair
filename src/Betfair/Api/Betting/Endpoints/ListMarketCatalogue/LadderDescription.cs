namespace Betfair.Api.Betting.Endpoints.ListMarketCatalogue;

public sealed class LadderDescription
{
    [JsonPropertyName("type")]
    public string Type { get; init; } = string.Empty;
}
