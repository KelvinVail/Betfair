using System.Collections.Generic;

namespace Betfair.Tests
{
    using System;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using Betfair.Tests.TestDoubles;
    using Xunit;

    public class SessionTests : IDisposable
    {
        private readonly MockHttpMessageHandler httpMessageHandler = new MockHttpMessageHandler();

        private readonly HttpClient httpClient;

        private Session session = new Session("AppKey", "Username", "Password");

        public SessionTests()
        {
            this.httpMessageHandler.WithReturnContent(new FakeApiLoginResponse());
            this.httpClient = new HttpClient(this.httpMessageHandler.Build());
            this.session.WithHttpClient(this.httpClient);
        }

        [Fact]
        public void WhenInitializedThrowIfAppKeyIsNull()
        {
            var exception = Assert.Throws<NullReferenceException>(() => new Session(null, "Username", "Password"));
            Assert.Equal("appKey not set.", exception.Message);
        }

        [Fact]
        public void WhenInitializedThrowIfUsernameIsNull()
        {
            var exception = Assert.Throws<NullReferenceException>(() => new Session("AppKey", null, "Password"));
            Assert.Equal("username not set.", exception.Message);
        }

        [Fact]
        public void WhenInitializedThrowIfPasswordIsNull()
        {
            var exception = Assert.Throws<NullReferenceException>(() => new Session("AppKey", "Username", null));
            Assert.Equal("password not set.", exception.Message);
        }

        [Fact]
        public void ThrowWhenInitializedWithNullHttpClient()
        {
            var exception = Assert.Throws<NullReferenceException>(() =>
                new Session("AppKey", "Username", "Password").WithHttpClient(null));
            Assert.Equal("httpClient is null.", exception.Message);
        }

        [Fact]
        public void WhenInitializedHttpClientTimeoutIsThirtySeconds()
        {
            const int defaultTimeout = 30;
            Assert.Equal(defaultTimeout, this.httpClient.Timeout.TotalSeconds);
        }

        [Fact]
        public void WhenInitializedHttpClientAcceptsApplicationJson()
        {
            var applicationJson = new MediaTypeWithQualityHeaderValue("application/json");
            Assert.Contains(applicationJson, this.httpClient.DefaultRequestHeaders.Accept);
        }

        [Fact]
        public async Task OnLoginHttpClientIsCalled()
        {
            await this.session.LoginAsync();
            this.httpMessageHandler.VerifyTimesCalled(1);
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
            this.session.WithHttpClient(this.httpClient);
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
            this.session.WithHttpClient(this.httpClient);
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
            this.session.WithHttpClient(this.httpClient);
            await this.session.LoginAsync();
            this.httpMessageHandler.VerifyRequestContent(ContentString("Username", password));
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) return;

            this.httpClient.Dispose();
            this.session.Dispose();
            this.httpMessageHandler.Dispose();
        }

        private static string ContentString(string username, string password)
        {
            var dict = new Dictionary<string, string> { { "username", username }, { "password", password } };
            using (var content = new FormUrlEncodedContent(dict))
            {
                return content.ReadAsStringAsync().Result;
            }
        }
    }
}
