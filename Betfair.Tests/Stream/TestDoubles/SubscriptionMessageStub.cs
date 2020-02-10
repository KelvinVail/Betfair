namespace Betfair.Tests.Stream.TestDoubles
{
    using System.Runtime.Serialization;
    using Betfair.Streaming;
    using Newtonsoft.Json;

    [DataContract]
    public class SubscriptionMessageStub
    {
        public SubscriptionMessageStub(string operation, int id)
        {
            this.Operation = operation;
            this.Id = id;
        }

        [DataMember(Name = "op", EmitDefaultValue = false)]
        public string Operation { get; private set; }

        [DataMember(Name = "id", EmitDefaultValue = false)]
        public int Id { get; private set; }

        [DataMember(Name = "marketFilter", EmitDefaultValue = false)]
        public MarketFilter MarketFilter { get; private set; }

        [DataMember(Name = "marketDataFilter", EmitDefaultValue = false)]
        public MarketDataFilter MarketDataFilter { get; private set; }

        public SubscriptionMessageStub WithMarketFilter(MarketFilter marketFilter)
        {
            this.MarketFilter = marketFilter;
            return this;
        }

        public SubscriptionMessageStub WithMarketDateFilter(MarketDataFilter marketDataFilter)
        {
            this.MarketDataFilter = marketDataFilter;
            return this;
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.None);
        }
    }
}
