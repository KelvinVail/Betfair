using System.Security.Cryptography.X509Certificates;
using Betfair.Core.Client;
using Betfair.Tests.Core.Client.TestDoubles;
#pragma warning disable CA1806

namespace Betfair.Tests.Core.Client;

public class BetfairHttpClientTests : IDisposable
{
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

    [Fact]
    public void HandlerIsDisposedWithHttpClient()
    {
        using var handler = new BetfairClientHandler();
        handler.CheckCertificateRevocationList = true;
        var client = new BetfairHttpClient(handler);

        client.Dispose();

        bool handlerIsDisposed = false;
        try
        {
            handler.CheckCertificateRevocationList = false;
        }
        catch (ObjectDisposedException)
        {
            handlerIsDisposed = true;
        }

        handlerIsDisposed.Should().BeTrue();
    }

    [Fact]
    public void ConstructWithNullCertificateDoesNotThrow()
    {
        Action act = () => new BetfairHttpClient((X509Certificate2?)null);

        act.Should().NotThrow();
    }

    [Fact]
    public void ConstructWithCertificateDoesNotThrow()
    {
        using var cert = new X509Certificate2Stub();

        Action act = () => new BetfairHttpClient(cert);

        act.Should().NotThrow();
    }

    [Fact]
    public void ConstructWithNullHandlerThrowsArgumentNullException()
    {
        Action act = () => new BetfairHttpClient((HttpMessageHandler)null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void ConstructWithHandlerDoesNotThrow()
    {
        using var handler = new HttpClientHandler();

        Action act = () => new BetfairHttpClient(handler);

        act.Should().NotThrow();
    }

    [Fact]
    public void ConstructWithHandlerSetsHandler()
    {
        using var handler = new HttpClientHandler();
        handler.CheckCertificateRevocationList = true;

        using var client = new BetfairHttpClient(handler);

        client.DefaultRequestHeaders.Connection.Should().Contain("keep-alive");
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposedValue) return;
        if (disposing) _client.Dispose();
        _disposedValue = true;
    }
}
