namespace Betfair
{
    using System.Collections.Generic;
    using System.Net.Http;

    public class BetfairClient
    {
        private readonly HttpClient identityHttpClient;

        internal BetfairClient(HttpClient identityHttpClient)
        {
            this.identityHttpClient = identityHttpClient;
        }

        internal string AppKey { private get; set; }

        internal string Username { private get; set; }

        internal string Password { private get; set; }

        public void Login()
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "/api/certlogin");
            request.Headers.Add("X-Application", this.AppKey);
            request.Content = LoginContent(this.Username, this.Password);
            this.identityHttpClient.SendAsync(request);
        }
                
        private static FormUrlEncodedContent LoginContent(string username, string password)
        {
            return new FormUrlEncodedContent(
                new Dictionary<string, string> { { "username", username }, { "password", password } });
        }
    }
}
