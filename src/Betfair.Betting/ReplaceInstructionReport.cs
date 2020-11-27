namespace Betfair.Betting
{
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.Serialization;

    [SuppressMessage(
        "Microsoft.Performance",
        "CA1812:AvoidUninstantiatedInternalClasses",
        Justification = "Used to deserialize Json.")]
    [DataContract]
    internal sealed class ReplaceInstructionReport
    {
        [DataMember(Name = "placeInstructionReport", EmitDefaultValue = false)]
        public InstructionReport PlaceInstructionReport { get; set; }
    }
}