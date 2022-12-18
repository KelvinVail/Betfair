using Betfair.Stream;
using Betfair.Tests.Stream.TestDoubles;

namespace Betfair.Tests.Stream;

public sealed class StreamFactoryTests : IDisposable
{
    private readonly TcpClientSpy _tcpClient;
    private bool _disposedValue;

    public StreamFactoryTests()
    {
        _tcpClient = new TcpClientSpy("stream-api-integration.betfair.com", 443);
        using var stream = StreamFactory.Open(_tcpClient);
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
}