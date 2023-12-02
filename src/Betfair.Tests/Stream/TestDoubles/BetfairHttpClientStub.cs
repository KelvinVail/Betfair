using Betfair.Core.Client;
using Betfair.Core.Login;

namespace Betfair.Tests.Stream.TestDoubles;

public class BetfairHttpClientStub : BetfairHttpClient
{
    public BetfairHttpClientStub(Credentials credentials)
        : base(credentials)
    {
    }

    public string ReturnsAppKey { get; set; } = string.Empty;

    public string ReturnsToken { get; set; } = string.Empty;

    public override string AppKey => ReturnsAppKey;

    public bool IsDisposed { get; private set; }

    public int TimesDisposed { get; private set; }

    public override Task<string> GetToken(CancellationToken cancellationToken = default) => Task.FromResult(ReturnsToken);

    protected override void Dispose(bool disposing)
    {
        IsDisposed = true;
        TimesDisposed++;

        base.Dispose(disposing);
    }
}
