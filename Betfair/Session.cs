namespace Betfair
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Security.Authentication;
    using System.Threading.Tasks;
    using Newtonsoft.Json;

    public sealed class Session : IDisposable
    {
        private readonly string appKey;

        private readonly string username;

        private readonly string password;

        private readonly HttpRequestMessage apiRequest = new HttpRequestMessage(HttpMethod.Post, "api/login");

        private HttpClient client = new HttpClient();

        public Session(string appKey, string username, string password)
        {
            if (string.IsNullOrEmpty(appKey)) throw new ArgumentNullException(nameof(appKey));
            if (string.IsNullOrEmpty(username)) throw new ArgumentNullException(nameof(username));
            if (string.IsNullOrEmpty(password)) throw new ArgumentNullException(nameof(password));

            this.appKey = appKey;
            this.username = username;
            this.password = password;
            this.ConfigureHttpClient();
        }

        public string SessionToken { get; set; }

        public Session WithHttpClient(HttpClient httpClient)
        {
            this.client = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            this.ConfigureHttpClient();
            return this;
        }

        public async Task LoginAsync()
        {
            this.apiRequest.Headers.Add("X-Application", this.appKey);
            this.apiRequest.Content = new FormUrlEncodedContent(
                new Dictionary<string, string> { { "username", this.username }, { "password", this.password } });
            using (var response = await this.client.SendAsync(this.apiRequest))
            {
                if (!response.IsSuccessStatusCode) throw new AuthenticationException($"{response.StatusCode}");
                var session = JsonConvert.DeserializeObject<LoginResponse>(await response.Content.ReadAsStringAsync());
                if (session.Status != "SUCCESS") throw new AuthenticationException($"{session.Status}: {session.Error}");
                this.SessionToken = session.Token;
            }
        }

        public void Dispose()
        {
            this.apiRequest.Dispose();
            ((IDisposable)this.client).Dispose();
        }

        private void ConfigureHttpClient()
        {
            this.client.BaseAddress = new Uri("https://identitysso.betfair.com");
            this.client.Timeout = TimeSpan.FromSeconds(30);
            this.client.DefaultRequestHeaders.Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        private sealed class LoginResponse
        {
            [JsonProperty]
            internal string Token { get; set; }

            [JsonProperty]
            internal string Status { get; set; }

            [JsonProperty]
            internal string Error { get; set; }
        }
    }
}
