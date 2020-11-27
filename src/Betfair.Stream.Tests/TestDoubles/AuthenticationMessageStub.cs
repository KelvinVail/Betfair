namespace Betfair.Stream.Tests.TestDoubles
{
    using System.Runtime.Serialization;

    [DataContract]
    public sealed class AuthenticationMessageStub
    {
        public AuthenticationMessageStub(string appKey, string session)
        {
            this.AppKey = appKey;
            this.SessionToken = session;
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
