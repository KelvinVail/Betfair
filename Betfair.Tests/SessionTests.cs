namespace Betfair.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Net.Http;
    using System.Security.Authentication;
    using System.Security.Cryptography.X509Certificates;
    using System.Threading.Tasks;
    using Betfair.Tests.TestDoubles;
    using Xunit;

    public class SessionTests : IDisposable
    {
        private readonly X509Certificate2 certificate = new X509Certificate2();

        private HttpMessageHandlerMock httpMessageHandler = new HttpMessageHandlerMock();

        private Session session = new Session("AppKey", "Username", "Password");

        private bool disposedValue;

        public SessionTests()
        {
            this.httpMessageHandler.WithReturnContent(new LoginResponseStub());
            this.session.WithHandler(this.httpMessageHandler.Build());
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
        public void WhenInitializedAppKeyIsSet()
        {
            Assert.Equal("AppKey", this.session.AppKey);
        }

        [Fact]
        public void WhenInitializedSessionTimeoutIsEightHours()
        {
            const int defaultSessionTimeout = 8;
            Assert.Equal(defaultSessionTimeout, this.session.SessionTimeout.TotalHours, 2);
        }

        [Fact]
        public void AfterInitializedSessionTimeoutCanBeSet()
        {
            const int newSessionTimeout = 2;
            this.session.SessionTimeout = TimeSpan.FromHours(newSessionTimeout);
            Assert.Equal(newSessionTimeout, this.session.SessionTimeout.TotalHours);
        }

        [Fact]
        public void WhenInitializedKeepAliveOffsetIsOneHour()
        {
            const int defaultKeepAliveOffset = -1;
            Assert.Equal(defaultKeepAliveOffset, this.session.KeepAliveOffset.TotalHours, 0);
        }

        [Fact]
        public void AfterInitializedKeepAliveOffsetCanBeSet()
        {
            const int newKeepAliveOffset = -2;
            this.session.KeepAliveOffset = TimeSpan.FromHours(newKeepAliveOffset);
            Assert.Equal(newKeepAliveOffset, this.session.KeepAliveOffset.TotalHours, 0);
        }

        [Fact]
        public void WhenInitializedSessionExpiryTimeIsSet()
        {
            var nullDateTime = DateTime.Parse("0001-01-01T00:00:00.0000000", new DateTimeFormatInfo());
            Assert.Equal(nullDateTime + this.session.SessionTimeout, this.session.SessionExpiryTime);
        }

        [Fact]
        public void WhenInitializedSessionIsNotValid()
        {
            Assert.False(this.session.IsSessionValid);
        }

        [Fact]
        public async Task OnExpirySessionIsNotValid()
        {
            await this.SetExpiredSessionToken("SessionToken");
            Assert.False(this.session.IsSessionValid);
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
            this.session = new Session(appKey, "Username", "Password");
            this.session.WithHandler(this.httpMessageHandler.Build());
            await this.session.LoginAsync();
            this.httpMessageHandler.VerifyHeaderValues("X-Application", appKey);
        }

        [Theory]
        [InlineData("Username")]
        [InlineData("Kelvin")]
        [InlineData("Bob")]
        public async Task OnLoginUsernameIsInRequestContent(string username)
        {
            this.session = new Session("AppKey", username, "Password");
            this.session.WithHandler(this.httpMessageHandler.Build());
            await this.session.LoginAsync();
            this.httpMessageHandler.VerifyRequestContent(ContentString(username, "Password"));
        }

        [Theory]
        [InlineData("Password")]
        [InlineData("123456")]
        [InlineData("qwerty")]
        public async Task OnLoginPasswordIsInRequestContent(string password)
        {
            this.session = new Session("AppKey", "Username", password);
            this.session.WithHandler(this.httpMessageHandler.Build());
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
            this.session.WithHandler(responseHandler);
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
            Assert.Equal(sessionToken, await this.session.GetTokenAsync());
        }

        [Fact]
        public async Task OnLoginSessionExpiryTimeIsUpdated()
        {
            var expiryTimeBeforeRefresh = this.session.SessionExpiryTime;
            await this.session.LoginAsync();
            Assert.NotEqual(expiryTimeBeforeRefresh, this.session.SessionExpiryTime);
        }

        [Fact]
        public async Task OnLoginSessionIsValid()
        {
            await this.session.LoginAsync();
            Assert.True(this.session.IsSessionValid);
        }

        [Fact]
        public async Task OnGetSessionTokenLoginIsCalledIfSessionTokenIsNull()
        {
            Assert.Equal("SessionToken", await this.session.GetTokenAsync());
            this.httpMessageHandler.VerifyRequestUri(new Uri("https://identitysso.betfair.com/api/login"));
        }

        [Fact]
        public async Task OnGetSessionTokenTheSessionTokenIsNotRefreshedIfNotExpiredOrAboutToExpire()
        {
            await this.SetSessionToken("SessionToken");
            this.ResetMessageHandler();
            await this.session.GetTokenAsync();
            this.httpMessageHandler.VerifyTimesCalled(0);
        }

        [Fact]
        public async Task OnGetSessionTokenLoginIsCalledIfSessionIsExpired()
        {
            await this.SetExpiredSessionToken("SessionToken");
            this.ResetMessageHandler();
            await this.session.GetTokenAsync();
            this.httpMessageHandler.VerifyRequestUri(new Uri("https://identitysso.betfair.com/api/login"));
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

        [Fact]
        public async Task OnKeepAliveSessionIsValid()
        {
            await this.SetAboutToExpireSessionToken("SessionToken");
            await this.session.KeepAliveAsync();
            Assert.True(this.session.IsSessionValid);
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
            this.session.WithHandler(responseHandler);
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
                new LoginResponseStub().WithSessionToken(sessionToken));
            this.session.WithHandler(this.httpMessageHandler.Build());
            await this.session.KeepAliveAsync();
            Assert.Equal(sessionToken, await this.session.GetTokenAsync());
        }

        [Fact]
        public async Task OnLoginWithCertUriIsCalledIfCertAddedToSession()
        {
            await this.SetCertSessionToken("SessionToken");
            await this.session.LoginAsync();
            this.httpMessageHandler.VerifyRequestUri(new Uri("https://identitysso-cert.betfair.com/api/certlogin"));
        }

        [Fact]
        public async Task OnLoginWithCertHttpPostMethodIsUsed()
        {
            await this.SetCertSessionToken("SessionToken");
            await this.session.LoginAsync();
            this.httpMessageHandler.VerifyHttpMethod(HttpMethod.Post);
        }

        [Fact]
        public async Task OnLoginWithCertSessionIsValid()
        {
            await this.SetCertSessionToken("SessionToken");
            await this.session.LoginAsync();
            Assert.True(this.session.IsSessionValid);
        }

        [Fact]
        public async Task OnLoginWithCertTheRequestContainsCertificate()
        {
            await this.SetCertSessionToken("SessionToken");
            await this.session.LoginAsync();
            this.httpMessageHandler.VerifyHasCertificate();
        }

        [Theory]
        [InlineData("AppKey")]
        [InlineData("ABC")]
        [InlineData("12345")]
        public async Task OnLoginWithCertAppKeyIsInRequestHeader(string appKey)
        {
            this.session = new Session(appKey, "Username", "Password");
            await this.SetCertSessionToken("SessionToken");
            await this.session.LoginAsync();
            this.httpMessageHandler.VerifyHeaderValues("X-Application", appKey);
        }

        [Theory]
        [InlineData("Username")]
        [InlineData("Kelvin")]
        [InlineData("Bob")]
        public async Task OnLoginWithCertUsernameIsInRequestContent(string username)
        {
            this.session = new Session("AppKey", username, "Password");
            await this.SetCertSessionToken("SessionToken");
            await this.session.LoginAsync();
            this.httpMessageHandler.VerifyRequestContent(ContentString(username, "Password"));
        }

        [Theory]
        [InlineData("Password")]
        [InlineData("123456")]
        [InlineData("qwerty")]
        public async Task OnLoginWithCertPasswordIsInRequestContent(string password)
        {
            this.session = new Session("AppKey", "Username", password);
            await this.SetCertSessionToken("SessionToken");
            await this.session.LoginAsync();
            this.httpMessageHandler.VerifyRequestContent(ContentString("Username", password));
        }

        [Theory]
        [InlineData("FAIL")]
        [InlineData("LIMITED_ACCESS")]
        public async Task OnLoginWithCertThrowIfFailed(string status)
        {
            this.SetExpectedCertHttpResponse(status);
            this.session.WithCert(this.certificate);
            var exception = await Assert.ThrowsAsync<AuthenticationException>(this.session.LoginAsync);
            Assert.Equal($"{status}: NONE", exception.Message);
        }

        [Fact]
        public async Task OnLoginWithCertTheCertLoginResponseIsDeserialized()
        {
            await this.SetCertSessionToken("SessionToken");
        }

        [Theory]
        [InlineData("SessionToken")]
        [InlineData("NewSessionToken")]
        [InlineData("DifferentSessionToken")]
        public async Task OnLoginWithCertSessionTokenIsSet(string sessionToken)
        {
            await this.SetCertSessionToken(sessionToken);
            Assert.Equal(sessionToken, await this.session.GetTokenAsync());
        }

        [Fact]
        public async Task OnLoginWithCertExpiryTimeIsUpdated()
        {
            await this.SetCertSessionToken("SessionToken");
            var expiryTimeBeforeRefresh = this.session.SessionExpiryTime;
            await this.session.LoginAsync();
            Assert.NotEqual(expiryTimeBeforeRefresh, this.session.SessionExpiryTime);
        }

        [Fact]
        public async Task OnLogoutHttpPostMethodIsUsed()
        {
            await this.session.LogoutAsync();
            this.httpMessageHandler.VerifyHttpMethod(HttpMethod.Post);
        }

        [Fact]
        public async Task OnLogoutIdentityUriIsCalled()
        {
            await this.session.LogoutAsync();
            this.httpMessageHandler.VerifyRequestUri(new Uri("https://identitysso.betfair.com/api/logout"));
        }

        [Theory]
        [InlineData("SessionToken")]
        [InlineData("NewSessionToken")]
        [InlineData("DifferentSessionToken")]
        public async Task OnLogoutSessionTokenIsInRequestHeader(string sessionToken)
        {
            await this.SetSessionToken(sessionToken);
            await this.session.LogoutAsync();
            this.httpMessageHandler.VerifyHeaderValues("X-Authentication", sessionToken);
        }

        [Fact]
        public async Task OnLogoutSessionExpiryTimeIsCleared()
        {
            var nullDateTime = DateTime.Parse("0001-01-01T00:00:00.0000000", new DateTimeFormatInfo());
            await this.SetSessionToken("SessionToken");
            await this.session.LogoutAsync();
            Assert.Equal(nullDateTime + this.session.SessionTimeout, this.session.SessionExpiryTime);
        }

        [Fact]
        public async Task OnLogoutSessionTokenIsCleared()
        {
            await this.SetSessionToken("SessionToken");
            Assert.True(this.session.IsSessionValid);
            await this.session.LogoutAsync();
            Assert.False(this.session.IsSessionValid);
        }

        [Theory]
        [InlineData("FAIL")]
        [InlineData("LIMITED_ACCESS")]
        public async Task OnLogoutThrowIfFailed(string status)
        {
            var responseHandler = this.SetExpectedHttpResponse(status, null).Build();
            this.session.WithHandler(responseHandler);
            var exception = await Assert.ThrowsAsync<AuthenticationException>(this.session.LogoutAsync);
            Assert.Equal($"{status}: NONE", exception.Message);
        }

        [Fact]
        public async Task OnDisposeHttpClientIsDisposed()
        {
            this.session.Dispose();
            await Assert.ThrowsAsync<ObjectDisposedException>(() => this.session.LoginAsync());
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
                this.httpMessageHandler.Dispose();
                this.certificate.Dispose();
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

        private HttpMessageHandlerMock SetExpectedHttpResponse(string status, string error)
        {
            return this.httpMessageHandler.WithReturnContent(
                new LoginResponseStub().WithStatus(status).WithError(error));
        }

        private void SetExpectedCertHttpResponse(string status)
        {
            this.httpMessageHandler.WithReturnContent(
                new CertLoginResponseStub { LoginStatus = status });
            this.session.WithHandler(this.httpMessageHandler.Build());
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
                new LoginResponseStub().WithSessionToken(sessionToken));
            this.session.WithHandler(this.httpMessageHandler.Build());
            await this.session.LoginAsync();
        }

        private async Task SetCertSessionToken(string sessionToken)
        {
            this.httpMessageHandler.WithReturnContent(
                new CertLoginResponseStub { SessionToken = sessionToken });
            this.session.WithHandler(this.httpMessageHandler.Build());
            this.session.WithCert(this.certificate);
            await this.session.LoginAsync();
        }

        private void ResetMessageHandler()
        {
            this.httpMessageHandler = new HttpMessageHandlerMock();
            this.httpMessageHandler.WithReturnContent(new LoginResponseStub());
            this.session.WithHandler(this.httpMessageHandler.Build());
        }
    }
}
