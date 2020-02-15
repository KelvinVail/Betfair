namespace Betfair.Stream.Responses
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    [DataContract]
    public class MarketDefinition
    {
        [DataMember(Name = "bspMarket", EmitDefaultValue = false)]
        public bool? BspMarket { get; set; }

        [DataMember(Name = "turnInPlayEnabled", EmitDefaultValue = false)]
        public bool? TurnInPlayEnabled { get; set; }

        [DataMember(Name = "persistenceEnabled", EmitDefaultValue = false)]
        public bool? PersistenceEnabled { get; set; }

        [DataMember(Name = "marketBaseRate", EmitDefaultValue = false)]
        public double? MarketBaseRate { get; set; }

        [DataMember(Name = "bettingType", EmitDefaultValue = false)]
        public string BettingType { get; set; }

        [DataMember(Name = "status", EmitDefaultValue = false)]
        public string Status { get; set; }

        [DataMember(Name = "venue", EmitDefaultValue = false)]
        public string Venue { get; set; }

        [DataMember(Name = "settledTime", EmitDefaultValue = false)]
        public DateTime? SettledTime { get; set; }

        [DataMember(Name = "timezone", EmitDefaultValue = false)]
        public string Timezone { get; set; }

        [DataMember(Name = "eachWayDivisor", EmitDefaultValue = false)]
        public double? EachWayDivisor { get; set; }

        [DataMember(Name = "regulators", EmitDefaultValue = false)]
        public List<string> Regulators { get; set; }

        [DataMember(Name = "marketType", EmitDefaultValue = false)]
        public string MarketType { get; set; }

        [DataMember(Name = "numberOfWinners", EmitDefaultValue = false)]
        public int? NumberOfWinners { get; set; }

        [DataMember(Name = "countryCode", EmitDefaultValue = false)]
        public string CountryCode { get; set; }

        [DataMember(Name = "inPlay", EmitDefaultValue = false)]
        public bool? InPlay { get; set; }

        [DataMember(Name = "betDelay", EmitDefaultValue = false)]
        public int? BetDelay { get; set; }

        [DataMember(Name = "numberOfActiveRunners", EmitDefaultValue = false)]
        public int? NumberOfActiveRunners { get; set; }

        [DataMember(Name = "eventId", EmitDefaultValue = false)]
        public string EventId { get; set; }

        [DataMember(Name = "crossMatching", EmitDefaultValue = false)]
        public bool? CrossMatching { get; set; }

        [DataMember(Name = "runnersVoidable", EmitDefaultValue = false)]
        public bool? RunnersVoidable { get; set; }

        [DataMember(Name = "suspendTime", EmitDefaultValue = false)]
        public DateTime? SuspendTime { get; set; }

        [DataMember(Name = "discountAllowed", EmitDefaultValue = false)]
        public bool? DiscountAllowed { get; set; }

        [DataMember(Name = "runners", EmitDefaultValue = false)]
        public List<RunnerDefinition> Runners { get; set; }

        [DataMember(Name = "version", EmitDefaultValue = false)]
        public long? Version { get; set; }

        [DataMember(Name = "eventTypeId", EmitDefaultValue = false)]
        public string EventTypeId { get; set; }

        [DataMember(Name = "complete", EmitDefaultValue = false)]
        public bool? Complete { get; set; }

        [DataMember(Name = "openDate", EmitDefaultValue = false)]
        public DateTime? OpenDate { get; set; }

        [DataMember(Name = "marketTime", EmitDefaultValue = false)]
        public DateTime? MarketTime { get; set; }

        [DataMember(Name = "bspReconciled", EmitDefaultValue = false)]
        public bool? BspReconciled { get; set; }
    }
}
