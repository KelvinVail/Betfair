namespace Betfair.Tests.Account.TestDoubles
{
    using System;
    using Newtonsoft.Json;

    public class StatementItemStub
    {
        [JsonProperty(PropertyName = "refId")]
        public string RefId { get; set; }

        [JsonProperty(PropertyName = "itemDate")]
        public DateTime ItemDate { get; set; }

        [JsonProperty(PropertyName = "amount")]
        public double Amount { get; set; }

        [JsonProperty(PropertyName = "balance")]
        public double Balance { get; set; }
    }
}
