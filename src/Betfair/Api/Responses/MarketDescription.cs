namespace Betfair.Api.Responses;

public sealed class MarketDescription
{
    [JsonPropertyName("persistenceEnabled")]
    public bool PersistenceEnabled { get; init; }

    [JsonPropertyName("bspMarket")]
    public bool BspMarket { get; init; }

    [JsonPropertyName("marketTime")]
    public DateTimeOffset MarketTime { get; init; }

    [JsonPropertyName("suspendTime")]
    public DateTimeOffset SuspendTime { get; init; }

    [JsonPropertyName("bettingType")]
    public string BettingType { get; init; } = string.Empty;

    [JsonPropertyName("turnInPlayEnabled")]
    public bool TurnInPlayEnabled { get; init; }

    [JsonPropertyName("marketType")]
    public string MarketType { get; init; } = string.Empty;

    [JsonPropertyName("regulator")]
    public string Regulator { get; init; } = string.Empty;

    [JsonPropertyName("marketBaseRate")]
    public decimal MarketBaseRate { get; init; }

    [JsonPropertyName("discountAllowed")]
    public bool DiscountAllowed { get; init; }

    [JsonPropertyName("wallet")]
    public string Wallet { get; init; } = string.Empty;

    [JsonPropertyName("rules")]
    public string Rules { get; init; } = string.Empty;

    [JsonPropertyName("rulesHasDate")]
    public bool RulesHasDate { get; init; }

    [JsonPropertyName("clarifications")]
    public string Clarifications { get; init; } = string.Empty;

    [JsonPropertyName("raceType")]
    public string RaceType { get; init; } = string.Empty;

    [JsonPropertyName("priceLadderDescription")]
    public LadderDescription PriceLadderDescription { get; init; } = new ();
}
