using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Betfair.Stream.Responses
{
    [DataContract]
    public class RunnerChange
    {
        [DataMember(Name = "tv", EmitDefaultValue = false)]
        public double? TotalMatched { get; set; }

        [DataMember(Name = "batb", EmitDefaultValue = false)]
        public List<List<double?>> BestAvailableToBack { get; set; }

        [DataMember(Name = "spb", EmitDefaultValue = false)]
        public List<List<double?>> StartingPriceBack { get; set; }

        [DataMember(Name = "bdatl", EmitDefaultValue = false)]
        public List<List<double?>> BestDisplayAvailableToLay { get; set; }

        [DataMember(Name = "trd", EmitDefaultValue = false)]
        public List<List<double?>> Traded { get; set; }

        [DataMember(Name = "spf", EmitDefaultValue = false)]
        public double? StartingPriceFar { get; set; }

        [DataMember(Name = "ltp", EmitDefaultValue = false)]
        public double? LastTradedPrice { get; set; }

        [DataMember(Name = "atb", EmitDefaultValue = false)]
        public List<List<double?>> AvailableToBack { get; set; }

        [DataMember(Name = "spl", EmitDefaultValue = false)]
        public List<List<double?>> StartingPriceLay { get; set; }

        [DataMember(Name = "spn", EmitDefaultValue = false)]
        public double? StartingPriceNear { get; set; }

        [DataMember(Name = "atl", EmitDefaultValue = false)]
        public List<List<double?>> AvailableToLay { get; set; }

        [DataMember(Name = "batl", EmitDefaultValue = false)]
        public List<List<double?>> BestAvailableToLay { get; set; }

        [DataMember(Name = "id", EmitDefaultValue = false)]
        public long? SelectionId { get; set; }

        [DataMember(Name = "hc", EmitDefaultValue = false)]
        public double? Handicap { get; set; }

        [DataMember(Name = "bdatb", EmitDefaultValue = false)]
        public List<List<double?>> BestDisplayAvailableToBack { get; set; }
    }
}
