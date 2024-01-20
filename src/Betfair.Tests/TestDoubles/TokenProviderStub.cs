using Betfair.Core.Login;

namespace Betfair.Tests.TestDoubles;

internal class TokenProviderStub : TokenProvider
{
    internal string RespondsWithToken { get; set; } = "token";

    internal override Task<string> GetToken(CancellationToken cancellationToken)
    {
        return Task.FromResult(RespondsWithToken);
    }
}
