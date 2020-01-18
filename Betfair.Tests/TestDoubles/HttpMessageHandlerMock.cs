namespace Betfair.Tests.TestDoubles
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    using Moq;
    using Moq.Protected;

    using Newtonsoft.Json;

    using Xunit;

    public class HttpMessageHandlerMock : IDisposable
    {
        private Mock<HttpMessageHandler> messageHandler;

        private HttpContent returnContent;

        private HttpStatusCode httpStatusCode;

        private bool buildWithException;

        private HttpResponseMessage responseMessage;

        public HttpMessageHandlerMock()
        {
            this.returnContent = new StringContent("StringContent");
            this.httpStatusCode = HttpStatusCode.OK;
        }

        public HttpMessageHandlerMock WithReturnContent(dynamic dynamicContent)
        {
            var stringContent = dynamicContent is string ? dynamicContent : JsonConvert.SerializeObject(dynamicContent);

            this.returnContent = new StringContent(stringContent);
            return this;
        }

        public HttpMessageHandlerMock WithStatusCode(HttpStatusCode statusCode)
        {
            this.httpStatusCode = statusCode;
            return this;
        }

        public HttpMessageHandlerMock WithException()
        {
            this.buildWithException = true;
            return this;
        }

        public HttpMessageHandler Build()
        {
            return this.buildWithException ? this.BuildWithException() : this.BuildHandler();
        }

        public void VerifyTimesCalled(int timesCalled)
        {
            this.messageHandler.Protected().Verify(
                "SendAsync",
                Times.Exactly(timesCalled),
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>());
        }

        public void VerifyHttpMethod(HttpMethod method)
        {
            this.messageHandler.Protected().Verify(
                "SendAsync",
                Times.AtLeastOnce(),
                ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == method),
                ItExpr.IsAny<CancellationToken>());
        }

        public void VerifyRequestUri(Uri requestUri)
        {
            this.messageHandler.Protected().Verify(
                "SendAsync",
                Times.AtLeastOnce(),
                ItExpr.Is<HttpRequestMessage>(req =>
                        req.RequestUri == requestUri),
                ItExpr.IsAny<CancellationToken>());
        }

        public void VerifyRequestContent(string body)
        {
            this.messageHandler.Protected().Verify(
                "SendAsync",
                Times.AtLeastOnce(),
                ItExpr.Is<HttpRequestMessage>(req =>
                        req.Content.ReadAsStringAsync().Result == body),
                ItExpr.IsAny<CancellationToken>());
        }

        public void VerifyHeaderValues(string header, string value)
        {
            IEnumerable<string> values = new List<string>();
            this.messageHandler.Protected().Verify(
                "SendAsync",
                Times.AtLeastOnce(),
                ItExpr.Is<HttpRequestMessage>(req =>
                        req.Headers.TryGetValues(header, out values)),
                ItExpr.IsAny<CancellationToken>());
            Assert.Contains(value, values);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.returnContent.Dispose();
                this.responseMessage?.Dispose();
            }
        }

        private HttpMessageHandler BuildHandler()
        {
            this.messageHandler = new Mock<HttpMessageHandler>();
            this.responseMessage = new HttpResponseMessage
                { StatusCode = this.httpStatusCode, Content = this.returnContent };
            this.messageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(this.responseMessage)
                .Verifiable();

            return this.messageHandler.Object;
        }

        private HttpMessageHandler BuildWithException()
        {
            this.messageHandler = new Mock<HttpMessageHandler>();

            this.messageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ThrowsAsync(new HttpRequestException("This is an exception message."))
                .Verifiable();

            return this.messageHandler.Object;
        }
    }
}
