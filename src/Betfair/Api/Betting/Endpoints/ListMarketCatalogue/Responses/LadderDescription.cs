namespace Betfair.Api.Betting.Endpoints.ListMarketCatalogue.Responses;

public sealed class LadderDescription
{
    [JsonPropertyName("type")]
    public string Type { get; init; } = string.Empty;
}
