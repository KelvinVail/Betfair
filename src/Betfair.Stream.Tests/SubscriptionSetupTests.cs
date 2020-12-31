using System;
using System.Net.Security;
using System.Threading.Tasks;
using Betfair.Stream.Tests.TestDoubles;
using Xunit;

namespace Betfair.Stream.Tests
{
    public sealed class SubscriptionSetupTests : SubscriptionTests
    {
        private readonly TcpClientSpy _tcpClient;

        public SubscriptionSetupTests()
        {
            _tcpClient = new TcpClientSpy("stream-api-integration.betfair.com", 443);
            Subscription.WithTcpClient(_tcpClient);
        }

        [Fact]
        public void SubscriptionIsSealed()
        {
            Assert.True(typeof(Subscription).IsSealed);
        }

        [Fact]
        public void OnWithTcpClientThrowIfNull()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => Subscription.WithTcpClient(null));
            Assert.Equal("client", exception.ParamName);
        }

        [Fact]
        public void OnConnectReceiveBufferSizeIsSet()
        {
            Subscription.Connect();
            Assert.Equal(1024 * 1000 * 2, _tcpClient.ReceiveBufferSize);
        }

        [Fact]
        public void OnConnectSendTimeoutIsThirtySeconds()
        {
            Subscription.Connect();
            Assert.Equal(TimeSpan.FromSeconds(30).TotalMilliseconds, _tcpClient.SendTimeout);
        }

        [Fact]
        public void OnConnectReceiveTimeoutIsThirtySeconds()
        {
            Subscription.Connect();
            Assert.Equal(TimeSpan.FromSeconds(30).TotalMilliseconds, _tcpClient.ReceiveTimeout);
        }

        [Fact]
        public void OnConnectTcpClientIsConnectedToBetfairHost()
        {
            Subscription.Connect();
            Assert.Equal("stream-api.betfair.com", _tcpClient.Host);
        }

        [Fact]
        public void OnConnectTcpClientIsConnectedToBetfairPort()
        {
            Subscription.Connect();
            Assert.Equal(443, _tcpClient.Port);
        }

        // This test creates a network connection, I haven't been able to figure out
        // a good way to mock this yet.
        [Fact]
        public void OnConnectReaderStreamIsAuthenticated()
        {
            var connectedSubscription = new Subscription(Session);
            connectedSubscription.Connect();
            var sslStream = (SslStream)connectedSubscription.Reader.BaseStream;
            Assert.True(sslStream.IsAuthenticated);
        }

        // This test creates a network connection, I haven't been able to figure out
        // a good way to mock this yet.
        [Fact]
        public void OnConnectWriterStreamIsAuthenticated()
        {
            var connectedSubscription = new Subscription(Session);
            connectedSubscription.Connect();
            var sslStream = (SslStream)connectedSubscription.Writer.BaseStream;
            Assert.True(sslStream.IsAuthenticated);
        }

        [Fact]
        public void OnConnectWriterIsAutoFlushed()
        {
            Subscription.Connect();
            Assert.True(Subscription.Writer.AutoFlush);
        }

        [Fact]
        public async Task OnStopConnectedIsFalse()
        {
            await SendLineAsync("{\"op\":\"connection\",\"connectionId\":\"ConnectionId\"}");
            Assert.True(Subscription.Connected);
            Subscription.Disconnect();
            Assert.False(Subscription.Connected);
        }

        [Fact]
        public async Task OnStopTcpClientIsClosed()
        {
            await SendLineAsync("{\"op\":\"connection\",\"connectionId\":\"ConnectionId\"}");
            Assert.True(Subscription.Connected);
            Subscription.Disconnect();
            Assert.False(_tcpClient.Connected);
        }
    }
}
