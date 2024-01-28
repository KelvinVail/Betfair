using Betfair.Core.Login;

namespace Betfair.Tests.TestDoubles;

internal class TokenProviderStub : TokenProvider
{
    internal List<string> RespondsWithToken { get; set; } = new ();

    internal int TokensUsed { get; private set; }

    internal override Task<string> GetToken(CancellationToken cancellationToken)
    {
        TokensUsed++;
        if (RespondsWithToken.Count == 0)
            return Task.FromResult("Token");

        return Task.FromResult(RespondsWithToken[TokensUsed - 1]);
    }
}
