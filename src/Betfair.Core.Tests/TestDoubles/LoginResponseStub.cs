using System.Runtime.Serialization;

namespace Betfair.Core.Tests.TestDoubles
{
    [DataContract]
    public class LoginResponseStub
    {
        public LoginResponseStub()
        {
            Status = "SUCCESS";
            Error = string.Empty;
            Token = "SessionToken";
        }

        [DataMember(Name = "product", EmitDefaultValue = false)]
        public static string Product => "ApiKey";

        [DataMember(Name = "token", EmitDefaultValue = false)]
        public string Token { get; set; }

        [DataMember(Name = "status", EmitDefaultValue = false)]
        public string Status { get; set; }

        [DataMember(Name = "error", EmitDefaultValue = false)]
        public string Error { get; set; }

        public LoginResponseStub WithStatus(string status)
        {
            Status = status;
            return this;
        }

        public LoginResponseStub WithError(string error)
        {
            Error = error;
            return this;
        }

        public LoginResponseStub WithSessionToken(string sessionToken)
        {
            Token = sessionToken;
            return this;
        }
    }
}
