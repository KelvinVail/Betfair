using System.Security.Cryptography.X509Certificates;
using Betfair.Core.Client;
using Betfair.Tests.Core.Client.TestDoubles;

namespace Betfair.Tests.Core.Client;

public class BetfairHttpClientTests : IDisposable
{
    private readonly HttpMessageHandlerSpy _handler = new ();
    private readonly BetfairHttpClient _client = new ((X509Certificate2?)null);
    private bool _disposedValue;

    [Fact]
    public void TimeoutIsSetToThirtySeconds() =>
        _client.Timeout.Should().Be(TimeSpan.FromSeconds(30));

    [Fact]
    public void AcceptsApplicationJson() =>
        _client.DefaultRequestHeaders.Accept.Should().Contain(
            new MediaTypeWithQualityHeaderValue("application/json"));

    [Fact]
    public void ConnectionIsKeptAlive() =>
        _client.DefaultRequestHeaders.Connection.Should().Contain("keep-alive");

    [Fact]
    public void AcceptGzipEncoding() =>
        _client.DefaultRequestHeaders.AcceptEncoding.Should().Contain(
            new StringWithQualityHeaderValue("gzip"));

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
            _client.Dispose();
            _handler.Dispose();
        }

        _disposedValue = true;
    }
}
