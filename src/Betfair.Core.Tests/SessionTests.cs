using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Betfair.Core.Tests.TestDoubles;
using Xunit;

namespace Betfair.Core.Tests
{
    public class SessionTests : IDisposable
    {
        private readonly X509Certificate2 _certificate = new X509Certificate2();
        private HttpMessageHandlerMock _httpMessageHandler = new HttpMessageHandlerMock();
        private Session _session = new Session("AppKey", "Username", "Password");
        private bool _disposedValue;

        public SessionTests()
        {
            _httpMessageHandler.WithReturnContent(new LoginResponseStub());
            _session.WithHandler(_httpMessageHandler.Build());
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
            Assert.Equal("AppKey", _session.AppKey);
        }

        [Fact]
        public void WhenInitializedSessionTimeoutIsEightHours()
        {
            const int defaultSessionTimeout = 8;
            Assert.Equal(defaultSessionTimeout, _session.SessionTimeout.TotalHours, 2);
        }

        [Fact]
        public void AfterInitializedSessionTimeoutCanBeSet()
        {
            const int newSessionTimeout = 2;
            _session.SessionTimeout = TimeSpan.FromHours(newSessionTimeout);
            Assert.Equal(newSessionTimeout, _session.SessionTimeout.TotalHours);
        }

        [Fact]
        public void WhenInitializedKeepAliveOffsetIsOneHour()
        {
            const int defaultKeepAliveOffset = -1;
            Assert.Equal(defaultKeepAliveOffset, _session.KeepAliveOffset.TotalHours, 0);
        }

        [Fact]
        public void AfterInitializedKeepAliveOffsetCanBeSet()
        {
            const int newKeepAliveOffset = -2;
            _session.KeepAliveOffset = TimeSpan.FromHours(newKeepAliveOffset);
            Assert.Equal(newKeepAliveOffset, _session.KeepAliveOffset.TotalHours, 0);
        }

        [Fact]
        public void WhenInitializedSessionExpiryTimeIsSet()
        {
            var nullDateTime = DateTime.Parse("0001-01-01T00:00:00.0000000", new DateTimeFormatInfo());
            Assert.Equal(nullDateTime + _session.SessionTimeout, _session.SessionExpiryTime);
        }

        [Fact]
        public void WhenInitializedSessionIsNotValid()
        {
            Assert.False(_session.IsSessionValid);
        }

        [Fact]
        public async Task OnExpirySessionIsNotValid()
        {
            await SetExpiredSessionToken("SessionToken");
            Assert.False(_session.IsSessionValid);
        }

        [Fact]
        public async Task OnLoginHttpPostMethodIsUsed()
        {
            await _session.LoginAsync();
            _httpMessageHandler.VerifyHttpMethod(HttpMethod.Post);
        }

        [Fact]
        public async Task OnLoginIdentityUriIsCalled()
        {
            await _session.LoginAsync();
            _httpMessageHandler.VerifyRequestUri(new Uri("https://identitysso.betfair.com/api/login"));
        }

        [Theory]
        [InlineData("AppKey")]
        [InlineData("ABC")]
        [InlineData("12345")]
        public async Task OnLoginAppKeyIsInRequestHeader(string appKey)
        {
            _session = new Session(appKey, "Username", "Password");
            _session.WithHandler(_httpMessageHandler.Build());
            await _session.LoginAsync();
            _httpMessageHandler.VerifyHeaderValues("X-Application", appKey);
        }

        [Theory]
        [InlineData("Username")]
        [InlineData("Kelvin")]
        [InlineData("Bob")]
        public async Task OnLoginUsernameIsInRequestContent(string username)
        {
            _session = new Session("AppKey", username, "Password");
            _session.WithHandler(_httpMessageHandler.Build());
            await _session.LoginAsync();
            _httpMessageHandler.VerifyRequestContent(ContentString(username, "Password"));
        }

        [Theory]
        [InlineData("Password")]
        [InlineData("123456")]
        [InlineData("qwerty")]
        public async Task OnLoginPasswordIsInRequestContent(string password)
        {
            _session = new Session("AppKey", "Username", password);
            _session.WithHandler(_httpMessageHandler.Build());
            await _session.LoginAsync();
            _httpMessageHandler.VerifyRequestContent(ContentString("Username", password));
        }

