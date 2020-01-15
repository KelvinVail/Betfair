namespace Betfair
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Security.Authentication;
    using System.Threading.Tasks;

    using Betfair.DataStructures;

    using Newtonsoft.Json;

    public class SessionService
    {
        private readonly Uri identityApiUri = 
            new Uri("https://identitysso.betfair.com/");

        private readonly HttpClient httpClient;

        private readonly HttpRequestMessage apiRequest = new HttpRequestMessage(HttpMethod.Post, "/api/login");

        public SessionService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
            this.httpClient.BaseAddress = this.identityApiUri;
            this.httpClient.Timeout = TimeSpan.FromSeconds(30);
            this.httpClient.DefaultRequestHeaders.Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));
            this.apiRequest.Content = new FormUrlEncodedContent(
                new Dictionary<string, string> { { "username", this.Username }, { "password", this.Password } });
        }

        public string SessionToken { get; internal set; }

        public int Timeout => (int)this.httpClient.Timeout.TotalSeconds;

        public string AcceptMediaType => this.httpClient.DefaultRequestHeaders.Accept.ToString();

        internal string AppKey { private get; set; }

        internal string Username { private get; set; }

        internal string Password { private get; set; }

        public async Task ApiLoginAsync()
        {
            if (string.IsNullOrEmpty(this.AppKey)) throw new InvalidOperationException("AppKey not set.");

            this.apiRequest.Headers.Add("X-Application", this.AppKey);
            this.apiRequest.Content = new FormUrlEncodedContent(
                new Dictionary<string, string> { { "username", this.Username }, { "password", this.Password } });
            using (var response = await this.httpClient.SendAsync(this.apiRequest))
            {
                if (!response.IsSuccessStatusCode) throw new AuthenticationException($"{response.StatusCode}");
                var session = JsonConvert.DeserializeObject<ApiLoginResponse>(await response.Content.ReadAsStringAsync());
                if (session.Status != "SUCCESS") throw new AuthenticationException($"{session.Status}: {session.Error}");
                this.SessionToken = session.Token;
            }
        }
    }
}