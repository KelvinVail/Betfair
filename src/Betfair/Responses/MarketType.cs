using System.Runtime.Serialization;

namespace Betfair.Responses;

public class MarketType
{
    [DataMember(Name = "marketType")]
    public string Id { get; set; } = string.Empty;

    public int MarketCount { get; set; }
}
