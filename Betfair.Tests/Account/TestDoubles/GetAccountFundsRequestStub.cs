namespace Betfair.Tests.Account.TestDoubles
{
    using Newtonsoft.Json;

    public sealed class GetAccountFundsRequestStub
    {
        [JsonProperty(PropertyName = "jsonrpc")]
        public static string Jsonrpc => "2.0";

        [JsonProperty(PropertyName = "id")]
        public static int Id => 1;

        [JsonProperty(PropertyName = "method")]
        public static string Method => "AccountAPING/v1.0/getAccountFunds";
    }
}
