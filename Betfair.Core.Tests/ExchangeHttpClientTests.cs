namespace Betfair.Core.Tests
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using Betfair.Core.Tests.TestDoubles;
    using Xunit;

    public class ExchangeHttpClientTests : IDisposable
    {
        private readonly Client client = new Client("AppKey", "Username", "Password");

        private readonly HttpMessageHandlerMock httpMessageHandler = new HttpMessageHandlerMock().WithReturnContent(new LoginResponseStub());

        private readonly HttpClientHandler handler;

        private bool disposed;

        public ExchangeHttpClientTests()
        {
            this.handler = this.httpMessageHandler.Build();
            this.client.WithHandler(this.handler);
        }

        [Fact]
        public void OnWithHandlerThrowIfHandlerIsNull()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => this.client.WithHandler(null));
            Assert.Equal("newHandler", exception.ParamName);
        }

        [Fact]
        public async Task WhenInitializedAcceptHeaderIsApplicationJson()
        {
            var applicationJson = new MediaTypeWithQualityHeaderValue("application/json");
            await this.client.GetTokenAsync();
            this.httpMessageHandler.VerifyHeaderValues("Accept", applicationJson.ToString());
        }

        [Fact]
        public async Task WhenInitializedHeaderContainsConnectionKeepAlive()
        {
            await this.client.GetTokenAsync();
            this.httpMessageHandler.VerifyHeaderValues("Connection", "Keep-Alive");
        }

        [Fact]
        public async Task WhenInitializedHeaderContainsAcceptGzip()
        {
            await this.client.GetTokenAsync();
            this.httpMessageHandler.VerifyHeaderValues("Accept-Encoding", "gzip");
        }

        [Fact]
        public async Task WhenInitializedHeaderContainsAcceptDeflate()
        {
            await this.client.GetTokenAsync();
            this.httpMessageHandler.VerifyHeaderValues("Accept-Encoding", "deflate");
        }

        [Fact]
        public async Task OnDisposeHttpClientIsDisposed()
        {
            await this.client.LoginAsync();
            this.client.Dispose();
            await Assert.ThrowsAsync<ObjectDisposedException>(() => this.client.LoginAsync());
        }

        [Theory]
        [InlineData(HttpStatusCode.BadRequest)]
        [InlineData(HttpStatusCode.RequestTimeout)]
        [InlineData(HttpStatusCode.NotFound)]
        public async void OnSendThrowIfNotSuccessful(HttpStatusCode statusCode)
        {
            this.httpMessageHandler.WithStatusCode(statusCode);
            this.client.WithHandler(this.httpMessageHandler.Build());
            var exception = await Assert.ThrowsAsync<HttpRequestException>(() => this.client.GetTokenAsync());
            Assert.Equal($"{statusCode}", exception.Message);
        }

        [Fact]
        public void WhenInitializedHandlerCheckCertificateRevocationListIsTrue()
        {
            Assert.True(this.handler.CheckCertificateRevocationList);
        }

        [Fact]
        public async Task OnLoginHandlerCheckCertificateRevocationListIsTrue()
        {
            await this.client.LoginAsync();
            Assert.True(this.handler.CheckCertificateRevocationList);
        }

        [Fact]
        public async Task OnLoginRequestHasAutoDecompressIsGzip()
        {
            await this.client.LoginAsync();
            this.httpMessageHandler.VerifyHeaderValues("Accept-Encoding", "gzip");
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed) return;
            if (disposing)
            {
                this.httpMessageHandler.Dispose();
                this.handler.Dispose();
            }

            this.client.Dispose();

            this.disposed = true;
        }
    }
}
