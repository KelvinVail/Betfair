using Betfair.Core.Client;

namespace Betfair.Tests.Api.TestDoubles;

internal class HttpAdapterStub : HttpAdapter, IDisposable
{
    private readonly HttpMessageHandlerSpy _handler = new ();
    private readonly BetfairHttpClient _client;
    private readonly HttpAdapter _adapter;
    private bool _disposedValue;

    public HttpAdapterStub()
    {
        _client = new BetfairHttpClient(_handler);
        var deserializer = new HttpDeserializer(_client);
        _adapter = new HttpAdapter(deserializer);
    }

    public Uri? LastUriCalled { get; private set; }

    public object? LastContentSent { get; private set; }

    public object? RespondsWithBody { get; set; }

    public override Task<T> PostAsync<T>(Uri uri, object body, CancellationToken ct = default)
    {
        LastUriCalled = uri;
        LastContentSent = body;

        _handler.RespondsWithBody = RespondsWithBody;

        return _adapter.PostAsync<T>(uri, body, ct);
    }

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
            _handler.Dispose();
            _client.Dispose();
        }

        _disposedValue = true;
    }
}
