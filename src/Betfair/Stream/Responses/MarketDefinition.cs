namespace Betfair.Stream.Responses;

[DataContract]
public class MarketDefinition
{
    [DataMember(Name = "bspMarket", EmitDefaultValue = false)]
    public bool? BspMarket { get; init; }

    [DataMember(Name = "turnInPlayEnabled", EmitDefaultValue = false)]
    public bool? TurnInPlayEnabled { get; init; }

    [DataMember(Name = "persistenceEnabled", EmitDefaultValue = false)]
    public bool? PersistenceEnabled { get; init; }

    [DataMember(Name = "marketBaseRate", EmitDefaultValue = false)]
    public double? MarketBaseRate { get; init; }

    [DataMember(Name = "bettingType", EmitDefaultValue = false)]
    public string? BettingType { get; init; }

    [DataMember(Name = "status", EmitDefaultValue = false)]
    public string? Status { get; init; }

    [DataMember(Name = "venue", EmitDefaultValue = false)]
    public string? Venue { get; init; }

    [DataMember(Name = "settledTime", EmitDefaultValue = false)]
    public DateTime? SettledTime { get; init; }

    [DataMember(Name = "timezone", EmitDefaultValue = false)]
    public string? Timezone { get; init; }

    [DataMember(Name = "eachWayDivisor", EmitDefaultValue = false)]
    public double? EachWayDivisor { get; init; }

    [DataMember(Name = "regulators", EmitDefaultValue = false)]
    public List<string>? Regulators { get; init; }

    [DataMember(Name = "marketType", EmitDefaultValue = false)]
    public string? MarketType { get; init; }

    [DataMember(Name = "numberOfWinners", EmitDefaultValue = false)]
    public int? NumberOfWinners { get; init; }

    [DataMember(Name = "countryCode", EmitDefaultValue = false)]
    public string? CountryCode { get; init; }

    [DataMember(Name = "inPlay", EmitDefaultValue = false)]
    public bool? InPlay { get; init; }

    [DataMember(Name = "betDelay", EmitDefaultValue = false)]
    public int? BetDelay { get; init; }

    [DataMember(Name = "numberOfActiveRunners", EmitDefaultValue = false)]
    public int? NumberOfActiveRunners { get; init; }

    [DataMember(Name = "eventId", EmitDefaultValue = false)]
    public string? EventId { get; init; }

    [DataMember(Name = "crossMatching", EmitDefaultValue = false)]
    public bool? CrossMatching { get; init; }

    [DataMember(Name = "runnersVoidable", EmitDefaultValue = false)]
    public bool? RunnersVoidable { get; init; }

    [DataMember(Name = "suspendTime", EmitDefaultValue = false)]
    public DateTime? SuspendTime { get; init; }

    [DataMember(Name = "discountAllowed", EmitDefaultValue = false)]
    public bool? DiscountAllowed { get; init; }

    [DataMember(Name = "runners", EmitDefaultValue = false)]
    public List<RunnerDefinition>? Runners { get; init; }

    [DataMember(Name = "version", EmitDefaultValue = false)]
    public long? Version { get; init; }

    [DataMember(Name = "eventTypeId", EmitDefaultValue = false)]
    public string? EventTypeId { get; init; }

    [DataMember(Name = "complete", EmitDefaultValue = false)]
    public bool? Complete { get; init; }

    [DataMember(Name = "openDate", EmitDefaultValue = false)]
    public DateTime? OpenDate { get; init; }

    [DataMember(Name = "marketTime", EmitDefaultValue = false)]
    public DateTime? MarketTime { get; init; }

    [DataMember(Name = "bspReconciled", EmitDefaultValue = false)]
    public bool? BspReconciled { get; init; }
}