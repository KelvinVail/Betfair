namespace Betfair.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Net.Http;
    using System.Security.Authentication;
    using System.Threading.Tasks;
    using Betfair.Tests.TestDoubles;
    using Xunit;

    public class SessionTests : IDisposable
    {
        private MockHttpMessageHandler httpMessageHandler = new MockHttpMessageHandler();

        private HttpClient httpClient;

        private Session session = new Session("AppKey", "Username", "Password");

        private bool disposedValue;

        public SessionTests()
        {
            this.httpMessageHandler.WithReturnContent(new FakeApiLoginResponse());
            this.httpClient = new HttpClient(this.httpMessageHandler.Build());
            this.session.WithHttpClient(this.httpClient);
        }

        [Fact]
        public void WhenInitializedThrowIfAppKeyIsNull()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => new Session(null, "Username", "Password"));
            Assert.Equal("appKey", exception.ParamName);
        }

        [Fact]
        public void WhenInitializedThrowIfUsernameIsNull()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => new Session("AppKey", null, "Password"));
            Assert.Equal("username", exception.ParamName);
        }

        [Fact]
        public void WhenInitializedThrowIfPasswordIsNull()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => new Session("AppKey", "Username", null));
            Assert.Equal("password", exception.ParamName);
        }

        [Fact]
        public async Task OnLoginHttpPostMethodIsUsed()
        {
            await this.session.LoginAsync();
            this.httpMessageHandler.VerifyHttpMethod(HttpMethod.Post);
        }

        [Fact]
        public async Task OnLoginIdentityUriIsCalled()
        {
            await this.session.LoginAsync();
            this.httpMessageHandler.VerifyRequestUri(new Uri("https://identitysso.betfair.com/api/login"));
        }

        [Theory]
        [InlineData("AppKey")]
        [InlineData("ABC")]
        [InlineData("12345")]
        public async Task OnLoginAppKeyIsInRequestHeader(string appKey)
        {
            using (var localSession = new Session(appKey, "Username", "Password"))
            {
                localSession.WithHttpClient(this.httpClient);
                await localSession.LoginAsync();
                this.httpMessageHandler.VerifyHeaderValues("X-Application", appKey);
            }
        }

        [Theory]
        [InlineData("Username")]
        [InlineData("Kelvin")]
        [InlineData("Bob")]
        public async Task OnLoginUsernameIsInRequestContent(string username)
        {
            using (var localSession = new Session("AppKey", username, "Password"))
            {
                localSession.WithHttpClient(this.httpClient);
                await localSession.LoginAsync();
                this.httpMessageHandler.VerifyRequestContent(ContentString(username, "Password"));
            }
        }

        [Theory]
        [InlineData("Password")]
        [InlineData("123456")]
        [InlineData("qwerty")]
        public async Task OnLoginPasswordIsInRequestContent(string password)
        {
            this.session = new Session("AppKey", "Username", password);
            this.httpClient = new HttpClient(this.httpMessageHandler.Build());
            this.session.WithHttpClient(this.httpClient);
            await this.session.LoginAsync();
            this.httpMessageHandler.VerifyRequestContent(ContentString("Username", password));
        }

        [Theory]
        [InlineData("FAIL", "INVALID_USERNAME_OR_PASSWORD")]
        [InlineData("FAIL", "INPUT_VALIDATION_ERROR")]
        [InlineData("LIMITED_ACCESS", "PENDING_AUTH")]
        public async Task OnLoginThrowIfFailed(string status, string error)
        {
            var responseHandler = this.SetExpectedHttpResponse(status, error).Build();
            this.httpClient = new HttpClient(responseHandler);
            this.session.WithHttpClient(this.httpClient);
            var exception = await Assert.ThrowsAsync<AuthenticationException>(this.session.LoginAsync);
            Assert.Equal($"{status}: {error}", exception.Message);
        }

        [Theory]
        [InlineData("SessionToken")]
        [InlineData("NewSessionToken")]
        [InlineData("DifferentSessionToken")]
        public async Task OnLoginSessionTokenIsSet(string sessionToken)
        {
            await this.SetSessionToken(sessionToken);
            Assert.Equal(sessionToken, await this.session.GetSessionTokenAsync());
        }

        [Fact]
        public void WhenInitializedSessionTimeoutIsEightHours()
        {
            const int defaultSessionTimeout = 8;
            Assert.Equal(defaultSessionTimeout, this.session.SessionTimeout.TotalHours, 2);
        }

        [Fact]
        public void SessionTimeoutCanBeSet()
        {
            const int newSessionTimeout = 2;
            this.session.SessionTimeout = TimeSpan.FromHours(newSessionTimeout);
            Assert.Equal(newSessionTimeout, this.session.SessionTimeout.TotalHours);
        }

        [Fact]
        public async Task OnGetSessionTokenLoginIsCalledIfSessionTokenIsNull()
        {
            Assert.Equal("SessionToken", await this.session.GetSessionTokenAsync());
            this.httpMessageHandler.VerifyRequestUri(new Uri("https://identitysso.betfair.com/api/login"));
        }

        [Fact]
        public async Task OnKeepAliveLoginIsCalledIfSessionTokenIsNull()
        {
            await this.session.KeepAliveAsync();
            this.httpMessageHandler.VerifyRequestUri(new Uri("https://identitysso.betfair.com/api/login"));
        }

        [Fact]
        public async Task OnKeepAliveSessionTokenIsNotRefreshedIfNotExpiredOrAboutToExpire()
        {
            await this.SetSessionToken("SessionToken");
            this.ResetMessageHandler();
            await this.session.KeepAliveAsync();
            this.httpMessageHandler.VerifyTimesCalled(0);
        }

        [Fact]
        public async Task OnKeepAliveLoginIsCalledIfSessionIsExpired()
        {
            await this.SetExpiredSessionToken("SessionToken");
            this.ResetMessageHandler();
            await this.session.KeepAliveAsync();
            this.httpMessageHandler.VerifyRequestUri(new Uri("https://identitysso.betfair.com/api/login"));
        }

        [Fact]
        public void WhenInitializedKeepAliveOffsetIsOneHour()
        {
            const int defaultKeepAliveOffset = -1;
            Assert.Equal(defaultKeepAliveOffset, this.session.KeepAliveOffset.TotalHours, 0);
        }

        [Fact]
        public void KeepAliveOffsetCanBeSet()
        {
            const int newKeepAliveOffset = -2;
            this.session.KeepAliveOffset = TimeSpan.FromHours(newKeepAliveOffset);
            Assert.Equal(newKeepAliveOffset, this.session.KeepAliveOffset.TotalHours, 0);
        }

        [Fact]
        public async Task OnKeepAliveTheKeepAliveUriIsCalledIfSessionIsAboutToExpire()
        {
            await this.SetAboutToExpireSessionToken("SessionToken");
            await this.session.KeepAliveAsync();
            this.httpMessageHandler.VerifyRequestUri(new Uri("https://identitysso.betfair.com/api/keepAlive"));
        }

        [Fact]
        public async Task OnKeepAliveHttpPostMethodIsUsed()
        {
            await this.SetAboutToExpireSessionToken("SessionToken");
            await this.session.KeepAliveAsync();
            this.httpMessageHandler.VerifyHttpMethod(HttpMethod.Post);
        }

        [Theory]
        [InlineData("SessionToken")]
        [InlineData("NewSessionToken")]
        [InlineData("DifferentSessionToken")]
        public async Task OnKeepAliveSessionTokenIsInRequestHeader(string sessionToken)
        {
            await this.SetAboutToExpireSessionToken(sessionToken);
            await this.session.KeepAliveAsync();
            this.httpMessageHandler.VerifyHeaderValues("X-Authentication", sessionToken);
        }

        [Fact]
        public void WhenInitializedSessionExpiryTimeIsSet()
        {
            var nullDateTime = DateTime.Parse("0001-01-01T00:00:00.0000000", new DateTimeFormatInfo());
            Assert.Equal(nullDateTime + this.session.SessionTimeout, this.session.SessionExpiryTime);
        }

        [Fact]
        public async Task OnLoginSessionExpiryTimeIsUpdated()
        {
            var expiryTimeBeforeRefresh = this.session.SessionExpiryTime;
            await this.session.LoginAsync();
            Assert.NotEqual(expiryTimeBeforeRefresh, this.session.SessionExpiryTime);
        }

        [Fact]
        public async Task OnKeepAliveSessionExpiryTimeIsUpdated()
        {
            await this.SetAboutToExpireSessionToken("SessionToken");
            var expiryTimeBeforeRefresh = this.session.SessionExpiryTime;
            await this.session.KeepAliveAsync();
            Assert.NotEqual(expiryTimeBeforeRefresh, this.session.SessionExpiryTime);
        }

        [Theory]
        [InlineData("FAIL", "INVALID_USERNAME_OR_PASSWORD")]
        [InlineData("FAIL", "INPUT_VALIDATION_ERROR")]
        [InlineData("LIMITED_ACCESS", "PENDING_AUTH")]
        public async Task OnKeepAliveThrowIfFailed(string status, string error)
        {
            await this.SetAboutToExpireSessionToken("SessionToken");
            var responseHandler = this.SetExpectedHttpResponse(status, error).Build();
            this.httpClient = new HttpClient(responseHandler);
            this.session.WithHttpClient(this.httpClient);
            var exception = await Assert.ThrowsAsync<AuthenticationException>(this.session.KeepAliveAsync);
            Assert.Equal($"{status}: {error}", exception.Message);
        }

        [Theory]
        [InlineData("SessionToken")]
        [InlineData("NewSessionToken")]
        [InlineData("DifferentSessionToken")]
        public async Task OnKeepAliveSessionTokenIsSet(string sessionToken)
        {
            await this.SetAboutToExpireSessionToken("AboutToExpireSessionToken");
            this.httpMessageHandler.WithReturnContent(
                new FakeApiLoginResponse().WithSessionToken(sessionToken));
            this.httpClient = new HttpClient(this.httpMessageHandler.Build());
            this.session.WithHttpClient(this.httpClient);
            await this.session.KeepAliveAsync();
            Assert.Equal(sessionToken, await this.session.GetSessionTokenAsync());
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (this.disposedValue) return;
            if (disposing)
            {
                this.httpClient.Dispose();
                this.httpMessageHandler.Dispose();
            }

            this.session.Dispose();

            this.disposedValue = true;
        }

        private static string ContentString(string username, string password)
        {
            var dict = new Dictionary<string, string> { { "username", username }, { "password", password } };
            using (var content = new FormUrlEncodedContent(dict))
            {
                return content.ReadAsStringAsync().Result;
            }
        }

        private MockHttpMessageHandler SetExpectedHttpResponse(string status, string error)
        {
            return this.httpMessageHandler.WithReturnContent(
                new FakeApiLoginResponse().WithStatus(status).WithError(error));
        }

        private async Task SetExpiredSessionToken(string sessionToken)
        {
            this.session.SessionTimeout = TimeSpan.FromHours(-1);
            await this.SetSessionToken(sessionToken);
        }

        private async Task SetAboutToExpireSessionToken(string sessionToken)
        {
            this.session.SessionTimeout = TimeSpan.FromMinutes(30);
            await this.SetSessionToken(sessionToken);
            this.ResetMessageHandler();
            this.httpMessageHandler.VerifyTimesCalled(0);
        }

        private async Task SetSessionToken(string sessionToken)
        {
            this.httpMessageHandler.WithReturnContent(
                new FakeApiLoginResponse().WithSessionToken(sessionToken));
            this.httpClient = new HttpClient(this.httpMessageHandler.Build());
            this.session.WithHttpClient(this.httpClient);
            await this.session.LoginAsync();
        }

        private void ResetMessageHandler()
        {
            this.httpMessageHandler = new MockHttpMessageHandler();
            this.httpMessageHandler.WithReturnContent(new FakeApiLoginResponse());
            this.httpClient = new HttpClient(this.httpMessageHandler.Build());
            this.session.WithHttpClient(this.httpClient);
        }
    }
}