        [Theory]
        [InlineData("FAIL", "INVALID_USERNAME_OR_PASSWORD")]
        [InlineData("FAIL", "INPUT_VALIDATION_ERROR")]
        [InlineData("LIMITED_ACCESS", "PENDING_AUTH")]
        public async Task OnLoginThrowIfFailed(string status, string error)
        {
            var responseHandler = SetExpectedHttpResponse(status, error).Build();
            _session.WithHandler(responseHandler);
            var exception = await Assert.ThrowsAsync<AuthenticationException>(_session.LoginAsync);
            Assert.Equal($"{status}: {error}", exception.Message);
        }

        [Theory]
        [InlineData("SessionToken")]
        [InlineData("NewSessionToken")]
        [InlineData("DifferentSessionToken")]
        public async Task OnLoginSessionTokenIsSet(string sessionToken)
        {
            await SetSessionToken(sessionToken);
            Assert.Equal(sessionToken, await _session.GetTokenAsync());
        }

        [Fact]
        public async Task OnLoginSessionExpiryTimeIsUpdated()
        {
            var expiryTimeBeforeRefresh = _session.SessionExpiryTime;
            await _session.LoginAsync();
            Assert.NotEqual(expiryTimeBeforeRefresh, _session.SessionExpiryTime);
        }

        [Fact]
        public async Task OnLoginSessionIsValid()
        {
            await _session.LoginAsync();
            Assert.True(_session.IsSessionValid);
        }

        [Fact]
        public async Task OnGetSessionTokenLoginIsCalledIfSessionTokenIsNull()
        {
            Assert.Equal("SessionToken", await _session.GetTokenAsync());
            _httpMessageHandler.VerifyRequestUri(new Uri("https://identitysso.betfair.com/api/login"));
        }

        [Fact]
        public async Task OnGetSessionTokenTheSessionTokenIsNotRefreshedIfNotExpiredOrAboutToExpire()
        {
            await SetSessionToken("SessionToken");
            ResetMessageHandler();
            await _session.GetTokenAsync();
            _httpMessageHandler.VerifyTimesCalled(0);
        }

        [Fact]
        public async Task OnGetSessionTokenLoginIsCalledIfSessionIsExpired()
        {
            await SetExpiredSessionToken("SessionToken");
            ResetMessageHandler();
            await _session.GetTokenAsync();
            _httpMessageHandler.VerifyRequestUri(new Uri("https://identitysso.betfair.com/api/login"));
        }

        [Fact]
        public async Task OnKeepAliveTheKeepAliveUriIsCalledIfSessionIsAboutToExpire()
        {
            await SetAboutToExpireSessionToken("SessionToken");
            await _session.KeepAliveAsync();
            _httpMessageHandler.VerifyRequestUri(new Uri("https://identitysso.betfair.com/api/keepAlive"));
        }

        [Fact]
        public async Task OnKeepAliveHttpPostMethodIsUsed()
        {
            await SetAboutToExpireSessionToken("SessionToken");
            await _session.KeepAliveAsync();
            _httpMessageHandler.VerifyHttpMethod(HttpMethod.Post);
        }

        [Fact]
        public async Task OnKeepAliveSessionIsValid()
        {
            await SetAboutToExpireSessionToken("SessionToken");
            await _session.KeepAliveAsync();
            Assert.True(_session.IsSessionValid);
        }

        [Theory]
        [InlineData("SessionToken")]
        [InlineData("NewSessionToken")]
        [InlineData("DifferentSessionToken")]
        public async Task OnKeepAliveSessionTokenIsInRequestHeader(string sessionToken)
        {
            await SetAboutToExpireSessionToken(sessionToken);
            await _session.KeepAliveAsync();
            _httpMessageHandler.VerifyHeaderValues("X-Authentication", sessionToken);
        }

        [Fact]
        public async Task OnKeepAliveSessionExpiryTimeIsUpdated()
        {
            await SetAboutToExpireSessionToken("SessionToken");
            var expiryTimeBeforeRefresh = _session.SessionExpiryTime;
            await _session.KeepAliveAsync();
            Assert.NotEqual(expiryTimeBeforeRefresh, _session.SessionExpiryTime);
        }

        [Theory]
        [InlineData("FAIL", "INVALID_USERNAME_OR_PASSWORD")]
        [InlineData("FAIL", "INPUT_VALIDATION_ERROR")]
        [InlineData("LIMITED_ACCESS", "PENDING_AUTH")]
        public async Task OnKeepAliveThrowIfFailed(string status, string error)
        {
            await SetAboutToExpireSessionToken("SessionToken");
            var responseHandler = SetExpectedHttpResponse(status, error).Build();
            _session.WithHandler(responseHandler);
            var exception = await Assert.ThrowsAsync<AuthenticationException>(_session.KeepAliveAsync);
            Assert.Equal($"{status}: {error}", exception.Message);
        }

