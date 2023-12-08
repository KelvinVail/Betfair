using Betfair.Stream;
using Betfair.Tests.Stream.TestDoubles;

namespace Betfair.Tests.Stream;

public class PipelineTests : IDisposable
{
    private readonly BetfairTcpClientStub _tcp = new ();
    private readonly Pipeline _pipe;
    private bool _disposedValue;

    public PipelineTests() => _pipe = new Pipeline(_tcp);

    [Fact(Skip = "Changes based on environment")]
    public void TcpClientReceiveBufferSizeIsCorrect() =>
        _tcp.ReceiveBufferSize.Should().Be(1024 * 1000 * 2);

    [Fact]
    public void TcpSendTimeoutIsCorrect() =>
        _tcp.SendTimeout.Should().Be((int)TimeSpan.FromSeconds(30).TotalMilliseconds);

    [Fact]
    public void TcpReceiveTimeoutIsCorrect() =>
        _tcp.ReceiveTimeout.Should().Be((int)TimeSpan.FromSeconds(30).TotalMilliseconds);

    [Fact]
    public void ConnectionIsMadeToCorrectHost() =>
        _tcp.HostConnectedTo.Should().Be("stream-api.betfair.com");

    [Fact]
    public void ConnectionIsMadeToCorrectPort() =>
        _tcp.PortConnectTo.Should().Be(443);

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposedValue) return;
        if (disposing)
        {
            _pipe.Dispose();
            _tcp.Dispose();
        }

        _disposedValue = true;
    }
}
