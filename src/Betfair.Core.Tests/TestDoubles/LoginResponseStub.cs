namespace Betfair.Core.Tests.TestDoubles
{
    using System.Runtime.Serialization;

    [DataContract]
    public class LoginResponseStub
    {
        public LoginResponseStub()
        {
            this.Status = "SUCCESS";
            this.Error = string.Empty;
            this.Token = "SessionToken";
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
            this.Status = status;
            return this;
        }

        public LoginResponseStub WithError(string error)
        {
            this.Error = error;
            return this;
        }

        public LoginResponseStub WithSessionToken(string sessionToken)
        {
            this.Token = sessionToken;
            return this;
        }
    }
}
