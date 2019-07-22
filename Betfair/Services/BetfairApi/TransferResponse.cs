namespace Betfair.Services.BetfairApi
{
    using Newtonsoft.Json;

    public class TransferResponse
    {
        [JsonProperty(PropertyName = "transactionId")]
        public string TransactionId { get; set; }
    }
}