using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Betfair.Core.Tests.TestDoubles;
using Xunit;

namespace Betfair.Core.Tests
{
    public class ExchangeHttpClientTests : IDisposable
    {
        private readonly Session _session = new Session("AppKey", "Username", "Password");
        private readonly HttpMessageHandlerMock _httpMessageHandler = new HttpMessageHandlerMock().WithReturnContent(new LoginResponseStub());
        private readonly HttpClientHandler _handler;
        private bool _disposed;

        public ExchangeHttpClientTests()
        {
            _handler = _httpMessageHandler.Build();
            _session.WithHandler(_handler);
        }

        [Fact]
        public void OnWithHandlerThrowIfHandlerIsNull()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => _session.WithHandler(null));
            Assert.Equal("newHandler", exception.ParamName);
        }

        [Fact]
        public async Task WhenInitializedAcceptHeaderIsApplicationJson()
        {
            var applicationJson = new MediaTypeWithQualityHeaderValue("application/json");
            await _session.GetTokenAsync();
            _httpMessageHandler.VerifyHeaderValues("Accept", applicationJson.ToString());
        }

        [Fact]
        public async Task WhenInitializedHeaderContainsConnectionKeepAlive()
        {
            await _session.GetTokenAsync();
            _httpMessageHandler.VerifyHeaderValues("Connection", "keep-alive");
        }

        [Fact]
        public async Task WhenInitializedHeaderContainsAcceptGzip()
        {
            await _session.GetTokenAsync();
            _httpMessageHandler.VerifyHeaderValues("Accept-Encoding", "gzip");
        }

        [Fact]
        public async Task WhenInitializedHeaderContainsAcceptDeflate()
        {
            await _session.GetTokenAsync();
            _httpMessageHandler.VerifyHeaderValues("Accept-Encoding", "deflate");
        }

        [Fact]
        public async Task OnDisposeHttpClientIsDisposed()
        {
            await _session.LoginAsync();
            _session.Dispose();
            await Assert.ThrowsAsync<ObjectDisposedException>(() => _session.LoginAsync());
        }

        [Theory]
        [InlineData(HttpStatusCode.BadRequest)]
        [InlineData(HttpStatusCode.RequestTimeout)]
        [InlineData(HttpStatusCode.NotFound)]
        public async Task OnSendThrowIfNotSuccessful(HttpStatusCode statusCode)
        {
            _httpMessageHandler.WithStatusCode(statusCode);
            _session.WithHandler(_httpMessageHandler.Build());
            var exception = await Assert.ThrowsAsync<HttpRequestException>(() => _session.GetTokenAsync());
            Assert.Equal($"{statusCode}", exception.Message);
        }

        [Fact]
        public void WhenInitializedHandlerCheckCertificateRevocationListIsTrue()
        {
            Assert.True(_handler.CheckCertificateRevocationList);
        }

        [Fact]
        public async Task OnLoginHandlerCheckCertificateRevocationListIsTrue()
        {
            await _session.LoginAsync();
            Assert.True(_handler.CheckCertificateRevocationList);
        }

        [Fact]
        public async Task OnLoginRequestHasAutoDecompressIsGzip()
        {
            await _session.LoginAsync();
            _httpMessageHandler.VerifyHeaderValues("Accept-Encoding", "gzip");
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (disposing)
            {
                _httpMessageHandler.Dispose();
                _handler.Dispose();
            }

            _session.Dispose();

            _disposed = true;
        }
    }
}
