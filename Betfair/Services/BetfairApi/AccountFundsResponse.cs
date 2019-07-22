namespace Betfair.Services.BetfairApi
{
    using Newtonsoft.Json;

    public class AccountFundsResponse
    {
        [JsonProperty(PropertyName = "jsonrpc")] public string Jsonrpc;

        [JsonProperty(PropertyName = "result")] public AccountFunds Result;
    }
}