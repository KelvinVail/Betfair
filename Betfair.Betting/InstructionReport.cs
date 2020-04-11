namespace Betfair.Betting
{
    using System.Runtime.Serialization;

    [System.Diagnostics.CodeAnalysis.SuppressMessage(
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
    }
}
