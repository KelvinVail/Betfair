using System;
using System.Runtime.Serialization;

namespace Betfair.Stream.Responses
{
    [DataContract]
    public class RunnerDefinition
    {
        [DataMember(Name = "status", EmitDefaultValue = false)]
        public string Status { get; set; }

        [DataMember(Name = "sortPriority", EmitDefaultValue = false)]
        public int? SortPriority { get; set; }

        [DataMember(Name = "removalDate", EmitDefaultValue = false)]
        public DateTime? RemovalDate { get; set; }

        [DataMember(Name = "id", EmitDefaultValue = false)]
        public long? SelectionId { get; set; }

        [DataMember(Name = "hc", EmitDefaultValue = false)]
        public double? Handicap { get; set; }

        [DataMember(Name = "adjustmentFactor", EmitDefaultValue = false)]
        public double? AdjustmentFactor { get; set; }

        [DataMember(Name = "bsp", EmitDefaultValue = false)]
        public double? BspLiability { get; set; }
    }
}