        [Theory]
        [InlineData("SessionToken")]
        [InlineData("NewSessionToken")]
        [InlineData("DifferentSessionToken")]
        public async Task OnKeepAliveSessionTokenIsSet(string sessionToken)
        {
            await SetAboutToExpireSessionToken("AboutToExpireSessionToken");
            _httpMessageHandler.WithReturnContent(
                new LoginResponseStub().WithSessionToken(sessionToken));
            _session.WithHandler(_httpMessageHandler.Build());
            await _session.KeepAliveAsync();
            Assert.Equal(sessionToken, await _session.GetTokenAsync());
        }

        [Fact]
        public async Task OnLoginWithCertUriIsCalledIfCertAddedToSession()
        {
            await SetCertSessionToken("SessionToken");
            await _session.LoginAsync();
            _httpMessageHandler.VerifyRequestUri(new Uri("https://identitysso-cert.betfair.com/api/certlogin"));
        }

        [Fact]
        public async Task OnLoginWithCertHttpPostMethodIsUsed()
        {
            await SetCertSessionToken("SessionToken");
            await _session.LoginAsync();
            _httpMessageHandler.VerifyHttpMethod(HttpMethod.Post);
        }

        [Fact]
        public async Task OnLoginWithCertSessionIsValid()
        {
            await SetCertSessionToken("SessionToken");
            await _session.LoginAsync();
            Assert.True(_session.IsSessionValid);
        }

        [Fact]
        public async Task OnLoginWithCertTheRequestContainsCertificate()
        {
            await SetCertSessionToken("SessionToken");
            await _session.LoginAsync();
            _httpMessageHandler.VerifyHasCertificate();
        }

        [Theory]
        [InlineData("AppKey")]
        [InlineData("ABC")]
        [InlineData("12345")]
        public async Task OnLoginWithCertAppKeyIsInRequestHeader(string appKey)
        {
            _session = new Session(appKey, "Username", "Password");
            await SetCertSessionToken("SessionToken");
            await _session.LoginAsync();
            _httpMessageHandler.VerifyHeaderValues("X-Application", appKey);
        }

        [Theory]
        [InlineData("Username")]
        [InlineData("Kelvin")]
        [InlineData("Bob")]
        public async Task OnLoginWithCertUsernameIsInRequestContent(string username)
        {
            _session = new Session("AppKey", username, "Password");
            await SetCertSessionToken("SessionToken");
            await _session.LoginAsync();
            _httpMessageHandler.VerifyRequestContent(ContentString(username, "Password"));
        }

        [Theory]
        [InlineData("Password")]
        [InlineData("123456")]
        [InlineData("qwerty")]
        public async Task OnLoginWithCertPasswordIsInRequestContent(string password)
        {
            _session = new Session("AppKey", "Username", password);
            await SetCertSessionToken("SessionToken");
            await _session.LoginAsync();
            _httpMessageHandler.VerifyRequestContent(ContentString("Username", password));
        }

        [Theory]
        [InlineData("FAIL")]
        [InlineData("LIMITED_ACCESS")]
        public async Task OnLoginWithCertThrowIfFailed(string status)
        {
            SetExpectedCertHttpResponse(status);
            _session.WithCert(_certificate);
            var exception = await Assert.ThrowsAsync<AuthenticationException>(_session.LoginAsync);
            Assert.Equal($"{status}: NONE", exception.Message);
        }

        [Fact]
        public async Task OnLoginWithCertTheCertLoginResponseIsDeserialized()
        {
            await SetCertSessionToken("SessionToken");
        }

        [Theory]
        [InlineData("SessionToken")]
        [InlineData("NewSessionToken")]
        [InlineData("DifferentSessionToken")]
        public async Task OnLoginWithCertSessionTokenIsSet(string sessionToken)
        {
            await SetCertSessionToken(sessionToken);
            Assert.Equal(sessionToken, await _session.GetTokenAsync());
        }

        [Fact]
        public async Task OnLoginWithCertExpiryTimeIsUpdated()
        {
            await SetCertSessionToken("SessionToken");
            var expiryTimeBeforeRefresh = _session.SessionExpiryTime;
            await _session.LoginAsync();
            Assert.NotEqual(expiryTimeBeforeRefresh, _session.SessionExpiryTime);
        }

