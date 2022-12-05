using System.Runtime.Serialization;

namespace Betfair.Betting.Models;

public class MarketType : ValueObject
{
    [DataMember(Name = "marketType")]
    public string Id { get; set; } = string.Empty;

    public int MarketCount { get; set; }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Id;
    }
}
