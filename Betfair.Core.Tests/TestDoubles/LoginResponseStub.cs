namespace Betfair.Core.Tests.TestDoubles
{
    public class LoginResponseStub
    {
        public LoginResponseStub()
        {
            this.Status = "SUCCESS";
            this.Error = string.Empty;
            this.Token = "SessionToken";
        }

        public static string Product => "ApiKey";

        public string Token { get; set; }

        public string Status { get; set; }

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
