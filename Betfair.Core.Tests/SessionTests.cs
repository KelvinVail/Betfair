namespace Betfair.Core.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Net.Http;
    using System.Security.Authentication;
    using System.Security.Cryptography.X509Certificates;
    using System.Threading.Tasks;
    using Betfair.Core.Tests.TestDoubles;
    using Xunit;

    public class SessionTests : IDisposable
    {
        private readonly X509Certificate2 certificate = new X509Certificate2();

        private HttpMessageHandlerMock httpMessageHandler = new HttpMessageHandlerMock();

        private Client client = new Client("AppKey", "Username", "Password");

        private bool disposedValue;

        public SessionTests()
        {
            this.httpMessageHandler.WithReturnContent(new LoginResponseStub());
            this.client.WithHandler(this.httpMessageHandler.Build());
        }

        [Fact]
        public void WhenInitializedThrowIfAppKeyIsNull()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => new Client(null, "Username", "Password"));
            Assert.Equal("appKey", exception.ParamName);
        }

        [Fact]
        public void WhenInitializedThrowIfUsernameIsNull()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => new Client("AppKey", null, "Password"));
            Assert.Equal("username", exception.ParamName);
        }

        [Fact]
        public void WhenInitializedThrowIfPasswordIsNull()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => new Client("AppKey", "Username", null));
            Assert.Equal("password", exception.ParamName);
        }

        [Fact]
        public void WhenInitializedAppKeyIsSet()
        {
            Assert.Equal("AppKey", this.client.AppKey);
        }

        [Fact]
        public void WhenInitializedSessionTimeoutIsEightHours()
        {
            const int defaultSessionTimeout = 8;
            Assert.Equal(defaultSessionTimeout, this.client.SessionTimeout.TotalHours, 2);
        }

        [Fact]
        public void AfterInitializedSessionTimeoutCanBeSet()
        {
            const int newSessionTimeout = 2;
            this.client.SessionTimeout = TimeSpan.FromHours(newSessionTimeout);
            Assert.Equal(newSessionTimeout, this.client.SessionTimeout.TotalHours);
        }

        [Fact]
        public void WhenInitializedKeepAliveOffsetIsOneHour()
        {
            const int defaultKeepAliveOffset = -1;
            Assert.Equal(defaultKeepAliveOffset, this.client.KeepAliveOffset.TotalHours, 0);
        }

        [Fact]
        public void AfterInitializedKeepAliveOffsetCanBeSet()
        {
            const int newKeepAliveOffset = -2;
            this.client.KeepAliveOffset = TimeSpan.FromHours(newKeepAliveOffset);
            Assert.Equal(newKeepAliveOffset, this.client.KeepAliveOffset.TotalHours, 0);
        }

        [Fact]
        public void WhenInitializedSessionExpiryTimeIsSet()
        {
            var nullDateTime = DateTime.Parse("0001-01-01T00:00:00.0000000", new DateTimeFormatInfo());
            Assert.Equal(nullDateTime + this.client.SessionTimeout, this.client.SessionExpiryTime);
        }

        [Fact]
        public void WhenInitializedSessionIsNotValid()
        {
            Assert.False(this.client.IsSessionValid);
        }

        [Fact]
        public async Task OnExpirySessionIsNotValid()
        {
            await this.SetExpiredSessionToken("SessionToken");
            Assert.False(this.client.IsSessionValid);
        }

        [Fact]
        public async Task OnLoginHttpPostMethodIsUsed()
        {
            await this.client.LoginAsync();
            this.httpMessageHandler.VerifyHttpMethod(HttpMethod.Post);
        }

        [Fact]
        public async Task OnLoginIdentityUriIsCalled()
        {
            await this.client.LoginAsync();
            this.httpMessageHandler.VerifyRequestUri(new Uri("https://identitysso.betfair.com/api/login"));
        }

        [Theory]
        [InlineData("AppKey")]
        [InlineData("ABC")]
        [InlineData("12345")]
        public async Task OnLoginAppKeyIsInRequestHeader(string appKey)
        {
            this.client = new Client(appKey, "Username", "Password");
            this.client.WithHandler(this.httpMessageHandler.Build());
            await this.client.LoginAsync();
            this.httpMessageHandler.VerifyHeaderValues("X-Application", appKey);
        }

        [Theory]
        [InlineData("Username")]
        [InlineData("Kelvin")]
        [InlineData("Bob")]
        public async Task OnLoginUsernameIsInRequestContent(string username)
        {
            this.client = new Client("AppKey", username, "Password");
            this.client.WithHandler(this.httpMessageHandler.Build());
            await this.client.LoginAsync();
            this.httpMessageHandler.VerifyRequestContent(ContentString(username, "Password"));
        }

        [Theory]
        [InlineData("Password")]
        [InlineData("123456")]
        [InlineData("qwerty")]
        public async Task OnLoginPasswordIsInRequestContent(string password)
        {
            this.client = new Client("AppKey", "Username", password);
            this.client.WithHandler(this.httpMessageHandler.Build());
            await this.client.LoginAsync();
            this.httpMessageHandler.VerifyRequestContent(ContentString("Username", password));
        }

        [Theory]
        [InlineData("FAIL", "INVALID_USERNAME_OR_PASSWORD")]
        [InlineData("FAIL", "INPUT_VALIDATION_ERROR")]
        [InlineData("LIMITED_ACCESS", "PENDING_AUTH")]
        public async Task OnLoginThrowIfFailed(string status, string error)
        {
            var responseHandler = this.SetExpectedHttpResponse(status, error).Build();
            this.client.WithHandler(responseHandler);
            var exception = await Assert.ThrowsAsync<AuthenticationException>(this.client.LoginAsync);
            Assert.Equal($"{status}: {error}", exception.Message);
        }

        [Theory]
        [InlineData("SessionToken")]
        [InlineData("NewSessionToken")]
        [InlineData("DifferentSessionToken")]
        public async Task OnLoginSessionTokenIsSet(string sessionToken)
        {
            await this.SetSessionToken(sessionToken);
            Assert.Equal(sessionToken, await this.client.GetTokenAsync());
        }

        [Fact]
        public async Task OnLoginSessionExpiryTimeIsUpdated()
        {
            var expiryTimeBeforeRefresh = this.client.SessionExpiryTime;
            await this.client.LoginAsync();
            Assert.NotEqual(expiryTimeBeforeRefresh, this.client.SessionExpiryTime);
        }

        [Fact]
        public async Task OnLoginSessionIsValid()
        {
            await this.client.LoginAsync();
            Assert.True(this.client.IsSessionValid);
        }

        [Fact]
        public async Task OnGetSessionTokenLoginIsCalledIfSessionTokenIsNull()
        {
            Assert.Equal("SessionToken", await this.client.GetTokenAsync());
            this.httpMessageHandler.VerifyRequestUri(new Uri("https://identitysso.betfair.com/api/login"));
        }

        [Fact]
        public async Task OnGetSessionTokenTheSessionTokenIsNotRefreshedIfNotExpiredOrAboutToExpire()
        {
            await this.SetSessionToken("SessionToken");
            this.ResetMessageHandler();
            await this.client.GetTokenAsync();
            this.httpMessageHandler.VerifyTimesCalled(0);
        }

        [Fact]
        public async Task OnGetSessionTokenLoginIsCalledIfSessionIsExpired()
        {
            await this.SetExpiredSessionToken("SessionToken");
            this.ResetMessageHandler();
            await this.client.GetTokenAsync();
            this.httpMessageHandler.VerifyRequestUri(new Uri("https://identitysso.betfair.com/api/login"));
        }

        [Fact]
        public async Task OnKeepAliveTheKeepAliveUriIsCalledIfSessionIsAboutToExpire()
        {
            await this.SetAboutToExpireSessionToken("SessionToken");
            await this.client.KeepAliveAsync();
            this.httpMessageHandler.VerifyRequestUri(new Uri("https://identitysso.betfair.com/api/keepAlive"));
        }

        [Fact]
        public async Task OnKeepAliveHttpPostMethodIsUsed()
        {
            await this.SetAboutToExpireSessionToken("SessionToken");
            await this.client.KeepAliveAsync();
            this.httpMessageHandler.VerifyHttpMethod(HttpMethod.Post);
        }

        [Fact]
        public async Task OnKeepAliveSessionIsValid()
        {
            await this.SetAboutToExpireSessionToken("SessionToken");
            await this.client.KeepAliveAsync();
            Assert.True(this.client.IsSessionValid);
        }

        [Theory]
        [InlineData("SessionToken")]
        [InlineData("NewSessionToken")]
        [InlineData("DifferentSessionToken")]
        public async Task OnKeepAliveSessionTokenIsInRequestHeader(string sessionToken)
        {
            await this.SetAboutToExpireSessionToken(sessionToken);
            await this.client.KeepAliveAsync();
            this.httpMessageHandler.VerifyHeaderValues("X-Authentication", sessionToken);
        }

        [Fact]
        public async Task OnKeepAliveSessionExpiryTimeIsUpdated()
        {
            await this.SetAboutToExpireSessionToken("SessionToken");
            var expiryTimeBeforeRefresh = this.client.SessionExpiryTime;
            await this.client.KeepAliveAsync();
            Assert.NotEqual(expiryTimeBeforeRefresh, this.client.SessionExpiryTime);
        }

        [Theory]
        [InlineData("FAIL", "INVALID_USERNAME_OR_PASSWORD")]
        [InlineData("FAIL", "INPUT_VALIDATION_ERROR")]
        [InlineData("LIMITED_ACCESS", "PENDING_AUTH")]
        public async Task OnKeepAliveThrowIfFailed(string status, string error)
        {
            await this.SetAboutToExpireSessionToken("SessionToken");
            var responseHandler = this.SetExpectedHttpResponse(status, error).Build();
            this.client.WithHandler(responseHandler);
            var exception = await Assert.ThrowsAsync<AuthenticationException>(this.client.KeepAliveAsync);
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
            this.client.WithHandler(this.httpMessageHandler.Build());
            await this.client.KeepAliveAsync();
            Assert.Equal(sessionToken, await this.client.GetTokenAsync());
        }

        [Fact]
        public async Task OnLoginWithCertUriIsCalledIfCertAddedToSession()
        {
            await this.SetCertSessionToken("SessionToken");
            await this.client.LoginAsync();
            this.httpMessageHandler.VerifyRequestUri(new Uri("https://identitysso-cert.betfair.com/api/certlogin"));
        }

        [Fact]
        public async Task OnLoginWithCertHttpPostMethodIsUsed()
        {
            await this.SetCertSessionToken("SessionToken");
            await this.client.LoginAsync();
            this.httpMessageHandler.VerifyHttpMethod(HttpMethod.Post);
        }

        [Fact]
        public async Task OnLoginWithCertSessionIsValid()
        {
            await this.SetCertSessionToken("SessionToken");
            await this.client.LoginAsync();
            Assert.True(this.client.IsSessionValid);
        }

        [Fact]
        public async Task OnLoginWithCertTheRequestContainsCertificate()
        {
            await this.SetCertSessionToken("SessionToken");
            await this.client.LoginAsync();
            this.httpMessageHandler.VerifyHasCertificate();
        }

        [Theory]
        [InlineData("AppKey")]
        [InlineData("ABC")]
        [InlineData("12345")]
        public async Task OnLoginWithCertAppKeyIsInRequestHeader(string appKey)
        {
            this.client = new Client(appKey, "Username", "Password");
            await this.SetCertSessionToken("SessionToken");
            await this.client.LoginAsync();
            this.httpMessageHandler.VerifyHeaderValues("X-Application", appKey);
        }

        [Theory]
        [InlineData("Username")]
        [InlineData("Kelvin")]
        [InlineData("Bob")]
        public async Task OnLoginWithCertUsernameIsInRequestContent(string username)
        {
            this.client = new Client("AppKey", username, "Password");
            await this.SetCertSessionToken("SessionToken");
            await this.client.LoginAsync();
            this.httpMessageHandler.VerifyRequestContent(ContentString(username, "Password"));
        }

        [Theory]
        [InlineData("Password")]
        [InlineData("123456")]
        [InlineData("qwerty")]
        public async Task OnLoginWithCertPasswordIsInRequestContent(string password)
        {
            this.client = new Client("AppKey", "Username", password);
            await this.SetCertSessionToken("SessionToken");
            await this.client.LoginAsync();
            this.httpMessageHandler.VerifyRequestContent(ContentString("Username", password));
        }

        [Theory]
        [InlineData("FAIL")]
        [InlineData("LIMITED_ACCESS")]
        public async Task OnLoginWithCertThrowIfFailed(string status)
        {
            this.SetExpectedCertHttpResponse(status);
            this.client.WithCert(this.certificate);
            var exception = await Assert.ThrowsAsync<AuthenticationException>(this.client.LoginAsync);
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
            Assert.Equal(sessionToken, await this.client.GetTokenAsync());
        }

        [Fact]
        public async Task OnLoginWithCertExpiryTimeIsUpdated()
        {
            await this.SetCertSessionToken("SessionToken");
            var expiryTimeBeforeRefresh = this.client.SessionExpiryTime;
            await this.client.LoginAsync();
            Assert.NotEqual(expiryTimeBeforeRefresh, this.client.SessionExpiryTime);
        }

        [Fact]
        public async Task OnLogoutHttpPostMethodIsUsed()
        {
            await this.client.LogoutAsync();
            this.httpMessageHandler.VerifyHttpMethod(HttpMethod.Post);
        }

        [Fact]
        public async Task OnLogoutIdentityUriIsCalled()
        {
            await this.client.LogoutAsync();
            this.httpMessageHandler.VerifyRequestUri(new Uri("https://identitysso.betfair.com/api/logout"));
        }

        [Theory]
        [InlineData("SessionToken")]
        [InlineData("NewSessionToken")]
        [InlineData("DifferentSessionToken")]
        public async Task OnLogoutSessionTokenIsInRequestHeader(string sessionToken)
        {
            await this.SetSessionToken(sessionToken);
            await this.client.LogoutAsync();
            this.httpMessageHandler.VerifyHeaderValues("X-Authentication", sessionToken);
        }

        [Fact]
        public async Task OnLogoutSessionExpiryTimeIsCleared()
        {
            var nullDateTime = DateTime.Parse("0001-01-01T00:00:00.0000000", new DateTimeFormatInfo());
            await this.SetSessionToken("SessionToken");
            await this.client.LogoutAsync();
            Assert.Equal(nullDateTime + this.client.SessionTimeout, this.client.SessionExpiryTime);
        }

        [Fact]
        public async Task OnLogoutSessionTokenIsCleared()
        {
            await this.SetSessionToken("SessionToken");
            Assert.True(this.client.IsSessionValid);
            await this.client.LogoutAsync();
            Assert.False(this.client.IsSessionValid);
        }

        [Theory]
        [InlineData("FAIL")]
        [InlineData("LIMITED_ACCESS")]
        public async Task OnLogoutThrowIfFailed(string status)
        {
            var responseHandler = this.SetExpectedHttpResponse(status, null).Build();
            this.client.WithHandler(responseHandler);
            var exception = await Assert.ThrowsAsync<AuthenticationException>(this.client.LogoutAsync);
            Assert.Equal($"{status}: NONE", exception.Message);
        }

        [Fact]
        public async Task OnDisposeHttpClientIsDisposed()
        {
            this.client.Dispose();
            await Assert.ThrowsAsync<ObjectDisposedException>(() => this.client.LoginAsync());
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

            this.client.Dispose();

            this.disposedValue = true;
        }

        private static string ContentString(string username, string password)
        {
            var dict = new Dictionary<string, string> { { "username", username }, { "password", password } };
            using var content = new FormUrlEncodedContent(dict);
            return content.ReadAsStringAsync().Result;
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
            this.client.WithHandler(this.httpMessageHandler.Build());
        }

        private async Task SetExpiredSessionToken(string sessionToken)
        {
            this.client.SessionTimeout = TimeSpan.FromHours(-1);
            await this.SetSessionToken(sessionToken);
        }

        private async Task SetAboutToExpireSessionToken(string sessionToken)
        {
            this.client.SessionTimeout = TimeSpan.FromMinutes(30);
            await this.SetSessionToken(sessionToken);
            this.ResetMessageHandler();
            this.httpMessageHandler.VerifyTimesCalled(0);
        }

        private async Task SetSessionToken(string sessionToken)
        {
            this.httpMessageHandler.WithReturnContent(
                new LoginResponseStub().WithSessionToken(sessionToken));
            this.client.WithHandler(this.httpMessageHandler.Build());
            await this.client.LoginAsync();
        }

        private async Task SetCertSessionToken(string sessionToken)
        {
            this.httpMessageHandler.WithReturnContent(
                new CertLoginResponseStub { SessionToken = sessionToken });
            this.client.WithHandler(this.httpMessageHandler.Build());
            this.client.WithCert(this.certificate);
            await this.client.LoginAsync();
        }

        private void ResetMessageHandler()
        {
            this.httpMessageHandler = new HttpMessageHandlerMock();
            this.httpMessageHandler.WithReturnContent(new LoginResponseStub());
            this.client.WithHandler(this.httpMessageHandler.Build());
        }
    }
}
