namespace Betfair.Api.Responses;

/// <summary>
/// Market description.
/// </summary>
public sealed class MarketDescription
{
    /// <summary>
    /// Gets a value indicating whether persistence is enabled for this market.
    /// </summary>
    [JsonPropertyName("persistenceEnabled")]
    public bool PersistenceEnabled { get; init; }

    /// <summary>
    /// Gets a value indicating whether this market supports Betfair Starting Price betting.
    /// </summary>
    [JsonPropertyName("bspMarket")]
    public bool BspMarket { get; init; }

    /// <summary>
    /// Gets the market start time.
    /// </summary>
    [JsonPropertyName("marketTime")]
    public DateTimeOffset MarketTime { get; init; }

    /// <summary>
    /// Gets the market suspend time.
    /// </summary>
    [JsonPropertyName("suspendTime")]
    public DateTimeOffset SuspendTime { get; init; }

    /// <summary>
    /// Gets the betting type (e.g. ODDS, LINE, RANGE, ASIAN_HANDICAP_DOUBLE_LINE, ASIAN_HANDICAP_SINGLE_LINE).
    /// </summary>
    [JsonPropertyName("bettingType")]
    public string BettingType { get; init; } = string.Empty;

    /// <summary>
    /// Gets a value indicating whether the market will turn in-play.
    /// </summary>
    [JsonPropertyName("turnInPlayEnabled")]
    public bool TurnInPlayEnabled { get; init; }

    /// <summary>
    /// Gets the market type.
    /// </summary>
    [JsonPropertyName("marketType")]
    public string MarketType { get; init; } = string.Empty;

    /// <summary>
    /// Gets the regulator of the market.
    /// </summary>
    [JsonPropertyName("regulator")]
    public string Regulator { get; init; } = string.Empty;

    /// <summary>
    /// Gets the market base rate.
    /// </summary>
    [JsonPropertyName("marketBaseRate")]
    public decimal MarketBaseRate { get; init; }

    /// <summary>
    /// Gets a value indicating whether discount is allowed on this market.
    /// </summary>
    [JsonPropertyName("discountAllowed")]
    public bool DiscountAllowed { get; init; }

    /// <summary>
    /// Gets the wallet associated with this market.
    /// </summary>
    [JsonPropertyName("wallet")]
    public string Wallet { get; init; } = string.Empty;

    /// <summary>
    /// Gets the rules for this market.
    /// </summary>
    [JsonPropertyName("rules")]
    public string Rules { get; init; } = string.Empty;

    /// <summary>
    /// Gets a value indicating whether the rules have a date.
    /// </summary>
    [JsonPropertyName("rulesHasDate")]
    public bool RulesHasDate { get; init; }

    /// <summary>
    /// Gets any clarifications for this market.
    /// </summary>
    [JsonPropertyName("clarifications")]
    public string Clarifications { get; init; } = string.Empty;

    /// <summary>
    /// Gets the race type (for horse racing markets).
    /// </summary>
    [JsonPropertyName("raceType")]
    public string RaceType { get; init; } = string.Empty;

    /// <summary>
    /// Gets the price ladder description.
    /// </summary>
    [JsonPropertyName("priceLadderDescription")]
    public LadderDescription PriceLadderDescription { get; init; } = new ();
}
