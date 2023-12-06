using Betfair.Core.Client;
using Betfair.Core.Login;

namespace Betfair.Tests.TestDoubles;

public class BetfairHttpClientStub : BetfairHttpClient
{
    public BetfairHttpClientStub()
        : base(new Credentials("u", "p", "a"))
    {
    }

    public string ReturnsAppKey { get; set; } = string.Empty;

    public string ReturnsToken { get; set; } = string.Empty;

    public override string AppKey => ReturnsAppKey;

    public Uri? LastPostedUri { get; set; }

    public object? LastPostedBody { get; set; }

    public object? ReturnsBody { get; set; }

    public bool IsDisposed { get; private set; }

    public int TimesDisposed { get; private set; }

    public override Task<string> GetToken(CancellationToken cancellationToken = default) => Task.FromResult(ReturnsToken);

    public override Task<T> Post<T>(Uri uri, object? body = null, CancellationToken cancellationToken = default)
        where T : class
    {
        LastPostedUri = uri;
        LastPostedBody = body;

        if (ReturnsBody != null) return Task.FromResult((T)ReturnsBody);
        return Task.FromResult((T)default!);
    }

    protected override void Dispose(bool disposing)
    {
        IsDisposed = true;
        TimesDisposed++;

        base.Dispose(disposing);
    }
}
