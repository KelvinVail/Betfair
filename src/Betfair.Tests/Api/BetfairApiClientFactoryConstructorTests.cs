using Betfair.Api;
using Betfair.Api.Betting.Endpoints.ListEventTypes.Responses;
using Betfair.Core.Authentication;
using Betfair.Core.Client;
using Betfair.Tests.Api.TestDoubles;

namespace Betfair.Tests.Api;

public class BetfairApiClientFactoryConstructorTests : IDisposable
{
    private readonly Credentials _credentials = new ("user", "pass", "appKey");
    private bool _disposedValue;

    // --- Null guard tests ---

    [Fact]
    public void ThrowsWhenCredentialsIsNull()
    {
        var factory = new HttpClientFactoryStub(new HttpMessageHandlerSpy());

        Action act = () => new BetfairApiClient(null!, factory);

        act.Should().Throw<ArgumentNullException>().WithParameterName("credentials");
    }

    [Fact]
    public void ThrowsWhenHttpClientFactoryIsNull()
    {
        Action act = () => new BetfairApiClient(_credentials, null!);

        act.Should().Throw<ArgumentNullException>().WithParameterName("httpClientFactory");
    }

    // --- Factory integration tests ---

    [Fact]
    public void CreatesTheNamedBetfairClient()
    {
        var factory = new HttpClientFactoryStub(new HttpMessageHandlerSpy());

        _ = new BetfairApiClient(_credentials, factory);

        factory.LastClientNameRequested.Should().Be("Betfair");
    }

    [Fact]
    public async Task UsesTheHttpClientProvidedByTheFactory()
    {
        var handler = new HttpMessageHandlerSpy();
        handler.RespondsWithBody = Array.Empty<EventTypeResult>();
        var factory = new HttpClientFactoryStub(handler);

        using var api = new BetfairApiClient(_credentials, factory);
        await api.EventTypes(cancellationToken: TestContext.Current.CancellationToken);

        handler.UriCalled.Should().Be(
            new Uri("https://api.betfair.com/exchange/betting/rest/v1.0/listEventTypes/"));
    }

    [Fact]
    public async Task InjectsAppKeyHeaderFromCredentials()
    {
        var handler = new HttpMessageHandlerSpy();
        handler.RespondsWithBody = Array.Empty<EventTypeResult>();
        var credentials = new Credentials("user", "pass", "my-app-key");
        var factory = new HttpClientFactoryStub(handler);

        using var api = new BetfairApiClient(credentials, factory);
        await api.EventTypes(cancellationToken: TestContext.Current.CancellationToken);

        handler.ContentHeadersSent.Should().ContainKey("X-Application")
            .WhoseValue.Should().Contain("my-app-key");
    }

    [Fact]
    public void DoesNotDisposeTheHttpClientOnDispose()
    {
        // The factory owns the client lifetime — BetfairApiClient must not dispose it.
        var handler = new HttpMessageHandlerSpy();
        var factory = new HttpClientFactoryStub(handler);
        var api = new BetfairApiClient(_credentials, factory);

        api.Dispose();

        // If the client were disposed, the handler would be disposed too.
        // Verify the handler is still usable.
        bool handlerDisposed = false;
        try
        {
            handler.CheckCertificateRevocationList = true;
        }
        catch (ObjectDisposedException)
        {
            handlerDisposed = true;
        }

        handlerDisposed.Should().BeFalse();
    }

    // --- BetfairClientHandler public visibility test ---

    [Fact]
    public void BetfairClientHandlerIsPublic()
    {
        // Consumers need to register BetfairClientHandler with IHttpClientFactory.
        // This test ensures it remains publicly accessible.
        using var handler = new BetfairClientHandler();

        handler.Should().NotBeNull();
    }

    [Fact]
    public void BetfairClientHandlerCanBeUsedAsPrimaryHandler()
    {
        using var handler = new BetfairClientHandler();
        var factory = new HttpClientFactoryStub(handler);

        Action act = () => _ = new BetfairApiClient(_credentials, factory);

        act.Should().NotThrow();
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposedValue) return;
        _disposedValue = true;
    }
}
