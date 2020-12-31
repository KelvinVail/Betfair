using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Betfair.Identity;

namespace Betfair.Core
{
    public sealed class Session : ISession, IDisposable
    {
        private readonly string _username;
        private readonly string _password;
        private ExchangeClient _client;
        private string _sessionToken;
        private DateTime _sessionCreateTime;
        private HttpClientHandler _clientHandler;
        private X509Certificate2 _certificate;

        public Session(string appKey, string username, string password)
        {
            AppKey = Validate(appKey, nameof(appKey));
            _username = Validate(username, nameof(username));
            _password = Validate(password, nameof(password));
            _client = new ExchangeClient("identitysso");
        }

        public string AppKey { get; }

        public TimeSpan SessionTimeout { get; set; } = TimeSpan.FromHours(8);

        public TimeSpan KeepAliveOffset { get; set; } = TimeSpan.FromHours(-1);

        public DateTime SessionExpiryTime => _sessionCreateTime + SessionTimeout;

        public bool IsSessionValid => !string.IsNullOrEmpty(_sessionToken) && !SessionExpired;

        private bool SessionExpired => SessionExpiryTime <= DateTime.UtcNow;

        private bool SessionAboutToExpire => SessionExpiryTime + KeepAliveOffset <= DateTime.UtcNow;

        public Session WithHandler(HttpClientHandler handler)
        {
            _clientHandler = handler;
            _client.WithHandler(_clientHandler);
            return this;
        }

        public Session WithCert(X509Certificate2 cert)
        {
            _certificate = cert;
            _client = new ExchangeClient("identitysso-cert");
            _client.WithHandler(_clientHandler);
            _clientHandler.ClientCertificates.Add(cert);
            return this;
        }

        public async Task LoginAsync()
        {
            var request = GetLoginRequest();
            _sessionToken = await GetSessionFromBetfairAsync(request);
            _sessionCreateTime = DateTime.UtcNow;
        }

        public async Task LogoutAsync()
        {
            var request = GetRequest("logout");
            _sessionToken = await GetSessionFromBetfairAsync(request);
            _sessionCreateTime = DateTime.Parse("0001-01-01T00:00:00.0000000", new DateTimeFormatInfo());
        }

        public async Task KeepAliveAsync()
        {
            var request = GetRequest("keepAlive");
            _sessionToken = await GetSessionFromBetfairAsync(request);
            _sessionCreateTime = DateTime.UtcNow;
        }

        public async Task<string> GetTokenAsync()
        {
            await LoginIfSessionNotValidAsync();
            await LoginIfSessionExpiredAsync();
            await KeepAliveIfAboutToExpireAsync();

            return _sessionToken;
        }

        public void Dispose()
        {
            _client.Dispose();
        }

        private static string Validate(string value, string name)
        {
            if (string.IsNullOrEmpty(value)) throw new ArgumentNullException(name);
            return value;
        }

        private HttpRequestMessage GetLoginRequest()
        {
            var requestUri = _certificate == null ? "api/login" : "api/certlogin";
            var request = new HttpRequestMessage(HttpMethod.Post, requestUri);
            request.Headers.Add("X-Application", AppKey);
            request.Content = new FormUrlEncodedContent(
                new Dictionary<string, string> { { "username", _username }, { "password", _password } });

            return request;
        }

        private HttpRequestMessage GetRequest(string requestUri)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, $"api/{requestUri}");
            request.Headers.Add("X-Authentication", _sessionToken);
            return request;
        }

        private async Task LoginIfSessionNotValidAsync()
        {
            if (!IsSessionValid)
                await LoginAsync();
        }

        private async Task LoginIfSessionExpiredAsync()
        {
            if (SessionExpired)
                await LoginAsync();
        }

        private async Task KeepAliveIfAboutToExpireAsync()
        {
            if (SessionAboutToExpire)
                await KeepAliveAsync();
        }

        private async Task<string> GetSessionFromBetfairAsync(HttpRequestMessage request)
        {
            var session = await _client.SendAsync<LoginResponse>(request);
            session.ValidateResponse();
            return session.GetToken;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Microsoft.Performance",
            "CA1812:AvoidUninstantiatedInternalClasses",
            Justification = "Used to deserialize SendAsync response.")]
        [DataContract]
        private sealed class LoginResponse
        {
            internal string GetToken => Token ?? SessionToken;

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

            private string GetStatus => Status ?? LoginStatus;

            internal void ValidateResponse()
            {
                if (GetStatus != "SUCCESS") throw new AuthenticationException($"{GetStatus}: {Error ?? "NONE"}");
            }
        }
    }
}
