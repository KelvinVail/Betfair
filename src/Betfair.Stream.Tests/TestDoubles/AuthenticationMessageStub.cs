using System.Runtime.Serialization;

namespace Betfair.Stream.Tests.TestDoubles
{
    [DataContract]
    public sealed class AuthenticationMessageStub
    {
        public AuthenticationMessageStub(string appKey, string session)
        {
            AppKey = appKey;
            SessionToken = session;
        }

        [DataMember(Name = "op", EmitDefaultValue = false)]
        public string Op { get; set; } = "authentication";

        [DataMember(Name = "id", EmitDefaultValue = false)]
        public int? Id { get; set; } = 1;

        [DataMember(Name = "session", EmitDefaultValue = false)]
        public string SessionToken { get; set; }

        [DataMember(Name = "appKey", EmitDefaultValue = false)]
        public string AppKey { get; set; }
    }
}
