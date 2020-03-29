namespace Betfair.Core.Tests.TestDoubles
{
    using System.Runtime.Serialization;

    [DataContract]
    public class CertLoginResponseStub
    {
        [DataMember(Name = "sessionToken", EmitDefaultValue = false)]
        public string SessionToken { get; set; } = "SessionToken";

        [DataMember(Name = "loginStatus", EmitDefaultValue = false)]
        public string LoginStatus { get; set; } = "SUCCESS";
    }
}
