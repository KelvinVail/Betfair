using System.Runtime.Serialization;
using MemoryPack;

namespace Betfair.Benchmarks.Responses;

[MemoryPackable]
public partial class StatusMessage
{
    [DataMember(Name = "id")]
    public int Id { get; set; }

    [DataMember(Name = "connectionClosed")]
    public bool IsClosed { get; set; }
}