        [Fact]
        public async Task OnLogoutHttpPostMethodIsUsed()
        {
            await _session.LogoutAsync();
            _httpMessageHandler.VerifyHttpMethod(HttpMethod.Post);
        }

        [Fact]
        public async Task OnLogoutIdentityUriIsCalled()
        {
            await _session.LogoutAsync();
            _httpMessageHandler.VerifyRequestUri(new Uri("https://identitysso.betfair.com/api/logout"));
        }

        [Theory]
        [InlineData("SessionToken")]
        [InlineData("NewSessionToken")]
        [InlineData("DifferentSessionToken")]
        public async Task OnLogoutSessionTokenIsInRequestHeader(string sessionToken)
        {
            await SetSessionToken(sessionToken);
            await _session.LogoutAsync();
            _httpMessageHandler.VerifyHeaderValues("X-Authentication", sessionToken);
        }

        [Fact]
        public async Task OnLogoutSessionExpiryTimeIsCleared()
        {
            var nullDateTime = DateTime.Parse("0001-01-01T00:00:00.0000000", new DateTimeFormatInfo());
            await SetSessionToken("SessionToken");
            await _session.LogoutAsync();
            Assert.Equal(nullDateTime + _session.SessionTimeout, _session.SessionExpiryTime);
        }

        [Fact]
        public async Task OnLogoutSessionTokenIsCleared()
        {
            await SetSessionToken("SessionToken");
            Assert.True(_session.IsSessionValid);
            await _session.LogoutAsync();
            Assert.False(_session.IsSessionValid);
        }

        [Theory]
        [InlineData("FAIL")]
        [InlineData("LIMITED_ACCESS")]
        public async Task OnLogoutThrowIfFailed(string status)
        {
            var responseHandler = SetExpectedHttpResponse(status, null).Build();
            _session.WithHandler(responseHandler);
            var exception = await Assert.ThrowsAsync<AuthenticationException>(_session.LogoutAsync);
            Assert.Equal($"{status}: NONE", exception.Message);
        }

        [Fact]
        public async Task OnDisposeHttpClientIsDisposed()
        {
            _session.Dispose();
            await Assert.ThrowsAsync<ObjectDisposedException>(() => _session.LoginAsync());
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposedValue) return;
            if (disposing)
            {
                _httpMessageHandler.Dispose();
                _certificate.Dispose();
            }

            _session.Dispose();

            _disposedValue = true;
        }

        private static string ContentString(string username, string password)
        {
            var dict = new Dictionary<string, string> { { "username", username }, { "password", password } };
            using var content = new FormUrlEncodedContent(dict);
            return content.ReadAsStringAsync().Result;
        }

        private HttpMessageHandlerMock SetExpectedHttpResponse(string status, string error)
        {
            return _httpMessageHandler.WithReturnContent(
                new LoginResponseStub().WithStatus(status).WithError(error));
        }

        private void SetExpectedCertHttpResponse(string status)
        {
            _httpMessageHandler.WithReturnContent(
                new CertLoginResponseStub { LoginStatus = status });
            _session.WithHandler(_httpMessageHandler.Build());
        }

        private async Task SetExpiredSessionToken(string sessionToken)
        {
            _session.SessionTimeout = TimeSpan.FromHours(-1);
            await SetSessionToken(sessionToken);
        }

        private async Task SetAboutToExpireSessionToken(string sessionToken)
        {
            _session.SessionTimeout = TimeSpan.FromMinutes(30);
            await SetSessionToken(sessionToken);
            ResetMessageHandler();
            _httpMessageHandler.VerifyTimesCalled(0);
        }

        private async Task SetSessionToken(string sessionToken)
        {
            _httpMessageHandler.WithReturnContent(
                new LoginResponseStub().WithSessionToken(sessionToken));
            _session.WithHandler(_httpMessageHandler.Build());
            await _session.LoginAsync();
        }

        private async Task SetCertSessionToken(string sessionToken)
        {
            _httpMessageHandler.WithReturnContent(
                new CertLoginResponseStub { SessionToken = sessionToken });
            _session.WithHandler(_httpMessageHandler.Build());
            _session.WithCert(_certificate);
            await _session.LoginAsync();
        }

        private void ResetMessageHandler()
        {
            _httpMessageHandler = new HttpMessageHandlerMock();
            _httpMessageHandler.WithReturnContent(new LoginResponseStub());
            _session.WithHandler(_httpMessageHandler.Build());
        }
    }
}
