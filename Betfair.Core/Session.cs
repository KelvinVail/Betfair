namespace Betfair.Core
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Net.Http;
    using System.Runtime.Serialization;
    using System.Security.Authentication;
    using System.Security.Cryptography.X509Certificates;
    using System.Threading.Tasks;
    using Betfair.Identity;

    public sealed class Session : ISession, IDisposable
    {
        private readonly ExchangeHttpClient client = new ExchangeHttpClient(new Uri("https://identitysso.betfair.com"));

        private readonly string username;

        private readonly string password;

        private string sessionToken;

        private DateTime sessionCreateTime;

        private HttpClientHandler clientHandler;

        private X509Certificate2 certificate;

        public Session(string appKey, string username, string password)
        {
            this.AppKey = Validate(appKey, nameof(appKey));
            this.username = Validate(username, nameof(username));
            this.password = Validate(password, nameof(password));
        }

        public string AppKey { get; }

        public TimeSpan SessionTimeout { get; set; } = TimeSpan.FromHours(8);

        public TimeSpan KeepAliveOffset { get; set; } = TimeSpan.FromHours(-1);

        public DateTime SessionExpiryTime => this.sessionCreateTime + this.SessionTimeout;

        public bool IsSessionValid => !string.IsNullOrEmpty(this.sessionToken) && !this.SessionExpired;

        private bool SessionExpired => this.SessionExpiryTime <= DateTime.UtcNow;

        private bool SessionAboutToExpire => this.SessionExpiryTime + this.KeepAliveOffset <= DateTime.UtcNow;

        public Session WithHandler(HttpClientHandler handler)
        {
            this.clientHandler = handler;
            this.client.WithHandler(this.clientHandler);
            return this;
        }

        public Session WithCert(X509Certificate2 cert)
        {
            this.certificate = cert;
            this.clientHandler.ClientCertificates.Add(cert);
            return this;
        }

        public async Task LoginAsync()
        {
            var request = this.GetLoginRequest();
            this.sessionToken = await this.GetSessionFromBetfairAsync(request);
            this.sessionCreateTime = DateTime.UtcNow;
        }

        public async Task LogoutAsync()
        {
            var request = this.GetRequest("logout");
            this.sessionToken = await this.GetSessionFromBetfairAsync(request);
            this.sessionCreateTime = DateTime.Parse("0001-01-01T00:00:00.0000000", new DateTimeFormatInfo());
        }

        public async Task KeepAliveAsync()
        {
            var request = this.GetRequest("keepAlive");
            this.sessionToken = await this.GetSessionFromBetfairAsync(request);
            this.sessionCreateTime = DateTime.UtcNow;
        }

        public async Task<string> GetTokenAsync()
        {
            await this.LoginIfSessionNotValidAsync();
            await this.LoginIfSessionExpiredAsync();
            await this.KeepAliveIfAboutToExpireAsync();

            return this.sessionToken;
        }

        public void Dispose()
        {
            this.client.Dispose();
        }

        private static string Validate(string value, string name)
        {
            if (string.IsNullOrEmpty(value)) throw new ArgumentNullException(name);
            return value;
        }

        private HttpRequestMessage GetLoginRequest()
        {
            var requestUri = this.certificate == null ? "api/login" : "https://identitysso-cert.betfair.com/api/certlogin";
            var request = new HttpRequestMessage(HttpMethod.Post, requestUri);
            request.Headers.Add("X-Application", this.AppKey);
            request.Content = new FormUrlEncodedContent(
                new Dictionary<string, string> { { "username", this.username }, { "password", this.password } });

            return request;
        }

        private HttpRequestMessage GetRequest(string requestUri)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, $"api/{requestUri}");
            request.Headers.Add("X-Authentication", this.sessionToken);
            return request;
        }

        private async Task LoginIfSessionNotValidAsync()
        {
            if (!this.IsSessionValid)
                await this.LoginAsync();
        }

        private async Task LoginIfSessionExpiredAsync()
        {
            if (this.SessionExpired)
                await this.LoginAsync();
        }

        private async Task KeepAliveIfAboutToExpireAsync()
        {
            if (this.SessionAboutToExpire)
                await this.KeepAliveAsync();
        }

        private async Task<string> GetSessionFromBetfairAsync(HttpRequestMessage request)
        {
            var session = await this.client.SendAsync<LoginResponse>(request);
            session.Validate();
            return session.GetToken;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Performance",
            "CA1812:AvoidUninstantiatedInternalClasses",
            Justification = "Used to deserialize SendAsync response.")]
        [DataContract]
        private sealed class LoginResponse
        {
            internal string GetToken => this.Token ?? this.SessionToken;

            [DataMember(Name = "token", EmitDefaultValue = false)]
            private string Token { get; set; }

            [DataMember(Name = "sessionToken", EmitDefaultValue = false)]
            private string SessionToken { get; set; }

            [DataMember(Name = "status", EmitDefaultValue = false)]
            private string Status { get; set; }

            [DataMember(Name = "loginStatus", EmitDefaultValue = false)]
            private string LoginStatus { get; set; }

            [DataMember(Name = "error", EmitDefaultValue = false)]
            private string Error { get; set; }

            private string GetStatus => this.Status ?? this.LoginStatus;

            internal void Validate()
            {
                if (this.GetStatus != "SUCCESS") throw new AuthenticationException($"{this.GetStatus}: {this.Error ?? "NONE"}");
            }
        }
    }
}
