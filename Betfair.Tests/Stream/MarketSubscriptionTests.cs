namespace Betfair.Tests.Stream
{
    using System;
    using System.Net.Sockets;
    using Betfair.Streaming;
    using Betfair.Tests.TestDoubles;
    using Xunit;

    public class MarketSubscriptionTests : IDisposable
    {
        private readonly SessionSpy session = new SessionSpy();
        private readonly Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private readonly NetworkStream stream;
        private readonly TcpClientSpy client;
        private readonly MarketSubscription marketSubscription;
        private bool disposed;

        public MarketSubscriptionTests()
        {
            this.socket.Connect("test.com", 443);
            this.stream = new NetworkStream(this.socket);
            this.marketSubscription = new MarketSubscription(this.session);
            this.client = new TcpClientSpy();
            this.client.WithStream(this.stream);
            this.marketSubscription.WithTcpClient(this.client);
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
        public void WhenInitializedSendTimeoutIsThirtySeconds()
        {
            Assert.Equal(TimeSpan.FromSeconds(30).TotalMilliseconds, this.client.SendTimeout);
        }

        [Fact]
        public void WhenInitializedReceiveTimeoutIsThirtySeconds()
        {
            Assert.Equal(TimeSpan.FromSeconds(30).TotalMilliseconds, this.client.ReceiveTimeout);
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
                    this.socket.Dispose();
                    this.stream.Dispose();
                }

                this.disposed = true;
            }
        }
    }
}
