namespace Betfair.Services.BetfairApi
{
    using System.Collections.Generic;

    using Newtonsoft.Json;

    public class MarketProfitAndLossResponse
    {
        [JsonProperty(PropertyName = "id")] public int Id;

        [JsonProperty(PropertyName = "jsonrpc")] public string Jsonrpc;

        [JsonProperty(PropertyName = "result")] public List<MarketProfitAndLoss> Result;
    }
}