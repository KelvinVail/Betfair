using System.Diagnostics;

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
        public async Task OnReadConnectionResponseConnectionStatusIsUpdated(string id)
        {
            this.SendLine($"{{\"op\":\"connection\",\"connectionId\":\"{id}\"}}");
            await this.marketSubscription.Start();
            Assert.Equal(id, this.marketSubscription.ConnectionId);
        }

        [Theory]
        [InlineData("106-300120115247-8647")]
        [InlineData("ConnectionId")]
        [InlineData("ConnectionId2")]
        [InlineData("ConnectionId3")]
        [InlineData("ConnectionId4")]
        public async Task Timer(string id)
        {
            var timer = new Stopwatch();
            var ticksPerSecond = Stopwatch.Frequency;
            this.SendLine($"{{\"op\":\"connection\",\"connectionId\":\"{id}\"}}");
            timer.Start();
            await this.marketSubscription.Start();
            timer.Stop();
            var ms = timer.ElapsedMilliseconds;
            var t = timer.ElapsedTicks;
            Assert.Equal(id, this.marketSubscription.ConnectionId);
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

        private void SendLine(string line)
        {
            this.sendLines.AppendLine(line);
            this.marketSubscription.Reader = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(this.sendLines.ToString())));
        }
    }
}
