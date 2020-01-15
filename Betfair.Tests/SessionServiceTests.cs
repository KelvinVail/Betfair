namespace Betfair.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Security.Authentication;
    using System.Threading.Tasks;

    using TestDoubles;

    using Xunit;

    public class SessionServiceTests
    {
        private readonly SessionService sessionService;

        private readonly MockHttpMessageHandler httpMessageHandler = new MockHttpMessageHandler();

        private readonly HttpClient client;

        public SessionServiceTests()
        {
            this.httpMessageHandler.WithReturnContent(new FakeApiLoginResponse());
            this.client = new HttpClient(this.httpMessageHandler.Build());
            this.sessionService = new SessionService(this.client);
        }

        [Fact]
        public void WhenInitializedDefaultTimeoutIsThirtySeconds()
        {
            const int defaultTimeout = 30;
            Assert.Equal(defaultTimeout, this.sessionService.Timeout);
        }

        [Fact]
        public void AcceptJsonMediaType()
        {
            Assert.Equal("application/json", this.sessionService.AcceptMediaType);
        }

        [Fact]
        public async Task OnLoginThrowIfAppKeyNotSet()
        {
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => this.sessionService.ApiLoginAsync());
            Assert.Equal("AppKey not set.", exception.Message);
        }

        //[Fact]
        //public async Task LoginCallsApiUriIfNoCertProvided()
        //{
        //    await this.sessionService.ApiLoginAsync();
        //    this.AssertIdentityApiUriIsCalled();
        //}

        //[Theory]
        //[InlineData("AppKey")]
        //[InlineData("ABC")]
        //[InlineData("12345")]
        //public async Task AppKeySetInLoginRequestHeader(string appKey)
        //{
        //    await this.LoginWithAppKey(appKey);
        //    this.AssertAppKeyInInHeader(appKey);
        //}

        //[Theory]
        //[InlineData("Username")]
        //[InlineData("Kelvin")]
        //[InlineData("Bob")]
        //public async Task UsernameSetInLoginContent(string username)
        //{
        //    await this.LoginWithUsername(username);
        //    this.AssertUsernameInContent(username);
        //}

        //[Theory]
        //[InlineData("Password")]
        //[InlineData("123456")]
        //[InlineData("qwerty")]
        //public async Task PasswordSetInLoginContent(string password)
        //{
        //    await this.LoginWithPassword(password);
        //    this.AssertPasswordInContent(password);
        //}

        //[Theory]
        //[InlineData("FAIL", "INVALID_USERNAME_OR_PASSWORD")]
        //[InlineData("FAIL", "INPUT_VALIDATION_ERROR")]
        //[InlineData("LIMITED_ACCESS", "PENDING_AUTH")]
        //public async Task ThrowsIfApiLoginFails(string status, string error)
        //{
        //    var exception = await Assert.ThrowsAsync<AuthenticationException>(() => this.LoginWithResponse(status, error));
        //    Assert.Equal($"{status}: {error}", exception.Message);
        //}

        //[Theory]
        //[InlineData(HttpStatusCode.BadRequest)]
        //[InlineData(HttpStatusCode.RequestTimeout)]
        //[InlineData(HttpStatusCode.NotFound)]
        //public async Task ThrowIfApiLoginNotSuccess(HttpStatusCode statusCode)
        //{
        //    var exception = await Assert.ThrowsAsync<AuthenticationException>(() => this.LoginWithResponseCode(statusCode));
        //    Assert.Equal($"{statusCode}", exception.Message);
        //}

        //[Theory]
        //[InlineData("SessionToken")]
        //[InlineData("NewSessionToken")]
        //[InlineData("DifferentSessionToken")]
        //public async Task SessionTokenIsSet(string sessionToken)
        //{
        //    var sessionService = this.SessionServiceWithTokenResponse(sessionToken);
        //    await sessionService.ApiLoginAsync();
        //    Assert.Equal(sessionToken, sessionService.SessionToken);
        //}

        private void AssertTimeoutIs(int timeoutSeconds)
        {
            if (timeoutSeconds <= 0) throw new ArgumentOutOfRangeException(nameof(timeoutSeconds));
            Assert.Equal(
                timeoutSeconds,
                this.sessionService.Timeout);
        }

        private static string ContentString(string username, string password)
        {
            var dict = new Dictionary<string, string> { { "username", username }, { "password", password } };
            return new FormUrlEncodedContent(dict).ReadAsStringAsync().Result;
        }

        //private async Task BuildBetfairClient()
        //{
        //    await new BetfairFactory("AppKey", "Username", "Password")
        //        .WithIdentityHttpClient(this.client)
        //        .Build();
        //}

        //private async Task LoginWithAppKey(string appKey)
        //{
        //    await new BetfairFactory(appKey, "Username", "Password")
        //        .WithIdentityHttpClient(this.client)
        //        .Build();
        //}

        //private async Task LoginWithUsername(string username)
        //{
        //    await new BetfairFactory("AppKey", username, "Password")
        //        .WithIdentityHttpClient(this.client)
        //        .Build();
        //}

        //private async Task LoginWithPassword(string password)
        //{
        //    await new BetfairFactory("AppKey", "Username", password)
        //        .WithIdentityHttpClient(this.client)
        //        .Build();
        //}

        //private async Task LoginWithResponse(string status, string error)
        //{
        //    this.httpMessageHandler.WithReturnContent(
        //        new FakeApiLoginResponse().WithStatus(status).WithError(error));
        //    var httpClient = new HttpClient(this.httpMessageHandler.Build());
        //    await new BetfairFactory("AppKey", "Username", "Password")
        //        .WithIdentityHttpClient(httpClient)
        //        .Build();
        //}

        //private async Task LoginWithResponseCode(HttpStatusCode statusCode)
        //{
        //    this.httpMessageHandler.WithStatusCode(statusCode);
        //    var httpClient = new HttpClient(this.httpMessageHandler.Build());
        //    await new BetfairFactory("AppKey", "Username", "Password")
        //        .WithIdentityHttpClient(httpClient)
        //        .Build();
        //}

        //private SessionService SessionServiceWithTokenResponse(string sessionToken)
        //{
        //    this.httpMessageHandler.WithReturnContent(
        //        new FakeApiLoginResponse().WithSessionToken(sessionToken));
        //    var httpClient = new HttpClient(this.httpMessageHandler.Build());
        //    return new SessionService(httpClient);
        //}

        //private void AssertCallIsMade()
        //{
        //    this.httpMessageHandler.VerifyTimesCalled(1);
        //}

        //private void AssertIdentityApiUriIsCalled()
        //{
        //    this.httpMessageHandler.VerifyRequestUri(new Uri("https://identitysso.betfair.com/api/login"));
        //}

        //private void AssertAppKeyInInHeader(string appKey)
        //{
        //    this.httpMessageHandler.VerifyHeaderValues("X-Application", appKey);
        //}

        private void AssertUsernameInContent(string username)
        {
            this.httpMessageHandler.VerifyRequestContent(ContentString(username, "Password"));
        }

        //private void AssertPasswordInContent(string password)
        //{
        //    this.httpMessageHandler.VerifyRequestContent(ContentString("Username", password));
        //}
    }
}
