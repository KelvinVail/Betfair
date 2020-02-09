namespace Betfair.Tests.Stream
{
    using System;
    using System.IO;
    using System.Net.Security;
    using System.Text;
    using System.Threading.Tasks;
    using Betfair.Streaming;
    using Betfair.Tests.Stream.TestDoubles;
    using Betfair.Tests.TestDoubles;
    using Newtonsoft.Json;
    using Xunit;

    public class StreamSubscriptionTests : IDisposable
    {
        private readonly SessionSpy session = new SessionSpy();
        private readonly TcpClientSpy client;
        private readonly StreamWriterSpy writer = new StreamWriterSpy();
        private readonly StringBuilder sendLines = new StringBuilder();
        private readonly StreamSubscription streamSubscription;
        private bool disposed;

        public StreamSubscriptionTests()
        {
            this.streamSubscription = new StreamSubscription(this.session);
            this.client = new TcpClientSpy("stream-api-integration.betfair.com", 443);
            this.streamSubscription.WithTcpClient(this.client);
        }

        [Fact]
        public void MarketSubscriptionIsSealed()
        {
            Assert.True(typeof(StreamSubscription).IsSealed);
        }

        [Fact]
        public void OnWithTcpClientThrowIfNull()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => this.streamSubscription.WithTcpClient(null));
            Assert.Equal("client", exception.ParamName);
        }

        [Fact]
        public void OnConnectReceiveBufferSizeIsSet()
        {
            this.streamSubscription.Connect();
            Assert.Equal(1024 * 1000 * 2, this.client.ReceiveBufferSize);
        }

        [Fact]
        public void OnConnectReaderWriteTimeoutIsThirtySeconds()
        {
            this.streamSubscription.Connect();
            Assert.Equal(TimeSpan.FromSeconds(30).TotalMilliseconds, this.client.SendTimeout);
        }

        [Fact]
        public void OnConnectReaderReadTimeoutIsThirtySeconds()
        {
            this.streamSubscription.Connect();
            Assert.Equal(TimeSpan.FromSeconds(30).TotalMilliseconds, this.client.ReceiveTimeout);
        }

        [Fact]
        public void OnConnectWriterWriteTimeoutIsThirtySeconds()
        {
            this.streamSubscription.Connect();
            Assert.Equal(TimeSpan.FromSeconds(30).TotalMilliseconds, this.client.SendTimeout);
        }

        [Fact]
        public void OnConnectWriterReadTimeoutIsThirtySeconds()
        {
            this.streamSubscription.Connect();
            Assert.Equal(TimeSpan.FromSeconds(30).TotalMilliseconds, this.client.ReceiveTimeout);
        }

        [Fact]
        public void OnConnectTcpClientIsConnectedToBetfairHost()
        {
            this.streamSubscription.Connect();
            Assert.Equal("stream-api.betfair.com", this.client.Host);
        }

        [Fact]
        public void OnConnectTcpClientIsConnectedToBetfairPort()
        {
            this.streamSubscription.Connect();
            Assert.Equal(443, this.client.Port);
        }

        // This test creates a network connection, I haven't been able to figure out
        // a good way to mock this yet.
        [Fact]
        public void OnConnectReaderStreamIsAuthenticated()
        {
            var subscription = new StreamSubscription(this.session);
            subscription.Connect();
            var sslStream = (SslStream)subscription.Reader.BaseStream;
            Assert.True(sslStream.IsAuthenticated);
        }

        // This test creates a network connection, I haven't been able to figure out
        // a good way to mock this yet.
        [Fact]
        public void OnConnectWriterStreamIsAuthenticated()
        {
            var subscription = new StreamSubscription(this.session);
            subscription.Connect();
            var sslStream = (SslStream)subscription.Writer.BaseStream;
            Assert.True(sslStream.IsAuthenticated);
        }

        [Fact]
        public void OnConnectWriterIsAutoFlushed()
        {
            this.streamSubscription.Connect();
            Assert.True(this.streamSubscription.Writer.AutoFlush);
        }

        [Fact]
        public async Task OnReadConnectionOperationConnectedIsTrue()
        {
            Assert.False(this.streamSubscription.Connected);
            await this.SendLineAsync("{\"op\":\"connection\",\"connectionId\":\"ConnectionId\"}");
            Assert.True(this.streamSubscription.Connected);
        }

        [Fact]
        public async Task OnReadStatusTimeoutOperationConnectedIsFalse()
        {
            await this.SendLineAsync("{\"op\":\"connection\",\"connectionId\":\"ConnectionId\"}");

            var message =
                "{\"op\":\"status\"," +
                "\"statusCode\":\"FAILURE\"," +
                "\"errorCode\":\"TIMEOUT\"," +
                "\"errorMessage\":\"Timed out trying to read message make sure to add \\\\r\\\\n\\nRead data : ﻿\"," +
                "\"connectionClosed\":true," +
                "\"connectionId\":\"ConnectionId\"}";

            await this.SendLineAsync(message);
            Assert.False(this.streamSubscription.Connected);
        }

        [Fact]
        public async Task OnReadStatusUpdateConnectionStatusIsUpdated()
        {
            await this.SendLineAsync("{\"op\":\"connection\",\"connectionId\":\"ConnectionId\"}");
            await this.SendLineAsync("{\"op\":\"status\",\"connectionClosed\":false}");
            Assert.True(this.streamSubscription.Connected);

            await this.SendLineAsync("{\"op\":\"status\",\"connectionClosed\":true}");
            Assert.False(this.streamSubscription.Connected);
        }

        [Fact]
        public async Task OnReadHandleUnknownOperationWithoutThrowing()
        {
            await this.SendLineAsync("{\"op\":\"unknown\"}");
        }

        [Fact]
        public async Task OnAuthenticateGetSessionTokenIsCalled()
        {
            this.streamSubscription.Writer = this.writer;
            await this.streamSubscription.AuthenticateAsync();
            Assert.Equal(1, this.session.TimesGetSessionTokenAsyncCalled);
        }

        [Fact]
        public async Task OnAuthenticate()
        {
            var authMessage = JsonConvert.SerializeObject(
                new AuthenticationMessageStub(
                    this.session.AppKey,
                    await this.session.GetTokenAsync()));

            this.streamSubscription.Writer = this.writer;
            await this.streamSubscription.AuthenticateAsync();
            Assert.Equal(authMessage, this.writer.LineWritten);
        }

        [Theory]
        [InlineData("1.120684740")]
        [InlineData("1.234567890")]
        [InlineData("1.098765432")]
        public async Task OnSubscribeSubscriptionMessageIsSent(string marketId)
        {
            var subscriptionMessage = $"{{\"op\":\"marketSubscription\",\"id\":1,\"marketFilter\":{{\"marketIds\":[\"{marketId}\"]}},\"marketDataFilter\":{{}}}}";
            this.streamSubscription.Writer = this.writer;

            var marketFilter = new MarketFilter().WithMarketId(marketId);
            await this.streamSubscription.Subscribe(marketFilter, null);
            Assert.Equal(subscriptionMessage, this.writer.LineWritten);
        }

        [Fact]
        public async Task OnSubscribeMarketDataFilterIsSet()
        {
            var dataFilter = new MarketDataFilter().WithBestPrices();
            var dataString = JsonConvert.SerializeObject(dataFilter);
            var subscriptionMessage = $"{{\"op\":\"marketSubscription\",\"id\":1,\"marketFilter\":{{}},\"marketDataFilter\":{dataString}}}";
            this.streamSubscription.Writer = this.writer;
            await this.streamSubscription.Subscribe(null, dataFilter);
            Assert.Equal(subscriptionMessage, this.writer.LineWritten);
        }

        [Fact]
        public async Task IdIncrementsOnMarketSubscription()
        {
            this.streamSubscription.Writer = this.writer;

            var subscriptionMessage = $"{{\"op\":\"marketSubscription\",\"id\":1,\"marketFilter\":{{}},\"marketDataFilter\":{{}}}}";
            await this.streamSubscription.Subscribe(null, null);
            Assert.Equal(subscriptionMessage, this.writer.LineWritten);

            var subscriptionMessage2 = $"{{\"op\":\"marketSubscription\",\"id\":2,\"marketFilter\":{{}},\"marketDataFilter\":{{}}}}";
            await this.streamSubscription.Subscribe(null, null);
            Assert.Equal(subscriptionMessage2, this.writer.LineWritten);
        }

        [Fact]
        public async Task OnSubscribeToOrdersOrderSubscriptionIsSent()
        {
            var subscriptionMessage = $"{{\"op\":\"orderSubscription\",\"id\":1}}";
            this.streamSubscription.Writer = this.writer;
            await this.streamSubscription.SubscribeToOrders();
            Assert.Equal(subscriptionMessage, this.writer.LineWritten);
        }

        [Fact]
        public async Task IdIncrementsOnOrderSubscription()
        {
            this.streamSubscription.Writer = this.writer;

            var subscriptionMessage = $"{{\"op\":\"orderSubscription\",\"id\":1}}";
            await this.streamSubscription.SubscribeToOrders();
            Assert.Equal(subscriptionMessage, this.writer.LineWritten);

            var subscriptionMessage2 = $"{{\"op\":\"orderSubscription\",\"id\":2}}";
            await this.streamSubscription.SubscribeToOrders();
            Assert.Equal(subscriptionMessage2, this.writer.LineWritten);
        }

        [Fact]
        public async Task IdIncrementsOnAnySubscription()
        {
            this.streamSubscription.Writer = this.writer;

            var subscriptionMessage = $"{{\"op\":\"marketSubscription\",\"id\":1,\"marketFilter\":{{}},\"marketDataFilter\":{{}}}}";
            await this.streamSubscription.Subscribe(null, null);
            Assert.Equal(subscriptionMessage, this.writer.LineWritten);

            var subscriptionMessage2 = $"{{\"op\":\"orderSubscription\",\"id\":2}}";
            await this.streamSubscription.SubscribeToOrders();
            Assert.Equal(subscriptionMessage2, this.writer.LineWritten);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed) return;
            if (disposing) this.writer.Dispose();

            this.disposed = true;
        }

        private async Task SendLineAsync(string line)
        {
            this.sendLines.AppendLine(line);
            this.streamSubscription.Reader = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(this.sendLines.ToString())));
            await foreach (var message in this.streamSubscription.GetChanges())
            {
            }
        }
    }
}
