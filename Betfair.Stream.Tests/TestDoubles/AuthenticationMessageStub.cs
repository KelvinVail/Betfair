namespace Betfair.Stream.Tests.TestDoubles
{
    using Newtonsoft.Json;

    public sealed class AuthenticationMessageStub
    {
        public AuthenticationMessageStub(string appKey, string session)
        {
            this.AppKey = appKey;
            this.SessionToken = session;
        }

        [JsonProperty(PropertyName = "op")]
        public string Op { get; set; } = "authentication";

        [JsonProperty(PropertyName = "id")]
        public int? Id { get; set; } = 1;

        [JsonProperty(PropertyName = "session")]
        public string SessionToken { get; set; }

        [JsonProperty(PropertyName = "appKey")]
        public string AppKey { get; set; }
    }
}
