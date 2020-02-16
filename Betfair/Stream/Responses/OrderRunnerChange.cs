namespace Betfair.Stream.Responses
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    [DataContract]
    public class OrderRunnerChange
    {
        [DataMember(Name = "mb", EmitDefaultValue = false)]
        public List<List<double?>> MatchedBacks { get; set; }

        [DataMember(Name = "id", EmitDefaultValue = false)]
        public long? SelectionId { get; set; }

        [DataMember(Name = "hc", EmitDefaultValue = false)]
        public double? Handicap { get; set; }

        [DataMember(Name = "fullImage", EmitDefaultValue = false)]
        public bool? FullImage { get; set; }

        [DataMember(Name = "ml", EmitDefaultValue = false)]
        public List<List<double?>> MatchedLays { get; set; }

        [DataMember(Name = "uo", EmitDefaultValue = false)]
        public List<UnmatchedOrder> UnmatchedOrders { get; set; }
    }
}
