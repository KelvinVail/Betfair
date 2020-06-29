namespace Betfair.Core.Tests
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Betfair.Core.Tests.TestDoubles;
    using Betfair.Exchange.Interfaces;
    using Xunit;

    public class ExchangeServiceTests : IDisposable
    {
        private readonly HttpMessageHandlerMock httpMessageHandler = new HttpMessageHandlerMock();

        private readonly SessionStub session = new SessionStub();

        private readonly LoggerSpy log = new LoggerSpy();

        private readonly ExchangeService exchange;

        private bool disposedValue;

        public ExchangeServiceTests()
        {
            this.exchange = new ExchangeService(this.session, this.log);
            this.exchange.WithHandler(this.httpMessageHandler.Build());
        }

        [Fact]
        public void ClientShouldImplementIExchangeService()
        {
            Assert.True(typeof(IExchangeService).IsAssignableFrom(typeof(ExchangeService)));
        }

        [Fact]
        public void ExchangeClientIsSealed()
        {
            Assert.True(typeof(ExchangeService).IsSealed);
        }

        [Theory]
        [InlineData("endpoint")]
        [InlineData("betting")]
        [InlineData("account")]
        public async Task TheSpecifiedEndpointIsCalled(string endpoint)
        {
            await this.exchange.SendAsync<string>(endpoint, "method", "parameters");
            this.httpMessageHandler.VerifyRequestUri(new Uri($"https://api.betfair.com/exchange/{endpoint}/json-rpc/v1"));
        }

        [Fact]
        public async Task OnDisposeHttpClientIsDisposed()
        {
            this.exchange.Dispose();
            await Assert.ThrowsAsync<ObjectDisposedException>(() => this.exchange.SendAsync<dynamic>("endpoint", "method", "parameters"));
        }

        [Fact]
        public void ExchangeServiceIsDisposable()
        {
            Assert.True(typeof(IDisposable).IsAssignableFrom(typeof(ExchangeService)));
        }

        [Fact]
        public async Task KeepAliveIsInHttpRequestHeader()
        {
            await this.exchange.SendAsync<dynamic>("endpoint", "method", "parameters");
            this.httpMessageHandler.VerifyHeaderValues("Connection", "keep-alive");
        }

        [Theory]
        [InlineData("Token")]
        [InlineData("SessionToken")]
        [InlineData("NewToken")]
        public async Task SessionTokenIsInHttpRequestHeader(string token)
        {
            this.session.Token = token;
            await this.exchange.SendAsync<dynamic>("endpoint", "method", "parameters");
            this.httpMessageHandler.VerifyHeaderValues("X-Authentication", token);
        }

        [Fact]
        public async Task AppKeyIsInHttpRequestHeader()
        {
            await this.exchange.SendAsync<dynamic>("endpoint", "method", "parameters");
            this.httpMessageHandler.VerifyHeaderValues("X-Application", "AppKey");
        }

        [Theory]
        [InlineData("Method", "Parameters")]
        [InlineData("placeOrder", "{\"selectionId\":12345}")]
        public async Task HttpRequestContentBodyIsSet(string method, string parameters)
        {
            await this.exchange.SendAsync<dynamic>("endpoint", method, parameters);
            var expected = "{\"jsonrpc\":\"2.0\"," +
                           $"\"method\":\"SportsAPING/v1.0/{method}\"," +
                           $"\"id\":1,\"params\":{parameters}}}";
            this.httpMessageHandler.VerifyRequestContent(expected);
        }

        [Theory]
        [InlineData("Method", "Parameters")]
        [InlineData("placeOrder", "{\"selectionId\":12345}")]
        public async Task HttpRequestContentBodyIsLogged(string method, string parameters)
        {
            await this.exchange.SendAsync<dynamic>("endpoint", method, parameters);
            var request = "{\"jsonrpc\":\"2.0\"," +
                           $"\"method\":\"SportsAPING/v1.0/{method}\"," +
                           $"\"id\":1,\"params\":{parameters}}}";
            var expected = $"Betfair API called: {request}.";

            Assert.Contains(expected, this.log.MessageList.Values);
        }

        [Fact]
        public async Task HttpResponseIsDeserialized()
        {
            var result = new ExchangeResponseStub
            {
                TestString = "Test",
                TestDouble = 1234,
            };

            var response = "{\"jsonrpc\":\"2.0\",\"result\":" +
                           result.ToJson() +
                           ",\"id\":1}";
            this.httpMessageHandler.WithReturnContent(response);
            using var local = new ExchangeService(this.session, this.log);
            local.WithHandler(this.httpMessageHandler.Build());
            var actual = await local.SendAsync<ExchangeResponseStub>("endpoint", "method", "parameters");
            Assert.Equal(result.ToJson(), actual.ToJson());
        }

        [Theory]
        [InlineData("This is the response string")]
        [InlineData("A different response!")]
        public async Task HttpResponseIsLogged(string responseString)
        {
            var result = new ExchangeResponseStub
            {
                TestString = responseString,
                TestDouble = 1234,
            };

            var response = "{\"jsonrpc\":\"2.0\",\"result\":" +
                           result.ToJson() +
                           ",\"id\":1}";
            this.httpMessageHandler.WithReturnContent(response);
            using var local = new ExchangeService(this.session, this.log);
            local.WithHandler(this.httpMessageHandler.Build());

            var actual = await local.SendAsync<ExchangeResponseStub>("endpoint", "method", "parameters");

            var expected = $"Betfair API responded: {{\"result\":{actual.ToJson()}}}.";

            Assert.Equal(expected, this.log.LastMessage);
        }

        [Theory]
        [InlineData("NO_APP_KEY")]
        [InlineData("INVALID_SESSION_INFORMATION")]
        [InlineData("INVALID_INPUT_DATA")]
        public async Task ExceptionIsRaisedIfRequestIsNotSuccess(string code)
        {
            var errorResponse = "{\"jsonrpc\":\"2.0\"," +
                                "\"error\":{\"code\":-32099," +
                                "\"message\":\"ANGX-0004\"," +
                                "\"data\":{" +
                                "\"APINGException\":{" +
                                "\"requestUUID\":\"ie2-ang04a-prd-04150817-00147adc91\"," +
                                $"\"errorCode\":\"{code}\"," +
                                "\"errorDetails\":\"\"}," +
                                "\"exceptionname\":\"APINGException\"}}," +
                                "\"id\":1}";
            this.httpMessageHandler.WithReturnContent(errorResponse);
            var local = new ExchangeService(this.session, this.log);
            local.WithHandler(this.httpMessageHandler.Build());
            var ex = await Assert.ThrowsAsync<HttpRequestException>(() => local.SendAsync<ExchangeResponseStub>("endpoint", "method", "parameters"));
            Assert.Equal(code, ex.Message);
            local.Dispose();
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
                this.httpMessageHandler.Dispose();
            }

            this.exchange.Dispose();

            this.disposedValue = true;
        }
    }
}
