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
        private readonly Session session = new Session("AppKey", "Username", "Password");

        private readonly HttpMessageHandlerMock httpMessageHandler = new HttpMessageHandlerMock().WithReturnContent(new LoginResponseStub());

        private readonly HttpClientHandler handler;

        private bool disposed;

        public ExchangeHttpClientTests()
        {
            this.handler = this.httpMessageHandler.Build();
            this.session.WithHandler(this.handler);
        }

        [Fact]
        public void OnWithHandlerThrowIfHandlerIsNull()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => this.session.WithHandler(null));
            Assert.Equal("newHandler", exception.ParamName);
        }

        [Fact]
        public async Task WhenInitializedAcceptHeaderIsApplicationJson()
        {
            var applicationJson = new MediaTypeWithQualityHeaderValue("application/json");
            await this.session.GetTokenAsync();
            this.httpMessageHandler.VerifyHeaderValues("Accept", applicationJson.ToString());
        }

        [Fact]
        public async Task WhenInitializedHeaderContainsConnectionKeepAlive()
        {
            await this.session.GetTokenAsync();
            this.httpMessageHandler.VerifyHeaderValues("Connection", "keep-alive");
        }

        [Fact]
        public async Task WhenInitializedHeaderContainsAcceptGzip()
        {
            await this.session.GetTokenAsync();
            this.httpMessageHandler.VerifyHeaderValues("Accept-Encoding", "gzip");
        }

        [Fact]
        public async Task WhenInitializedHeaderContainsAcceptDeflate()
        {
            await this.session.GetTokenAsync();
            this.httpMessageHandler.VerifyHeaderValues("Accept-Encoding", "deflate");
        }

        [Fact]
        public async Task OnDisposeHttpClientIsDisposed()
        {
            await this.session.LoginAsync();
            this.session.Dispose();
            await Assert.ThrowsAsync<ObjectDisposedException>(() => this.session.LoginAsync());
        }

        [Theory]
        [InlineData(HttpStatusCode.BadRequest)]
        [InlineData(HttpStatusCode.RequestTimeout)]
        [InlineData(HttpStatusCode.NotFound)]
        public async void OnSendThrowIfNotSuccessful(HttpStatusCode statusCode)
        {
            this.httpMessageHandler.WithStatusCode(statusCode);
            this.session.WithHandler(this.httpMessageHandler.Build());
            var exception = await Assert.ThrowsAsync<HttpRequestException>(() => this.session.GetTokenAsync());
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
            await this.session.LoginAsync();
            Assert.True(this.handler.CheckCertificateRevocationList);
        }

        [Fact]
        public async Task OnLoginRequestHasAutoDecompressIsGzip()
        {
            await this.session.LoginAsync();
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

            this.session.Dispose();

            this.disposed = true;
        }
    }
}
