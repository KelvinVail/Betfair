namespace Betfair.Tests.TestDoubles
{
    using Newtonsoft.Json;

    public class ResponseStub<T>
    {
        public ResponseStub(T result)
        {
            this.Result = result;
        }

        [JsonProperty(PropertyName = "jsonrpc")]
        public string Jsonrpc => "2.0";

        [JsonProperty(PropertyName = "id")]
        public int Id => 1;

        [JsonProperty(PropertyName = "result")]
        public T Result { get; set; }

        [JsonProperty(PropertyName = "error")]
        public string Error { get; set; }
    }
}
