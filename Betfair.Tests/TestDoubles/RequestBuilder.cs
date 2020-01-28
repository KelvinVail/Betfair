namespace Betfair.Tests.TestDoubles
{
    using Newtonsoft.Json;

    public class RequestBuilder
    {
        [JsonProperty(PropertyName = "jsonrpc")]
        public static string Jsonrpc => "2.0";

        [JsonProperty(PropertyName = "id")]
        public static int Id => 1;

        [JsonProperty(PropertyName = "method")]
        public string Method { get; set; }

        public RequestBuilder WithMethod(string method)
        {
            this.Method = method;
            return this;
        }
    }
}
