namespace Betfair.Stream.Responses
{
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    [DataContract]
    public class OrderChange
    {
        [DataMember(Name = "id", EmitDefaultValue = false)]
        public string MarketId { get; set; }

        [DataMember(Name = "accountId", EmitDefaultValue = false)]
        public long? AccountId { get; set; }

        [DataMember(Name = "closed", EmitDefaultValue = false)]
        public bool? Closed { get; set; }

        [DataMember(Name = "orc", EmitDefaultValue = false)]
        public List<OrderRunnerChange> OrderRunnerChanges { get; set; }
    }
}
