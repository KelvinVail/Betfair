﻿namespace Betfair.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Security.Authentication;
    using System.Threading.Tasks;
    using Betfair.Tests.TestDoubles;
    using Xunit;

    public class SessionTests : IDisposable
    {
        private readonly MockHttpMessageHandler httpMessageHandler = new MockHttpMessageHandler();

        private readonly HttpClient httpClient;

        private readonly Session session = new Session("AppKey", "Username", "Password");

        private bool disposedValue = false;

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
        public void ThrowWhenInitializedWithNullHttpClient()
        {
            var exception = Assert.Throws<ArgumentNullException>(() =>
                new Session("AppKey", "Username", "Password").WithHttpClient(null));
            Assert.Equal("httpClient", exception.ParamName);
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
            using (var session = new Session(appKey, "Username", "Password"))
            {
                session.WithHttpClient(this.httpClient);
                await session.LoginAsync();
                this.httpMessageHandler.VerifyHeaderValues("X-Application", appKey);
            }
        }

        [Theory]
        [InlineData("Username")]
        [InlineData("Kelvin")]
        [InlineData("Bob")]
        public async Task OnLoginUsernameIsInRequestContent(string username)
        {
            using (var session = new Session("AppKey", username, "Password"))
            {
                session.WithHttpClient(this.httpClient);
                await session.LoginAsync();
                this.httpMessageHandler.VerifyRequestContent(ContentString(username, "Password"));
            }
        }

        [Theory]
        [InlineData("Password")]
        [InlineData("123456")]
        [InlineData("qwerty")]
        public async Task OnLoginPasswordIsInRequestContent(string password)
        {
            using (var session = new Session("AppKey", "Username", password))
            {
                session.WithHttpClient(this.httpClient);
                await session.LoginAsync();
                this.httpMessageHandler.VerifyRequestContent(ContentString("Username", password));
            }
        }

        [Fact]
        public async Task OnDisposeHttpClientIsDisposed()
        {
            this.session.Dispose();
            await Assert.ThrowsAsync<ObjectDisposedException>(this.session.LoginAsync);
        }

        [Theory]
        [InlineData("FAIL", "INVALID_USERNAME_OR_PASSWORD")]
        [InlineData("FAIL", "INPUT_VALIDATION_ERROR")]
        [InlineData("LIMITED_ACCESS", "PENDING_AUTH")]
        public async Task OnLoginThrowIfFailed(string status, string error)
        {
            var responseHandler = this.SetExpectedHttpResponse(status, error).Build();
            using (var client = new HttpClient(responseHandler))
            {
                this.session.WithHttpClient(client);
                var exception = await Assert.ThrowsAsync<AuthenticationException>(this.session.LoginAsync);
                Assert.Equal($"{status}: {error}", exception.Message);
            }
        }

        [Theory]
        [InlineData(HttpStatusCode.BadRequest)]
        [InlineData(HttpStatusCode.RequestTimeout)]
        [InlineData(HttpStatusCode.NotFound)]
        public async void OnLoginThrowIfNotSuccessful(HttpStatusCode statusCode)
        {
            this.httpMessageHandler.WithStatusCode(statusCode);
            using (var client = new HttpClient(this.httpMessageHandler.Build()))
            {
                this.session.WithHttpClient(client);
                var exception = await Assert.ThrowsAsync<AuthenticationException>(this.session.LoginAsync);
                Assert.Equal($"{statusCode}", exception.Message);
            }
        }

        [Theory]
        [InlineData("SessionToken")]
        [InlineData("NewSessionToken")]
        [InlineData("DifferentSessionToken")]
        public async Task OnLoginSessionTokenIsSet(string sessionToken)
        {
            this.httpMessageHandler.WithReturnContent(
                new FakeApiLoginResponse().WithSessionToken(sessionToken));
            using (var client = new HttpClient(this.httpMessageHandler.Build()))
            {
                this.session.WithHttpClient(client);
                await this.session.LoginAsync();
                Assert.Equal(sessionToken, this.session.SessionToken);
            }
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
    }
}