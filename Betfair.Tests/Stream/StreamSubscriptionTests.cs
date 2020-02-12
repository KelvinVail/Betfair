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
        private readonly StreamSubscription streamSubscription;
        private bool disposed;

        public StreamSubscriptionTests()
        {
            this.streamSubscription = new StreamSubscription(this.session);
            this.client = new TcpClientSpy("stream-api-integration.betfair.com", 443);
            this.streamSubscription.WithTcpClient(this.client);
            this.streamSubscription.Writer = this.writer;
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

        [Theory]
        [InlineData("ConnectionId")]
        [InlineData("NewConnectionId")]
        [InlineData("RefreshedConnectionId")]
        public async Task OnReadConnectionOperationConnectionIdIsRecorded(string connectionId)
        {
            await this.SendLineAsync($"{{\"op\":\"connection\",\"connectionId\":\"{connectionId}\"}}");
            Assert.Equal(connectionId, this.streamSubscription.ConnectionId);
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

            await this.streamSubscription.AuthenticateAsync();
            Assert.Equal(authMessage, this.writer.LastLineWritten);
        }

        [Theory]
        [InlineData("1.120684740")]
        [InlineData("1.234567890")]
        [InlineData("1.098765432")]
        public async Task OnSubscribeSubscriptionMessageIsSent(string marketId)
        {
            var marketFilter = new MarketFilter().WithMarketId(marketId);
            var subscriptionMessage = new SubscriptionMessageStub("marketSubscription", 1)
                .WithMarketFilter(marketFilter)
                .ToJson();
            await this.streamSubscription.Subscribe(marketFilter, null);
            Assert.Equal(subscriptionMessage, this.writer.LastLineWritten);
        }

        [Fact]
        public async Task OnSubscribeMarketDataFilterIsSet()
        {
            var dataFilter = new MarketDataFilter().WithBestPrices();
            var subscriptionMessage = new SubscriptionMessageStub("marketSubscription", 1)
                .WithMarketDateFilter(dataFilter)
                .ToJson();
            await this.streamSubscription.Subscribe(null, dataFilter);
            Assert.Equal(subscriptionMessage, this.writer.LastLineWritten);
        }

        [Fact]
        public async Task IdIncrementsOnMarketSubscription()
        {
            var subscriptionMessage = new SubscriptionMessageStub("marketSubscription", 1).ToJson();
            await this.streamSubscription.Subscribe(null, null);
            Assert.Equal(subscriptionMessage, this.writer.LastLineWritten);

            var subscriptionMessage2 = new SubscriptionMessageStub("marketSubscription", 2).ToJson();
            await this.streamSubscription.Subscribe(null, null);
            Assert.Equal(subscriptionMessage2, this.writer.LastLineWritten);
        }

        [Fact]
        public async Task OnSubscribeToOrdersOrderSubscriptionIsSent()
        {
            var subscriptionMessage = new SubscriptionMessageStub("orderSubscription", 1).ToJson();
            await this.streamSubscription.SubscribeToOrders();
            Assert.Equal(subscriptionMessage, this.writer.LastLineWritten);
        }

        [Fact]
        public async Task IdIncrementsOnOrderSubscription()
        {
            var subscriptionMessage = new SubscriptionMessageStub("orderSubscription", 1).ToJson();
            await this.streamSubscription.SubscribeToOrders();
            Assert.Equal(subscriptionMessage, this.writer.LastLineWritten);

            var subscriptionMessage2 = new SubscriptionMessageStub("orderSubscription", 2).ToJson();
            await this.streamSubscription.SubscribeToOrders();
            Assert.Equal(subscriptionMessage2, this.writer.LastLineWritten);
        }

        [Fact]
        public async Task IdIncrementsOnAnySubscription()
        {
            var subscriptionMessage = new SubscriptionMessageStub("marketSubscription", 1).ToJson();
            await this.streamSubscription.Subscribe(null, null);
            Assert.Equal(subscriptionMessage, this.writer.LastLineWritten);

            var subscriptionMessage2 = new SubscriptionMessageStub("orderSubscription", 2).ToJson();
            await this.streamSubscription.SubscribeToOrders();
            Assert.Equal(subscriptionMessage2, this.writer.LastLineWritten);
        }

        [Fact]
        public async Task OnResubscribeOriginalMarketSubscriptionMessageIsSent()
        {
            var subscriptionMessage = new SubscriptionMessageStub("marketSubscription", 1).ToJson();
            await this.streamSubscription.Subscribe(null, null);
            Assert.Equal(subscriptionMessage, this.writer.LastLineWritten);

            var reSubscriptionMessage = new SubscriptionMessageStub("marketSubscription", 1).ToJson();
            await this.TriggerResubscribe();
            Assert.Equal(reSubscriptionMessage, this.writer.LastLineWritten);
        }

        [Fact]
        public async Task OnResubscribeOriginalOrderSubscriptionMessageIsSent()
        {
            var subscriptionMessage = new SubscriptionMessageStub("orderSubscription", 1).ToJson();
            await this.streamSubscription.SubscribeToOrders();
            Assert.Equal(subscriptionMessage, this.writer.LastLineWritten);

            var reSubscriptionMessage = new SubscriptionMessageStub("orderSubscription", 1).ToJson();
            await this.TriggerResubscribe();
            Assert.Equal(reSubscriptionMessage, this.writer.LastLineWritten);
        }

        [Fact]
        public async Task OnResubscribeAllSubscriptionMessagesAreSent()
        {
            await this.streamSubscription.Subscribe(null, null);
            await this.streamSubscription.Subscribe(null, null);
            await this.streamSubscription.SubscribeToOrders();
            var subscriptionMessage = new SubscriptionMessageStub("marketSubscription", 1).ToJson();
            var subscriptionMessage2 = new SubscriptionMessageStub("marketSubscription", 2).ToJson();
            var subscriptionMessage3 = new SubscriptionMessageStub("orderSubscription", 3).ToJson();

            await this.TriggerResubscribe();
            Assert.Contains(subscriptionMessage, this.writer.AllLinesWritten);
            Assert.Contains(subscriptionMessage2, this.writer.AllLinesWritten);
            Assert.Contains(subscriptionMessage3, this.writer.AllLinesWritten);
        }

        [Theory]
        [InlineData("InitialClock")]
        [InlineData("NewClock")]
        [InlineData("RefreshedClock")]
        public async Task OnResubscribeInitialClockIsInMessage(string initialClock)
        {
            await this.streamSubscription.Subscribe(null, null);
            await this.SendLineAsync($"{{\"op\":\"mcm\",\"id\":1,\"initialClk\":\"{initialClock}\",\"clk\":\"Clock\"}}");

            var subscriptionMessage = new SubscriptionMessageStub("marketSubscription", 1)
                .WithInitialClock(initialClock)
                .ToJson();

            await this.TriggerResubscribe();
            Assert.Equal(subscriptionMessage, this.writer.LastLineWritten);
        }

        [Fact]
        public async Task OnResubscribeAllSubscriptionMessagesSentContainInitialClock()
        {
            await this.streamSubscription.Subscribe(null, null);
            await this.streamSubscription.Subscribe(null, null);
            await this.streamSubscription.SubscribeToOrders();

            await this.SendLineAsync("{\"op\":\"mcm\",\"id\":1,\"initialClk\":\"One\",\"clk\":\"Clock\"}");
            await this.SendLineAsync("{\"op\":\"mcm\",\"id\":2,\"initialClk\":\"Two\",\"clk\":\"Clock\"}");
            await this.SendLineAsync("{\"op\":\"ocm\",\"id\":3,\"initialClk\":\"Three\",\"clk\":\"Clock\"}");

            var subscriptionMessage = new SubscriptionMessageStub("marketSubscription", 1).WithInitialClock("One").ToJson();
            var subscriptionMessage2 = new SubscriptionMessageStub("marketSubscription", 2).WithInitialClock("Two").ToJson();
            var subscriptionMessage3 = new SubscriptionMessageStub("orderSubscription", 3).WithInitialClock("Three").ToJson();

            await this.TriggerResubscribe();
            Assert.Contains(subscriptionMessage, this.writer.AllLinesWritten);
            Assert.Contains(subscriptionMessage2, this.writer.AllLinesWritten);
            Assert.Contains(subscriptionMessage3, this.writer.AllLinesWritten);
        }

        [Fact]
        public async Task OnReadDoNotSetInitialClockIfNoSubscriptionMessageExists()
        {
            await this.SendLineAsync("{\"op\":\"mcm\",\"id\":1,\"initialClk\":\"One\",\"clk\":\"Clock\"}");
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
            var sendLines = new StringBuilder();
            sendLines.AppendLine(line);
            this.streamSubscription.Reader = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(sendLines.ToString())));
            await foreach (var message in this.streamSubscription.GetChanges())
            {
            }
        }

        private async Task TriggerResubscribe()
        {
            this.writer.ClearPreviousResults();
            await this.streamSubscription.Resubscribe();
        }
    }
}
