using System.Runtime.Serialization;

namespace Betfair.Betting;

[DataContract(Name = "filter")]
public class MarketFilter
{
    public List<int> EventTypeIds { get; } = new ();
}
