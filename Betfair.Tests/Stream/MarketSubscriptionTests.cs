namespace Betfair.Tests.Stream
{
    using System;
    using System.Net.Security;
    using System.Net.Sockets;
    using Betfair.Streaming;
    using Betfair.Tests.TestDoubles;
    using Xunit;

    public class MarketSubscriptionTests : IDisposable
    {
        private readonly SessionSpy session = new SessionSpy();
        private readonly TcpClientSpy client;
        private readonly MarketSubscription marketSubscription;
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
        public void WhenInitializedReceiveBufferSizeIsSet()
        {
            Assert.Equal(1024 * 1000 * 2, this.client.ReceiveBufferSize);
        }

        [Fact]
        public void WhenInitializedReaderWriteTimeoutIsThirtySeconds()
        {
            Assert.Equal(TimeSpan.FromSeconds(30).TotalMilliseconds, this.marketSubscription.Reader.BaseStream.WriteTimeout);
        }

        [Fact]
        public void WhenInitializedReaderReadTimeoutIsThirtySeconds()
        {
            Assert.Equal(TimeSpan.FromSeconds(30).TotalMilliseconds, this.marketSubscription.Reader.BaseStream.ReadTimeout);
        }

        [Fact]
        public void WhenInitializedWriterWriteTimeoutIsThirtySeconds()
        {
            Assert.Equal(TimeSpan.FromSeconds(30).TotalMilliseconds, this.marketSubscription.Writer.BaseStream.WriteTimeout);
        }

        [Fact]
        public void WhenInitializedWriterReadTimeoutIsThirtySeconds()
        {
            Assert.Equal(TimeSpan.FromSeconds(30).TotalMilliseconds, this.marketSubscription.Writer.BaseStream.ReadTimeout);
        }

        [Fact]
        public void WhenInitializedTcpClientIsConnectedToBetfairHost()
        {
            Assert.Equal("stream-api.betfair.com", this.client.Host);
        }

        [Fact]
        public void WhenInitializedTcpClientIsConnectedToBetfairPort()
        {
            Assert.Equal(443, this.client.Port);
        }

        [Fact]
        public void WhenInitializedReaderStreamIsAuthenticated()
        {
            var sslStream = (SslStream)this.marketSubscription.Reader.BaseStream;
            Assert.True(sslStream.IsAuthenticated);
        }

        [Fact]
        public void WhenInitializedWriterStreamIsAuthenticated()
        {
            var sslStream = (SslStream)this.marketSubscription.Writer.BaseStream;
            Assert.True(sslStream.IsAuthenticated);
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
    }
}
