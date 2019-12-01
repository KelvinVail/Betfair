namespace Betfair.Tests.TestDoubles
{
    public class FakeApiLoginResponse
    {
        public FakeApiLoginResponse()
        {
            this.Status = "SUCCESS";
            this.Error = string.Empty;
            this.Token = "SessionToken";
        }

        public string Token { get; set; }

        public string Product => "ApiKey";

        public string Status { get; set; }

        public string Error { get; set; }

        public FakeApiLoginResponse WithStatus(string status)
        {
            this.Status = status;
            return this;
        }

        public FakeApiLoginResponse WithError(string error)
        {
            this.Error = error;
            return this;
        }

        public FakeApiLoginResponse WithSessionToken(string sessionToken)
        {
            this.Token = sessionToken;
            return this;
        }
    }
}
