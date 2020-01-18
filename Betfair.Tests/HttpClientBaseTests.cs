namespace Betfair.Tests
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using Betfair.Tests.TestDoubles;
    using Xunit;

    public class HttpClientBaseTests : HttpClientBase
    {
        private readonly HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "test");

        private readonly HttpMessageHandlerMock httpMessageHandler = new HttpMessageHandlerMock();

        public HttpClientBaseTests()
            : base(new Uri("https://www.test.com"))
        {
        }

        [Fact]
        public void WhenInitializedHttpClientIsNotNull()
        {
            Assert.NotNull(this.HttpClient);
        }

        [Fact]
        public void OnWithHttpClientThrowIfHttpClientIsNull()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => this.WithHttpClient(null));
            Assert.Equal("httpClient", exception.ParamName);
        }

        [Fact]
        public void WhenInitializedBaseAddressIsSet()
        {
            var expectedUri = new Uri("https://www.test.com");
            Assert.Equal(expectedUri, this.HttpClient.BaseAddress);
        }

        [Fact]
        public void WhenInitializedTimeoutIsThirtySeconds()
        {
            var defaultTimeout = TimeSpan.FromSeconds(30);
            Assert.Equal(defaultTimeout, this.HttpClient.Timeout);
        }

        [Fact]
        public void WhenInitializedAcceptHeaderIsApplicationJson()
        {
            var applicationJson = new MediaTypeWithQualityHeaderValue("application/json");
            Assert.Contains(applicationJson, this.HttpClient.DefaultRequestHeaders.Accept);
        }

        [Fact]
        public async Task OnDisposeHttpClientIsDisposed()
        {
            this.Dispose();
            await Assert.ThrowsAsync<ObjectDisposedException>(() => this.SendAsync<dynamic>(this.request));
        }

        [Theory]
        [InlineData(HttpStatusCode.BadRequest)]
        [InlineData(HttpStatusCode.RequestTimeout)]
        [InlineData(HttpStatusCode.NotFound)]
        public async void OnSendThrowIfNotSuccessful(HttpStatusCode statusCode)
        {
            this.httpMessageHandler.WithStatusCode(statusCode);
            using (var client = new HttpClient(this.httpMessageHandler.Build()))
            {
                this.WithHttpClient(client);
                var exception = await Assert.ThrowsAsync<HttpRequestException>(() => this.SendAsync<dynamic>(this.request));
                Assert.Equal($"{statusCode}", exception.Message);
            }
        }
    }
}
