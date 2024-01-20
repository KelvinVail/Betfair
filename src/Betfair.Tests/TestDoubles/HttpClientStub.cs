using Betfair.Tests.Core.Client.TestDoubles;

namespace Betfair.Tests.TestDoubles;

internal class HttpClientStub : HttpClient
{
    public HttpClientStub(HttpMessageHandlerSpy handler)
        : base(handler) => Handler = handler;

    public HttpMessageHandlerSpy Handler { get; }

    public bool IsDisposed { get; private set; }

    public int TimesDisposed { get; private set; }

    protected override void Dispose(bool disposing)
    {
        IsDisposed = true;
        TimesDisposed++;

        base.Dispose(disposing);
    }
}
