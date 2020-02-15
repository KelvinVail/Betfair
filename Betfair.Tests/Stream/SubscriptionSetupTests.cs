namespace Betfair.Tests.Stream
{
    using System;
    using System.Net.Security;
    using System.Threading.Tasks;
    using Betfair.Stream;
    using Betfair.Tests.Stream.TestDoubles;
    using Xunit;

    public sealed class SubscriptionSetupTests : SubscriptionTests
    {
        private readonly TcpClientSpy tcpClient;

        public SubscriptionSetupTests()
        {
            this.tcpClient = new TcpClientSpy("stream-api-integration.betfair.com", 443);
            this.Subscription.WithTcpClient(this.tcpClient);
        }

        [Fact]
        public void SubscriptionIsSealed()
        {
            Assert.True(typeof(Subscription).IsSealed);
        }

        [Fact]
        public void OnWithTcpClientThrowIfNull()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => this.Subscription.WithTcpClient(null));
            Assert.Equal("client", exception.ParamName);
        }

        [Fact]
        public void OnConnectReceiveBufferSizeIsSet()
        {
            this.Subscription.Connect();
            Assert.Equal(1024 * 1000 * 2, this.tcpClient.ReceiveBufferSize);
        }

        [Fact]
        public void OnConnectSendTimeoutIsThirtySeconds()
        {
            this.Subscription.Connect();
            Assert.Equal(TimeSpan.FromSeconds(30).TotalMilliseconds, this.tcpClient.SendTimeout);
        }

        [Fact]
        public void OnConnectReceiveTimeoutIsThirtySeconds()
        {
            this.Subscription.Connect();
            Assert.Equal(TimeSpan.FromSeconds(30).TotalMilliseconds, this.tcpClient.ReceiveTimeout);
        }

        [Fact]
        public void OnConnectTcpClientIsConnectedToBetfairHost()
        {
            this.Subscription.Connect();
            Assert.Equal("stream-api.betfair.com", this.tcpClient.Host);
        }

        [Fact]
        public void OnConnectTcpClientIsConnectedToBetfairPort()
        {
            this.Subscription.Connect();
            Assert.Equal(443, this.tcpClient.Port);
        }

        // This test creates a network connection, I haven't been able to figure out
        // a good way to mock this yet.
        [Fact]
        public void OnConnectReaderStreamIsAuthenticated()
        {
            var connectedSubscription = new Subscription(this.Session);
            connectedSubscription.Connect();
            var sslStream = (SslStream)connectedSubscription.Reader.BaseStream;
            Assert.True(sslStream.IsAuthenticated);
        }

        // This test creates a network connection, I haven't been able to figure out
        // a good way to mock this yet.
        [Fact]
        public void OnConnectWriterStreamIsAuthenticated()
        {
            var connectedSubscription = new Subscription(this.Session);
            connectedSubscription.Connect();
            var sslStream = (SslStream)connectedSubscription.Writer.BaseStream;
            Assert.True(sslStream.IsAuthenticated);
        }

        [Fact]
        public void OnConnectWriterIsAutoFlushed()
        {
            this.Subscription.Connect();
            Assert.True(this.Subscription.Writer.AutoFlush);
        }

        [Fact]
        public async Task OnStopConnectedIsFalse()
        {
            await this.SendLineAsync("{\"op\":\"connection\",\"connectionId\":\"ConnectionId\"}");
            Assert.True(this.Subscription.Connected);
            this.Subscription.Stop();
            Assert.False(this.Subscription.Connected);
        }

        [Fact]
        public async Task OnStopTcpClientIsClosed()
        {
            await this.SendLineAsync("{\"op\":\"connection\",\"connectionId\":\"ConnectionId\"}");
            Assert.True(this.Subscription.Connected);
            this.Subscription.Stop();
            Assert.False(this.tcpClient.Connected);
        }
    }
}
