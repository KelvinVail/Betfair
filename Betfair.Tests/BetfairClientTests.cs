namespace Betfair.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Security.Authentication;
    using System.Threading.Tasks;

    using Betfair.Tests.TestDoubles;

    using Xunit;

    public class BetfairClientTests
    {
        private readonly MockHttpMessageHandler httpMessageHandler = new MockHttpMessageHandler();

        private readonly HttpClient client;

        public BetfairClientTests()
        {
            this.httpMessageHandler.WithReturnContent(new FakeApiLoginResponse());
            this.client = new HttpClient(this.httpMessageHandler.Build());
        }

        [Fact]
        public void HttpClientIsNotCalledWhenBuilt()
        {
            this.GetBetfairClient();
            this.AssertCallIsNotMade();
        }

        [Fact]
        public async Task LoginCallsApiUriIfNoCertProvided()
        {
            await this.GetBetfairClient().ApiLoginAsync();
            this.AssertIdentityApiUriIsCalled();
        }

        [Theory]
        [InlineData("AppKey")]
        [InlineData("ABC")]
        [InlineData("12345")]
        public async Task AppKeySetInLoginRequestHeader(string appKey)
        {
            await this.LoginWithAppKey(appKey);
            this.AssertAppKeyInInHeader(appKey);
        }

        [Theory]
        [InlineData("Username")]
        [InlineData("Kelvin")]
        [InlineData("Bob")]
        public async Task UsernameSetInLoginContent(string username)
        {
            await this.LoginWithUsername(username);
            this.AssertUsernameInContent(username);
        }

        [Theory]
        [InlineData("Password")]
        [InlineData("123456")]
        [InlineData("qwerty")]
        public async Task PasswordSetInLoginContent(string password)
        {
            await this.LoginWithPassword(password);
            this.AssertPasswordInContent(password);
        }

        [Theory]
        [InlineData("FAIL", "INVALID_USERNAME_OR_PASSWORD")]
        [InlineData("FAIL", "INPUT_VALIDATION_ERROR")]
        [InlineData("LIMITED_ACCESS", "PENDING_AUTH")]
        public async Task ThrowsIfApiLoginFails(string status, string error)
        {
            var exception = await Assert.ThrowsAsync<AuthenticationException>(() => this.LoginWithResponse(status, error));
            Assert.Equal($"{status}: {error}", exception.Message);
        }

        [Theory]
        [InlineData(HttpStatusCode.BadRequest)]
        [InlineData(HttpStatusCode.RequestTimeout)]
        [InlineData(HttpStatusCode.NotFound)]
        public async Task ThrowIfApiLoginNotSuccess(HttpStatusCode statusCode)
        {
            var exception = await Assert.ThrowsAsync<AuthenticationException>(() => this.LoginWithResponseCode(statusCode));
            Assert.Equal($"{statusCode}", exception.Message);
        }

        [Theory]
        [InlineData("SessionToken")]
        [InlineData("ABCDEF")]
        [InlineData("HIGKLM")]
        public async Task SessionTokenIsSet(string sessionToken)
        {
            var betfair = this.GetBetfairClientSessionResponse(sessionToken);
            await betfair.ApiLoginAsync();
            Assert.Equal(sessionToken, betfair.SessionToken);
        }

        private static string ContentString(string username, string password)
        {
            var dict = new Dictionary<string, string> { { "username", username }, { "password", password } };
            return new FormUrlEncodedContent(dict).ReadAsStringAsync().Result;
        }

        private BetfairClient GetBetfairClient()
        {
            return new BetfairFactory("AppKey", "Username", "Password")
                .WithIdentityHttpClient(this.client)
                .Build();
        }

        private async Task LoginWithAppKey(string appKey)
        {
            await new BetfairFactory(appKey, "Username", "Password")
                .WithIdentityHttpClient(this.client)
                .Build()
                .ApiLoginAsync();
        }

        private async Task LoginWithUsername(string username)
        {
            await new BetfairFactory("AppKey", username, "Password")
                .WithIdentityHttpClient(this.client)
                .Build()
                .ApiLoginAsync();
        }

        private async Task LoginWithPassword(string password)
        {
            await new BetfairFactory("AppKey", "Username", password)
                .WithIdentityHttpClient(this.client)
                .Build()
                .ApiLoginAsync();
        }

        private async Task LoginWithResponse(string status, string error)
        {
            this.httpMessageHandler.WithReturnContent(
                new FakeApiLoginResponse().WithStatus(status).WithError(error));
            var httpClient = new HttpClient(this.httpMessageHandler.Build());
            await new BetfairFactory("AppKey", "Username", "Password")
                .WithIdentityHttpClient(httpClient)
                .Build()
                .ApiLoginAsync();
        }

        private async Task LoginWithResponseCode(HttpStatusCode statusCode)
        {
            this.httpMessageHandler.WithStatusCode(statusCode);
            var httpClient = new HttpClient(this.httpMessageHandler.Build());
            await new BetfairFactory("AppKey", "Username", "Password")
                .WithIdentityHttpClient(httpClient)
                .Build()
                .ApiLoginAsync();
        }

        private BetfairClient GetBetfairClientSessionResponse(string sessionToken)
        {
            this.httpMessageHandler.WithReturnContent(
                new FakeApiLoginResponse().WithSessionToken(sessionToken));
            var httpClient = new HttpClient(this.httpMessageHandler.Build());
            return new BetfairFactory("AppKey", "Username", "Password").WithIdentityHttpClient(httpClient).Build();
        }

        private void AssertCallIsNotMade()
        {
            this.httpMessageHandler.VerifyTimesCalled(0);
        }

        private void AssertIdentityApiUriIsCalled()
        {
            this.httpMessageHandler.VerifyRequestUri(new Uri("https://identitysso.betfair.com/api/login"));
        }

        private void AssertAppKeyInInHeader(string appKey)
        {
            this.httpMessageHandler.VerifyHeaderValues("X-Application", appKey);
        }

        private void AssertUsernameInContent(string username)
        {
            this.httpMessageHandler.VerifyRequestContent(ContentString(username, "Password"));
        }

        private void AssertPasswordInContent(string password)
        {
            this.httpMessageHandler.VerifyRequestContent(ContentString("Username", password));
        }
    }
}
