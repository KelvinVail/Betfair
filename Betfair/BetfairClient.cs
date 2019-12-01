namespace Betfair
{
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Security.Authentication;
    using System.Threading.Tasks;

    using Betfair.DataStructures;

    using Newtonsoft.Json;

    public class BetfairClient
    {
        private readonly HttpClient identityHttpClient;

        private readonly HttpRequestMessage apiRequest = new HttpRequestMessage(HttpMethod.Post, "/api/login");

        internal BetfairClient(HttpClient identityHttpClient)
        {
            this.identityHttpClient = identityHttpClient;
        }

        public string SessionToken { get; internal set; }

        internal string AppKey { private get; set; }

        internal string Username { private get; set; }

        internal string Password { private get; set; }

        public async Task ApiLoginAsync()
        {
            this.SetHeaders();
            this.SetLoginContent();
            using (var response = await this.identityHttpClient.SendAsync(this.apiRequest))
            {
                ThrowIfNotSuccess(response);
                var session = await DeserializeResponseAsync<ApiLoginResponse>(response);
                if (session.Status != "SUCCESS") throw new AuthenticationException($"{session.Status}: {session.Error}");
                this.SessionToken = session.Token;
            }
        }

        private static FormUrlEncodedContent LoginContent(string username, string password)
        {
            return new FormUrlEncodedContent(
                new Dictionary<string, string> { { "username", username }, { "password", password } });
        }

        private static void ThrowIfNotSuccess(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode) throw new AuthenticationException($"{response.StatusCode}");
        }

        private static async Task<T> DeserializeResponseAsync<T>(HttpResponseMessage response)
        {
            return JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync());
        }

        private void SetHeaders()
        {
            this.apiRequest.Headers.Add("X-Application", this.AppKey);
        }

        private void SetLoginContent()
        {
            this.apiRequest.Content = LoginContent(this.Username, this.Password);
        }
    }
}
