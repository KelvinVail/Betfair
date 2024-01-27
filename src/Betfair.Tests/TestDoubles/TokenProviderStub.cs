using Betfair.Core.Login;

namespace Betfair.Tests.TestDoubles;

internal class TokenProviderStub : TokenProvider
{
    internal string RespondsWithToken { get; set; } = "token";

    internal int TokensUsed { get; private set; }

    internal override Task<string> GetToken(CancellationToken cancellationToken)
    {
        TokensUsed++;
        return Task.FromResult(RespondsWithToken);
    }
}
