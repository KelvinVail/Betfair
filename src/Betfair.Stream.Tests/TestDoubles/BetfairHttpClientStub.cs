using Betfair.Core.Client;

namespace Betfair.Stream.Tests.TestDoubles;

public class BetfairHttpClientStub : BetfairHttpClient
{
    public string ReturnsAppKey { get; set; } = string.Empty;

    public string ReturnsToken { get; set; } = string.Empty;

    public override string AppKey => ReturnsAppKey;

    public override Task<string> GetToken(CancellationToken cancellationToken = default) => Task.FromResult(ReturnsToken);
}
