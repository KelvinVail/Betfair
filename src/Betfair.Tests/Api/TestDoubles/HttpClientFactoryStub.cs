namespace Betfair.Tests.Api.TestDoubles;

/// <summary>
/// A test double for <see cref="IHttpClientFactory"/> that returns an <see cref="HttpClient"/>
/// backed by a supplied <see cref="HttpMessageHandler"/>.
/// The factory does not own the handler or client lifetime.
/// </summary>
internal sealed class HttpClientFactoryStub : IHttpClientFactory
{
    private readonly HttpMessageHandler _handler;

    internal HttpClientFactoryStub(HttpMessageHandler handler) =>
        _handler = handler;

    public string? LastClientNameRequested { get; private set; }

    public HttpClient CreateClient(string name)
    {
        LastClientNameRequested = name;

        // disposeHandler: false — the stub owns the handler, not the client
        return new HttpClient(_handler, disposeHandler: false);
    }
}
