namespace Betfair.Stream.Responses
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    [DataContract]
    public class MarketChange
    {
        [DataMember(Name = "id", EmitDefaultValue = false)]
        public string MarketId { get; set; }

        [DataMember(Name = "marketDefinition", EmitDefaultValue = false)]
        public MarketDefinition MarketDefinition { get; set; }

        [DataMember(Name = "rc", EmitDefaultValue = false)]
        public List<RunnerChange> RunnerChanges { get; set; }

        [DataMember(Name = "img", EmitDefaultValue = false)]
        public bool? ReplaceCache { get; set; }

        [DataMember(Name = "tv", EmitDefaultValue = false)]
        public double? TotalAmountMatched { get; set; }

        [DataMember(Name = "con", EmitDefaultValue = false)]
        public bool? Conflated { get; set; }
    }
}
