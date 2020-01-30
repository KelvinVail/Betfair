namespace Betfair.Tests.Stream
{
    using System;
    using System.IO;
    using System.Net.Security;
    using System.Text;
    using System.Threading.Tasks;
    using Betfair.Streaming;
    using Betfair.Tests.TestDoubles;
    using Xunit;

    public class MarketSubscriptionTests : IDisposable
    {
        private readonly SessionSpy session = new SessionSpy();
        private readonly TcpClientSpy client;
        private readonly MarketSubscription marketSubscription;
        private readonly StringBuilder sendLines = new StringBuilder();
        private bool disposed;

        public MarketSubscriptionTests()
        {
            this.marketSubscription = new MarketSubscription(this.session);
            this.client = new TcpClientSpy("stream-api-integration.betfair.com");
            this.marketSubscription.WithTcpClient(this.client);
        }

        [Fact]
        public void MarketSubscriptionIsSealed()
        {
            Assert.True(typeof(MarketSubscription).IsSealed);
        }

        [Fact]
        public void OnWithTcpClientThrowIfNull()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => this.marketSubscription.WithTcpClient(null));
            Assert.Equal("client", exception.ParamName);
        }

        [Fact]
        public void OnConnectReceiveBufferSizeIsSet()
        {
            this.marketSubscription.Connect();
            Assert.Equal(1024 * 1000 * 2, this.client.ReceiveBufferSize);
        }

        [Fact]
        public void OnConnectReaderWriteTimeoutIsThirtySeconds()
        {
            this.marketSubscription.Connect();
            Assert.Equal(TimeSpan.FromSeconds(30).TotalMilliseconds, this.marketSubscription.Reader.BaseStream.WriteTimeout);
        }

        [Fact]
        public void OnConnectReaderReadTimeoutIsThirtySeconds()
        {
            this.marketSubscription.Connect();
            Assert.Equal(TimeSpan.FromSeconds(30).TotalMilliseconds, this.marketSubscription.Reader.BaseStream.ReadTimeout);
        }

        [Fact]
        public void OnConnectWriterWriteTimeoutIsThirtySeconds()
        {
            this.marketSubscription.Connect();
            Assert.Equal(TimeSpan.FromSeconds(30).TotalMilliseconds, this.marketSubscription.Writer.BaseStream.WriteTimeout);
        }

        [Fact]
        public void OnConnectWriterReadTimeoutIsThirtySeconds()
        {
            this.marketSubscription.Connect();
            Assert.Equal(TimeSpan.FromSeconds(30).TotalMilliseconds, this.marketSubscription.Writer.BaseStream.ReadTimeout);
        }

        [Fact]
        public void OnConnectTcpClientIsConnectedToBetfairHost()
        {
            this.marketSubscription.Connect();
            Assert.Equal("stream-api.betfair.com", this.client.Host);
        }

        [Fact]
        public void OnConnectTcpClientIsConnectedToBetfairPort()
        {
            this.marketSubscription.Connect();
            Assert.Equal(443, this.client.Port);
        }

        [Fact]
        public void OnConnectReaderStreamIsAuthenticated()
        {
            this.marketSubscription.Connect();
            var sslStream = (SslStream)this.marketSubscription.Reader.BaseStream;
            Assert.True(sslStream.IsAuthenticated);
        }

        [Fact]
        public void OnConnectWriterStreamIsAuthenticated()
        {
            this.marketSubscription.Connect();
            var sslStream = (SslStream)this.marketSubscription.Writer.BaseStream;
            Assert.True(sslStream.IsAuthenticated);
        }

        [Fact]
        public void OnConnectWriterIsAutoFlushed()
        {
            this.marketSubscription.Connect();
            Assert.True(this.marketSubscription.Writer.AutoFlush);
        }

        [Theory]
        [InlineData("106-300120115247-8647")]
        [InlineData("ConnectionId")]
        public async Task OnReadConnectionOperationConnectionIdIsUpdated(string id)
        {
            await this.SendLineAsync($"{{\"op\":\"connection\",\"connectionId\":\"{id}\"}}");
            Assert.Equal(id, this.marketSubscription.ConnectionId);
        }

        [Fact]
        public async Task OnReadConnectionOperationConnectedIsTrue()
        {
            Assert.False(this.marketSubscription.Connected);
            await this.SendLineAsync($"{{\"op\":\"connection\",\"connectionId\":\"ConnectionId\"}}");
            Assert.True(this.marketSubscription.Connected);
        }

        [Fact]
        public async Task OnReadStatusTimeoutOperationConnectionIdIsSetToNull()
        {
            const string id = "ConnectionId";
            await this.SendLineAsync($"{{\"op\":\"connection\",\"connectionId\":\"{id}\"}}");
            Assert.Equal(id, this.marketSubscription.ConnectionId);

            var message =
                $"{{\"op\":\"status\"," +
                $"\"statusCode\":\"FAILURE\"," +
                $"\"errorCode\":\"TIMEOUT\"," +
                $"\"errorMessage\":\"Timed out trying to read message make sure to add \\\\r\\\\n\\nRead data : ﻿\"," +
                $"\"connectionClosed\":true," +
                $"\"connectionId\":\"{id}\"}}";

            await this.SendLineAsync(message);
            Assert.Null(this.marketSubscription.ConnectionId);
            Assert.False(this.marketSubscription.Connected);
        }

        [Fact]
        public async Task OnReadConnectionIdPersistsIfMessageNotConnectionOperation()
        {
            await this.SendLineAsync($"{{\"op\":\"connection\",\"connectionId\":\"ConnectionId\"}}");
            await this.SendLineAsync($"{{\"op\":\"other\"}}");
            Assert.Equal("ConnectionId", this.marketSubscription.ConnectionId);
            Assert.True(this.marketSubscription.Connected);
        }

        [Fact]
        public async void OnReadStatusConnectionIdPersistsIfConnectionClosedIsFalse()
        {
            await this.SendLineAsync($"{{\"op\":\"connection\",\"connectionId\":\"ConnectionId\"}}");
            await this.SendLineAsync($"{{\"op\":\"status\",\"connectionClosed\":false,}}");
            Assert.Equal("ConnectionId", this.marketSubscription.ConnectionId);
            Assert.True(this.marketSubscription.Connected);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    this.client.Dispose();
                }

                this.disposed = true;
            }
        }

        private async Task SendLineAsync(string line)
        {
            this.sendLines.AppendLine(line);
            this.marketSubscription.Reader = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(this.sendLines.ToString())));
            await this.marketSubscription.Start();
        }
    }
}
