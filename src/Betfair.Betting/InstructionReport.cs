using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Betfair.Betting
{
    [SuppressMessage(
        "Microsoft.Performance",
        "CA1812:AvoidUninstantiatedInternalClasses",
        Justification = "Used to deserialize Json.")]
    [DataContract]
    internal sealed class InstructionReport
    {
        [DataMember(Name = "instruction", EmitDefaultValue = false)]
        internal Instruction Instruction { get; set; }

        [DataMember(Name = "sizeMatched", EmitDefaultValue = false)]
        internal double SizeMatched { get; set; }

        [DataMember(Name = "averagePriceMatched", EmitDefaultValue = false)]
        internal double AveragePriceMatched { get; set; }

        [DataMember(Name = "status", EmitDefaultValue = false)]
        internal string Status { get; set; }

        [DataMember(Name = "errorCode", EmitDefaultValue = false)]
        internal string ErrorCode { get; set; }

        [DataMember(Name = "betId", EmitDefaultValue = false)]
        internal string BetId { get; set; }

        [DataMember(Name = "orderStatus", EmitDefaultValue = false)]
        internal string OrderStatus { get; set; }

        [DataMember(Name = "placedDate", EmitDefaultValue = false)]
        internal DateTime PlacedDate { get; set; }
    }
}
