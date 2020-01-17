namespace Betfair
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Security.Authentication;
    using System.Threading.Tasks;
    using Newtonsoft.Json;

    public sealed class Session : HttpClientBase
    {
        private readonly string appKey;

        private readonly string username;

        private readonly string password;

        private string sessionToken;

        private DateTime sessionCreateTime;

        public Session(string appKey, string username, string password)
            : base(new Uri("https://identitysso.betfair.com"))
        {
            this.appKey = Validate(appKey, nameof(appKey));
            this.username = Validate(username, nameof(username));
            this.password = Validate(password, nameof(password));
        }

        public TimeSpan SessionTimeout { get; set; } = TimeSpan.FromHours(8);

        public TimeSpan KeepAliveOffset { get; set; } = TimeSpan.FromHours(-1);

        public DateTime SessionExpiryTime => this.sessionCreateTime + this.SessionTimeout;

        public new Session WithHttpClient(HttpClient httpClient)
        {
            base.WithHttpClient(httpClient);
            return this;
        }

        public async Task LoginAsync()
        {
            var loginRequest = this.GetLoginRequest();
            var session = await this.SendAsync<LoginResponse>(loginRequest);
            session.Validate();
            this.sessionCreateTime = DateTime.UtcNow;
            this.sessionToken = session.Token;
        }

        public async Task KeepAliveAsync()
        {
            if (string.IsNullOrEmpty(this.sessionToken))
                await this.LoginAsync();

            if (this.SessionExpired())
                await this.LoginAsync();

            if (this.SessionAboutToExpire())
            {
                var request = new HttpRequestMessage(HttpMethod.Post, "api/keepAlive");
                request.Headers.Add("X-Authentication", this.sessionToken);
                var session = await this.SendAsync<LoginResponse>(request);
                session.Validate();
                this.sessionCreateTime = DateTime.UtcNow;
                this.sessionToken = session.Token;
            }
        }

        public async Task<string> GetSessionTokenAsync()
        {
            if (string.IsNullOrEmpty(this.sessionToken))
                await this.LoginAsync();

            return this.sessionToken;
        }

        private static string Validate(string value, string name)
        {
            if (string.IsNullOrEmpty(value)) throw new ArgumentNullException(name);
            return value;
        }

        private HttpRequestMessage GetLoginRequest()
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "api/login");
            request.Headers.Add("X-Application", this.appKey);
            request.Content = new FormUrlEncodedContent(
                new Dictionary<string, string> { { "username", this.username }, { "password", this.password } });

            return request;
        }

        private bool SessionExpired()
        {
            return this.SessionExpiryTime <= DateTime.UtcNow;
        }

        private bool SessionAboutToExpire()
        {
            return this.SessionExpiryTime + this.KeepAliveOffset <= DateTime.UtcNow;
        }

        private sealed class LoginResponse
        {
            [JsonProperty]
            internal string Token { get; set; }

            [JsonProperty]
            internal string Status { get; set; }

            [JsonProperty]
            internal string Error { get; set; }

            internal void Validate()
            {
                if (this.Status != "SUCCESS") throw new AuthenticationException($"{this.Status}: {this.Error}");
            }
        }
    }
}
