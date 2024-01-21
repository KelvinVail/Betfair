using Betfair.Tests.Stream.TestDoubles;

namespace Betfair.Tests.Stream;

public class BetfairTcpClientTests : IDisposable
{
    private readonly BetfairTcpClientStub _tcp = new ();
    private bool _disposedValue;

    [Fact(Skip = "Changes based on environment")]
    public void TcpClientReceiveBufferSizeIsCorrect() =>
        _tcp.ReceiveBufferSize.Should().Be(1024 * 1000 * 2);

    [Fact]
    public void TcpSendTimeoutIsCorrect() =>
        _tcp.SendTimeout.Should().Be((int)TimeSpan.FromSeconds(30).TotalMilliseconds);

    [Fact]
    public void TcpReceiveTimeoutIsCorrect() =>
        _tcp.ReceiveTimeout.Should().Be((int)TimeSpan.FromSeconds(30).TotalMilliseconds);

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposedValue) return;
        if (disposing) _tcp.Dispose();

        _disposedValue = true;
    }
}
