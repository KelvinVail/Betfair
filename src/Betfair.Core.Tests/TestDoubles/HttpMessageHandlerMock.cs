using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Moq.Protected;
using Utf8Json;
using Xunit;

namespace Betfair.Core.Tests.TestDoubles
{
    public class HttpMessageHandlerMock : IDisposable
    {
        private Mock<HttpClientHandler> _messageHandler;
        private HttpContent _returnContent;
        private HttpStatusCode _httpStatusCode;
        private bool _buildWithException;
        private HttpResponseMessage _responseMessage;

        public HttpMessageHandlerMock()
        {
            _httpStatusCode = HttpStatusCode.OK;
            WithReturnContent(new { Test = "Test" });
        }

        public HttpMessageHandlerMock WithReturnContent(dynamic dynamicContent)
        {
            var stringContent = dynamicContent is string ? dynamicContent : JsonSerializer.ToJsonString(dynamicContent);

            _returnContent = new StringContent(stringContent);
            return this;
        }

        public HttpMessageHandlerMock WithStatusCode(HttpStatusCode statusCode)
        {
            _httpStatusCode = statusCode;
            return this;
        }

        public HttpMessageHandlerMock WithException()
        {
            _buildWithException = true;
            return this;
        }

        public HttpClientHandler Build()
        {
            return _buildWithException ? BuildWithException() : BuildHandler();
        }

        public void VerifyTimesCalled(int timesCalled)
        {
            _messageHandler.Protected().Verify(
                "SendAsync",
                Times.Exactly(timesCalled),
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>());
        }

        public void VerifyHttpMethod(HttpMethod method)
        {
            _messageHandler.Protected().Verify(
                "SendAsync",
                Times.AtLeastOnce(),
                ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == method),
                ItExpr.IsAny<CancellationToken>());
        }

        public void VerifyRequestUri(Uri requestUri)
        {
            _messageHandler.Protected().Verify(
                "SendAsync",
                Times.AtLeastOnce(),
                ItExpr.Is<HttpRequestMessage>(req =>
                        req.RequestUri == requestUri),
                ItExpr.IsAny<CancellationToken>());
        }

        public void VerifyRequestContent(string body)
        {
            _messageHandler.Protected().Verify(
                "SendAsync",
                Times.AtLeastOnce(),
                ItExpr.Is<HttpRequestMessage>(req =>
                        req.Content.ReadAsStringAsync().Result == body),
                ItExpr.IsAny<CancellationToken>());
        }

        public void VerifyHeaderValues(string header, string value)
        {
            IEnumerable<string> values = new List<string>();
            _messageHandler.Protected().Verify(
                "SendAsync",
                Times.AtLeastOnce(),
                ItExpr.Is<HttpRequestMessage>(req =>
                        req.Headers.TryGetValues(header, out values)),
                ItExpr.IsAny<CancellationToken>());
            Assert.Contains(value, values);
        }

        public void VerifyHasCertificate()
        {
            Assert.NotEmpty(_messageHandler.Object.ClientCertificates);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _returnContent.Dispose();
                _responseMessage?.Dispose();
            }
        }

        private HttpClientHandler BuildHandler()
        {
            _messageHandler = new Mock<HttpClientHandler>();
            _responseMessage = new HttpResponseMessage
                { StatusCode = _httpStatusCode, Content = _returnContent };
            _messageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(_responseMessage)
                .Verifiable();

            return _messageHandler.Object;
        }

        private HttpClientHandler BuildWithException()
        {
            _messageHandler = new Mock<HttpClientHandler>();

            _messageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ThrowsAsync(new HttpRequestException("This is an exception message."))
                .Verifiable();

            return _messageHandler.Object;
        }
    }
}
