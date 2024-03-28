namespace Betfair.Benchmarks.JsonBenchmarks.Responses;

public struct MarketDefinition
{
    [JsonPropertyName("bspMarket")]
    public bool? BspMarket { get; init; }

    [JsonPropertyName("turnInPlayEnabled")]
    public bool? TurnInPlayEnabled { get; init; }

    [JsonPropertyName("persistenceEnabled")]
    public bool? PersistenceEnabled { get; init; }

    [JsonPropertyName("marketBaseRate")]
    public double? MarketBaseRate { get; init; }

    [JsonPropertyName("bettingType")]
    public string? BettingType { get; init; }

    [JsonPropertyName("status")]
    public string? Status { get; init; }

    [JsonPropertyName("venue")]
    public string? Venue { get; init; }

    [JsonPropertyName("settledTime")]
    public DateTime? SettledTime { get; init; }

    [JsonPropertyName("timezone")]
    public string? Timezone { get; init; }

    [JsonPropertyName("eachWayDivisor")]
    public double? EachWayDivisor { get; init; }

    [JsonPropertyName("regulators")]
    public List<string>? Regulators { get; init; }

    [JsonPropertyName("marketType")]
    public string? MarketType { get; init; }

    [JsonPropertyName("numberOfWinners")]
    public int? NumberOfWinners { get; init; }

    [JsonPropertyName("countryCode")]
    public string? CountryCode { get; init; }

    [JsonPropertyName("inPlay")]
    public bool? InPlay { get; init; }

    [JsonPropertyName("betDelay")]
    public int? BetDelay { get; init; }

    [JsonPropertyName("numberOfActiveRunners")]
    public int? NumberOfActiveRunners { get; init; }

    [JsonPropertyName("eventId")]
    public string? EventId { get; init; }

    [JsonPropertyName("crossMatching")]
    public bool? CrossMatching { get; init; }

    [JsonPropertyName("runnersVoidable")]
    public bool? RunnersVoidable { get; init; }

    [JsonPropertyName("suspendTime")]
    public DateTime? SuspendTime { get; init; }

    [JsonPropertyName("discountAllowed")]
    public bool? DiscountAllowed { get; init; }

    [JsonPropertyName("runners")]
    public List<RunnerDefinition>? Runners { get; init; }

    [JsonPropertyName("version")]
    public long? Version { get; init; }

    [JsonPropertyName("eventTypeId")]
    public string? EventTypeId { get; init; }

    [JsonPropertyName("complete")]
    public bool? Complete { get; init; }

    [JsonPropertyName("openDate")]
    public DateTime? OpenDate { get; init; }

    [JsonPropertyName("marketTime")]
    public DateTime? MarketTime { get; init; }

    [JsonPropertyName("bspReconciled")]
    public bool? BspReconciled { get; init; }
}