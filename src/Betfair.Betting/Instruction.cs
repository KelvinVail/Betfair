namespace Betfair.Betting
{
    using System.Runtime.Serialization;

    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Microsoft.Performance",
        "CA1812:AvoidUninstantiatedInternalClasses",
        Justification = "Used to deserialize Json.")]
    [DataContract]
    internal sealed class Instruction
    {
        [DataMember(Name = "selectionId", EmitDefaultValue = false)]
        internal long SelectionId { get; set; }

        [DataMember(Name = "side", EmitDefaultValue = false)]
        internal Side Side { get; set; }
    }
}
