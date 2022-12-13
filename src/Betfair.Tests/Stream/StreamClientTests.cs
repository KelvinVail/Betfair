using Betfair.Stream;
using Betfair.Tests.Stream.TestDoubles;

namespace Betfair.Tests.Stream;

public sealed class StreamClientTests : IDisposable
{
    private readonly TcpClientSpy _tcpClient;
    private readonly StreamClient _client;
    private bool _disposedValue;

    public StreamClientTests()
    {
        _tcpClient = new TcpClientSpy("stream-api-integration.betfair.com", 443);
        _client = new StreamClient(_tcpClient);
    }

    [Fact]
    public void ReceiveBufferSizeIsSet() =>
        _tcpClient.ReceiveBufferSize.Should().Be(1024 * 1000 * 2);

    [Fact]
    public void SendTimeoutIsThirtySeconds() =>
        _tcpClient.SendTimeout.Should().Be(30000);

    [Fact]
    public void ReceiveTimeoutIsThirtySeconds() =>
        _tcpClient.ReceiveTimeout.Should().Be(30000);

    [Fact]
    public void IsConnectedToBetfairHost() =>
        _tcpClient.Host.Should().Be("stream-api.betfair.com");

    [Fact]
    public void IsConnectedToBetfairPort() =>
        _tcpClient.Port.Should().Be(443);

    [Fact]
    public void StreamIsAuthenticated() =>
        _tcpClient.IsAuthenticated.Should().BeTrue();

    [Fact]
    public void WriterIsAutoFlushed() =>
        _client.Writer.AutoFlush.Should().BeTrue();

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (_disposedValue)
            return;

        if (disposing)
            _tcpClient.Dispose();

        _disposedValue = true;
    }

    //[Fact]
    //public async Task OnStopConnectedIsFalse()
    //{
    //    await SendLineAsync("{\"op\":\"connection\",\"connectionId\":\"ConnectionId\"}");
    //    Assert.True(Subscription.Connected);
    //    Subscription.Disconnect();
    //    Assert.False(Subscription.Connected);
    //}

    //[Fact]
    //public async Task OnStopTcpClientIsClosed()
    //{
    //    await SendLineAsync("{\"op\":\"connection\",\"connectionId\":\"ConnectionId\"}");
    //    Assert.True(Subscription.Connected);
    //    Subscription.Disconnect();
    //    Assert.False(_tcpClient.Connected);
    //}
}