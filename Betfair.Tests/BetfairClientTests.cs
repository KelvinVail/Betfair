namespace Betfair.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;

    using Betfair.Tests.TestDoubles;

    using Xunit;

    public class BetfairClientTests
    {
        private readonly MockHttpMessageHandler httpMessageHandler = new MockHttpMessageHandler();

        private readonly HttpClient client;

        public BetfairClientTests()
        {
            this.client = new HttpClient(this.httpMessageHandler.Build());
        }

        [Fact]
        public void HttpClientIsNotCalledWhenBuilt()
        {
            this.GetBetfairClient();
            this.AssertCallIsNotMade();
        }

        [Fact]
        public void LoginCallsCorrectUri()
        {
            this.GetBetfairClient().Login();
            this.AssertIdentityUriIsCalled();
        }

        [Theory]
        [InlineData("AppKey")]
        [InlineData("ABC")]
        [InlineData("12345")]
        public void AppKeySetInLoginRequestHeader(string appKey)
        {
            this.LoginWithAppKey(appKey);
            this.AssertAppKeyInInHeader(appKey);
        }

        [Theory]
        [InlineData("Username")]
        [InlineData("Kelvin")]
        [InlineData("Bob")]
        public void UsernameSetInLoginContent(string username)
        {
            this.LoginWithUsername(username);
            this.AssertUsernameInContent(username);
        }

        [Theory]
        [InlineData("Password")]
        [InlineData("123456")]
        [InlineData("qwerty")]
        public void PasswordSetInLoginContent(string password)
        {
            this.LoginWithPassword(password);
            this.AssertPasswordInContent(password);
        }

        private BetfairClient GetBetfairClient()
        {
            return new BetfairFactory("AppKey", "Username", "Password")
                .WithIdentityHttpClient(this.client)
                .Build();
        }

        private void LoginWithAppKey(string appKey)
        {
            new BetfairFactory(appKey, "Username", "Password")
                .WithIdentityHttpClient(this.client)
                .Build()
                .Login();
        }

        private void LoginWithUsername(string username)
        {
            new BetfairFactory("AppKey", username, "Password")
                .WithIdentityHttpClient(this.client)
                .Build()
                .Login();
        }

        private void LoginWithPassword(string password)
        {
            new BetfairFactory("AppKey", "Username", password)
                .WithIdentityHttpClient(this.client)
                .Build()
                .Login();
        }

        private void AssertCallIsNotMade()
        {
            this.httpMessageHandler.VerifyTimesCalled(0);
        }

        private void AssertIdentityUriIsCalled()
        {
            this.httpMessageHandler.VerifyRequestUri(new Uri("https://identitysso-cert.betfair.com/api/certlogin"));
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
                
        private static string ContentString(string username, string password)
        {
            var postData = new List<KeyValuePair<string, string>>
                               {
                                   new KeyValuePair<string, string>("username", username),
                                   new KeyValuePair<string, string>("password", password)
                               };

            return new FormUrlEncodedContent(postData).ReadAsStringAsync().Result;
        }

    }
}
