using System.Runtime.Serialization;

namespace Betfair.Stream.Responses
{
    [DataContract]
    public class UnmatchedOrder
    {
        [DataMember(Name = "side", EmitDefaultValue = false)]
        public string Side { get; set; }

        [DataMember(Name = "pt", EmitDefaultValue = false)]
        public string PersistenceType { get; set; }

        [DataMember(Name = "ot", EmitDefaultValue = false)]
        public string OrderType { get; set; }

        [DataMember(Name = "status", EmitDefaultValue = false)]
        public string OrderStatus { get; set; }

        [DataMember(Name = "sv", EmitDefaultValue = false)]
        public double? SizeVoided { get; set; }

        [DataMember(Name = "p", EmitDefaultValue = false)]
        public double? Price { get; set; }

        [DataMember(Name = "sc", EmitDefaultValue = false)]
        public double? SizeCancelled { get; set; }

        [DataMember(Name = "rc", EmitDefaultValue = false)]
        public string RegulatorCode { get; set; }

        [DataMember(Name = "s", EmitDefaultValue = false)]
        public double? Size { get; set; }

        [DataMember(Name = "pd", EmitDefaultValue = false)]
        public long? PlacedDate { get; set; }

        [DataMember(Name = "rac", EmitDefaultValue = false)]
        public string RegulatorAuthCode { get; set; }

        [DataMember(Name = "md", EmitDefaultValue = false)]
        public long? MatchedDate { get; set; }

        [DataMember(Name = "sl", EmitDefaultValue = false)]
        public double? SizeLapsed { get; set; }

        [DataMember(Name = "avp", EmitDefaultValue = false)]
        public double? AveragePriceMatched { get; set; }

        [DataMember(Name = "sm", EmitDefaultValue = false)]
        public double? SizeMatched { get; set; }

        [DataMember(Name = "id", EmitDefaultValue = false)]
        public string BetId { get; set; }

        [DataMember(Name = "bsp", EmitDefaultValue = false)]
        public double? BspLiability { get; set; }

        [DataMember(Name = "sr", EmitDefaultValue = false)]
        public double? SizeRemaining { get; set; }
    }
}
